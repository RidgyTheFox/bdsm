using UnityEngine;
using HarmonyLib;

namespace BDSM.Patches.Buses
{
    class CitaroK_Patch
    {
        [HarmonyPatch(typeof(CitaroK), "Start")]
        [HarmonyPostfix]
        public static void Start(CitaroK __instance)
        {
            StaticData.clientInstance.localPlayerBus = __instance.gameObject;
        }
    }
}
