using HarmonyLib;

namespace BDSM.Patches
{
    class GameTime_Patch
    {
        [HarmonyPatch(typeof(FreeMode.GameTime), "SetOtherTimeKeeper")]
        [HarmonyPostfix]
        public static void SetOtherTimeKeeper(FreeMode.TimeKeeper newTimeKeeper)
        {
            StaticData.timeKeeper = newTimeKeeper;
        }
    }
}
