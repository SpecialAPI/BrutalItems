using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrutalItems.Tools
{
    public class SynegyBuilder
    {
        public static AdvancedSynergyEntry CreateSynergy(string name, CustomSynergyType synergyType, List<int> mandatoryIds, List<int> optionalIds = null, bool activeWhenGunsUnequipped = true, List<StatModifier> statModifiers = null, bool ignoreLichsEyeBullets = false,
            int numberObjectsRequired = 2, bool suppressVfx = false, bool requiresAtLeastOneGunAndOneItem = false)
        {
            var entry = new AdvancedSynergyEntry();

            var key = $"#SPAPI_{name.ToID().ToUpperInvariant()}";

            entry.NameKey = key;
            ETGMod.Databases.Strings.Synergy.Set(key, name);

            if (mandatoryIds != null)
            {
                foreach (int id in mandatoryIds)
                {
                    var po = PickupObjectDatabase.GetById(id);

                    if (po is Gun)
                        entry.MandatoryGunIDs.Add(id);
                    else if (po is PassiveItem or PlayerItem)
                        entry.MandatoryItemIDs.Add(id);
                }
            }

            if (optionalIds != null)
            {
                foreach (int id in optionalIds)
                {
                    var po = PickupObjectDatabase.GetById(id);

                    if (po is Gun)
                        entry.OptionalGunIDs.Add(id);
                    else if (po is PassiveItem or PlayerItem)
                        entry.OptionalItemIDs.Add(id);
                }
            }

            entry.ActiveWhenGunUnequipped = activeWhenGunsUnequipped;
            entry.IgnoreLichEyeBullets = ignoreLichsEyeBullets;
            entry.NumberObjectsRequired = numberObjectsRequired;
            entry.RequiresAtLeastOneGunAndOneItem = requiresAtLeastOneGunAndOneItem;
            entry.SuppressVFX = suppressVfx;

            entry.statModifiers = statModifiers ?? new();
            entry.bonusSynergies = new() { synergyType };

            addedSynergies.Add(entry);

            return entry;
        }

        public static void AddDualWieldSynergyProcessor(Gun first, Gun second, CustomSynergyType requiredSynergy)
        {
            var p1 = first.gameObject.AddComponent<DualWieldSynergyProcessor>();
            p1.SynergyToCheck = requiredSynergy;
            p1.PartnerGunID = second.PickupObjectId;

            var p2 = second.gameObject.AddComponent<DualWieldSynergyProcessor>();
            p2.SynergyToCheck = requiredSynergy;
            p2.PartnerGunID = first.PickupObjectId;
        }

        public static void AddSynergiesToDB()
        {
            var synergyManager = GameManager.Instance.SynergyManager;

            synergyManager.synergies = [.. synergyManager.synergies, .. addedSynergies];
        }

        public static List<AdvancedSynergyEntry> addedSynergies = new();
    }
}
