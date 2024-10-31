using Dungeonator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrutalItems.Content.Components
{
    public class LeaveRoomResetter : MonoBehaviour
    {
        public void Start()
        {
            parentRoom = parentRoom ?? transform.position.GetAbsoluteRoom();

            if (parentRoom == null)
                return;

            parentRoom.BecameInvisible += ResetParentRoom;
        }

        public void ResetParentRoom()
        {
            if (parentRoom == null)
                return;

            if (parentRoom.GetActiveEnemiesCount(RoomHandler.ActiveEnemyType.RoomClear) > 0)
                parentRoom.ResetPredefinedRoomLikeDarkSouls();

            parentRoom.BecameInvisible -= ResetParentRoom;
            parentRoom = null;

            Destroy(gameObject);
        }

        public void OnDestroy()
        {
            if (parentRoom == null)
                return;

            parentRoom.BecameInvisible -= ResetParentRoom;
            parentRoom = null;
        }

        public RoomHandler parentRoom;
    }
}
