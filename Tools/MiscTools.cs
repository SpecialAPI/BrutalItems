using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrutalItems.Tools
{
    public static class MiscTools
    {
        public static Projectile GetProjectile(this Gun gun, int projectile = 0, int module = 0)
        {
            if(gun == null)
                return null;

            if(module < 0 || projectile < 0 || (gun.Volley == null && module > 0) || (gun.Volley != null && module >= gun.Volley.projectiles.Count))
                return null;

            var mod = gun.Volley != null ? gun.Volley.projectiles[module] : gun.singleModule;

            if(mod == null)
                return null;

            if(mod.chargeProjectiles != null && mod.shootStyle == ProjectileModule.ShootStyle.Charged)
            {
                var i = 0;

                foreach(var ch in mod.chargeProjectiles)
                {
                    if (ch.Projectile == null)
                        continue;

                    if (i == projectile)
                        return ch.Projectile;

                    i++;
                }

                return null;
            }

            else
            {
                if (projectile >= mod.projectiles.Count)
                    return null;

                return mod.projectiles[projectile];
            }
        }

        public static tk2dSprite ProjectileSprite(this GameObject go)
        {
            if(go == null)
                return null;

            var spriteTransform = go.transform.Find("Sprite");

            if(spriteTransform == null)
                return null;

            return spriteTransform.GetComponent<tk2dSprite>();
        }

        public static tk2dSprite ProjectileSprite(this Component comp)
        {
            if(comp == null)
                return null;

            return comp.gameObject.ProjectileSprite();
        }

        public static RuntimeGameActorEffectData GetEffectData(this GameActor actor, GameActorEffect effect)
        {
            if(actor == null || effect == null)
                return null;

            if (actor.m_activeEffects == null || actor.m_activeEffectData == null)
                return null;

            var effectIndex = actor.m_activeEffects.IndexOf(effect);

            if (effectIndex < 0 || effectIndex >= actor.m_activeEffectData.Count)
                return null;

            return actor.m_activeEffectData[effectIndex];
        }

        public static bool TryGetEffectData(this GameActor actor, GameActorEffect effect, out RuntimeGameActorEffectData data)
        {
            return (data = actor.GetEffectData(effect)) != null;
        }
    }
}
