using BepInEx;
using BepInEx.Logging;
using UnityEngine;

namespace MobileBridge
{
    // Added specific process targeting to help BepInEx find the game
    [BepInPlugin("com.shafin.mobile.bridge", "Ultimate Mobile Bridge", "1.2.5")]
    [BepInProcess("Silksong.exe")] // Even on mobile, the internal process name often mimics PC
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource Log;
        private bool _active = true;
        private Rect _winRect = new Rect(20, 20, 250, 120);

        void Awake()
        {
            Log = Logger;
            Log.LogInfo("MOBILE BRIDGE DETECTED AND LOADED!");
        }

        void OnGUI()
        {
            GUI.depth = -1000;
            // Use a simpler scaling method for detection
            float s = Screen.height / 720f; 
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(s, s, 1));

            _winRect = GUI.Window(0, _winRect, DrawWindow, "Bridge");
        }

        void DrawWindow(int id)
        {
            if (GUILayout.Button(_active ? "BYPASS: ON" : "BYPASS: OFF", GUILayout.ExpandHeight(true)))
                _active = !_active;
            GUI.DragWindow();
        }

        void Update()
        {
            if (!_active) return;

            // Manual override for Bench State
            GameObject pd = GameObject.Find("PlayerData");
            if (pd != null)
            {
                pd.SendMessage("SetBool", new object[] { "atBench", true }, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
