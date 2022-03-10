using HarmonyLib;

namespace BDSM.Patches
{
    class BusControl_Patch
    {
        [HarmonyPatch(typeof(BusControl), "Update")]
        [HarmonyPostfix]
        public static void Update()
        {

        }
    }
}
