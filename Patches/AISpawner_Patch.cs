using HarmonyLib;

namespace BDSM.Patches
{
    class AISpawner_Patch
    {
        [HarmonyPatch(typeof(AI.Spawner), "Awake")]
        [HarmonyPrefix]
        public static bool Awake()
        {
            return false;
        }
    }
}
