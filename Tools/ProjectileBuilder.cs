using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrutalItems.Tools
{
    public static class ProjectileBuilder
    {
        public static T SetupBasicProjectileComponents<T>(GameObject obj) where T : Projectile
        {
            var body = obj.AddComponent<SpeculativeRigidbody>();
            body.TK2DSprite = obj.GetComponentInChildren<tk2dSprite>();

            body.PixelColliders = new()
            {
                new()
                {
                    ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Tk2dPolygon,
                    CollisionLayer = CollisionLayer.Projectile,
                    Sprite = body.TK2DSprite
                }
            };

            var proj = obj.AddComponent<T>();
            proj.baseData = new ProjectileData();
            proj.persistTime = 0f;
            proj.AdditionalBurstLimits = [];

            return proj;
        }

        public static Projectile EasyProjectileInit(string assetPath, float damage, float speed, float range, float knockback, bool shouldRotate, bool ignoreDamageCaps, bool pierceMinorBreakables, int? overrideSpriteId = null, tk2dSpriteCollectionData overrideSpriteCollection = null, tk2dBaseSprite.Anchor anchor = tk2dBaseSprite.Anchor.MiddleCenter, int? overrideColliderPixelWidth = null, int? overrideColliderPixelHeight = null, int? overrideColliderOffsetX = null, int? overrideColliderOffsetY = null)
        {
            return EasyProjectileInit<Projectile>(assetPath, damage, speed, range, knockback, shouldRotate, ignoreDamageCaps, pierceMinorBreakables, overrideSpriteId, overrideSpriteCollection, anchor, overrideColliderPixelWidth, overrideColliderPixelHeight, overrideColliderOffsetX, overrideColliderOffsetY);
        }

        public static T EasyProjectileInit<T>(string assetPath, float damage, float speed, float range, float knockback, bool shouldRotate, bool ignoreDamageCaps, bool pierceMinorBreakables, int? overrideSpriteId = null, tk2dSpriteCollectionData overrideSpriteCollection = null, tk2dBaseSprite.Anchor anchor = tk2dBaseSprite.Anchor.MiddleCenter, int? overrideColliderPixelWidth = null, int? overrideColliderPixelHeight = null, int? overrideColliderOffsetX = null, int? overrideColliderOffsetY = null) where T : Projectile
        {
            var proj = SetupBasicProjectileComponents<T>(LoadAsset<GameObject>(assetPath));
            var sprite = proj.ProjectileSprite();

            if (overrideSpriteId != null && sprite != null)
                sprite.SetSprite(overrideSpriteCollection ?? ETGMod.Databases.Items.ProjectileCollection, overrideSpriteId.Value);

            else if (sprite == null)
            {
                var xOffset = ((int)anchor % 3) switch
                {
                    1 => -(overrideColliderPixelHeight.GetValueOrDefault() / 2),
                    2 => -overrideColliderPixelHeight.GetValueOrDefault(),

                    _ => 0
                };

                var yOffset = ((int)anchor / 3) switch
                {
                    1 => -(overrideColliderPixelHeight.GetValueOrDefault() / 2),
                    2 => -overrideColliderPixelHeight.GetValueOrDefault(),

                    _ => 0
                };

                proj.specRigidbody.PixelColliders = new List<PixelCollider>()
                {
                    new()
                    {
                        ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                        CollisionLayer = CollisionLayer.Projectile,

                        ManualHeight = overrideColliderPixelHeight.GetValueOrDefault(),
                        ManualWidth = overrideColliderPixelWidth.GetValueOrDefault(),
                        ManualOffsetX = overrideColliderOffsetX.GetValueOrDefault() + xOffset,
                        ManualOffsetY = overrideColliderOffsetY.GetValueOrDefault() + yOffset
                    }
                };
            }

            proj.baseData.damage = damage;
            proj.baseData.speed = speed;
            proj.baseData.range = range;
            proj.baseData.force = knockback;

            proj.shouldRotate = shouldRotate;
            proj.shouldFlipVertically = true;
            proj.ignoreDamageCaps = ignoreDamageCaps;
            proj.pierceMinorBreakables = pierceMinorBreakables;

            return proj;
        }
    }
}
