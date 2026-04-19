using BepInEx;
using UnityEngine;

namespace MobileBridge
{
    // Simplified ID and version for better detection
    [BepInPlugin("com.shafin.bridge", "MobileBridge", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private bool _active = true;
        private Rect _winRect = new Rect(50, 50, 250, 150);

        // This runs the moment the mod is detected
        void Awake()
        {
            Logger.LogInfo("!!! MOBILE BRIDGE AWAKE !!!");
        }

        void OnGUI()
        {
            GUI.depth = -1000;
            // Use standard scaling
            float s = Screen.height / 1080f;
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(s, s, 1));

            _winRect = GUI.Window(0, _winRect, (id) => {
                if (GUILayout.Button(_active ? "BYPASS: ON" : "BYPASS: OFF", GUILayout.ExpandHeight(true)))
                    _active = !_active;
                GUI.DragWindow();
            }, "Bridge");
        }

        void Update()
        {
            if (!_active) return;

            // Direct force: Telling the game we are always at a bench
            GameObject pd = GameObject.Find("PlayerData");
            if (pd != null)
            {
                pd.SendMessage("SetBool", new object[] { "atBench", true }, SendMessageOptions.DontRequireReceiver);
                pd.SendMessage("SetBool", new object[] { "canEquip", true }, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
