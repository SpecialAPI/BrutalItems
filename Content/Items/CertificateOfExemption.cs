using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrutalItems.Content.Items
{
    public class CertificateOfExemption : PassiveItem
    {
        public static void Init()
        {
            var name = "Certificate of Exemption";
            var shortdesc = "Dodge more than the Draft";
            var longdesc = "While held, guns consume no ammo. This item is destroyed and spawn ammo when the owner takes any damage.";

            var item = EasyItemInit<CertificateOfExemption>("certificateofexemption", name, shortdesc, longdesc, ItemQuality.C);
        }

        public override void Pickup(PlayerController player)
        {
            player.stats.AdditionalVolleyModifiers += PreventAmmoConsumption;
            player.OnReceivedDamage += BreakOnDamage;

            base.Pickup(player);
        }

        public void PreventAmmoConsumption(ProjectileVolleyData volley)
        {
            foreach (var mod in volley.projectiles)
                mod.ammoCost = 0;
        }

        public void BreakOnDamage(PlayerController p)
        {
            if (p.HasActiveBonusSynergy(CustomSynergyTypeE.GLASS_CASE))
                return;

            LootEngine.SpawnItem(AmmoObject.gameObject, p.CenterPosition, Random.insideUnitCircle, 4f);

            var obj = p.DropPassiveItem(this);
            Destroy(obj.gameObject, 1f);

            if (obj == null)
                return;

            var passive = obj.GetComponent<PassiveItem>();

            if (passive == null)
                return;

            passive.m_pickedUp = true;
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);

            if (player == null)
                return;

            player.stats.AdditionalVolleyModifiers -= PreventAmmoConsumption;
            player.OnReceivedDamage -= BreakOnDamage;
        }
    }
}
