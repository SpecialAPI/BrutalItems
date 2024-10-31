using Alexandria.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrutalItems.Content.Items
{
    public class BarelyUsedGauze : PassiveItem
    {
        public static void Init()
        {
            var name = "Barely Used Gauze";
            var shortdesc = "This was a lifesaver";
            var longdesc = "Adds one empty heart container.";

            var item = EasyItemInit<BarelyUsedGauze>("barelyusedgauze", name, shortdesc, longdesc, PickupObject.ItemQuality.D);

            item.AddPassiveStatModifier(PlayerStats.StatType.Health, 1f);
        }

        public override void Pickup(PlayerController player)
        {
            HasBeenStatProcessed = true;
            base.Pickup(player);
        }
    }
}
