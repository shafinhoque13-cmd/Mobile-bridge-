using BepInEx;
using UnityEngine;

namespace MobileBridge
{
    [BepInPlugin("com.shafin.bridge", "MobileBridge", "1.3.1")]
    public class Plugin : BaseUnityPlugin
    {
        private bool _active = true;
        private Rect _winRect = new Rect(30, 30, 200, 100);

        void Awake()
        {
            Logger.LogInfo("!!! MOBILE BRIDGE AWAKE (v1.3.1) !!!");
        }

        void OnGUI()
        {
            GUI.depth = -1000;
            float s = Screen.height / 1080f;
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(s, s, 1));

            _winRect = GUI.Window(0, _winRect, (id) => {
                string color = _active ? "cyan" : "red";
                if (GUILayout.Button($"<color={color}>BYPASS: {(_active ? "ON" : "OFF")}</color>", GUILayout.ExpandHeight(true)))
                    _active = !_active;
                GUI.DragWindow();
            }, "Bridge");
        }

        void Update()
        {
            if (!_active) return;

            // Using the modern, faster Unity 2023+ methods to avoid warnings
            // This tells the game to constantly check the bench status
            GameObject gm = GameObject.Find("GameManager");
            if (gm != null)
            {
                gm.SendMessage("SetAtBench", true, SendMessageOptions.DontRequireReceiver);
            }

            GameObject pdObj = GameObject.Find("PlayerData");
            if (pdObj != null)
            {
                pdObj.SendMessage("SetBool", new object[] { "atBench", true }, SendMessageOptions.DontRequireReceiver);
                pdObj.SendMessage("SetBool", new object[] { "canEquip", true }, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
