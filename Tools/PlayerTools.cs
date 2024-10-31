using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrutalItems.Tools
{
    public static class PlayerTools
    {
        public static void HandleStealth(this PlayerController user, string reason, bool canSteal = true)
        {
            void BreakStealthOnStolen(PlayerController x, ShopItemController x2)
            {
                BreakStealth(x, reason, BreakStealthOnUnstealthy, BreakStealthOnStolen);
            }
            void BreakStealthOnUnstealthy(PlayerController x)
            {
                BreakStealth(x, reason, BreakStealthOnUnstealthy, BreakStealthOnStolen);
            }

            user.ChangeSpecialShaderFlag(1, 1f);
            user.SetIsStealthed(true, reason);

            if(canSteal)
                user.SetCapableOfStealing(true, reason, null);

            user.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider));
            user.PlayEffectOnActor(SmokeBombObject.poofVfx, Vector3.zero, false, true, false);

            user.OnDidUnstealthyAction += BreakStealthOnUnstealthy;
            user.OnItemStolen += BreakStealthOnStolen;
        }
        public static void BreakStealth(PlayerController obj, string reason, Action<PlayerController> unstealthyActionDelegate, Action<PlayerController, ShopItemController> stolenDelegate)
        {
            obj.PlayEffectOnActor(SmokeBombObject.poofVfx, Vector3.zero, false, true, false);

            obj.OnDidUnstealthyAction -= unstealthyActionDelegate;
            obj.OnItemStolen -= stolenDelegate;

            obj.specRigidbody.RemoveCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider));
            obj.ChangeSpecialShaderFlag(1, 0f);

            obj.SetIsStealthed(false, reason);
            obj.SetCapableOfStealing(false, reason, null);

            AkSoundEngine.PostEvent("Play_ENM_wizardred_appear_01", obj.gameObject);
        }
    }
}
