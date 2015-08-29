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
        private string timeScaleStr;
        private TimeSliderObject timeSliderObject;
        private bool isLoaded, isGuiVisible, isGuiFirstRun;
        private int windowId = Util.GetWindowID();
        private float timeScale;
        private Rect windowRect = new Rect(20, 300, 138, 350/*237*/);
        
        private List<Button> buttons = new List<Button>();

        private void OnLevelWasLoaded(int level)
        {
            buttons.Add(gameObject.AddComponent<Button>().Init(new TranslateButtonAdapter(), "Translate", "Translate Tool"));
            buttons.Add(gameObject.AddComponent<Button>().Init(new EraseButtonAdapter(), "Erase", "EraseTool"));
            buttons.Add(gameObject.AddComponent<Button>().Init(new KeyMapModeButtonAdapter(), "KM+PT", "KeyMapInfoTool"));

            foreach (Button b in buttons)
            {
                b.key = Configuration.GetString("key:" + b.name.ToLower(), "");
                b.OnMouseOver += () => Debug.Log(b.name + " button is moused over.");
                b.OnMouseOut += () => Debug.Log(b.name + " button is not moused over.");
            }

            timeSliderObject = GameObject.FindObjectOfType<TimeSliderObject>();
            timeScale = timeSliderObject.timeSliderCode.delegateTimeScale;
            timeScaleStr = (timeScale * 100f).ToString();

            isLoaded = true;
        }

        public void Start()
        {
            Configuration.OnConfigurationChange += OnConfigurationChange;
            SettingsMenu.RegisterSettingsButton("Button\nMapper", new SettingsToggle(SetGuiVisible), false, 13);
        }

        private void SetGuiVisible(bool active)
        {
            isGuiVisible = active;
            isGuiFirstRun = true;
        }

        private void OnConfigurationChange(object sender, ConfigurationEventArgs e)
        {
            foreach (Button b in buttons)
            {
                if (b.key.Equals(e.Key, StringComparison.OrdinalIgnoreCase))
                    b.key = e.Value;
            }
        }

        private void OnGUI()
        {
            if (!isGuiVisible) return;
            if (isGuiFirstRun)
            {
                timeScale = timeSliderObject.timeSliderCode.delegateTimeScale;
                timeScaleStr = (timeScale * 100f).ToString();
                isGuiFirstRun = false;
            }
            GUI.skin = ModGUI.Skin;
            windowRect = GUI.Window(windowId, windowRect, new GUI.WindowFunction(MakeWindow), "Button Mapper");
        }

        private void MakeWindow(int id)
        {
            foreach (Button b in buttons)
            {
                GUILayout.Label(b.name + " key:", new GUILayoutOption[0]);
                b.key = GUILayout.TextField(b.key, new GUILayoutOption[0]);
            }
            timeScaleStr = GUILayout.TextField(timeScaleStr, new GUILayoutOption[0]);
            if (GUILayout.Button("Set timescale", new GUILayoutOption[0]) || Event.current.keyCode == KeyCode.Return)
            {
                timeScale = float.Parse(timeScaleStr) / 200f;
                timeSliderObject.SetPercentage(timeScale);
            }
            GUI.DragWindow();
        }

        public void Update()
        {
            if (isLoaded && !AddPiece.isSimulating && !LevelEditController.levelEditActive)
            {
                foreach (Button b in buttons)
                {
                    if (b.isMousedOver && Input.anyKeyDown) { b.key = Input.inputString; Debug.Log(Input.inputString); }
                    if (Input.GetKeyDown(b.key)) b.Trigger();
                }
            }
        }
    }
}
