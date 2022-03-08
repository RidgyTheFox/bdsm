using HarmonyLib;

namespace BDSM.Patches
{
    class GarageController_Pach
    {
        [HarmonyPatch(typeof(FreeMode.Garage.GarageController), "OpenGarage")]
        [HarmonyPostfix]
        public static void OpenGarage()
        {
            StaticData.clientInstance.isSceneLoaded = true;
            StaticData.clientInstance.RequestTimeUpdate();
        }
    }
}
