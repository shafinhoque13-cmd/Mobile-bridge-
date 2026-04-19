using BepInEx;
using BepInEx.Configuration;
using BepInEx.Bootstrap;
using UnityEngine;

namespace MobileBridge
{
    [BepInPlugin("com.shafin.bridge", "Mobile Mod Bridge", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private bool _menuOpen = false;

        void OnGUI()
        {
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(Screen.width / 1920f, Screen.height / 1080f, 1));
            if (GUI.Button(new Rect(50, 50, 300, 100), "MOD MENU")) _menuOpen = !_menuOpen;

            if (_menuOpen)
            {
                GUILayout.BeginArea(new Rect(100, 150, 600, 700), GUI.skin.box);
                GUILayout.Label("<size=30><b>Mobile Mod Manager</b></size>");
                
                foreach (var plugin in Chainloader.PluginInfos.Values)
                {
                    GUILayout.Label($"Mod: {plugin.Metadata.Name}");
                    foreach (var entry in plugin.Instance.Config.Keys)
                    {
                        var config = plugin.Instance.Config[entry];
                        if (config.SettingType == typeof(bool))
                            config.BoxedValue = GUILayout.Toggle((bool)config.BoxedValue, $" {config.Definition.Key}");
                    }
                }
                if (GUILayout.Button("CLOSE", GUILayout.Height(80))) _menuOpen = false;
                GUILayout.EndArea();
            }
        }
    }
}
