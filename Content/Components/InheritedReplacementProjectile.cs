using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrutalItems.Content.Components
{
    [HarmonyPatch]
    public abstract class InheritedReplacementProjectile : BraveBehaviour
    {
        public static MethodInfo iop_slsi = AccessTools.Method(typeof(InheritedReplacementProjectile), nameof(InheritOriginalProjectile_SetLastShotIndex));
        public static MethodInfo iop_so = AccessTools.Method(typeof(InheritedReplacementProjectile), nameof(InheritOriginalProjectile_SaveOriginal));
        public static MethodInfo iop_i = AccessTools.Method(typeof(InheritedReplacementProjectile), nameof(InheritOriginalProjectile_Inherit));

        [HarmonyPatch(typeof(Gun), nameof(Gun.ShootSingleProjectile))]
        [HarmonyILManipulator]
        public static void InheritOriginalProjectile_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpBeforeNext(x => x.MatchCallOrCallvirt<Object>("op_Implicit"), 2))
                return;

            var savedProjectileLoc = crs.DeclareLocal<Projectile>();

            crs.Emit(OpCodes.Ldloca_S, savedProjectileLoc);
            crs.Emit(OpCodes.Call, iop_so);

            if (!crs.JumpBeforeNext(x => x.MatchLdfld<PlayerController>(nameof(PlayerController.OnPreFireProjectileModifier))))
                return;

            crs.Emit(OpCodes.Ldarg_0);
            crs.Emit(OpCodes.Ldarg_1);
            crs.Emit(OpCodes.Call, iop_slsi);

            if (!crs.JumpBeforeNext(x => x.MatchLdfld<Projectile>(nameof(Projectile.baseData))))
                return;

            crs.Emit(OpCodes.Ldloc_S, savedProjectileLoc);
            crs.Emit(OpCodes.Call, iop_i);
        }

        public static PlayerController InheritOriginalProjectile_SetLastShotIndex(PlayerController player, Gun g, ProjectileModule mod)
        {
            if(g == null || g.RuntimeModuleData == null)
                return player;

            if (!g.RuntimeModuleData.TryGetValue(mod, out var dat))
                return player;

            g.LastShotIndex = dat.numberShotsFired;

            return player;
        }

        public static Projectile InheritOriginalProjectile_SaveOriginal(Projectile proj, out Projectile saveProj)
        {
            saveProj = proj;

            return proj;
        }

        public static Projectile InheritOriginalProjectile_Inherit(Projectile proj, Projectile original)
        {
            if (proj == null)
                return proj;

            var inheriter = proj.GetComponent<InheritedReplacementProjectile>();

            if(inheriter != null && original != null)
                inheriter.InheritProjectile(original);

            return proj;
        }

        public abstract void InheritProjectile(Projectile original);
    }
}
