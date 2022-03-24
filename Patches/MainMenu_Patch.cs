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
        public static void Loading()
        {

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
