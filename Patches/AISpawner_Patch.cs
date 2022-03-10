using HarmonyLib;

namespace BDSM.Patches
{
    class AISpawner_Patch
    {
        [HarmonyPatch(typeof(AI.Spawner), "Awake")]
        [HarmonyPrefix]
        public static bool Awake()
        {
            if (StaticData.clientInstance._isConnected)
                return false;
            else
                return true;
        }
    }
}
