﻿using UnityEngine;
using HarmonyLib;

namespace BDSM.Patches.Buses
{
    class LIAZ677_Patch
    {
        [HarmonyPatch(typeof(liaz5292), "Start")]
        [HarmonyPostfix]
        public static void Start(liaz5292 __instance)
        {
            StaticData.clientInstance.localPlayerBus = __instance.gameObject;
        }

        [HarmonyPatch(typeof(liaz5292), "Update")]
        [HarmonyPostfix]
        public static void Update(liaz5292 __instance)
        {

        }
    }
}
