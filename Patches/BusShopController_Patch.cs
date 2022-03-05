using HarmonyLib;

namespace BDSM.Patches
{
    class BusShopController_Patch
    {
        [HarmonyPatch(typeof(FreeMode.Garage.UI.BusShopController), "SelectBus")]
        [HarmonyPostfix]
        public static void SelectBus()
        {

        }
    }
}
