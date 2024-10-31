using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrutalItems.Content.Items
{
    public class PrussianBlue : PassiveItem
    {
        public float DamageTime;
        public float Damage;

        public float SynergyDamageUpChance;
        public float SynergyDamageUp;

        public float DamageStack;
        private float Timer;

        public static void Init()
        {
            var name = "Prussian Blue";
            var shortdesc = "...";
            var longdesc = "Slowly damages all enemies while in combat";

            var item = EasyItemInit<PrussianBlue>("prussianblue", name, shortdesc, longdesc, ItemQuality.D);

            item.DamageTime = 1f;
            item.Damage = 1f;

            item.SynergyDamageUpChance = 0.5f;
            item.SynergyDamageUp = 1f;
        }

        public override void Update()
        {
            base.Update();

            if (!PickedUp || Owner == null || !Owner.IsInCombat || Owner.CurrentRoom == null)
            {
                DamageStack = 0f;
                Timer = 0f;

                return;
            }

            Timer += BraveTime.DeltaTime;

            if (Timer < DamageTime)
                return;

            var count = Mathf.FloorToInt(Timer / DamageTime); // to account for really low framerates lol

            var dmg = 0f;
            Timer = 0f;

            if (count <= 0)
                return; // wtf?

            for(int i = 0; i < count; i++)
            {
                dmg += Damage;

                if (!Owner.HasActiveBonusSynergy(CustomSynergyTypeE.PERSISTENCE_OF_TIME))
                    continue;

                dmg += DamageStack;

                if (Random.value <= (SynergyDamageUpChance / Mathf.Max(DamageStack + 1, 1f)))
                    DamageStack += SynergyDamageUp;
            }

            var enm = Owner.CurrentRoom.GetActiveEnemies(Dungeonator.RoomHandler.ActiveEnemyType.All);

            if (enm == null)
                return;

            foreach (var en in enm.ToList())
            {
                if (en == null || en.healthHaver == null || !en.healthHaver.IsAlive)
                    continue;

                en.healthHaver.ApplyDamage(dmg, (en.CenterPosition - Owner.CenterPosition).normalized, Owner.ActorName, CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
            }
        }
    }
}
