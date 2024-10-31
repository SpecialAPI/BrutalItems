using BrutalItems.Content.StatusEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrutalItems.Content.Items
{
    public class BoxOfMedals : PassiveItem
    {
        public static ScarsStatusEffect Scars;

        public int ScarsMin;
        public int ScarsMax;

        public float ScarInitialDelay;
        public float ScarDelay;

        public static void Init()
        {
            var name = "Box of Medals";
            var shortdesc = "Unfortunate thing to have extras";
            var longdesc = "When entering combat, applies 3 scars to all enemies.";

            var item = EasyItemInit<BoxOfMedals>("boxofmedals", name, shortdesc, longdesc, ItemQuality.B, null, null);

            item.ScarsMin = 3;
            item.ScarsMax = 3;

            item.ScarInitialDelay = 0.5f;
            item.ScarDelay = 0.15f;

            Scars = ScarsStatusEffect.ScarsGenerator();
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);

            player.OnEnteredCombat += ScarEnemies;
        }

        public void ScarEnemies()
        {
            if (Owner == null || Owner.CurrentRoom == null)
                return;

            var enm = Owner.CurrentRoom.GetActiveEnemies(Dungeonator.RoomHandler.ActiveEnemyType.All);

            if (enm == null || enm.Count <= 0)
                return;

            Owner.StartCoroutine(ScarEnemiesCR(enm.ToList()));
        }

        public IEnumerator ScarEnemiesCR(List<AIActor> enemies)
        {
            yield return new WaitForSeconds(ScarInitialDelay);

            while(enemies.Count > 0)
            {
                var idx = Random.Range(0, enemies.Count);

                var enemy = enemies[idx];
                enemies.RemoveAt(idx);

                if (enemy == null || enemy.healthHaver == null || !enemy.healthHaver.IsAlive)
                    continue;

                var scars = Random.Range(ScarsMin, ScarsMax + 1);

                for (var i = 0; i < scars; i++)
                    enemy.ApplyEffect(Scars);

                yield return new WaitForSeconds(ScarDelay);
            }
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);

            if (player == null)
                return;

            player.OnEnteredCombat -= ScarEnemies;
        }
    }
}
