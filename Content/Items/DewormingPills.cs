using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrutalItems.Content.Items
{
    public class DewormingPills : PassiveItem
    {
        public float smallEnemyDamageMult;
        public float smallEnemyAreaThreshold;

        public static void Init()
        {
            var name = "Deworming Pills";
            var shortdesc = "No more little friends";
            var longdesc = "Bullets deal more damage to small enemies.";

            var item = EasyItemInit<DewormingPills>("dewormingpills", name, shortdesc, longdesc, ItemQuality.C);

            item.smallEnemyDamageMult = 1.4f;
            item.smallEnemyAreaThreshold = 3.5f;
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

            if (area > smallEnemyAreaThreshold)
                return;

            var mult = smallEnemyDamageMult;

            if (Owner.HasActiveBonusSynergy(CustomSynergyTypeE.STANDARD_DEVIATION))
            {
                mult *= (Mathf.Sqrt(Mathf.Abs(area - smallEnemyAreaThreshold)) / 4) - 1;
            }

            args.ModifiedDamage *= mult;

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
