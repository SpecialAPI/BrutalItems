using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrutalItems.Content.Components
{
    public class ModernMedicineInheritedProjectile : InheritedReplacementProjectile
    {
        public float inheritenceDamageMult = 1f;
        public float inheritenceKnockbackMult = 1f;

        public override void InheritProjectile(Projectile original)
        {
            projectile.baseData.damage = original.baseData.damage * inheritenceDamageMult;
            projectile.baseData.force = original.baseData.force * inheritenceKnockbackMult;
        }
    }
}
