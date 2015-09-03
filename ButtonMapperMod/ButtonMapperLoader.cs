using spaar.ModLoader;
using System;
using UnityEngine;

namespace yut23.ButtonMapper
{
    public class ButtonMapperLoader : Mod
    {
        public override string Name { get { return "button-mapper-mod"; } }
        public override string DisplayName { get { return "Button Mapper Mod"; } }
        public override string Author { get { return "Yut23"; } }
        public override Version Version { get { return new Version(2, 2); } }
        public override string BesiegeVersion { get { return "v0.11"; } }
        public override bool CanBeUnloaded { get { return true; } }

        public override void OnLoad()
        {
            // toolbar mapping configs
            AddConfigIfNotExist("key:translate", "t");
            AddConfigIfNotExist("key:erase", "n");
            AddConfigIfNotExist("key:km+pt", "m");

            // time slider mapping configs
            AddConfigIfNotExist("key:speedup", "=");
            AddConfigIfNotExist("key:speeddown", "-");

            // custom time hotkey configs
            if (!Configuration.DoesKeyExist("boundKey:["))
                Configuration.SetFloat("boundKey:[", 0);
            if (!Configuration.DoesKeyExist("boundKey:]"))
                Configuration.SetFloat("boundKey:]", 1);
            if (!Configuration.DoesKeyExist("boundKey:\\"))
                Configuration.SetFloat("boundKey:\\", 100);

            GameObject.DontDestroyOnLoad(SingleInstance<ButtonMapper>.Instance);
        }

        public override void OnUnload()
        {
            GameObject.Destroy(SingleInstance<ButtonMapper>.Instance.gameObject);
            Configuration.Save();
        }

        private void AddConfigIfNotExist(string name, string defaultValue)
        {
            if (!Configuration.DoesKeyExist(name))
                Configuration.SetString(name, defaultValue);
        }
    }
}
