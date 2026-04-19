using BepInEx;
using UnityEngine;
using UnityEngine.UI; // Required to find the actual buttons

namespace MobileBridge
{
    [BepInPlugin("com.shafin.bridge", "MobileBridge", "1.4.0")]
    public class Plugin : BaseUnityPlugin
    {
        private bool _active = true;
        private Rect _winRect = new Rect(30, 30, 200, 100);

        void OnGUI()
        {
            GUI.depth = -1000;
            float s = Screen.height / 1080f;
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(s, s, 1));

            _winRect = GUI.Window(0, _winRect, (id) => {
                if (GUILayout.Button(_active ? "BRIDGE: ON" : "BRIDGE: OFF", GUILayout.ExpandHeight(true)))
                    _active = !_active;
                GUI.DragWindow();
            }, "Bridge");
        }

        void Update()
        {
            if (!_active) return;

            // 1. Force the Data Flags
            GameObject pd = GameObject.Find("PlayerData");
            if (pd != null)
            {
                pd.SendMessage("SetBool", new object[] { "atBench", true }, SendMessageOptions.DontRequireReceiver);
                pd.SendMessage("SetBool", new object[] { "canEquip", true }, SendMessageOptions.DontRequireReceiver);
            }

            // 2. The "Deep Unlock": Find inventory buttons and force them active
            // This targets the UI buttons directly so they stop being grey
            Selectable[] allButtons = GameObject.FindObjectsOfType<Selectable>();
            foreach (Selectable btn in allButtons)
            {
                if (btn.name.Contains("Equip") || btn.name.Contains("Slot"))
                {
                    btn.interactable = true;
                }
            }
        }
    }
}
