using BrutalItems.Content.StatusEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrutalItems.Content.Items
{
    public class ShardOfNowak : PassiveItem
    {
        public static FocusStatusEffect Focus;
        public static FocusStatusEffect SynergyFocus;

        public float StressfulDamageMult;
        public float SynergyHomingRadius;
        public float SynergyHomingVelocity;

        public static void Init()
        {
            var name = "Shard of Nowak";
            var shortdesc = "So that's where this was";
            var longdesc = "Applies Focus to the owner when entering combat. Focus significantly increases damage, but is removed when getting hit or standing too close to bullets.";

            var item = EasyItemInit<ShardOfNowak>("shardofnowak", name, shortdesc, longdesc, ItemQuality.A);

            Focus = FocusStatusEffect.FocusGenerator();

            SynergyFocus = FocusStatusEffect.FocusGenerator(
                duration: 0.3f,
                radius: 1.8f);

            item.StressfulDamageMult = 1.25f;
            item.SynergyHomingRadius = 8f;
            item.SynergyHomingVelocity = 375f;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);

            player.OnEnteredCombat += FocusPlayer;
            player.PostProcessProjectile += ProcessFocusedSynergies;
        }

        public void ProcessFocusedSynergies(Projectile proj, float f)
        {
            if (Owner.HasActiveBonusSynergy(CustomSynergyTypeE.STRESSFUL_FOCUS))
                proj.Ext().ModifyDealtDamage += StressfulDamageUp;

            if (Owner.HasActiveBonusSynergy(CustomSynergyTypeE.FOCUSED_FURY))
            {
                var homing = proj.GetOrAddComponent<HomingModifier>();

                homing.HomingRadius += SynergyHomingRadius;
                homing.AngularVelocity += SynergyHomingVelocity;
            }
        }

        public void StressfulDamageUp(Projectile p, HealthHaver hh, HealthHaver.ModifyDamageEventArgs args)
        {
            if (this == null || Owner == null)
                return;

            if (hh == null || Owner.healthHaver == null)
                return;

            if (Owner.GetEffect("Focus") == null)
                return;

            var ownerPerc = Owner.healthHaver.GetCurrentHealthPercentage();

            if (Owner.healthHaver.NextShotKills)
                ownerPerc = Mathf.Min(ownerPerc, 0f);

            if (hh.GetCurrentHealthPercentage() <= ownerPerc)
                return;

            args.ModifiedDamage *= StressfulDamageMult;
            UITools.DoJumpingText("Crit!", hh.gameActor.CenterPosition, hh.specRigidbody.UnitCenter.y - hh.specRigidbody.UnitBottomCenter.y, Color.white);
        }

        public void FocusPlayer()
        {
            var focus = Focus;

            if (Owner.HasActiveBonusSynergy(CustomSynergyTypeE.HIGH_FOCUS))
                focus = SynergyFocus;

            Owner.ApplyEffect(focus);
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);

            if (player == null)
                return;

            player.OnEnteredCombat -= FocusPlayer;
            player.PostProcessProjectile -= ProcessFocusedSynergies;
        }
    }
}
