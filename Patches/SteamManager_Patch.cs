using HarmonyLib;
using UnityEngine;

namespace BDSM.Patches
{
    public static class SteamManager_Patch
    {
        [HarmonyPatch(typeof(SteamManager), "Awake")]
        [HarmonyPostfix]
        public static void Awake()
        {
            GameObject l_ridgysStuffGameObject = new GameObject("RidgysStuff");
            l_ridgysStuffGameObject.AddComponent<ClockMachine>();
            l_ridgysStuffGameObject.AddComponent<Server>();
            l_ridgysStuffGameObject.AddComponent<Client>();
            l_ridgysStuffGameObject.AddComponent<DummyClient>();
            l_ridgysStuffGameObject.AddComponent<SecondDummyClient>();
            l_ridgysStuffGameObject.AddComponent<AboutWindow>();
            GameObject.DontDestroyOnLoad(l_ridgysStuffGameObject);
            Debug.Log("GameObject for Ridgys stuff was created.");
        }
    }
}
