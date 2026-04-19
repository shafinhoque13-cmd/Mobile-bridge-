using BepInEx;
using BepInEx.Configuration;
using BepInEx.Bootstrap; 
using UnityEngine;
using System;

namespace MobileBridge
{
    [BepInPlugin("com.shafin.mobile.bridge", "Ultimate Mobile Bridge", "1.2.0")]
    public class Plugin : BaseUnityPlugin
    {
        private bool _active = true;
        private Rect _winRect = new Rect(20, 20, 250, 100); 

        void OnGUI()
        {
            GUI.depth = -1000;
            float scale = Screen.height / 1080f;
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scale, scale, 1));

            // Small draggable bubble
            _winRect = GUI.Window(0, _winRect, (id) => {
                if (GUILayout.Button(_active ? "<color=cyan>BYPASS: ON</color>" : "<color=red>BYPASS: OFF</color>", GUILayout.Height(50)))
                    _active = !_active;
                GUI.DragWindow();
            }, "Bridge");
        }

        void Update()
        {
            if (!_active) return;

            // 1. Force Global Player States
            GameObject pd = GameObject.Find("PlayerData");
            if (pd != null)
            {
                // This targets the specific flags for Bench and Equipping
                pd.SendMessage("SetBool", new object[] { "atBench", true }, SendMessageOptions.DontRequireReceiver);
                pd.SendMessage("SetBool", new object[] { "canEquip", true }, SendMessageOptions.DontRequireReceiver);
            }

            // 2. Force the Inventory UI to unlock
            // Mobile ports often use 'InventoryItemToolManager' for Silk Skills/Crests
            GameObject toolManager = GameObject.Find("InventoryItemToolManager");
            if (toolManager != null)
            {
                toolManager.SendMessage("SetCanChangeEquips", true, SendMessageOptions.DontRequireReceiver);
            }

            // 3. Keep the PC Mod you uploaded synced
            foreach (var plugin in Chainloader.PluginInfos.Values)
            {
                if (plugin.Metadata.Name.Contains("Bench"))
                {
                    foreach (var key in plugin.Instance.Config.Keys)
                    {
                        var entry = plugin.Instance.Config[key];
                        if (entry.SettingType == typeof(bool)) entry.BoxedValue = true;
                    }
                }
            }
        }
    }
}
