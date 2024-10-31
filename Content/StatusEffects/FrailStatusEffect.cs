using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrutalItems.Content.StatusEffects
{
    public class FrailStatusEffect : GameActorEffect
    {
        public float tickDownOnDamage;

        public static FrailStatusEffect FrailGenerator(float duration = 3f, float tickDown = 3f)
        {
            return new()
            {
                OverheadVFX = LoadAsset<GameObject>("frailvfx"),

                AffectsEnemies = true,
                AffectsPlayers = true,

                stackMode = EffectStackingMode.Stack,
                effectIdentifier = "Frail",
                duration = duration,

                tickDownOnDamage = tickDown
            };
        }

        public override void OnEffectApplied(GameActor actor, RuntimeGameActorEffectData effectData, float partialAmount = 1)
        {
            if (actor == null || actor.healthHaver == null)
                return;

            actor.healthHaver.Ext().ModifyProjectileDamage += ModifyDamage;
        }

        public void ModifyDamage(HealthHaver hh, HealthHaver.ModifyDamageEventArgs args)
        {
            if (args == EventArgs.Empty)
                return;

            args.ModifiedDamage *= 2f;

            if (hh == null || hh.gameActor == null)
                return;

            if(hh.specRigidbody != null)
                UITools.DoJumpingText("Frail!", hh.gameActor.CenterPosition, hh.specRigidbody.UnitCenter.y - hh.specRigidbody.UnitBottomCenter.y, Color.white);

            if (!hh.gameActor.TryGetEffectData(this, out var dat))
                return;

            dat.elapsed += tickDownOnDamage;
        }

        public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
        {
            if (actor == null || actor.healthHaver == null)
                return;

            actor.healthHaver.Ext().ModifyProjectileDamage -= ModifyDamage;
        }
    }
}
