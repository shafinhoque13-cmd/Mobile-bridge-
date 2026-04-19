using BepInEx;
using BepInEx.Configuration;
using BepInEx.Bootstrap; // This is the "Bridge" part
using UnityEngine;

namespace MobileBridge
{
    [BepInPlugin("com.shafin.mobile.bridge", "Mobile Bridge", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private bool _isUnlocked = false;

        void OnGUI()
        {
            // Scale for high-res phone screens
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(Screen.width / 1920f, Screen.height / 1080f, 1));

            // Floating Bridge Toggle
            string btnText = _isUnlocked ? "<color=green>BRIDGE: ACTIVE</color>" : "<color=red>BRIDGE: OFF</color>";
            if (GUI.Button(new Rect(50, 50, 300, 100), btnText))
            {
                _isUnlocked = !_isUnlocked;
                if (_isUnlocked) ForcePCModActivation();
            }
        }

        private void ForcePCModActivation()
        {
            // This searches all loaded plugins for the 'No Bench Restrictions' mod
            foreach (var plugin in Chainloader.PluginInfos.Values)
            {
                if (plugin.Metadata.Name.Contains("Bench"))
                {
                    // Force its config to 'True' in the phone's memory
                    foreach (var entry in plugin.Instance.Config.Keys)
                    {
                        var config = plugin.Instance.Config[entry];
                        if (config.Definition.Key.Contains("Enabled") || config.Definition.Key.Contains("Bench"))
                        {
                            config.BoxedValue = true;
                        }
                    }
                }
            }
        }

        void Update()
        {
            if (!_isUnlocked) return;

            // Constantly force the 'atBench' state so the inventory never locks
            GameObject pd = GameObject.Find("PlayerData");
            if (pd != null) pd.SendMessage("SetBool", new object[] { "atBench", true }, SendMessageOptions.DontRequireReceiver);
        }
    }
}
