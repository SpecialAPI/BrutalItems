using BrutalItems.Content.StatusEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrutalItems.Content.Items
{
    public class DumDum : PassiveItem
    {
        public static ScarsStatusEffect Scars;

        public float scarsApplyChance;
        public Color tintColor;

        public static void Init()
        {
            var name = "Dum-Dum";
            var shortdesc = "Hollow Point Pain";
            var longdesc = "Shots have a chance to apply a Scar to enemies. Each Scar increases the damage dealt to enemies by projectiles by 1.";

            var item = EasyItemInit<DumDum>("dumdum", name, shortdesc, longdesc, ItemQuality.B);

            item.scarsApplyChance = 0.1f;
            item.tintColor = Color.red;

            Scars = ScarsStatusEffect.ScarsGenerator();
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);

            player.PostProcessProjectile += PostProcessProjectile;
        }

        public void PostProcessProjectile(Projectile proj, float f)
        {
            if (Random.value >= scarsApplyChance * f)
                return;

            proj.statusEffectsToApply.Add(Scars);
            proj.AdjustPlayerProjectileTint(tintColor, 0);
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);

            if (player == null)
                return;

            player.PostProcessProjectile -= PostProcessProjectile;
        }
    }
}
