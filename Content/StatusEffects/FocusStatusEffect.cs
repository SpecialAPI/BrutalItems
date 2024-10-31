using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrutalItems.Content.StatusEffects
{
    public class FocusStatusEffect : GameActorEffect
    {
        public float FocusModifier;
        public float FocusPlayerBulletSpeedMultiplier;
        public float FocusRangeMultiplier;
        public float FocusRadius;

        public static readonly Color32 FocusColor = new(255, 247, 64, 255);

        public static FocusStatusEffect FocusGenerator(float duration = 0.25f, float damageModifier = 1.5f, float playerBulletSpeedModifier = 1.5f, float rangeModifier = 2f, float radius = 2.25f)
        {
            return new()
            {
                OverheadVFX = LoadAsset<GameObject>("focusvfx"),

                AffectsEnemies = true,
                AffectsPlayers = true,

                stackMode = EffectStackingMode.Refresh,
                effectIdentifier = "Focus",
                duration = duration,

                FocusModifier = damageModifier,
                FocusPlayerBulletSpeedMultiplier = playerBulletSpeedModifier,
                FocusRangeMultiplier = rangeModifier,
                FocusRadius = radius
            };
        }

        public override void OnEffectApplied(GameActor actor, RuntimeGameActorEffectData effectData, float partialAmount = 1)
        {
            base.OnEffectApplied(actor, effectData, partialAmount);

            if (actor is PlayerController player)
            {
                player.PostProcessProjectile += FocusDamage;
                player.OnNewFloorLoaded += RemoveFocusOnRoomClear;
                player.OnRoomClearEvent += RemoveFocusOnRoomClear;

                var radialIndicator = Object.Instantiate((GameObject)ResourceCache.Acquire("Global VFX/HeatIndicator"), player.CenterPosition.ToVector3ZisY(), Quaternion.identity, player.transform).GetComponent<HeatIndicatorController>();

                effectData.vfxObjects ??= [];
                effectData.vfxObjects.Add(new(radialIndicator.gameObject, 0f));

                radialIndicator.CurrentColor = FocusColor;
                radialIndicator.CurrentRadius = duration > 0f ? 0f : FocusRadius;
                radialIndicator.IsFire = false;
            }

            if (actor.healthHaver)
                actor.healthHaver.Ext().OnDamagedContext += LoseFocusOnDamage;

            if (actor.behaviorSpeculator)
                actor.behaviorSpeculator.CooldownScale *= FocusModifier;
        }

        public void RemoveFocusOnRoomClear(PlayerController player)
        {
            player.RemoveEffect(this);
        }

        public override void EffectTick(GameActor actor, RuntimeGameActorEffectData effectData)
        {
            if (actor is not PlayerController player)
                return;

            var r = FocusRadius;
            var immune = false;

            if (player.IsDodgeRolling)
            {
                effectData.elapsed = Mathf.Max(1 - (player.m_dodgeRollTimer / (0.25f * player.rollStats.GetModifiedTime(player))), 0f) * duration;

                immune = true;
            }

            if (duration >= 0f && effectData.elapsed < duration)
            {
                r *= Mathf.Max(effectData.elapsed, 0f) / duration;

                immune = true;
            }

            if (!immune)
            {
                foreach (var p in StaticReferenceManager.AllProjectiles)
                {
                    if (!p.collidesWithPlayer)
                        continue;

                    if (p.Owner is PlayerController)
                        continue;

                    if (Vector2.Distance(p.SafeCenter, player.CenterPosition) >= r)
                        continue;

                    player.RemoveEffect(this);
                    return;
                }
            }

            if (effectData.vfxObjects == null || effectData.vfxObjects.Count <= 0)
                return;

            var vf = effectData.vfxObjects[0];

            if (vf == null || vf.First == null)
                return;

            var indicator = vf.First.GetComponent<HeatIndicatorController>();
            indicator.CurrentRadius = r;
        }

        public void LoseFocusOnDamage(HealthHaver hh, float damage, string source, float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection, bool ignoreInvulnerabilityFrames, bool ignoreDamageCaps)
        {
            if(hh.gameActor == null)
                return;

            hh.gameActor.RemoveEffect(this);
        }

        public void FocusDamage(Projectile proj, float f)
        {
            proj.baseData.damage *= FocusModifier;
            proj.baseData.range *= FocusRangeMultiplier;

            proj.AdjustPlayerProjectileTint(FocusColor, 0);
        }

        public override bool IsFinished(GameActor actor, RuntimeGameActorEffectData effectData, float elapsedTime)
        {
            return false;
        }

        public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
        {
            base.OnEffectRemoved(actor, effectData);

            if (actor is PlayerController player)
            {
                player.PostProcessProjectile -= FocusDamage;
                player.OnNewFloorLoaded -= RemoveFocusOnRoomClear;
                player.OnRoomClearEvent -= RemoveFocusOnRoomClear;
            }

            if (actor.healthHaver)
                actor.healthHaver.Ext().OnDamagedContext -= LoseFocusOnDamage;

            if (actor.behaviorSpeculator)
                actor.behaviorSpeculator.CooldownScale /= FocusModifier;

            DestroyVFX(effectData);
        }

        public void DestroyVFX(RuntimeGameActorEffectData dat)
        {
            if (dat == null || dat.vfxObjects == null)
                return;

            foreach (var vf in dat.vfxObjects)
            {
                if (vf == null || vf.First == null)
                    return;

                Object.Destroy(vf.First);
            }

            dat.vfxObjects.Clear();
        }
    }
}
