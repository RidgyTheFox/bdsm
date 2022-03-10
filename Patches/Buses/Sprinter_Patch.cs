using UnityEngine;
using HarmonyLib;

namespace BDSM.Patches.Buses
{
    class Sprinter_Patch
    {
        [HarmonyPatch(typeof(Sprinter), "Start")]
        [HarmonyPostfix]
        public static void Start(Sprinter __instance)
        {
            StaticData.clientInstance.localPlayerBus = __instance.gameObject;
        }
    }
}
