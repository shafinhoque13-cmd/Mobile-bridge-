using BepInEx;
using BepInEx.Configuration;
using BepInEx.Bootstrap; 
using UnityEngine;

namespace MobileBridge
{
    [BepInPlugin("com.shafin.mobile.bridge", "Mobile Bridge for Bench Mod", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private bool _isBridgeActive = true; 

        void OnGUI()
        {
            // Scale UI for high-res mobile screens
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(Screen.width / 1920f, Screen.height / 1080f, 1));

            // Floating Button to check status
            string status = _isBridgeActive ? "<color=cyan>BRIDGE: CONNECTED</color>" : "<color=yellow>BRIDGE: STANDBY</color>";
            if (GUI.Button(new Rect(50, 50, 350, 100), $"<size=24>{status}</size>"))
            {
                _isBridgeActive = !_isBridgeActive;
            }
        }

        void Update()
        {
            if (!_isBridgeActive) return;

            // 1. Force the 'atBench' flag directly in the PlayerData
            GameObject pd = GameObject.Find("PlayerData");
            if (pd != null)
            {
                pd.SendMessage("SetBool", new object[] { "atBench", true }, SendMessageOptions.DontRequireReceiver);
                pd.SendMessage("SetBool", new object[] { "canEquip", true }, SendMessageOptions.DontRequireReceiver);
            }

            // 2. Scan for the PC mod and override its internal settings
            foreach (var plugin in Chainloader.PluginInfos.Values)
            {
                // We look for 'No Bench Restrictions' specifically
                if (plugin.Metadata.Name.Contains("Bench"))
                {
                    foreach (var configKey in plugin.Instance.Config.Keys)
                    {
                        var entry = plugin.Instance.Config[configKey];
                        // Force the 'Enabled' toggle to True so it bypasses Harmony checks
                        if (entry.SettingType == typeof(bool))
                        {
                            entry.BoxedValue = true;
                        }
                    }
                }
            }
        }
    }
}
