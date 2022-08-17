using UnityEngine;
using HarmonyLib;

namespace BDSM.Patches.Buses
{
    class PAZ3205_Patch
    {
        private static GameObject _wheelFL;
        private static GameObject _wheelFR;
        private static GameObject _wheelRL;
        private static GameObject _wheelRR;

        private static void FindAndAssignWheelsForLiaz677(PAZ3205 __instance)
        {
            GameObject l_modelRoot = __instance.gameObject.transform.Find("Mesh").gameObject;
            GameObject l_modelRootChild = l_modelRoot.transform.Find("Liaz-677").gameObject;
            if (l_modelRootChild == null)
            {
                Debug.LogError("PAZ3205_PATCH: Cannot find model's root game object! Wheels will not be synchronized!");
                return;
            }

            GameObject l_wheelFLRoot = l_modelRootChild.transform.Find("FL").gameObject;
            _wheelFL = l_wheelFLRoot.transform.Find("Bus_Front_Wheel_L").gameObject;

            GameObject l_wheelFRRoot = l_modelRootChild.transform.Find("FR").gameObject;
            _wheelFR = l_wheelFRRoot.transform.Find("Bus_Front_Wheel_R").gameObject;

            GameObject l_wheelRLRoot = l_modelRootChild.transform.Find("RL").gameObject;
            _wheelRL = l_wheelRLRoot.transform.Find("Bus_Back_Wheel_L").gameObject;

            GameObject l_wheelRRRoot = l_modelRootChild.transform.Find("RR").gameObject;
            _wheelRR = l_wheelRRRoot.transform.Find("Bus_Back_Wheel_R").gameObject;
        }

        [HarmonyPatch(typeof(PAZ3205), "Start")]
        [HarmonyPostfix]
        public static void Start(PAZ3205 __instance)
        {
            StaticData.clientInstance.localPlayerBus = __instance.gameObject;
            FindAndAssignWheelsForLiaz677(__instance);
        }

        [HarmonyPatch(typeof(PAZ3205), "Update")]
        [HarmonyPostfix]
        public static void Update(PAZ3205 __instance)
        {
            if (_wheelFL == null && _wheelFR == null && _wheelRL == null && _wheelRR == null)
                return;

            StaticData.clientInstance.localWheelFLRotation = _wheelFL.transform.rotation;
            StaticData.clientInstance.localWheelFRRotation = _wheelFR.transform.rotation;
            StaticData.clientInstance.localWheelRLRotation = _wheelRL.transform.rotation;
            StaticData.clientInstance.localWheelRRRotation = _wheelRR.transform.rotation;
        }

    }
}
