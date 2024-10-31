using BrutalItems.Content.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrutalItems.Content.Items
{
    public class Surrender : PassiveItem
    {
        public static void Init()
        {
            var name = "Surrender";
            var shortdesc = "#@&! this";
            var longdesc = "Taking damage will stealth the owner and unseal the current room's doors, allowing them to escape combat. This can't unseal boss rooms.";

            var item = EasyItemInit<Surrender>("surrender", name, shortdesc, longdesc, ItemQuality.D);
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);

            player.OnReceivedDamage += EscapeCombat;
        }

        public void EscapeCombat(PlayerController p)
        {
            p.HandleStealth("Surrender", false);

            if (p.CurrentRoom.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.BOSS || !p.CurrentRoom.IsSealed)
                return;

            p.CurrentRoom.UnsealRoom();

            var resetter = new GameObject("room resetter");
            resetter.transform.position = p.CurrentRoom.GetCenterCell().ToVector2();
            resetter.AddComponent<LeaveRoomResetter>().parentRoom = p.CurrentRoom;
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);

            if (player == null)
                return;

            player.OnReceivedDamage -= EscapeCombat;
        }
    }
}
