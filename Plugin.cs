using BepInEx;
using HarmonyLib;
using UnityEngine;

/*
 * Notes about some folders:
 *  "Libs" - libraries that not from NuGET. :P
 *   -Small note about LiteNetLib - this library exist in NuGET but do not use it! Becacuse you need to compile this library by yourself (use it as a source code like in this project). Thats important because author of this lib uses some tricks\traps for avoiding some problems with Unity.
 *  "Network" - This folder contains all network-only related classes: classes for their serialization into packets and serializator extensions (for Unity`s variable types.).
 *  "Patches" - This folder conatins files with clasess\funcitons for patching via Harmony. This classes will replace the original classes of the game. (After patching, class in game will looks like oldCLass+NewUserCreatedClass. But i`m not sure about this...)
 *  "Refs" - This folder contains all references. AKA some game files with files.
 *   -"Assembly-CSharp.dll" - game code base. I`m using this file as a library and also as a reference for patching via HarmonyX.
 *   -"UnityEngine.UI.dll" - this is a library from Unity game engine. I dont remember why its here. As far as i know, i`m using it for customizing fonts in Unity IMGUI (Its not a DearImGUI).
 *  "RemotePlayerControllers" - This folder contains bus-specific classes with their own lights\nickname positions\offsets and etc...
 */

namespace BDSM
{
    /// <summary>
    /// This is a core class that will patch all classes\\functions.
    /// </summary>
    [BepInPlugin("org.bepinex.plugins.ridgythefox.busdriversimulatormultiplayer", "Bus Driver SimulatorMultiplayer", "0.1.0")]
    public class Plugin : BaseUnityPlugin
    {
        // I`m using separate (NuGET package) patcher. Its a HarmonyX, fork of the original Harmony 2.0 with some improvements. Be careful, BepInEx have patcher, but i dont wanna to use it.
        private static Harmony _harmony;

        /// <summary>
        /// This is an entry point for plugin.
        /// </summary>
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} v{PluginInfo.PLUGIN_VERSION} ({PluginInfo.PLUGIN_GUID}) is loaded!");
            _harmony = new Harmony("mainPatcher"); // Creating a new patcher instance with him own ID.
            Logger.LogInfo("Patching methods...");
            //--------------------------------------------------------------//
            _harmony.PatchAll(typeof(Patches.SteamManager_Patch));          //
            _harmony.PatchAll(typeof(Patches.MainMenu_Patch));              //
            _harmony.PatchAll(typeof(Patches.PauseMenu_Patch));             //
            _harmony.PatchAll(typeof(Patches.BusShopController_Patch));     //
            _harmony.PatchAll(typeof(Patches.GarageController_Patch));      //
            _harmony.PatchAll(typeof(Patches.AISpawner_Patch));             //
            _harmony.PatchAll(typeof(Patches.TimeKeeper_Patch));            //
            _harmony.PatchAll(typeof(Patches.GameTime_Patch));              //
            _harmony.PatchAll(typeof(Patches.BusControl_Patch));            //
                                                                            //  Lets patch everything! :P
            _harmony.PatchAll(typeof(Patches.Buses.CitaroK_Patch));         //
            _harmony.PatchAll(typeof(Patches.Buses.Icarus260_Patch));       //
            _harmony.PatchAll(typeof(Patches.Buses.LAZ695_Patch));          //
            _harmony.PatchAll(typeof(Patches.Buses.LIAZ677_Patch));         //
            _harmony.PatchAll(typeof(Patches.Buses.MANLionsCoach_Patch));   //
            _harmony.PatchAll(typeof(Patches.Buses.PAZ3205_Patch));         //
            _harmony.PatchAll(typeof(Patches.Buses.PAZ672_Patch));          //
            _harmony.PatchAll(typeof(Patches.Buses.Sprinter_Patch));        //
            _harmony.PatchAll(typeof(Patches.Buses.VectorNext_Patch));      //
            //--------------------------------------------------------------//
            Logger.LogInfo("Patching complete! Plugin initialized!");
        }
    }
}
