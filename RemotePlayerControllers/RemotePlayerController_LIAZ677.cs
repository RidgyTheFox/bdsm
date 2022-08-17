using UnityEngine;

namespace BDSM.RemotePlayerControllers
{
    class RemotePlayerController_LIAZ677 : RemotePlayerControllerBase
    {
        private GameObject _wheelFL;
        private GameObject _wheelFR;
        private GameObject _wheelRL;
        private GameObject _wheelRR;

        private Color _tailLightsColor = new Color(0.8f, 0.0f, 0.0f, 1.0f);
        private Color _blinkerColor = new Color(1.0f, 0.5748f, 0.0368f, 1.0f);
        private Color _sideLightColor = new Color(0.6549f, 0.3804f, 0.1569f);
        private Color _headLightColor = new Color(0.9759f, 1.0f, 0.75f, 1.0f);
        private Color _insideLightColor = new Color(1.0f, 0.9465f, 0.5147f, 1.0f);
        private Color _driverLightColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        private void Awake()
        {
            SetFlyingNicknameYOffset(3.4f);
            _vehicleGroundOffset = new Vector3(0.0f, -0.1f, 0.75f);

            GameObject l_modelRoot = gameObject.transform.Find("Liaz-677").gameObject;
            GameObject l_wheelFLRoot = l_modelRoot.transform.Find("FL").gameObject;
            _wheelFL = l_wheelFLRoot.transform.Find("Bus_Front_Wheel_L").gameObject;

            GameObject l_wheelFRRoot = l_modelRoot.transform.Find("FR").gameObject;
            _wheelFR = l_wheelFRRoot.transform.Find("Bus_Front_Wheel_R").gameObject;

            GameObject l_wheelRLRoot = l_modelRoot.transform.Find("RL").gameObject;
            _wheelRL = l_wheelRLRoot.transform.Find("Bus_Back_Wheel_L").gameObject;

            GameObject l_wheelRRRoot = l_modelRoot.transform.Find("RR").gameObject;
            _wheelRR = l_wheelRRRoot.transform.Find("Bus_Back_Wheel_R").gameObject;

            #region Taillights.

            #region Rear lights.
            _rearLightsRoot = new GameObject("RearLights");
            _rearLightsRoot.transform.parent = this.gameObject.transform;
            _rearLightsRoot.transform.localPosition = new Vector3(0.0f, 1.31f, -5.48f);

            GameObject _rearLightLeft = new GameObject("RearLight_Left");
            _rearLightLeft.transform.parent = _rearLightsRoot.transform;
            _rearLightLeft.transform.localPosition = new Vector3(-0.87f, 0.0f, 0.0f);
            Light _rearLeftLightComponent = _rearLightLeft.AddComponent<Light>();
            _rearLeftLightComponent.color = _tailLightsColor;
            _rearLeftLightComponent.intensity = 5;
            _rearLeftLightComponent.range = 0.1364f;

            GameObject _rearLightRight = new GameObject("RearLight_Right");
            _rearLightRight.transform.parent = _rearLightsRoot.transform;
            _rearLightRight.transform.localPosition = new Vector3(0.87f, 0.0f, 0.0f);
            Light _rearRightLightComponent = _rearLightRight.AddComponent<Light>();
            _rearRightLightComponent.color = _tailLightsColor;
            _rearRightLightComponent.intensity = 5;
            _rearRightLightComponent.range = 0.1364f;
            #endregion

            #region Backward lights.
            _backwardLightsRoot = new GameObject("BackwardLights");
            _backwardLightsRoot.transform.parent = this.gameObject.transform;
            _backwardLightsRoot.transform.localPosition = new Vector3(0.0f, 1.025f, -5.48f);

            GameObject _leftBackward = new GameObject("BackwardLight_Left");
            _leftBackward.transform.parent = _backwardLightsRoot.transform;
            _leftBackward.transform.localPosition = new Vector3(-0.87f, 0.0f, 0.0f);
            Light _leftBackwardLightComponent = _leftBackward.AddComponent<Light>();
            _leftBackwardLightComponent.color = Color.white;
            _leftBackwardLightComponent.intensity = 5;
            _leftBackwardLightComponent.range = 0.1364f;

            GameObject _rightBackward = new GameObject("BackwardLight_Right");
            _rightBackward.transform.parent = _backwardLightsRoot.transform;
            _rightBackward.transform.localPosition = new Vector3(0.87f, 0.0f, 0.0f);
            Light _rightBackwardLightComponent = _rightBackward.AddComponent<Light>();
            _rightBackwardLightComponent.color = Color.white;
            _rightBackwardLightComponent.intensity = 5;
            _rightBackwardLightComponent.range = 0.1364f;
            #endregion

            #region Rear blinkers.
            GameObject _rearBlinkersRoot = new GameObject("RearBlinkers");
            _rearBlinkersRoot.transform.parent = this.gameObject.transform;
            _rearBlinkersRoot.transform.localPosition = new Vector3(0.0f, 1.24f, -5.48f);

            _rearLeftBlinker = new GameObject("RearLeftBlinker");
            _rearLeftBlinker.transform.parent = _rearBlinkersRoot.transform;
            _rearLeftBlinker.transform.localPosition = new Vector3(-0.87f, 0.0f, 0.0f);
            Light _rearLeftBlinkerLightComponent = _rearLeftBlinker.AddComponent<Light>();
            _rearLeftBlinkerLightComponent.color = _blinkerColor;
            _rearLeftBlinkerLightComponent.intensity = 5;
            _rearLeftBlinkerLightComponent.range = 0.1364f;

            _rearRightBlinker = new GameObject("RearRighttBlinker");
            _rearRightBlinker.transform.parent = _rearBlinkersRoot.transform;
            _rearRightBlinker.transform.localPosition = new Vector3(0.87f, 0.0f, 0.0f);
            Light _rearRightBlinkerLightComponent = _rearRightBlinker.AddComponent<Light>();
            _rearRightBlinkerLightComponent.color = _blinkerColor;
            _rearRightBlinkerLightComponent.intensity = 5;
            _rearRightBlinkerLightComponent.range = 0.1364f;
            #endregion

            #region Brake lights.
            _brakingLightsRoot = new GameObject("BrakingLights");
            _brakingLightsRoot.transform.parent = this.gameObject.transform;
            _brakingLightsRoot.transform.localPosition = new Vector3(0.0f, 1.12f, -5.48f);


            GameObject _leftBrake = new GameObject("BrakeLight_Left");
            _leftBrake.transform.parent = _brakingLightsRoot.transform;
            _leftBrake.transform.localPosition = new Vector3(-0.87f, 0.0f, 0.0f);
            Light _leftLightComponent = _leftBrake.AddComponent<Light>();
            _leftLightComponent.color = _tailLightsColor;
            _leftLightComponent.intensity = 5;
            _leftLightComponent.range = 0.1364f;

            GameObject _rightBrake = new GameObject("BrakeLight_Right");
            _rightBrake.transform.parent = _brakingLightsRoot.transform;
            _rightBrake.transform.localPosition = new Vector3(0.87f, 0.0f, 0.0f);
            Light _rightLightComponent = _rightBrake.AddComponent<Light>();
            _rightLightComponent.color = _tailLightsColor;
            _rightLightComponent.intensity = 5;
            _rightLightComponent.range = 0.1364f;
            #endregion

            #endregion

            #region Side lights.
            _sideLightsRoot = new GameObject("SideLightsRoot");
            _sideLightsRoot.transform.parent = gameObject.transform;
            _sideLightsRoot.transform.localPosition = new Vector3(0.0f, 2.7f, 0.0f);

            #region Side lights.
            GameObject _frontLeftSideLight = new GameObject("FrontSideLight_Left");
            _frontLeftSideLight.transform.parent = _sideLightsRoot.transform;
            _frontLeftSideLight.transform.localPosition = new Vector3(-1.03f, 0.0f, 4.6f);
            Light _frontLeftSideLightLC = _frontLeftSideLight.AddComponent<Light>();
            _frontLeftSideLightLC.color = new Color(0.9853f, 0.9853f, 0.9853f, 1.0f);
            _frontLeftSideLightLC.intensity = 5;
            _frontLeftSideLightLC.range = 0.1364f;

            GameObject _frontRightSideLight = new GameObject("FrontSideLight_Right");
            _frontRightSideLight.transform.parent = _sideLightsRoot.transform;
            _frontRightSideLight.transform.localPosition = new Vector3(1.03f, 0.0f, 4.6f);
            Light _frontRightSideLightLC = _frontRightSideLight.AddComponent<Light>();
            _frontRightSideLightLC.color = new Color(0.9853f, 0.9853f, 0.9853f, 1.0f);
            _frontRightSideLightLC.intensity = 5;
            _frontRightSideLightLC.range = 0.1364f;

            GameObject _rearLeftSideLight = new GameObject("RearSideLight_Left");
            _rearLeftSideLight.transform.parent = _sideLightsRoot.transform;
            _rearLeftSideLight.transform.localPosition = new Vector3(-0.825f, 0.0f, -5.45f);
            Light _rearLeftSideLightLC = _rearLeftSideLight.AddComponent<Light>();
            _rearLeftSideLightLC.color = new Color(0.8015f, 0.0f, 0.0f, 1.0f);
            _rearLeftSideLightLC.intensity = 5;
            _rearLeftSideLightLC.range = 0.1364f;

            GameObject _rearRightSideLight = new GameObject("RearSideLight_Right");
            _rearRightSideLight.transform.parent = _sideLightsRoot.transform;
            _rearRightSideLight.transform.localPosition = new Vector3(0.825f, 0.0f, -5.45f);
            Light _rearRightSideLightLC = _rearRightSideLight.AddComponent<Light>();
            _rearRightSideLightLC.color = new Color(0.8015f, 0.0f, 0.0f, 1.0f);
            _rearRightSideLightLC.intensity = 5;
            _rearRightSideLightLC.range = 0.1364f;
            #endregion

            #region Side blinkers.
            _middleLeftBlinker = new GameObject("MiddleLeftBlinker");
            _middleLeftBlinker.transform.parent = _sideLightsRoot.transform;
            _middleLeftBlinker.transform.localPosition = new Vector3(-1.29f, -1.2f, 3.35f);
            Light _middleLeftBlinkerLC = _middleLeftBlinker.AddComponent<Light>();
            _middleLeftBlinkerLC.color = _blinkerColor;
            _middleLeftBlinkerLC.intensity = 5.0f;
            _middleLeftBlinkerLC.range = 0.1364f;

            _middleRightBlinker = new GameObject("MiddleRightBlinker");
            _middleRightBlinker.transform.parent = _sideLightsRoot.transform;
            _middleRightBlinker.transform.localPosition = new Vector3(1.29f, -1.2f, 3.75f);
            Light _middleRightBlinkerLC = _middleRightBlinker.AddComponent<Light>();
            _middleRightBlinkerLC.color = _blinkerColor;
            _middleRightBlinkerLC.intensity = 5.0f;
            _middleRightBlinkerLC.range = 0.1364f;
            #endregion

            #endregion

            #region Headlights.

            GameObject _headLightsRootObject = new GameObject("HeadlightsRoot");
            _headLightsRootObject.transform.parent = this.gameObject.transform;
            _headLightsRootObject.transform.localPosition = new Vector3(0.0f, 1.26f, 4.965f);

            #region HighBeam Lights.
            _headLightsHighBeam = new GameObject("Headlights_HighBeam");
            _headLightsHighBeam.transform.parent = _headLightsRootObject.transform;
            _headLightsHighBeam.transform.localPosition = new Vector3(0.0f, -0.37f, 0.19f);
            _headLightsHighBeam.transform.localRotation.eulerAngles.Set(13.0f, 0.0f, 0.0f);

            GameObject _headLightsHighNeam_LeftLight = new GameObject("Headlights_HighBeam_Left");
            _headLightsHighNeam_LeftLight.transform.parent = _headLightsHighBeam.transform;
            _headLightsHighNeam_LeftLight.transform.localPosition = new Vector3(-1.03f, 0.0f, 0.0f);
            Light _headLightsHighBeam_LeftSpotlight = _headLightsHighNeam_LeftLight.AddComponent<Light>();
            _headLightsHighBeam_LeftSpotlight.color = _headLightColor;
            _headLightsHighBeam_LeftSpotlight.range = 20;
            _headLightsHighBeam_LeftSpotlight.intensity = 1.5f;
            _headLightsHighBeam_LeftSpotlight.shadows = LightShadows.Soft;
            _headLightsHighBeam_LeftSpotlight.type = LightType.Spot;
            _headLightsHighBeam_LeftSpotlight.bounceIntensity = 5;
            _headLightsHighBeam_LeftSpotlight.spotAngle = 70;
            _headLightsHighBeam_LeftSpotlight.innerSpotAngle = 53.5154f;
            _headLightsHighBeam_LeftSpotlight.renderMode = LightRenderMode.ForcePixel;

            GameObject _headLightsHighBeam_LeftFakeLight = new GameObject("Headlights_HighBeam_FakeLeft");
            _headLightsHighBeam_LeftFakeLight.transform.parent = _headLightsHighNeam_LeftLight.transform;
            _headLightsHighBeam_LeftFakeLight.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            Light _headLightsHighBeam_LeftFakeLight_LC = _headLightsHighBeam_LeftFakeLight.AddComponent<Light>();
            _headLightsHighBeam_LeftFakeLight_LC.color = _headLightColor;
            _headLightsHighBeam_LeftFakeLight_LC.intensity = 10;
            _headLightsHighBeam_LeftFakeLight_LC.range = 0.3f;

            GameObject _headLightsHighNeam_RightLight = new GameObject("Headlights_HighBeam_Right");
            _headLightsHighNeam_RightLight.transform.parent = _headLightsHighBeam.transform;
            _headLightsHighNeam_RightLight.transform.localPosition = new Vector3(1.03f, 0.0f, 0.07f);
            Light _headLightsHighBeam_RightSpotlight = _headLightsHighNeam_RightLight.AddComponent<Light>();
            _headLightsHighBeam_RightSpotlight.color = _headLightColor;
            _headLightsHighBeam_RightSpotlight.range = 20;
            _headLightsHighBeam_RightSpotlight.intensity = 1.5f;
            _headLightsHighBeam_RightSpotlight.shadows = LightShadows.Soft;
            _headLightsHighBeam_RightSpotlight.type = LightType.Spot;
            _headLightsHighBeam_RightSpotlight.bounceIntensity = 5;
            _headLightsHighBeam_RightSpotlight.spotAngle = 70;
            _headLightsHighBeam_RightSpotlight.innerSpotAngle = 53.5154f;
            _headLightsHighBeam_RightSpotlight.renderMode = LightRenderMode.ForcePixel;

            GameObject _headLightsHighBeam_RightFakeLight = new GameObject("Headlights_HighBeam_FakeRight");
            _headLightsHighBeam_RightFakeLight.transform.parent = _headLightsHighBeam_RightSpotlight.transform;
            _headLightsHighBeam_RightFakeLight.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            Light _headLightsHighBeam_RightFakeLight_LC = _headLightsHighBeam_RightFakeLight.AddComponent<Light>();
            _headLightsHighBeam_RightFakeLight_LC.color = _headLightColor;
            _headLightsHighBeam_RightFakeLight_LC.intensity = 10;
            _headLightsHighBeam_RightFakeLight_LC.range = 0.3f;


            #endregion

            #region Front blinkers.
            _frontLeftBlinker = new GameObject("FrontLeftBlinker");
            _frontLeftBlinker.transform.parent = _headLightsRootObject.transform;
            _frontLeftBlinker.transform.localPosition = new Vector3(-0.78f, 0.0f, 0.0f);
            Light _frontLeftBlinker_LC = _frontLeftBlinker.AddComponent<Light>();
            _frontLeftBlinker_LC.color = _blinkerColor;
            _frontLeftBlinker_LC.range = 0.1364f;
            _frontLeftBlinker_LC.intensity = 5;

            _frontRightBlinker = new GameObject("FrontRightBlinker");
            _frontRightBlinker.transform.parent = _headLightsRootObject.transform;
            _frontRightBlinker.transform.localPosition = new Vector3(0.78f, 0.0f, 0.0f);
            Light _frontRightBlinker_LC = _frontRightBlinker.AddComponent<Light>();
            _frontRightBlinker_LC.color = _blinkerColor;
            _frontRightBlinker_LC.range = 0.1364f;
            _frontRightBlinker_LC.intensity = 5;
            #endregion

            #endregion

            #region Inside lights.

            _insideLightRoot = new GameObject("InsideLightsRoot");
            _insideLightRoot.transform.parent = this.gameObject.transform;
            _insideLightRoot.transform.localPosition = new Vector3(0.0f, 2.36f, 0.0f);

            GameObject _insideLightFront = new GameObject("InsideLight_Front");
            _insideLightFront.transform.parent = _insideLightRoot.transform;
            _insideLightFront.transform.localPosition = new Vector3(0.0f, 0.0f, 2.18f);
            Light _insideLightFrontLC = _insideLightFront.AddComponent<Light>();
            _insideLightFrontLC.color = _insideLightColor;
            _insideLightFrontLC.intensity = 1.0f;
            _insideLightFrontLC.range = 5.0f;

            GameObject _insideLightMiddle = new GameObject("InsideLight_Middle");
            _insideLightMiddle.transform.parent = _insideLightRoot.transform;
            _insideLightMiddle.transform.localPosition = new Vector3(0.0f, 0.0f, -0.5f);
            Light _insideLightMiddleLC = _insideLightMiddle.AddComponent<Light>();
            _insideLightMiddleLC.color = _insideLightColor;
            _insideLightMiddleLC.intensity = 1.0f;
            _insideLightMiddleLC.range = 5.0f;

            GameObject _insideLightRear = new GameObject("InsideLight_Rear");
            _insideLightRear.transform.parent = _insideLightRoot.transform;
            _insideLightRear.transform.localPosition = new Vector3(0.0f, 0.0f, -3.6f);
            Light _insideLightRearLC = _insideLightRear.AddComponent<Light>();
            _insideLightRearLC.color = _insideLightColor;
            _insideLightRearLC.intensity = 1.0f;
            _insideLightRearLC.range = 5.0f;

            _driverLightRoot = new GameObject("DriverLights");
            _driverLightRoot.transform.parent = _insideLightRoot.transform;
            _driverLightRoot.transform.localPosition = new Vector3(-0.7f, 0.54f, 4.26f);
            Light _driverLightLC = _driverLightRoot.AddComponent<Light>();
            _driverLightLC.color = _driverLightColor;
            _driverLightLC.intensity = 1.0f;
            _driverLightLC.range = 3.0f;

            #endregion
        }

        protected override void OnTriggerToggleAction(string l_actionName)
        {
        }

        protected override void OnWheelUpdate(Quaternion l_wheelFL, Quaternion l_wheelFR, Quaternion l_wheelRL, Quaternion l_wheelRR)
        {
            if (_wheelFL == null || _wheelFR == null)
                return;

            _wheelFL.transform.rotation = l_wheelFL;
            _wheelFR.transform.rotation = l_wheelFR;

            if (_wheelRL == null || _wheelRR == null)
                return;

            _wheelRL.transform.rotation = l_wheelRL;
            _wheelRR.transform.rotation = l_wheelRR;
        }
    }
}
