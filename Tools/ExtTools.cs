using BrutalItems.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrutalItems.Tools
{
    public static class ExtTools
    {
        public static T AddComponent<T>(this Component c) where T : Component
        {
            return c.gameObject.AddComponent<T>();
        }

        public static T GetOrAddComponent<T>(this Component c) where T : Component
        {
            return c.gameObject.GetOrAddComponent<T>();
        }

        public static HealthHaverExt Ext(this HealthHaver hh)
        {
            if (hh == null)
                return null;

            return hh.GetOrAddComponent<HealthHaverExt>();
        }

        public static ProjectileExt Ext(this Projectile proj)
        {
            if (proj == null)
                return null;

            return proj.GetOrAddComponent<ProjectileExt>();
        }
    }
}
