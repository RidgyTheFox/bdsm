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
            _harmony.PatchAll(typeof(Patches.BusShopController_Patch));
            _harmony.PatchAll(typeof(Patches.GarageController_Patch));
            _harmony.PatchAll(typeof(Patches.AISpawner_Patch));
            _harmony.PatchAll(typeof(Patches.TimeKeeper_Patch));
            _harmony.PatchAll(typeof(Patches.GameTime_Patch));
            _harmony.PatchAll(typeof(Patches.BusControl_Patch));

            _harmony.PatchAll(typeof(Patches.Buses.CitaroK_Patch));
            _harmony.PatchAll(typeof(Patches.Buses.Icarus260_Patch));
            _harmony.PatchAll(typeof(Patches.Buses.LAZ695_Patch));
            _harmony.PatchAll(typeof(Patches.Buses.liaz_5292_Patch));
            _harmony.PatchAll(typeof(Patches.Buses.LIAZ677_Patch));
            _harmony.PatchAll(typeof(Patches.Buses.MANLionsCoach_Patch));
            _harmony.PatchAll(typeof(Patches.Buses.PAZ3205_Patch));
            _harmony.PatchAll(typeof(Patches.Buses.PAZ672_Patch));
            _harmony.PatchAll(typeof(Patches.Buses.Sprinter_Patch));
            _harmony.PatchAll(typeof(Patches.Buses.VectorNext_Patch));

            Logger.LogInfo("Patching complete! Plugin initialized!");
        }
    }
}
