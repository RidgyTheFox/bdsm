using HarmonyLib;

namespace BDSM.Patches
{
    class MainMenu_Patch
    {
        [HarmonyPatch(typeof(MainMenu), "Awake")]
        [HarmonyPostfix]
        public static void Awake(MainMenu __instance)
        {
            StaticData.mainMenuInstance = __instance;
        }

        [HarmonyPatch(typeof(MainMenu), "Loading")]
        [HarmonyPostfix]
        public static void Loading(MainMenu __instance, string levelName)
        {
            switch(levelName)
            {
                case "FreeModeRoutes":
                    if (__instance.winterToggle.isOn)
                        StaticData.discordIntegrationInstance.currentMap = "Serpukhov Winter";
                    else
                        StaticData.discordIntegrationInstance.currentMap = "Serpukhov";
                    break;
                case "FreeModeKeln":
                    StaticData.discordIntegrationInstance.currentMap = "Keln";
                    break;
                case "FreeModeMurom":
                    if (__instance.winterToggleMurom.isOn)
                        StaticData.discordIntegrationInstance.currentMap = "Murom Winter";
                    else
                        StaticData.discordIntegrationInstance.currentMap = "Murom";
                    break;
                case "FreeModeSolnechnogorsk":
                    StaticData.discordIntegrationInstance.currentMap = "Solnechnogorsk";
                    break;
            }

            if (!StaticData.clientInstance._isConnected)
                StaticData.discordIntegrationInstance.UpdateActivity(AvailbaleActivities.IN_SINGLEPLAYER);
        }

        [HarmonyPatch(typeof(MainMenu), "GoBuySolDLC")]
        [HarmonyPrefix]
        public static void GoBuySolDLC()
        {
            StaticData.clientInstance.Disconnect();
        }

        [HarmonyPatch(typeof(MainMenu), "GoBuyMuromDLC")]
        [HarmonyPrefix]
        public static void GoBuyMuromDLC()
        {
            StaticData.clientInstance.Disconnect();
        }
    }
}
