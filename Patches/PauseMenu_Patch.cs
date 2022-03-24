using HarmonyLib;

namespace BDSM.Patches
{
    class PauseMenu_Patch
    {
        [HarmonyPatch(typeof(PauseMenu), "Exit")]
        [HarmonyPostfix]
        public static void Exit()
        {
            StaticData.clientInstance.Disconnect();
            StaticData.discordIntegrationInstance.UpdateActivity(AvailbaleActivities.IN_MAIN_MENU);
        }
    }
}
