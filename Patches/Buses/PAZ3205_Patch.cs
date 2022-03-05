using HarmonyLib;

namespace BDSM.Patches.Buses
{
    class PAZ3205_Patch
    {
        [HarmonyPatch(typeof(PAZ3205), "Start")]
        [HarmonyPostfix]
        public static void Start(PAZ3205 __instance)
        {
            StaticData.clientInstance.localPlayerBus = __instance.gameObject;
        }
    }
}
