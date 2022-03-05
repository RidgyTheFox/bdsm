using HarmonyLib;

namespace BDSM.Patches.Buses
{
    class PAZ672_Patch
    {
        [HarmonyPatch(typeof(PAZ672), "Start")]
        [HarmonyPostfix]
        public static void Start(PAZ672 __instance)
        {
            StaticData.clientInstance.localPlayerBus = __instance.gameObject;
        }
    }
}
