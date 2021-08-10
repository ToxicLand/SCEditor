using System;
using System.Collections.Generic;
using System.IO;
using SCEditor.Sc.Loaders;

namespace SCEditor.Sc
{
    internal abstract class ScLoader
    {
        static ScLoader()
        {
            s_version2loader = new Dictionary<Sc.ScFormatVersion, Type> {{ScFormatVersion.Version7, typeof(ScLoader7)}};
        }

        private static readonly Dictionary<ScFormatVersion, Type> s_version2loader;

        public static ScLoader GetLoader(ScFormatVersion version)
        {
            var type = (Type)null;
            if (!s_version2loader.TryGetValue(version, out type))
                throw new Exception("Unable to find ScLoader for the specified version.");

            return (ScLoader)Activator.CreateInstance(type);
        }

        public abstract void Load(ref ScFile file, Stream stream);
    }
}
