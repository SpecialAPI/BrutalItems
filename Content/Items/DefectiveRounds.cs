using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrutalItems.Content.Items
{
    public class DefectiveRounds
    {
        public static void Init()
        {
            var name = "Defective Rounds";
            var shortdesc = "They're doing their best";
            var longdesc = "Shooting will sometimes shoot random additional projectiles.";

            var item = EasyItemInit<GunVolleyModificationItem>("defectiverounds", name, shortdesc, longdesc, PickupObject.ItemQuality.C);

            item.AddsModule = true;
            item.ModuleToAdd = new()
            {
                numberOfShotsInClip = -1,
                shootStyle = ProjectileModule.ShootStyle.Automatic,
                ammoCost = 0,
                ignoredForReloadPurposes = true,

                projectiles = new()
                {
                    MagnumObject.GetProjectile(),
                    M1911Object.GetProjectile(),
                    RustySidearmObject.GetProjectile(),
                    WinchesterRifleObject.GetProjectile(),
                    ThompsonObject.GetProjectile(),
                    ShellegunObject.GetProjectile(),
                    TheJudgeObject.GetProjectile(),
                    DirectionalPadObject.GetProjectile(),
                },
                angleVariance = 50f,
                cooldownTime = 0.7f
            };
        }
    }
}
