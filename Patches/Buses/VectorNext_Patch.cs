using UnityEngine;
using HarmonyLib;

namespace BDSM.Patches.Buses
{
    class VectorNext_Patch
    {
        [HarmonyPatch(typeof(VectorNext), "Start")]
        [HarmonyPostfix]
        public static void Start(VectorNext __instance)
        {
            StaticData.clientInstance.localPlayerBus = __instance.gameObject;
        }
    }
}
