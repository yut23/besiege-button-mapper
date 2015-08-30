using spaar.ModLoader;
using spaar.ModLoader.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace yut23.ButtonMapper
{
    public class ButtonMapper : SingleInstance<ButtonMapper>
    {
        public override string Name { get { return "ButtonMapper"; } }
        private bool isLoaded, isGuiVisible;
        private int windowId = Util.GetWindowID();
        private Rect windowRect = new Rect(20, 300, 260, 230);
        private TimeScaleMapper timeScaleMapper;
        
        private List<Button> buttons = new List<Button>();

        public void Start()
        {
            SettingsMenu.RegisterSettingsButton("Button\nMapper", new SettingsToggle(SetGuiVisible), false, 13);

            timeScaleMapper = gameObject.AddComponent<TimeScaleMapper>();
        }

        private void OnLevelWasLoaded(int level)
        {
            buttons.Add(gameObject.AddComponent<Button>().Init(new TranslateButtonAdapter(), "Translate", "Translate Tool"));
            buttons.Add(gameObject.AddComponent<Button>().Init(new EraseButtonAdapter(), "Erase", "EraseTool"));
            buttons.Add(gameObject.AddComponent<Button>().Init(new KeyMapModeButtonAdapter(), "KM+PT", "KeyMapInfoTool"));

            // load configuration
            foreach (var b in buttons)
            {
                b.key = Configuration.GetString("key:" + b.name.ToLower(), "");
            }

            isLoaded = true;
        }

        private void SetGuiVisible(bool active)
        {
            isGuiVisible = active;
        }

        private void OnGUI()
        {
            if (!isGuiVisible) return;
            GUI.skin = ModGUI.Skin;
            windowRect = GUI.Window(windowId, windowRect, new GUI.WindowFunction(MakeWindow), "Button Mapper");
        }

        private void MakeWindow(int id)
        {
            // toolbar button bindings
            foreach (var b in buttons)
            {
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.Label(b.name + " key:", new GUILayoutOption[0]);
                GUILayout.FlexibleSpace();
                GUI.SetNextControlName(b.name + "TextField");
                b.tempKey = GUILayout.TextField(b.tempKey, GUILayout.MinWidth(90));
                if (GUI.GetNameOfFocusedControl() != b.name + "TextField" && b.tempKey != "")
                    b.key = b.tempKey;
                GUILayout.EndHorizontal();
                GUILayout.Space(3);
            }
            // time slider bindings
            timeScaleMapper.GuiRun();
            GUI.DragWindow();
        }

        public void Update()
        {
            if (isLoaded && !AddPiece.isSimulating && !LevelEditController.levelEditActive)
            {
                foreach (var b in buttons)
                {
                    // handle assigning keybinds on mouseOver
                    if (b.isMousedOver && Input.anyKeyDown)
                    {
                        string tempKey = b.key;
                        b.key = Input.inputString;
                        try { Input.GetKey(b.key); }
                        catch { b.key = tempKey; }
                    }
                    // handle button keybinds
                    if (Input.GetKeyDown(b.key)) b.Trigger();
                }
            }
        }
    }
}
