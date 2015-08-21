using spaar.ModLoader;
using System;
using UnityEngine;

namespace yut23.ButtonMapper
{
    public class BesiegeModLoader : Mod
    {
        public GameObject temp;
        public override string Name { get { return "button-mapper-mod"; } }
        public override string DisplayName { get { return "Button Mapper Mod"; } }
        public override string Author { get { return "Yut23"; } }
        public override Version Version { get { return new Version(1, 0); } }
        public override string BesiegeVersion { get { return "v0.11"; } }
        public override bool CanBeUnloaded { get { return true; } }

        public override void OnLoad()
        {
            GameObject gameObject = new GameObject();
            gameObject.AddComponent<ButtonMapperMod>();
            GameObject.DontDestroyOnLoad(gameObject);
        }

        public override void OnUnload() { GameObject.Destroy(temp); }
    }
}
