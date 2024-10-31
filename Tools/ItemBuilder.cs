using Alexandria.ItemAPI;
using Gungeon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrutalItems.Tools
{
    public static class ItemBuilder
    {
        public static T EasyItemInit<T>(string objectPath, string itemName, string itemShortDesc, string itemLongDesc, PickupObject.ItemQuality quality, int? ammonomiconPlacement = null, string overrideConsoleID = null) where T : PickupObject
        {
            var go = LoadAsset<GameObject>(objectPath);
            var item = go.AddComponent<T>();

            SetupItem(item, itemName, itemShortDesc, itemLongDesc, MOD_PREFIX, overrideConsoleID);
            item.quality = quality;

            if (ammonomiconPlacement != null)
                item.PlaceItemInAmmonomiconAfterItemById(ammonomiconPlacement.Value);

            if (quality == PickupObject.ItemQuality.SPECIAL || quality == PickupObject.ItemQuality.EXCLUDED)
            {
                var t = GameManager.Instance.RewardManager.ItemsLootTable.defaultItemDrops.elements;

                t.RemoveAt(t.Count - 1);
            }

            return item;
        }

        public static void SetupItem(PickupObject item, string name, string shortDesc, string longDesc, string idPool, string overrideConsoleId = null)
        {
            var oldName = item.name;
            item.gameObject.name = $"{MOD_PREFIX}_{oldName}";

            ETGMod.Databases.Items.SetupItem(item, item.name);

            SpriteBuilder.AddToAmmonomicon(item.sprite.GetCurrentSpriteDef());
            item.encounterTrackable.journalData.AmmonomiconSprite = item.sprite.GetCurrentSpriteDef().name;

            item.SetName(name);
            item.SetShortDescription(shortDesc);
            item.SetLongDescription(longDesc);

            if (item is PlayerItem a)
                a.consumable = false;

            var consoleName = string.IsNullOrEmpty(overrideConsoleId) ? name.ToID() : overrideConsoleId;
            var consoleId = string.IsNullOrEmpty(idPool) ? consoleName : $"{idPool}:{consoleName}";

            Game.Items.Add(consoleId, item);
            ETGMod.Databases.Items.AddSpecific(false, item);

            var dictname = oldName.ToLowerInvariant();

            Ids[dictname] = item.PickupObjectId;
            Items[dictname] = item;

            if(item is PassiveItem passive)
                Passives[dictname] = passive;

            else if(item is PlayerItem active)
                Actives[dictname] = active;
        }

        public static PickupObject PlaceItemInAmmonomiconAfterItemById(this PickupObject item, int id)
        {
            item.ForcedPositionInAmmonomicon = PickupObjectDatabase.GetById(id).ForcedPositionInAmmonomicon;
            return item;
        }

        public static readonly Dictionary<string, int> Ids = [];
        public static readonly Dictionary<string, PickupObject> Items = [];
        public static readonly Dictionary<string, PassiveItem> Passives = [];
        public static readonly Dictionary<string, PlayerItem> Actives = [];
    }
}
