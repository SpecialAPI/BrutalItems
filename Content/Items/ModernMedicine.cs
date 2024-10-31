using BrutalItems.Content.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrutalItems.Content.Items
{
    public class ModernMedicine : PassiveItem
    {
        public Projectile ReplacementProjectile;
        public Projectile SynergyReplacementProjectile;

        public static void Init()
        {
            var name = "Modern Medicine";
            var shortdesc = "The cutting edge of medicine";
            var longdesc = "All bullets are replaced by basic bullets that deal more damage but don't have any special effects.";

            var item = EasyItemInit<ModernMedicine>("modernmedicine", name, shortdesc, longdesc, ItemQuality.A);

            var sprite = MagnumObject.GetProjectile().ProjectileSprite();
            var proj = EasyProjectileInit<Projectile>("lobotomyprojectile", 10f, 23f, 20f, 15f, false, false, false, sprite.spriteId, sprite.Collection);

            proj.hitEffects = MagnumObject.GetProjectile().hitEffects;

            var inheriter = proj.AddComponent<ModernMedicineInheritedProjectile>();
            inheriter.inheritenceDamageMult = 1.45f;
            inheriter.inheritenceKnockbackMult = 1f;

            var synergyProj = EasyProjectileInit<Projectile>("synergylobotomyprojectile", 10f, 30f, 1000f, 15f, true, false, true);

            synergyProj.hitEffects = MagnumObject.GetProjectile().hitEffects;

            var synergyInheriter = synergyProj.AddComponent<ModernMedicineInheritedProjectile>();
            synergyInheriter.inheritenceDamageMult = 1.45f;
            synergyInheriter.inheritenceKnockbackMult = 1f;

            var pierce = synergyProj.GetOrAddComponent<PierceProjModifier>();
            pierce.penetration = 1;
            pierce.penetratesBreakables = true;

            synergyProj.AppliesStun = true;
            synergyProj.StunApplyChance = 1f;
            synergyProj.AppliedStunDuration = 1f;

            item.ReplacementProjectile = proj;
            item.SynergyReplacementProjectile = synergyProj;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);

            player.OnPreFireProjectileModifier += ReplaceProjectile;
        }

        public Projectile ReplaceProjectile(Gun g, Projectile p)
        {
            if (g.LastShotIndex == 0 && Owner.HasActiveBonusSynergy(CustomSynergyTypeE.FIRST_COME_FIRST_SERVE))
                return SynergyReplacementProjectile;

            return ReplacementProjectile;
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);

            if (player == null)
                return;

            player.OnPreFireProjectileModifier -= ReplaceProjectile;
        }
    }
}
