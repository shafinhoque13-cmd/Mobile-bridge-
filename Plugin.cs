using BepInEx;
using UnityEngine;

namespace MobileBridge
{
    [BepInPlugin("com.shafin.bridge", "MobileBridge", "1.4.1")]
    public class Plugin : BaseUnityPlugin
    {
        private bool _active = true;
        private Rect _winRect = new Rect(30, 30, 250, 120);

        void Awake()
        {
            Logger.LogInfo("!!! MOBILE BRIDGE v1.4.1 LOADED - UI SYSTEM DETECTED !!!");
        }

        void OnGUI()
        {
            GUI.depth = -1000;
            float s = Screen.height / 1080f;
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(s, s, 1));

            _winRect = GUI.Window(0, _winRect, (id) => {
                string statusColor = _active ? "cyan" : "red";
                if (GUILayout.Button($"<color={statusColor}>BRIDGE: {(_active ? "ON" : "OFF")}</color>", GUILayout.ExpandHeight(true)))
                    _active = !_active;
                GUI.DragWindow();
            }, "Bridge");
        }

        void Update()
        {
            if (!_active) return;

            // 1. Force the Player Data variables
            GameObject pd = GameObject.Find("PlayerData");
            if (pd != null)
            {
                pd.SendMessage("SetBool", new object[] { "atBench", true }, SendMessageOptions.DontRequireReceiver);
                pd.SendMessage("SetBool", new object[] { "canEquip", true }, SendMessageOptions.DontRequireReceiver);
            }

            // 2. Force the Game Manager
            GameObject gm = GameObject.Find("GameManager");
            if (gm != null)
            {
                gm.SendMessage("SetAtBench", true, SendMessageOptions.DontRequireReceiver);
            }

            // 3. The "UIModule" Force: This bypasses the need for UnityEngine.UI.dll
            // It scans for any object that looks like an Equip button and tells it to be active
            Object[] allObjects = Object.FindObjectsOfType<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                string name = obj.name.ToLower();
                if (name.Contains("equip") || name.Contains("slot") || name.Contains("button"))
                {
                    // This sends a direct instruction to the UI component to wake up
                    obj.SendMessage("set_interactable", true, SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }
}
