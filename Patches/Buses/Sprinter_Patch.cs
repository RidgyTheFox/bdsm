using UnityEngine;
using HarmonyLib;

namespace BDSM.Patches.Buses
{
    class Sprinter_Patch
    {
        private static GameObject _wheelFL;
        private static GameObject _wheelFR;
        private static GameObject _wheelRL;
        private static GameObject _wheelRR;

        [HarmonyPatch(typeof(Sprinter), "Start")]
        [HarmonyPostfix]
        public static void Start(Sprinter __instance)
        {
            StaticData.clientInstance.localPlayerBus = __instance.gameObject;

            GameObject l_modelRoot = __instance.gameObject.transform.Find("Model").gameObject;
            _wheelFL = l_modelRoot.transform.Find("wheel1").gameObject;

            GameObject l_wheelFRRoot = l_modelRoot.transform.Find("FR").gameObject;
            _wheelFR = l_wheelFRRoot.transform.Find("wheel2").gameObject;

            GameObject l_wheelRLRoot = l_modelRoot.transform.Find("RL").gameObject;
            _wheelRL = l_wheelRLRoot.transform.Find("wheel4").gameObject;

            GameObject l_wheelRRRoot = l_modelRoot.transform.Find("RR").gameObject;
            _wheelRR = l_wheelRRRoot.transform.Find("wheel3").gameObject;
        }

        [HarmonyPatch(typeof(Sprinter), "Update")]
        [HarmonyPostfix]
        public static void Update()
        {
            StaticData.clientInstance.localWheelFLRotation = _wheelFL.transform.rotation;
            StaticData.clientInstance.localWheelFRRotation = _wheelFR.transform.rotation;
            StaticData.clientInstance.localWheelRLRotation = _wheelRL.transform.rotation;
            StaticData.clientInstance.localWheelRRRotation = _wheelRR.transform.rotation;
        }
    }
}
