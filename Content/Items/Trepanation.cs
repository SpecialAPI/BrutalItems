using BrutalItems.Content.StatusEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrutalItems.Content.Items
{
    public class Trepanation : PassiveItem
    {
        public static LinkedStatusEffect Linked;
        public float linkChance;

        public static void Init()
        {
            var name = "Trepanation";
            var shortdesc = "A real head scratcher";
            var longdesc = "Each enemy has a chance to be Linked when entering a new room. Linked enemies share received damage.";

            var item = EasyItemInit<Trepanation>("trepanation", name, shortdesc, longdesc, ItemQuality.A);

            item.linkChance = 0.5f;
            Linked = LinkedStatusEffect.LinkedGerator();
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);

            player.OnEnteredCombat += LinkEnemies;
        }

        public void LinkEnemies()
        {
            if (Owner == null || Owner.CurrentRoom == null)
                return;

            var enemies = Owner.CurrentRoom.GetActiveEnemies(Dungeonator.RoomHandler.ActiveEnemyType.All);

            if (enemies == null)
                return;

            foreach(var enm in enemies)
            {
                if (Random.value >= linkChance)
                    continue;

                enm.ApplyEffect(Linked);
            }
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);

            if (player == null)
                return;

            player.OnEnteredCombat -= LinkEnemies;
        }
    }
}
