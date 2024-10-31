using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace BrutalItems.Tools
{
    public static class BundleLoader
    {
        public static AssetBundle Bundle;
        public static HashSet<Object> LoadedAssets = [];

        public static void LoadBundle(string name)
        {
            var asmbl = Assembly.GetExecutingAssembly();
            var platform = Application.platform switch
            {
                RuntimePlatform.LinuxPlayer => "linux",
                RuntimePlatform.OSXPlayer => "macos",
                _ => "windows"
            };

            var ending = $".{platform}.{name.ToLowerInvariant()}";

            foreach(var pth in asmbl.GetManifestResourceNames())
            {
                if (!pth.ToLowerInvariant().EndsWith(ending))
                    continue;

                using var strem = asmbl.GetManifestResourceStream(pth);

                try
                {
                    Bundle = AssetBundle.LoadFromStream(strem);
                    break;
                }
                catch
                {
                    Debug.LogWarning($"Failed loading bundle {pth}");
                }
            }

            if(Bundle == null)
            {
                Debug.LogError("No bundle was found");
                return;
            }

            var processCollections = new HashSet<tk2dSpriteCollectionData>();
           
            foreach(var obj in Bundle.LoadAllAssets<GameObject>())
            {
                if(obj == null)
                    continue;

                LoadedAssets.Add(obj);

                foreach(var coll in obj.GetComponentsInChildren<tk2dSpriteCollectionData>())
                    processCollections.Add(coll);

                foreach(var spr in obj.GetComponentsInChildren<tk2dSprite>())
                {
                    if(spr.Collection == null)
                        continue;

                    processCollections.Add(spr.Collection);
                }
            }

            foreach(var coll in processCollections)
            {
                LoadedAssets.Add(coll);

                if(coll.materials == null)
                    continue;

                foreach(var mat in coll.materials)
                {
                    if(mat == null || mat.shader == null || !mat.shader.name.Contains("BR_"))
                        continue;

                    var gungeonName = mat.shader.name.Replace("BR_", "");
                    var gungeonShader = ShaderCache.Acquire(gungeonName);

                    if (gungeonShader == null && gungeonName == "Brave/Enemy Projectile Emissive")
                        gungeonShader = MagnumObject.GetProjectile().ProjectileSprite().CurrentSprite.material.shader;

                    if(gungeonShader == null)
                    {
                        Debug.LogError($"Failed getting replacement gungeon shader: {gungeonName}");
                        continue;
                    }

                    mat.shader = gungeonShader;
                }
            }
        }

        public static T LoadAsset<T>(string name) where T : Object
        {
            if (Bundle == null)
                return null;

            var res = Bundle.LoadAsset<T>(name);

            if (res != null)
                LoadedAssets.Add(res);

            return res;
        }
    }
}
