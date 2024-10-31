using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrutalItems.Content.Items
{
    public class EsotericArtifact : PlayerItem
    {
        public static void Init()
        {
            var name = "Esoteric Artifact";
            var shortdesc = "Moving pictures dance within";
            var longdesc = "On use, instantly kills all enemies in the current room and removes all further waves. Enemies killed by this don't drop any loot. Doesn't work on bosses.";

            var item = EasyItemInit<EsotericArtifact>("EsotericArtifact", name, shortdesc, longdesc, ItemQuality.C);

            item.consumable = true;
            item.numberOfUses = 5;
        }

        public override void DoEffect(PlayerController user)
        {
            var room = user.CurrentRoom;

            if (room.remainingReinforcementLayers != null && room.remainingReinforcementLayers.Count > 0)
            {
                room.ClearReinforcementLayers();
                room.m_hasGivenReward = true;
            }

            var enm = user.CurrentRoom.GetActiveEnemies(Dungeonator.RoomHandler.ActiveEnemyType.All);
            if(enm != null)
            {
                foreach(var en in enm.ToList())
                {
                    if (en == null || en.healthHaver == null || !en.healthHaver.IsAlive)
                        continue;

                    if (en.healthHaver.IsBoss && !en.healthHaver.IsSubboss)
                        continue;

                    LootEngine.DoDefaultPurplePoof(en.CenterPosition);

                    room.m_hasGivenReward = true;
                    en.EraseFromExistence();
                }
            }
        }

        public override bool CanBeUsed(PlayerController user)
        {
            if(user == null || user.CurrentRoom == null)
                return false;

            if(user.CurrentRoom.area != null && user.CurrentRoom.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.BOSS && user.CurrentRoom.area.PrototypeRoomBossSubcategory == PrototypeDungeonRoom.RoomBossSubCategory.FLOOR_BOSS)
                return false;

            var enm = user.CurrentRoom.GetActiveEnemies(Dungeonator.RoomHandler.ActiveEnemyType.All);

            if ((enm == null || enm.Count <= 0) && (user.CurrentRoom.remainingReinforcementLayers == null || user.CurrentRoom.remainingReinforcementLayers.Count > 0))
                return false;

            foreach(var en in enm)
            {
                if (en == null || en.healthHaver == null)
                    continue;

                if (en.healthHaver.IsBoss && !en.healthHaver.IsSubboss)
                    return false;
            }

            return true;
        }
    }
}
