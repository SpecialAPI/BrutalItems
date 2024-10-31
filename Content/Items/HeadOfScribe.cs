using BrutalItems.Content.StatusEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.SocialPlatforms.Impl;

namespace BrutalItems.Content.Items
{
    public class HeadOfScribe : PassiveItem
    {
        public static FrailStatusEffect Frail;

        public float FrailInitialDelay;
        public float FrailDelay;

        public static void Init()
        {
            var name = "Head of Scribe";
            var shortdesc = "Heat exhaustion is bad...";
            var longdesc = "When entering combat, applies Frail all enemies. Frail doubles received damage from projectiles, but depletes very quickly.";

            var item = EasyItemInit<HeadOfScribe>("headofscribe", name, shortdesc, longdesc, ItemQuality.B);

            item.FrailInitialDelay = 0.5f;
            item.FrailDelay = 0.15f;

            Frail = FrailStatusEffect.FrailGenerator();
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);

            player.OnEnteredCombat += FrailEnemies;
        }

        public void FrailEnemies()
        {
            if (Owner == null || Owner.CurrentRoom == null)
                return;

            var enm = Owner.CurrentRoom.GetActiveEnemies(Dungeonator.RoomHandler.ActiveEnemyType.All);

            if (enm == null || enm.Count <= 0)
                return;

            Owner.StartCoroutine(FrailEnemiesCR(enm.ToList()));
        }

        public IEnumerator FrailEnemiesCR(List<AIActor> enemies)
        {
            yield return new WaitForSeconds(FrailInitialDelay);

            while (enemies.Count > 0)
            {
                var idx = Random.Range(0, enemies.Count);

                var enemy = enemies[idx];
                enemies.RemoveAt(idx);

                if (enemy == null || enemy.healthHaver == null || !enemy.healthHaver.IsAlive)
                    continue;

                enemy.ApplyEffect(Frail);
                yield return new WaitForSeconds(FrailDelay);
            }
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);

            if (player == null)
                return;

            player.OnEnteredCombat -= FrailEnemies;
        }
    }
}
