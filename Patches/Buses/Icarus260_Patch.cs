using HarmonyLib;

namespace BDSM.Patches.Buses
{
    class Icarus260_Patch
    {
        [HarmonyPatch(typeof(Icarus260), "Start")]
        [HarmonyPostfix]
        public static void Start(Icarus260 __instance)
        {
            StaticData.clientInstance.localPlayerBus = __instance.gameObject;
        }
    }
}
