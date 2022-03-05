using HarmonyLib;

namespace BDSM.Patches
{
    class BusShopController_Patch
    {
        [HarmonyPatch(typeof(FreeMode.Garage.UI.BusShopController), "SelectToDriveSelected")]
        [HarmonyPostfix]
        public static void SelectToDriveSelected()
        {
            StaticData.clientInstance.OnPlayerChangedBusInGarage();
        }
    }
}
