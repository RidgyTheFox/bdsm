using UnityEngine;
using HarmonyLib;

namespace BDSM.Patches.Buses
{
    class liaz_5292_Patch
    {
        [HarmonyPatch(typeof(liaz5292), "Start")]
        [HarmonyPostfix]
        public static void Start(liaz5292 __instance)
        {
            StaticData.clientInstance.localPlayerBus = __instance.gameObject;
        }
    }
}
