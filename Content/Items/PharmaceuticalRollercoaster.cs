using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrutalItems.Content.Items
{
    public class PharmaceuticalRollercoaster : PassiveItem
    {
        public float positiveDamageMult = 1f;
        public Color positiveDamageColor;

        public float negativeDamageMult = 1f;
        public Color negativeDamageColor;

        public static void Init()
        {
            var name = "Pharmaceutical Roller Coaster";
            var shortdesc = "Up and down and up and...";
            var longdesc = "Bullets have an equal to chance to deal either less or more damage.";

            var item = EasyItemInit<PharmaceuticalRollercoaster>("pharmaceuticalrollercoaster", name, shortdesc, longdesc, ItemQuality.D);

            item.positiveDamageMult = 1.5f;
            item.positiveDamageColor = Color.blue;

            item.negativeDamageMult = 0.5f;
            item.negativeDamageColor = Color.red;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);

            player.PostProcessProjectile += RandomDamage;
        }

        public void RandomDamage(Projectile proj, float f)
        {
            var positive = BraveUtility.RandomBool();

            proj.baseData.damage *= positive ? positiveDamageMult : negativeDamageMult;
            proj.AdjustPlayerProjectileTint(positive ? positiveDamageColor : negativeDamageColor, 0);
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);

            if (player == null)
                return;

            player.PostProcessProjectile -= RandomDamage;
        }
    }
}
