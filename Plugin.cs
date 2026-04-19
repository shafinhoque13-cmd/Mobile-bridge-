using BepInEx;
using UnityEngine;
using System.Reflection;

namespace MobileBridge
{
    [BepInPlugin("com.shafin.bridge", "MobileBridge", "1.3.0")]
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
                if (GUILayout.Button(_active ? "BYPASS: ON" : "BYPASS: OFF", GUILayout.ExpandHeight(true)))
                    _active = !_active;
                GUI.DragWindow();
            }, "Bridge");
        }

        void Update()
        {
            if (!_active) return;

            // Target the HeroController directly - this is where Bench logic usually lives on mobile
            var hero = GameObject.FindObjectOfType<Component>(); 
            // We search for a component that contains "HeroController" or "Player"
            
            GameObject gm = GameObject.Find("GameManager");
            if (gm != null)
            {
                // Force the global bench state in the GameManager
                gm.SendMessage("SetAtBench", true, SendMessageOptions.DontRequireReceiver);
            }

            // Force the specific PlayerData variables
            // On mobile, these are often accessed via a static 'instance'
            try {
                GameObject pdObj = GameObject.Find("PlayerData");
                if (pdObj != null)
                {
                    pdObj.SendMessage("SetBool", new object[] { "atBench", true }, SendMessageOptions.DontRequireReceiver);
                    pdObj.SendMessage("SetBool", new object[] { "canEquip", true }, SendMessageOptions.DontRequireReceiver);
                }
            } catch { }
        }
    }
}
