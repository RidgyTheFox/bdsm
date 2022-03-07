using HarmonyLib;

namespace BDSM.Patches
{
    class TimeKeeper_Patch
    {
        [HarmonyPatch(typeof(FreeMode.TimeKeeper), "AddDay")]
        [HarmonyPrefix]
        public static bool AddDay()
        {
            if (StaticData.clientInstance._isConnected)
                return false;
            else
                return true;
        }

        [HarmonyPatch(typeof(FreeMode.TimeKeeper), "AddHour")]
        [HarmonyPrefix]
        public static bool AddHour()
        {
            if (StaticData.clientInstance._isConnected)
                return false;
            else
                return true;
        }

        [HarmonyPatch(typeof(FreeMode.TimeKeeper), "AddMinute")]
        [HarmonyPrefix]
        public static bool AddMinute()
        {
            if (StaticData.clientInstance._isConnected)
                return false;
            else
                return true;
        }
    }
}
