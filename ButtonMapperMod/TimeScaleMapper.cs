using spaar.ModLoader;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace yut23.ButtonMapper
{
    class TimeScaleMapper : MonoBehaviour
    {
        private TimeSliderObject timeSliderObject;
        private string speedUpKey, speedDownKey;
        private Dictionary<string, float> percentHotkeys = new Dictionary<string, float>();
        private bool isLoaded = false;

        private float timeScale;
        private float timeScalePercent
        {
            get { return (float)Math.Round(timeScale * 200f, 3); }
            set { timeScale = value / 200f; }
        }

        public float keyRepeatDelay = 0.15f;
        private float timePassed = 0f;

        private float prevRealTime;
        private float thisRealTime;
        public float deltaTime {
            get {
                if (Time.timeScale > 0f) return Time.deltaTime / Time.timeScale;
                return Time.realtimeSinceStartup - prevRealTime; // Checks realtimeSinceStartup again because it may have changed since Update was called
            }
        }

        public const string ADD_TIME_HOTKEY_USAGE = "Usage: addTimeHotkey <float percent> <string key>";
        public const string REMOVE_TIME_HOTKEY_USAGE = "Usage: removeTimeHotkey <string key>";
        public const string LIST_TIME_HOTKEYS_USAGE = "Usage: listTimeHotkeys";

        private void OnLevelWasLoaded(int level)
        {
            timeSliderObject = GameObject.FindObjectOfType<TimeSliderObject>();
            timeScale = timeSliderObject.timeSliderCode.delegateTimeScale / 2f;

            // load configuration
            speedUpKey = Configuration.GetString("key:speedup", "=");
            speedDownKey = Configuration.GetString("key:speeddown", "-");

            string[] hotkeys = Configuration.GetString("boundKeys", "").Split(new char[] {' '},
                StringSplitOptions.RemoveEmptyEntries); // this shouldn't really be needed, but computers are weird sometimes
            foreach (var hotkey in hotkeys)
            {
                percentHotkeys[hotkey] = Configuration.GetFloat("boundKey:" + hotkey, -1);
            }

            // add commands
            Commands.RegisterCommand("addTimeHotkey", new CommandCallback(AddHotkey), ADD_TIME_HOTKEY_USAGE);
            Commands.RegisterCommand("removeTimeHotkey", new CommandCallback(RemoveHotkey), REMOVE_TIME_HOTKEY_USAGE);
            Commands.RegisterCommand("listTimeHotkeys", new CommandCallback(ListHotkeys), LIST_TIME_HOTKEYS_USAGE);
            Commands.RegisterHelpMessage("button-mapper-mod commands:\n" +
                ADD_TIME_HOTKEY_USAGE + "\n" +
                REMOVE_TIME_HOTKEY_USAGE + "\n" +
                LIST_TIME_HOTKEYS_USAGE
            );

            isLoaded = true;
        }

        public string AddHotkey(string[] args, IDictionary<string, string> namedArgs)
        {
            if(args.Length < 2) return ADD_TIME_HOTKEY_USAGE;

            float percent;
            if(!float.TryParse(args[0], out percent)) {
                return "Given percentage is not a valid float";
            }
            string key = args[1];

            try { Input.GetKey(key); }
            catch { return "Invalid key name"; }

            Configuration.SetFloat("boundKey:" + key, percent); // set value in config

            string hotkeyList = Configuration.GetString("boundKeys", "");
            int index = hotkeyList.IndexOf(key);
            if (index == -1) {
                Configuration.SetString("boundKeys", hotkeyList + " " + key); // add key to list in config
            }
            percentHotkeys[key] = percent; // add key to local list

            return "Hotkey for " + percent + " added on key \"" + key + "\"";
        }

        public string RemoveHotkey(string[] args, IDictionary<string, string> namedArgs)
        {
            if (args.Length < 1) return REMOVE_TIME_HOTKEY_USAGE;

            string key = args[0];

            string hotkeyList = Configuration.GetString("boundKeys", ""); // get list from config

            int index = hotkeyList.IndexOf(key);
            if (index < 0) return "Hotkey not found";

            hotkeyList = hotkeyList.Remove(index, 1).Trim(); // remove from hotkey list and cleans up spaces
            hotkeyList = Regex.Replace(hotkeyList, @"\s+", " "); // replaces multiple spaces with one space

            Configuration.SetString("boundKeys", hotkeyList); // sets new string in config
            Configuration.SetFloat("boundKey:" + key, -1); // disables hotkey, just in case
            percentHotkeys.Remove(key); // remove from local dictionary

            return "Hotkey on key \"" + key + "\" removed";
        }

        public string ListHotkeys(string[] args, IDictionary<string, string> namedArgs)
        {
            string output = "\n";
            foreach (var hotkey in percentHotkeys)
            {
                output += "Bound key: " + hotkey.Key + "\n";
                output += "\tPercentage: " + Math.Round(hotkey.Value, 4);
                output += "\n";
            }
            return output;
        }

        public void GuiRun()
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label("Increase speed key:", new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            speedUpKey = GUILayout.TextField(speedUpKey, GUILayout.MinWidth(90));
            GUILayout.EndHorizontal();

            GUILayout.Space(3);

            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label("Decrease speed key:", new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            speedDownKey = GUILayout.TextField(speedDownKey, GUILayout.MinWidth(90));
            GUILayout.EndHorizontal();
        }

        public void Update()
        {
            if (!isLoaded) return;
            
            // need real deltaTime for speed changing key repeat
            prevRealTime = thisRealTime;
            thisRealTime = Time.realtimeSinceStartup;
            timePassed += deltaTime;

            timeScale = timeSliderObject.timeSliderCode.delegateTimeScale / 2f;

            //handle speed changing keys
            if (Input.GetKey(speedUpKey) && timePassed >= keyRepeatDelay)
            {
                IncreaseTimeScale();
                timePassed = 0f;
            }
            if (Input.GetKey(speedDownKey) && timePassed >= keyRepeatDelay)
            {
                DecreaseTimeScale();
                timePassed = 0f;
            }

            // handle custom hotkeys
            foreach (var hotkey in percentHotkeys)
                if(Input.GetKeyDown(hotkey.Key) && hotkey.Value >= 0)
                    timeScalePercent = hotkey.Value;
            timeSliderObject.SetPercentage(timeScale);
        }

        // these curves make sense to me, but I'm not sure about everyone else
        private void IncreaseTimeScale()
        {
            if (timeScalePercent >= 50) timeScalePercent += 25;
            else if (timeScalePercent >= 10) timeScalePercent += 10;
            else if (timeScalePercent >= 5) timeScalePercent += 2.5f;
            else if (timeScalePercent >= 1) timeScalePercent += 1;
            else if (timeScalePercent >= 0.25f) timeScalePercent += 0.25f;
            else if (timeScalePercent >= 0.1f) timeScalePercent = 0.25f;
            else if (timeScalePercent >= 0.05f) timeScalePercent += 0.025f;
            else if (timeScalePercent >= 0) timeScalePercent += 0.01f;
        }

        private void DecreaseTimeScale()
        {
            if (timeScalePercent <= 0.05f) timeScalePercent -= 0.01f;
            else if (timeScalePercent <= 0.1f) timeScalePercent -= 0.025f;
            else if (timeScalePercent <= 0.25f) timeScalePercent = 0.1f;
            else if (timeScalePercent <= 1) timeScalePercent -= 0.25f;
            else if (timeScalePercent <= 5) timeScalePercent -= 1;
            else if (timeScalePercent <= 10) timeScalePercent -= 2.5f;
            else if (timeScalePercent <= 50) timeScalePercent -= 10;
            else timeScalePercent -= 25;
            if (timeScale < 0) timeScale = 0;
        }

        public void LateUpdate() // change slider text to display floats
        {
            if (!isLoaded) return;
            timeSliderObject.timeSliderCode.TextMeshy.text = timeScalePercent + "%";
        }
    }
}
