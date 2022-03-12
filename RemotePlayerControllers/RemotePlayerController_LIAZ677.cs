using UnityEngine;

namespace BDSM.RemotePlayerControllers
{
    class RemotePlayerController_LIAZ677 : RemotePlayerControllerBase
    {
        Color _tailLightsColor = new Color(0.8f, 0.0f, 0.0f, 1.0f);
        Color _blinkerColor = new Color(1.0f, 0.5f, 0.0f);
        Color _sideLightColor = new Color(0.6549f, 0.3804f, 0.1569f);
        Color _headLightColor = new Color(0.9854f, 1.0f, 0.8443f, 1.0f);
        Color _insideLightColor = new Color(0.8469f, 0.9811f, 0.9619f, 1.0f);
        Color _driverLightColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        private void Awake()
        {
            SetFlyingNicknameYOffset(2.8f);
            _vehicleGroundOffset = new Vector3(0.0f, -0.4f, 0.0f);
        }

        protected override void OnTriggerToggleAction(string l_actionName)
        {
        }

        protected override void OnWheelUpdate(Quaternion l_wheelFL, Quaternion l_wheelFR, Quaternion l_wheelRL, Quaternion l_wheelRR)
        {
        }
    }
}
