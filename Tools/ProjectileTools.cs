using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrutalItems.Tools
{
    public static class ProjectileTools
    {
        public static HomingModifier GetOrAddHoming(this Projectile proj)
        {
            if (proj == null)
                return null;

            var homing = proj.GetComponent<HomingModifier>();

            if(homing != null)
                return homing;

            homing = proj.AddComponent<HomingModifier>();

            homing.HomingRadius = 0f;
            homing.AngularVelocity = 0f;

            return homing;
        }
    }
}
