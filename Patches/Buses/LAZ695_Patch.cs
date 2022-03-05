using HarmonyLib;

namespace BDSM.Patches.Buses
{
    class LAZ695_Patch
    {
        [HarmonyPatch(typeof(LAZ695), "Start")]
        [HarmonyPostfix]
        public static void Start(LAZ695 __instance)
        {
            StaticData.clientInstance.localPlayerBus = __instance.gameObject;
        }
    }
}
