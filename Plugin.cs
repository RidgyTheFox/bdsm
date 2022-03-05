using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace BDSM
{
    [BepInPlugin("org.bepinex.plugins.ridgythefox.busdriversimulatormultiplayer", "Bus Driver SimulatorMultiplayer", "0.0.1")]
    public class Plugin : BaseUnityPlugin
    {
        private static Harmony _harmony;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} v{PluginInfo.PLUGIN_VERSION} ({PluginInfo.PLUGIN_GUID}) is loaded!");
            _harmony = new Harmony("mainPatcher");
            Logger.LogInfo("Patching methods...");
            _harmony.PatchAll(typeof(Patches.SteamManager_Patch));
            _harmony.PatchAll(typeof(Patches.MainMenu_Patch));
            _harmony.PatchAll(typeof(Patches.PauseMenu_Patch));
            Logger.LogInfo("Patching complete! Plugin initialized!");
        }
    }
}
