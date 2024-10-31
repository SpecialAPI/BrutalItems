using Alexandria.ItemAPI;
using Alexandria.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrutalItems.Content.Items
{
    public class SoggyBandages : PassiveItem
    {
        public float BleedChance;
        public float HeartDisappearTime;
        public float HeartFlashTime;

        public float BleedForceMin;
        public float BleedForceMax;
        public float HeartForceMult;

        public GoopDefinition BloodGoop;
        public GoopDefinition RobotWaterGoop;

        public static void Init()
        {
            var name = "Soggy Bandages";
            var shortdesc = "Not much more we can do";
            var longdesc = "Adds two heart containers. Getting hit has a high chance to consume additional health and turn it into a quickly disappearing heart pickup.";

            var item = EasyItemInit<SoggyBandages>("soggybandages", name, shortdesc, longdesc, ItemQuality.D);

            item.BleedChance = 0.75f;
            item.HeartDisappearTime = 2f;
            item.HeartFlashTime = 0.2f;

            item.BleedForceMin = 7f;
            item.BleedForceMax = 9f;
            item.HeartForceMult = 3f;

            item.BloodGoop = GoopUtility.BloodDef;
            item.RobotWaterGoop = GoopUtility.WaterDef;

            item.AddPassiveStatModifier(PlayerStats.StatType.Health, 2f);
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);

            player.OnReceivedDamage += BleedHealth;
        }

        public void BleedHealth(PlayerController p)
        {
            if (p == null || p.healthHaver == null || (p.healthHaver.GetCurrentHealth() <= 0.5f && p.characterIdentity != PlayableCharacters.Robot))
                return;

            if (Random.value > BleedChance)
                return;

            var bleedDir = Random.insideUnitCircle;
            var bleedForce = Random.Range(BleedForceMin, BleedForceMax);

            var goop = p.characterIdentity == PlayableCharacters.Robot ? RobotWaterGoop : BloodGoop;

            if(p.healthHaver.GetCurrentHealth() > 0.5f)
            {
                p.healthHaver.ForceSetCurrentHealth(p.healthHaver.GetCurrentHealth() - 0.5f);

                var h = LootEngine.SpawnItem(HalfHeartObject.gameObject, p.CenterPosition, bleedDir * HeartForceMult, bleedForce, true);
                h.StartCoroutine(HandleHeartDisappear(h.gameObject));
            }

            DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(goop).TimedAddGoopLine(p.CenterPosition, p.CenterPosition + (bleedDir * bleedForce), 1f, 0.75f);
        }

        public IEnumerator HandleHeartDisappear(GameObject go)
        {
            var renderer = go.GetComponent<Renderer>();

            for(float ela = 0f; ela < HeartDisappearTime; ela += BraveTime.DeltaTime)
            {
                if (renderer)
                    renderer.enabled = ela % (HeartFlashTime * 2f) <= HeartFlashTime;

                yield return null;
            }

            Destroy(go);
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);

            if (player == null)
                return;

            player.OnReceivedDamage -= BleedHealth;
        }
    }
}
