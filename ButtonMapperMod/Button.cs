using spaar.ModLoader;
using System;
using UnityEngine;

namespace yut23.ButtonMapper
{
    public delegate void OnMouseOver();
    public delegate void OnMouseOut();
    public class Button : MonoBehaviour
    {
        public event OnMouseOver OnMouseOver;
        public event OnMouseOut OnMouseOut;
        private string _key;
        public string key {
            get {
                return this._key;
            }
            set {
                _key = value;
                Configuration.SetString("key:" + name.ToLower(), value);
            }
        }
        public new string name { get; private set; }
        public bool isMousedOver { get; private set; }
        private bool isFirstMouseOver;
        private MeshRenderer tooltip;
        private string tooltipStr;
        private IButtonAdapter button;

        public Button Init(IButtonAdapter button, string name, string tooltip)
        {
            this.button = button;
            this.name = name;
            this.tooltipStr = tooltip;
            this.tooltip = GameObject.Find(tooltipStr + "/Tooltip").GetComponentInChildren<MeshRenderer>();
            this.key = Configuration.GetString("key:" + name.ToLower(), "");
            return this;
        }

        public void Trigger()
        {
            button.OnMouseDown();
        }

        public void Update()
        {
            if (tooltip.enabled && isFirstMouseOver)
            {
                isMousedOver = true;
                if (OnMouseOver != null) OnMouseOver();
                isFirstMouseOver = false;
            }
            else if (!tooltip.enabled && !isFirstMouseOver)
            {
                isMousedOver = false;
                if (OnMouseOut != null) OnMouseOut();
                isFirstMouseOver = true;
            }
        }
    }

    public interface IButtonAdapter
    {
        void OnMouseDown();
    }

    #region IButtonAdapter wrappers
    class TranslateButtonAdapter : IButtonAdapter
    {
        TranslateButton translateButton = GameObject.FindObjectOfType<TranslateButton>();
        public void OnMouseDown() { translateButton.OnMouseDown(); }
    }

    class EraseButtonAdapter : IButtonAdapter
    {
        EraseButton eraseButton = GameObject.FindObjectOfType<EraseButton>();
        public void OnMouseDown() { eraseButton.OnMouseDown(); }
    }

    class KeyMapModeButtonAdapter : IButtonAdapter
    {
        KeyMapModeButton keyMapModeButton = GameObject.FindObjectOfType<KeyMapModeButton>();
        public void OnMouseDown() { keyMapModeButton.OnMouseDown(); }
    }
    #endregion
}
