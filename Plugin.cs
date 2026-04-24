using BepInEx;
using UnityEngine;
using BepInEx.Bootstrap;

namespace MobileBridge
{
    [BepInPlugin("com.shafin.mobile.bridge", "Mobile Master Bridge", "1.7.0")]
    public class Plugin : BaseUnityPlugin
    {
        private bool _benchActive = true;
        private Rect _winRect = new Rect(20, 150, 220, 100);

        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        void OnGUI()
        {
            GUI.depth = -1000;
            float s = Screen.height / 1080f;
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(s, s, 1));

            _winRect = GUI.Window(0, _winRect, (id) => {
                // Toggle Bench Bypass
                if (GUILayout.Button(_benchActive ? "BENCH: ON" : "BENCH: OFF"))
                    _benchActive = !_benchActive;

                // TRIGGER CONFIG MANAGER
                if (GUILayout.Button("OPEN MOD SETTINGS"))
                {
                    TriggerConfigManager();
                }
                GUI.DragWindow();
            }, "Bridge");
        }

        private void TriggerConfigManager()
        {
            // This searches for the standard BepInEx Configuration Manager and tells it to open
            foreach (var plugin in Chainloader.PluginInfos.Values)
            {
                if (plugin.Metadata.GUID.Contains("ConfigurationManager"))
                {
                    plugin.Instance.SendMessage("ToggleWindow", SendMessageOptions.DontRequireReceiver);
                }
            }
        }

        void Update()
        {
            if (!_benchActive) return;

            // Bench Bypass logic
            GameObject gm = GameObject.Find("GameManager");
            if (gm != null) gm.SendMessage("SetAtBench", true, SendMessageOptions.DontRequireReceiver);

            GameObject pd = GameObject.Find("PlayerData");
            if (pd != null)
            {
                pd.SendMessage("SetBool", new object[] { "atBench", true }, SendMessageOptions.DontRequireReceiver);
                pd.SendMessage("SetBool", new object[] { "canEquip", true }, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
