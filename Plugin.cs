using BepInEx;
using BepInEx.Configuration;
using BepInEx.Bootstrap; 
using UnityEngine;

namespace MobileBridge
{
    [BepInPlugin("com.shafin.mobile.bridge", "Mobile Bridge", "1.1.0")]
    public class Plugin : BaseUnityPlugin
    {
        private bool _isBridgeActive = true;
        // Smaller initial size for a "bubble" feel
        private Rect _windowRect = new Rect(20, 20, 300, 150); 

        void OnGUI()
        {
            // Auto-scale for mobile DPI
            float scale = Screen.width / 1920f;
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scale, scale, 1));

            // The window ID (0) makes it interactable
            _windowRect = GUI.Window(0, _windowRect, DrawBubble, "Bridge");
        }

        void DrawBubble(int windowID)
        {
            // This line makes the entire window draggable by touch
            GUI.DragWindow(new Rect(0, 0, 10000, 40)); 

            GUILayout.BeginVertical();
            string status = _isBridgeActive ? "<color=cyan>ACTIVE</color>" : "<color=red>OFF</color>";
            
            if (GUILayout.Button(status, GUILayout.Height(60)))
            {
                _isBridgeActive = !_isBridgeActive;
            }
            GUILayout.EndVertical();

            // Also allows dragging from the body of the bubble
            GUI.DragWindow();
        }

        void Update()
        {
            if (!_isBridgeActive) return;

            // Force Bench State
            GameObject pd = GameObject.Find("PlayerData");
            if (pd != null)
            {
                pd.SendMessage("SetBool", new object[] { "atBench", true }, SendMessageOptions.DontRequireReceiver);
            }

            // Sync with PC Mod
            foreach (var plugin in Chainloader.PluginInfos.Values)
            {
                if (plugin.Metadata.Name.Contains("Bench"))
                {
                    foreach (var configKey in plugin.Instance.Config.Keys)
                    {
                        var entry = plugin.Instance.Config[configKey];
                        if (entry.SettingType == typeof(bool)) entry.BoxedValue = true;
                    }
                }
            }
        }
    }
}
