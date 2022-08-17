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

            if (StaticData.clientInstance._isConnected)
                StaticData.discordIntegrationInstance.UpdateActivity(AvailbaleActivities.IN_MULTIPLAYER);
            else
                StaticData.discordIntegrationInstance.UpdateActivity(AvailbaleActivities.IN_SINGLEPLAYER);
        }
    }
}
