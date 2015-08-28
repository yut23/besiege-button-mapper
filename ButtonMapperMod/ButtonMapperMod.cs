using spaar.ModLoader;
using spaar.ModLoader.UI;
using System;
using UnityEngine;

namespace yut23.ButtonMapper
{
    public class ButtonMapperMod : MonoBehaviour
    {
        private string translateKey, eraseKey, mapKey;
        private TranslateButton translateButton = null;
        private EraseButton eraseButton = null;
        private KeyMapModeButton mapButton = null;
        private bool isLoaded, isGuiVisible;
        private int windowId = Util.GetWindowID();
        private Rect windowRect = new Rect(20, 300, 138, 237);

        public void Start()
        {
            Configuration.OnConfigurationChange += OnConfigurationChange;
            translateKey = Configuration.GetString("key:translate", "t");
            eraseKey = Configuration.GetString("key:erase", "n");
            mapKey = Configuration.GetString("key:km+pt", "m");
            SettingsMenu.RegisterSettingsButton("Button\nMapper", new SettingsToggle(SetGuiVisible), false, 13);
        }

        private void SetGuiVisible(bool active)
        {
            isGuiVisible = active;
        }

        private void OnConfigurationChange(object sender, ConfigurationEventArgs e)
        {
            switch (e.Key)
            {
                case "key:translate":
                    translateKey = e.Value;
                    break;
                case "key:erase":
                    eraseKey = e.Value;
                    break;
                case "key:km+pt":
                    mapKey = e.Value;
                    break;
            }
        }

        private void OnGUI()
        {
            if (!isGuiVisible) return;
            GUI.skin = ModGUI.Skin;
            windowRect = GUI.Window(windowId, windowRect, new GUI.WindowFunction(MakeWindow), "Button Mapper");
        }

        private void MakeWindow(int id)
        {
            GUILayout.Label("Translate key:", new GUILayoutOption[0]);
            translateKey = GUILayout.TextField(translateKey, new GUILayoutOption[0]);
            GUILayout.Label("Erase key:", new GUILayoutOption[0]);
            eraseKey = GUILayout.TextField(eraseKey, new GUILayoutOption[0]);
            GUILayout.Label("KM+PT key:", new GUILayoutOption[0]);
            mapKey = GUILayout.TextField(mapKey, new GUILayoutOption[0]);
            GUI.DragWindow();
        }

        private void OnLevelWasLoaded(int level)
        {
            translateButton = GameObject.FindObjectOfType<TranslateButton>();
            eraseButton = GameObject.FindObjectOfType<EraseButton>();
            mapButton = GameObject.FindObjectOfType<KeyMapModeButton>();
            isLoaded = true;
        }

        public void Update()
        {
            if (isLoaded && !AddPiece.isSimulating && !LevelEditController.levelEditActive)
            {
                if (Input.GetKeyDown(translateKey)) translateButton.OnMouseDown();
                if (Input.GetKeyDown(eraseKey)) eraseButton.OnMouseDown();
                if (Input.GetKeyDown(mapKey)) mapButton.OnMouseDown();
            }
        }
    }
}
