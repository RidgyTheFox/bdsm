using HarmonyLib;

namespace BDSM.Patches
{
    class BusControl_Patch
    {
        private static int _oldBlinkersState;
        private static bool _oldHighBeamLightsState;
        private static bool _oldEngineState;
        private static bool _isReverseGearWasEnabledAndSynced;
        private static bool _isReverseGearWasDisabledAndSunced;
        private static bool _oldInsideLightsState;
        private static bool _oldDriverLightsState;

        [HarmonyPatch(typeof(BusControl), "Start")]
        [HarmonyPostfix]
        public static void Start(BusControl __instance)
        {
            _oldEngineState = __instance.DvigVukl;
            _oldBlinkersState = __instance.povorotniki;
            _oldHighBeamLightsState = __instance.LightFary.activeSelf;
            _isReverseGearWasEnabledAndSynced = false;
            _isReverseGearWasDisabledAndSunced = false;
            _oldInsideLightsState = __instance.LightSalon.activeSelf;
            _oldDriverLightsState = __instance.LightSpeedometer.activeSelf;
        }

        [HarmonyPatch(typeof(BusControl), "Update")]
        [HarmonyPostfix]
        public static void Update(BusControl __instance)
        {
            if (!__instance.DvigVukl != _oldEngineState)
            {
                _oldEngineState = !__instance.DvigVukl;
                StaticData.clientInstance.TriggerBusAction("engine", _oldEngineState);
            }
            if (__instance.povorotniki != _oldBlinkersState)
            {
                _oldBlinkersState = __instance.povorotniki;
                StaticData.clientInstance.TriggerBlinkers(_oldBlinkersState);
            }
            if (__instance.LightFary.activeSelf != _oldHighBeamLightsState)
            {
                _oldHighBeamLightsState = __instance.LightFary.activeSelf;
                StaticData.clientInstance.TriggerBusAction("highBeamLights", _oldHighBeamLightsState);
            }
            if (__instance.LightSalon.activeSelf != _oldInsideLightsState)
            {
                _oldInsideLightsState = __instance.LightSalon.activeSelf;
                StaticData.clientInstance.TriggerBusAction("insideLights", _oldInsideLightsState);
            }
            if (__instance.LightSpeedometer.activeSelf != _oldDriverLightsState)
            {
                _oldDriverLightsState = __instance.LightSpeedometer.activeSelf;
                StaticData.clientInstance.TriggerBusAction("driverLights", _oldDriverLightsState);
            }
            if (__instance.gear == -1 && !_isReverseGearWasEnabledAndSynced)
            {
                _isReverseGearWasEnabledAndSynced = true;
                _isReverseGearWasDisabledAndSunced = false;
                StaticData.clientInstance.TriggerBusAction("reverseGear", true);
            }
            else if (__instance.gear != -1 && !_isReverseGearWasDisabledAndSunced)
            {
                _isReverseGearWasEnabledAndSynced = false;
                _isReverseGearWasDisabledAndSunced = true;
                StaticData.clientInstance.TriggerBusAction("reverseGear", false);
            }
        }
    }
}
