using BrutalItems.Content.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrutalItems.Content.StatusEffects
{
    public class ScarsStatusEffect : GameActorEffect
    {
        public static ScarsStatusEffect ScarsGenerator()
        {
            return new()
            {
                OverheadVFX = LoadAsset<GameObject>("scarsvfx"),

                AffectsEnemies = true,
                AffectsPlayers = false,

                stackMode = EffectStackingMode.DarkSoulsAccumulate,
                effectIdentifier = "Scars"
            };
        }

        public override void OnEffectApplied(GameActor actor, RuntimeGameActorEffectData effectData, float partialAmount = 1)
        {
            effectData.accumulator++;

            if (actor.healthHaver)
            {
                actor.healthHaver.Ext().ModifyProjectileDamage += ApplyScarDamageIncrease;

                effectData.OnActorPreDeath += x =>
                {
                    if (actor != null)
                        DestroyVFX(effectData);
                };
                actor.healthHaver.OnPreDeath += effectData.OnActorPreDeath;
            }

            if (effectData.instanceOverheadVFX == null)
                return;

            var counter = ((GameObject)Object.Instantiate(BraveResources.Load("DamagePopupLabel"), GameUIRoot.Instance.transform)).GetComponent<dfLabel>();
            counter.gameObject.SetActive(false);

            counter.Color = Color.white;
            counter.Opacity = 1f;
            counter.Text = IntToStringSansGarbage.GetStringForInt(Mathf.RoundToInt(effectData.accumulator));

            effectData.vfxObjects ??= [];
            effectData.vfxObjects.Add(new(counter.gameObject, 0f));

            var follower = counter.AddComponent<dfFollowObject>();

            follower.followTransform = effectData.instanceOverheadVFX.transform;
            follower.offset = Vector3.right * 0.2f + Vector3.up * 0.9f;
            follower.anchor = dfPivotPoint.MiddleCenter;
            follower.attach = effectData.instanceOverheadVFX.gameObject;

            var dependency = counter.AddComponent<dfParentDependency>();
            dependency.dependsOn = effectData.instanceOverheadVFX.gameObject;

            counter.transform.position = dfFollowObject.ConvertWorldSpaces(effectData.instanceOverheadVFX.transform.position + follower.offset, GameManager.Instance.MainCameraController.Camera, GameUIRoot.Instance.Manager.RenderCamera).WithZ(0f);

            counter.gameObject.SetActive(true);
        }

        public override void OnDarkSoulsAccumulate(GameActor actor, RuntimeGameActorEffectData effectData, float partialAmount = 1, Projectile sourceProjectile = null)
        {
            effectData.accumulator++;

            if (effectData.vfxObjects == null || effectData.vfxObjects.Count <= 0)
                return;

            var obj = effectData.vfxObjects[0];

            if (obj == null || obj.First == null || obj.First.GetComponent<dfLabel>() is not dfLabel label)
                return;

            label.Text = IntToStringSansGarbage.GetStringForInt(Mathf.RoundToInt(effectData.accumulator));
        }

        public override bool IsFinished(GameActor actor, RuntimeGameActorEffectData effectData, float elapsedTime)
        {
            return false;
        }

        public override void EffectTick(GameActor actor, RuntimeGameActorEffectData effectData)
        {
            if (actor != null && actor.healthHaver != null && !actor.healthHaver.IsAlive)
                DestroyVFX(effectData);
        }

        public void ApplyScarDamageIncrease(HealthHaver hh, HealthHaver.ModifyDamageEventArgs args)
        {
            if (args == EventArgs.Empty)
                return;

            if(hh == null)
                return;

            if (!hh.gameActor.TryGetEffectData(this, out var dat))
                return;

            args.ModifiedDamage += Mathf.Max(0f, dat.accumulator);
        }

        public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
        {
            effectData.accumulator = 0f;

            if (actor.healthHaver)
            {
                actor.healthHaver.Ext().ModifyProjectileDamage -= ApplyScarDamageIncrease;
                actor.healthHaver.OnPreDeath -= effectData.OnActorPreDeath;
            }

            DestroyVFX(effectData);

            base.OnEffectRemoved(actor, effectData);
        }

        public void DestroyVFX(RuntimeGameActorEffectData dat)
        {
            if(dat == null || dat.vfxObjects == null)
                return;

            foreach(var vf in dat.vfxObjects)
            {
                if (vf == null || vf.First == null)
                    return;

                Object.Destroy(vf.First);
            }

            dat.vfxObjects.Clear();
        }
    }
}
