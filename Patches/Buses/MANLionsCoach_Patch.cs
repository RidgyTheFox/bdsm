using UnityEngine;
using HarmonyLib;

namespace BDSM.Patches.Buses
{
    class MANLionsCoach_Patch
    {
        [HarmonyPatch(typeof(MANLionsCoach), "Start")]
        [HarmonyPostfix]
        public static void Start(MANLionsCoach __instance)
        {
            StaticData.clientInstance.localPlayerBus = __instance.gameObject;
        }
    }
}
