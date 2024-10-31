using Alexandria.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrutalItems.Tools
{
    public static class AutoEnumExtension
    {
        public static void AutoExtendEnums()
        {
            var asm = Assembly.GetCallingAssembly();

            foreach (var type in asm.GetTypes())
            {
                var custom = type.GetCustomAttributes(false);

                if (custom == null)
                    continue;

                var extension = custom.OfType<EnumExtensionAttribute>().FirstOrDefault();

                if (extension == null || extension.type == null || !extension.type.IsEnum)
                    continue;

                foreach (var f in type.GetFields(BindingFlags.Public | BindingFlags.Static))
                    f.SetValue(null, ETGModCompatibility.ExtendEnum(MOD_GUID, f.Name, extension.type));
            }
        }
    }
}
