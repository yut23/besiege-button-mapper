using spaar.ModLoader;
using System;
using UnityEngine;

namespace yut23.ButtonMapper
{
    public class ButtonMapperMod : MonoBehaviour
    {
        string translateKey, eraseKey, mapKey;
        TranslateButton translateButton = null;
        EraseButton eraseButton = null;
        KeyMapModeButton mapButton = null;

        public void Start()
        {
            OnConfigurationChange(null, null);
            Configuration.OnConfigurationChange += OnConfigurationChange;
        }

        void OnConfigurationChange(object sender, ConfigurationEventArgs e)
        {
            translateKey = Configuration.GetString("translate", "t");
            eraseKey = Configuration.GetString("erase", "n");
            mapKey = Configuration.GetString("keymap", "m");
        }

        public void Update()
        {
            if (Game.AddPiece != null)
            {
                if (translateButton == null || eraseButton == null || mapButton == null)
                {
                    translateButton = GameObject.FindObjectOfType<TranslateButton>();
                    eraseButton = GameObject.FindObjectOfType<EraseButton>();
                    mapButton = GameObject.FindObjectOfType<KeyMapModeButton>();
                }
                if (!AddPiece.isSimulating && !LevelEditController.levelEditActive)
                {
                    if (Input.GetKeyDown(translateKey)) translateButton.OnMouseDown();
                    if (Input.GetKeyDown(eraseKey)) eraseButton.OnMouseDown();
                    if (Input.GetKeyDown(mapKey)) mapButton.OnMouseDown();
                }
            }
        }
    }
}
