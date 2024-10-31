using Dungeonator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrutalItems.Content.StatusEffects
{
    public class LinkedStatusEffect : GameActorEffect
    {
        public static CoreDamageTypes LinkedDamageType = ETGModCompatibility.ExtendEnum<CoreDamageTypes>(MOD_GUID, "Linked");

        public static LinkedStatusEffect LinkedGerator(float duration = 10f)
        {
            return new()
            {
                OverheadVFX = LoadAsset<GameObject>("linkedvfx"),

                AffectsEnemies = true,
                AffectsPlayers = true,

                stackMode = EffectStackingMode.Refresh,
                effectIdentifier = "Linked",
                duration = duration
            };
        }

        public override void OnEffectApplied(GameActor actor, RuntimeGameActorEffectData effectData, float partialAmount = 1)
        {
            base.OnEffectApplied(actor, effectData, partialAmount);

            if (actor.healthHaver)
                actor.healthHaver.Ext().OnDamagedContext += LinkDamage;
        }

        public void LinkDamage(HealthHaver hh, float damage, string source, float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection, bool ignoreInvulnerabilityFrames, bool ignoreDamageCaps)
        {
            if (damageTypes == LinkedDamageType || hh == null || hh.gameActor == null)
                return;

            LinkEffect(hh.gameActor, damage, source, damageDirection);
        }

        public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
        {
            base.OnEffectRemoved(actor, effectData);

            if (actor.healthHaver)
                actor.healthHaver.Ext().OnDamagedContext -= LinkDamage;
        }

        public static void LinkEffect(GameActor a, float damage, string source, Vector2 damageDirection)
        {
            if (a == null || damage <= 0f)
                return;

            RoomHandler r = null;
            PlayerController exceptionPlayer = null;
            AIActor exceptionEnemy = null;

            if (a is AIActor ai)
            {
                exceptionEnemy = ai;
                r = ai.ParentRoom;
            }
            else if (a is PlayerController play)
            {
                exceptionPlayer = play;
                r = play.CurrentRoom;
            }

            if (r != null)
            {
                var enemies = r.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                if (enemies != null)
                {
                    foreach (var e in enemies.ToList())
                    {
                        if (e == null || e == exceptionEnemy || e.GetEffect("Linked") == null)
                            continue;

                        e.healthHaver.ApplyDamage(damage * (exceptionPlayer != null ? 20f : 1f), damageDirection, source, LinkedDamageType, DamageCategory.Normal, true, null, true);
                    }
                }
            }

            foreach (var play in GameManager.Instance.AllPlayers)
            {
                if (play == null || play == exceptionPlayer || play.GetEffect("Linked") == null)
                    continue;

                play.healthHaver.ApplyDamage(0.5f, damageDirection, "Linked", LinkedDamageType, DamageCategory.Normal, true, null, true);
            }
        }
    }
}
