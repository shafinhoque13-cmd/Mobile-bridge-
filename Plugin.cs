using BepInEx;
using UnityEngine;

namespace MobileBridge
{
    [BepInPlugin("com.shafin.bridge", "MobileBridge", "1.4.4")]
    public class Plugin : BaseUnityPlugin
    {
        private bool _active = true;
        private Rect _winRect = new Rect(100, 100, 250, 120);
        
        // Dragging variables
        private bool _dragging = false;
        private Vector2 _lastMousePos;

        void OnGUI()
        {
            GUI.depth = -1000;
            
            // Scaling for high DPI mobile screens
            float scale = Screen.height / 1080f;
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scale, scale, 1));

            // Custom Drag Logic (Android Compatible)
            Vector2 currentMouse = new Vector2(Input.mousePosition.x / scale, (Screen.height - Input.mousePosition.y) / scale);
            
            if (Input.GetMouseButtonDown(0) && _winRect.Contains(currentMouse))
                _dragging = true;
            
            if (_dragging && Input.GetMouseButton(0))
            {
                Vector2 delta = currentMouse - _lastMousePos;
                _winRect.position += delta;
            }
            
            if (Input.GetMouseButtonUp(0))
                _dragging = false;

            _lastMousePos = currentMouse;

            // Draw Window
            _winRect = GUI.Window(0, _winRect, (id) => {
                string status = _active ? "<color=cyan>ON</color>" : "<color=red>OFF</color>";
                if (GUILayout.Button($"BRIDGE: {status}", GUILayout.ExpandHeight(true)))
                {
                    _active = !_active;
                }
            }, "Bridge");
        }

        void Update()
        {
            if (!_active) return;

            // Force Bench States
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
