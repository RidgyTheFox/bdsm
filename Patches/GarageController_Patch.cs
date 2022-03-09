using HarmonyLib;

namespace BDSM.Patches
{
    class GarageController_Patch
    {
        [HarmonyPatch(typeof(FreeMode.Garage.GarageController), "OpenGarage")]
        [HarmonyPostfix]
        public static void OpenGarage()
        {
            if (!StaticData.clientInstance.isTimeSynced)
                StaticData.clientInstance.RequestTimeUpdate();

            StaticData.clientInstance.isPlayerOnMap = false;
        }

        [HarmonyPatch(typeof(FreeMode.Garage.GarageController), "RespawnBus")]
        [HarmonyPostfix]
        public static void RespawnBus()
        {
            StaticData.clientInstance.isPlayerOnMap = true;
            StaticData.clientInstance.CreateRemotePlayersModels();
        }
    }
}
