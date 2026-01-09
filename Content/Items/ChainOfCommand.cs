using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrutalItems.Content.Items
{
    public class ChainOfCommand : PassiveItem
    {
        public float bigEnemyDamageMult;
        public float bigEnemyAreaThreshold;

        public static void Init()
        {
            var name = "Chain of Command";
            var shortdesc = "Kingslayer";
            var longdesc = "Bullets deal more damage to big enemies.";

            var item = EasyItemInit<ChainOfCommand>("chainofcommand", name, shortdesc, longdesc, ItemQuality.A, null, null);

            item.bigEnemyDamageMult = 1.4f;
            item.bigEnemyAreaThreshold = 4.5f;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);

            player.PostProcessProjectile += PostProcessProjectile;
        }

        public void PostProcessProjectile(Projectile proj, float f)
        {
            proj.Ext().ModifyDealtDamage += SmallEnemyDamage;
        }

        public void SmallEnemyDamage(Projectile proj, HealthHaver hh, HealthHaver.ModifyDamageEventArgs args)
        {
            if (args == EventArgs.Empty)
                return;

            if (hh == null || hh.specRigidbody == null)
                return;

            var collider = hh.specRigidbody.HitboxPixelCollider;

            if (collider == null)
                return;

            var area = collider.UnitWidth * collider.UnitHeight;

            if (area <= bigEnemyAreaThreshold)
                return;

            var mult = bigEnemyDamageMult;

            if (Owner.HasActiveBonusSynergy(CustomSynergyTypeE.STANDARD_DEVIATION))
            {
                mult *= (Mathf.Sqrt(Mathf.Abs(area - bigEnemyAreaThreshold)) / 12) - 1;
            }

            args.ModifiedDamage *= mult;

            if(hh.gameActor != null)
                UITools.DoJumpingText("Crit!", hh.gameActor.CenterPosition, hh.specRigidbody.UnitCenter.y - hh.specRigidbody.UnitBottomCenter.y, Color.white);
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
