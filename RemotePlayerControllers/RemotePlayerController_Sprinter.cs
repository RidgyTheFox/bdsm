using UnityEngine;

namespace BDSM.RemotePlayerControllers
{
    class RemotePlayerController_Sprinter : RemotePlayerControllerBase
    {
        private GameObject _wheelFL;
        private GameObject _wheelFR;
        private GameObject _wheelRL;
        private GameObject _wheelRR;

        void Awake()
        {
            Color _tailLightsColor = new Color(0.8f, 0.0f, 0.0f, 1.0f);
            Color _blinkerColor = new Color(1.0f, 0.5f, 0.0f);
            Color _sideLightColor = new Color(0.6549f, 0.3804f, 0.1569f);
            Color _headLightColor = new Color(0.9854f, 1.0f, 0.8443f, 1.0f);
            Color _insideLightColor = new Color(0.8469f, 0.9811f, 0.9619f, 1.0f);
            Color _driverLightColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

            SetFlyingNicknameYOffset(2.8f);
            _vehicleGroundOffset = new Vector3(0.0f, -0.4f, 0.0f);

            GameObject l_modelRoot = gameObject.transform.Find("Model").gameObject;
            _wheelFL = l_modelRoot.transform.Find("wheel1").gameObject;

            GameObject l_wheelFRRoot = l_modelRoot.transform.Find("FR").gameObject;
            _wheelFR = l_wheelFRRoot.transform.Find("wheel2").gameObject;

            GameObject l_wheelRLRoot = l_modelRoot.transform.Find("RL").gameObject;
            _wheelRL = l_wheelRLRoot.transform.Find("wheel4").gameObject;

            GameObject l_wheelRRRoot = l_modelRoot.transform.Find("RR").gameObject;
            _wheelRR = l_wheelRRRoot.transform.Find("wheel3").gameObject;

            #region Taillights.

            #region Rear lights.
            _rearLightsRoot = new GameObject("RearLights");
            _rearLightsRoot.transform.parent = this.gameObject.transform;
            _rearLightsRoot.transform.localPosition = new Vector3(0.0f, 1.225f, -3.28f);

            GameObject _leftRear = new GameObject("RearLight_Left");
            _leftRear.transform.parent = _rearLightsRoot.transform;
            _leftRear.transform.localPosition = new Vector3(-0.9f, 0.0f, 0.0f);
            Light _rearLeftLightComponent = _leftRear.AddComponent<Light>();
            _rearLeftLightComponent.color = _tailLightsColor;
            _rearLeftLightComponent.intensity = 5;
            _rearLeftLightComponent.range = 0.12f;

            GameObject _rightRear = new GameObject("RearLight_Right");
            _rightRear.transform.parent = _rearLightsRoot.transform;
            _rightRear.transform.localPosition = new Vector3(0.9f, 0.0f, 0.0f);
            Light _rearRightLightComponent = _rightRear.AddComponent<Light>();
            _rearRightLightComponent.color = _tailLightsColor;
            _rearRightLightComponent.intensity = 5;
            _rearRightLightComponent.range = 0.12f;
            #endregion

            #region Backward lights.
            _backwardLightsRoot = new GameObject("BackwardLights");
            _backwardLightsRoot.transform.parent = this.gameObject.transform;
            _backwardLightsRoot.transform.localPosition = new Vector3(0.0f, 1.1f, -3.28f);

            GameObject _leftBackward = new GameObject("BackwardLight_Left");
            _leftBackward.transform.parent = _backwardLightsRoot.transform;
            _leftBackward.transform.localPosition = new Vector3(-0.9f, 0.0f, 0.0f);
            Light _leftBackwardLightComponent = _leftBackward.AddComponent<Light>();
            _leftBackwardLightComponent.color = Color.white;
            _leftBackwardLightComponent.intensity = 5;
            _leftBackwardLightComponent.range = 0.12f;

            GameObject _rightBackward = new GameObject("BackwardLight_Right");
            _rightBackward.transform.parent = _backwardLightsRoot.transform;
            _rightBackward.transform.localPosition = new Vector3(0.9f, 0.0f, 0.0f);
            Light _rightBackwardLightComponent = _rightBackward.AddComponent<Light>();
            _rightBackwardLightComponent.color = Color.white;
            _rightBackwardLightComponent.intensity = 5;
            _rightBackwardLightComponent.range = 0.12f;
            #endregion

            #region Rear blinkers.
            GameObject _rearBlinkersRoot = new GameObject("RearBlinkers");
            _rearBlinkersRoot.transform.parent = this.gameObject.transform;
            _rearBlinkersRoot.transform.localPosition = new Vector3(0.0f, 1.0f, -3.28f);

            _rearLeftBlinker = new GameObject("RearLeftBlinker");
            _rearLeftBlinker.transform.parent = _rearBlinkersRoot.transform;
            _rearLeftBlinker.transform.localPosition = new Vector3(-0.9f, 0.0f, 0.0f);
            Light _rearLeftBlinkerLightComponent = _rearLeftBlinker.AddComponent<Light>();
            _rearLeftBlinkerLightComponent.color = _blinkerColor;
            _rearLeftBlinkerLightComponent.intensity = 5;
            _rearLeftBlinkerLightComponent.range = 0.12f;

            _rearRightBlinker = new GameObject("RearRighttBlinker");
            _rearRightBlinker.transform.parent = _rearBlinkersRoot.transform;
            _rearRightBlinker.transform.localPosition = new Vector3(0.9f, 0.0f, 0.0f);
            Light _rearRightBlinkerLightComponent = _rearRightBlinker.AddComponent<Light>();
            _rearRightBlinkerLightComponent.color = _blinkerColor;
            _rearRightBlinkerLightComponent.intensity = 5;
            _rearRightBlinkerLightComponent.range = 0.12f;
            #endregion

            #region Brake lights.
            _brakingLightsRoot = new GameObject("BrakingLights");
            _brakingLightsRoot.transform.parent = this.gameObject.transform;
            _brakingLightsRoot.transform.localPosition = new Vector3(0.0f, 0.9f, -3.28f);


            GameObject _leftBrake = new GameObject("BrakeLight_Left");
            _leftBrake.transform.parent = _brakingLightsRoot.transform;
            _leftBrake.transform.localPosition = new Vector3(-0.9f, 0.0f, 0.0f);
            Light _leftLightComponent = _leftBrake.AddComponent<Light>();
            _leftLightComponent.color = _tailLightsColor;
            _leftLightComponent.intensity = 5;
            _leftLightComponent.range = 0.138f;

            GameObject _rightBrake = new GameObject("BrakeLight_Right");
            _rightBrake.transform.parent = _brakingLightsRoot.transform;
            _rightBrake.transform.localPosition = new Vector3(0.9f, 0.0f, 0.0f);
            Light _rightLightComponent = _rightBrake.AddComponent<Light>();
            _rightLightComponent.color = _tailLightsColor;
            _rightLightComponent.intensity = 5;
            _rightLightComponent.range = 0.138f;

            GameObject _topBrake = new GameObject("BrakeLight_Top");
            _topBrake.transform.parent = _brakingLightsRoot.transform;
            _topBrake.transform.localPosition = new Vector3(0.0f, 1.58f, 0.015f);
            Light _topLightComponent = _topBrake.AddComponent<Light>();
            _topLightComponent.color = _tailLightsColor;
            _topLightComponent.intensity = 5;
            _topLightComponent.range = 0.138f;
            #endregion

            #endregion

            #region Side lights.

            GameObject _sideLightsRoot = new GameObject("SideLightsRoot");
            _sideLightsRoot.transform.parent = this.gameObject.transform;
            _sideLightsRoot.transform.position = new Vector3(0.0f, 1.05f, 0.0f);

            #region Side blinkers.
            _middleLeftBlinker = new GameObject("MiddleLeftBlinker");
            _middleLeftBlinker.transform.parent = _sideLightsRoot.transform;
            _middleLeftBlinker.transform.localPosition = new Vector3(-1.0f, 0.0f, 2.38f);
            Light _middleLeftBlinker_LC = _middleLeftBlinker.AddComponent<Light>();
            _middleLeftBlinker_LC.color = _blinkerColor;
            _middleLeftBlinker_LC.intensity = 5;
            _middleLeftBlinker_LC.range = 0.13f;

            _middleRightBlinker = new GameObject("MiddleRightBlinker");
            _middleRightBlinker.transform.parent = _sideLightsRoot.transform;
            _middleRightBlinker.transform.localPosition = new Vector3(1.0f, 0.0f, 2.38f);
            Light _middleRightBlinker_LC = _middleRightBlinker.AddComponent<Light>();
            _middleRightBlinker_LC.color = _blinkerColor;
            _middleRightBlinker_LC.intensity = 5;
            _middleRightBlinker_LC.range = 0.13f;
            #endregion

            #region Lights.
            this._sideLights = new GameObject("SideLights");
            this._sideLights.transform.parent = _sideLightsRoot.transform;
            this._sideLights.transform.localPosition = new Vector3(0.0f, -0.46f, 0.0f);

            GameObject _sideLight_FrontLeft = new GameObject("SL_FrontLeft");
            _sideLight_FrontLeft.transform.parent = this._sideLights.transform;
            _sideLight_FrontLeft.transform.localPosition = new Vector3(-0.93f, 0.0f, 1.14f);
            GameObject _sideLight_FrontRight = new GameObject("SL_FrontRight");
            _sideLight_FrontRight.transform.parent = this._sideLights.transform;
            _sideLight_FrontRight.transform.localPosition = new Vector3(0.93f, 0.0f, 1.14f);
            GameObject _sideLight_MiddleLeft = new GameObject("SL_MiddleLeft");
            _sideLight_MiddleLeft.transform.parent = this._sideLights.transform;
            _sideLight_MiddleLeft.transform.localPosition = new Vector3(-0.94f, 0.025f, -0.9f);
            GameObject _sideLight_MiddleRight = new GameObject("SL_MiddleRight");
            _sideLight_MiddleRight.transform.parent = this._sideLights.transform;
            _sideLight_MiddleRight.transform.localPosition = new Vector3(0.94f, 0.025f, -0.9f);
            GameObject _sideLight_RearLeft = new GameObject("SL_RearLeft");
            _sideLight_RearLeft.transform.parent = this._sideLights.transform;
            _sideLight_RearLeft.transform.localPosition = new Vector3(-0.94f, 0.055f, -2.82f);
            GameObject _sideLight_RearRight = new GameObject("SL_RearRight");
            _sideLight_RearRight.transform.parent = this._sideLights.transform;
            _sideLight_RearRight.transform.localPosition = new Vector3(0.94f, 0.055f, -2.82f);

            Light _sideLight_FrontLeft_LC = _sideLight_FrontLeft.AddComponent<Light>();
            Light _sideLight_FrontRight_LC = _sideLight_FrontRight.AddComponent<Light>();
            Light _sideLight_MiddleLeft_LC = _sideLight_MiddleLeft.AddComponent<Light>();
            Light _sideLight_MiddleRight_LC = _sideLight_MiddleRight.AddComponent<Light>();
            Light _sideLight_RearLeft_LC = _sideLight_RearLeft.AddComponent<Light>();
            Light _sideLight_RearRight_LC = _sideLight_RearRight.AddComponent<Light>();

            _sideLight_FrontLeft_LC.color = _sideLightColor;
            _sideLight_FrontRight_LC.color = _sideLightColor;
            _sideLight_MiddleLeft_LC.color = _sideLightColor;
            _sideLight_MiddleRight_LC.color = _sideLightColor;
            _sideLight_RearLeft_LC.color = _sideLightColor;
            _sideLight_RearRight_LC.color = _sideLightColor;

            _sideLight_FrontLeft_LC.intensity = 4;
            _sideLight_FrontRight_LC.intensity = 4;
            _sideLight_MiddleLeft_LC.intensity = 4;
            _sideLight_MiddleRight_LC.intensity = 4;
            _sideLight_RearLeft_LC.intensity = 4;
            _sideLight_RearRight_LC.intensity = 4;

            _sideLight_FrontLeft_LC.range = 0.1f;
            _sideLight_FrontRight_LC.range = 0.1f;
            _sideLight_MiddleLeft_LC.range = 0.1f;
            _sideLight_MiddleRight_LC.range = 0.1f;
            _sideLight_RearLeft_LC.range = 0.1f;
            _sideLight_RearRight_LC.range = 0.1f;

            #endregion

            #endregion

            #region Headlights.

            GameObject _headLightsRootObject = new GameObject("HeadlightsRoot");
            _headLightsRootObject.transform.parent = this.gameObject.transform;
            _headLightsRootObject.transform.localPosition = new Vector3(0.0f, 0.87f, 3.0f);

            #region HighBeam Lights.
            _headLightsHighBeam = new GameObject("Headlights_HighBeam");
            _headLightsHighBeam.transform.parent = _headLightsRootObject.transform;
            _headLightsHighBeam.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

            GameObject _headLightsHighNeam_LeftLight = new GameObject("Headlights_HighBeam_Left");
            _headLightsHighNeam_LeftLight.transform.parent = _headLightsHighBeam.transform;
            _headLightsHighNeam_LeftLight.transform.localPosition = new Vector3(-0.7f, 0.0f, 0.07f);
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
            _headLightsHighNeam_RightLight.transform.localPosition = new Vector3(0.7f, 0.0f, 0.07f);
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

            #region LowBeam Lights.

            _headlightsLowBeam = new GameObject("Headlights_LowBeam");
            _headlightsLowBeam.transform.parent = _headLightsRootObject.transform;
            _headlightsLowBeam.transform.localPosition = new Vector3(0.0f, -0.016f, 0.12f);

            GameObject _headLightsLowBeam_Left = new GameObject("Headlights_LowBeam_Left");
            _headLightsLowBeam_Left.transform.parent = _headlightsLowBeam.transform;
            _headLightsLowBeam_Left.transform.localPosition = new Vector3(-0.57f, 0.0f, 0.0f);
            Light _headLightsLowbeam_Left_LC = _headLightsLowBeam_Left.AddComponent<Light>();
            _headLightsLowbeam_Left_LC.color = _headLightColor;
            _headLightsLowbeam_Left_LC.intensity = 10;
            _headLightsLowbeam_Left_LC.range = 0.1f;

            GameObject _headLightsLowBeam_Right = new GameObject("Headlights_LowBeam_Right");
            _headLightsLowBeam_Right.transform.parent = _headlightsLowBeam.transform;
            _headLightsLowBeam_Right.transform.localPosition = new Vector3(0.57f, 0.0f, 0.0f);
            Light _headLightsLowbeam_Right_LC = _headLightsLowBeam_Right.AddComponent<Light>();
            _headLightsLowbeam_Right_LC.color = _headLightColor;
            _headLightsLowbeam_Right_LC.intensity = 10;
            _headLightsLowbeam_Right_LC.range = 0.1f;

            #endregion

            #region Blinkers.
            _frontLeftBlinker = new GameObject("FrontLeftBlinker");
            _frontLeftBlinker.transform.parent = _headLightsRootObject.transform;
            _frontLeftBlinker.transform.localPosition = new Vector3(-0.82f, 0.0f, 0.0f);
            Light _frontLeftBlinker_LC = _frontLeftBlinker.AddComponent<Light>();
            _frontLeftBlinker_LC.color = _blinkerColor;
            _frontLeftBlinker_LC.range = 0.1f;
            _frontLeftBlinker_LC.intensity = 8;

            _frontRightBlinker = new GameObject("FrontRightBlinker");
            _frontRightBlinker.transform.parent = _headLightsRootObject.transform;
            _frontRightBlinker.transform.localPosition = new Vector3(0.82f, 0.0f, 0.0f);
            Light _frontRightBlinker_LC = _frontRightBlinker.AddComponent<Light>();
            _frontRightBlinker_LC.color = _blinkerColor;
            _frontRightBlinker_LC.range = 0.1f;
            _frontRightBlinker_LC.intensity = 8;
            #endregion

            #endregion

            #region Inside lights.
            _insideLightRoot = new GameObject("InsideLightsRoot");
            _insideLightRoot.transform.parent = this.gameObject.transform;
            _insideLightRoot.transform.localPosition = new Vector3(0.0f, 2.0f, 0.0f);

            GameObject _insideLightFront = new GameObject("InsideLight_Front");
            _insideLightFront.transform.parent = _insideLightRoot.transform;
            _insideLightFront.transform.localPosition = new Vector3(0.0f, 0.0f, 0.25f);
            Light _insideLightFrontLC = _insideLightFront.AddComponent<Light>();
            _insideLightFrontLC.color = _insideLightColor;
            _insideLightFrontLC.intensity = 0.8f;
            _insideLightFrontLC.range = 5.0f;

            GameObject _insideLightMiddle = new GameObject("InsideLight_Middle");
            _insideLightMiddle.transform.parent = _insideLightRoot.transform;
            _insideLightMiddle.transform.localPosition = new Vector3(0.0f, 0.0f, -1.1f);
            Light _insideLightMiddleLC = _insideLightMiddle.AddComponent<Light>();
            _insideLightMiddleLC.color = _insideLightColor;
            _insideLightMiddleLC.intensity = 0.8f;
            _insideLightMiddleLC.range = 5.0f;

            GameObject _insideLightRear = new GameObject("InsideLight_Rear");
            _insideLightRear.transform.parent = _insideLightRoot.transform;
            _insideLightRear.transform.localPosition = new Vector3(0.0f, 0.0f, -2.6f);
            Light _insideLightRearLC = _insideLightRear.AddComponent<Light>();
            _insideLightRearLC.color = _insideLightColor;
            _insideLightRearLC.intensity = 0.8f;
            _insideLightRearLC.range = 5.0f;


            _driverLightRoot = new GameObject("DriverLightsRoot");
            _driverLightRoot.transform.parent = this.gameObject.transform;
            _driverLightRoot.transform.localPosition = new Vector3(-0.22f, 2.14f, 1.6f);
            Light _driverLightLC = _driverLightRoot.AddComponent<Light>();
            _driverLightLC.color = _driverLightColor;
            _driverLightLC.intensity = 1.0f;
            _driverLightLC.range = 1.0f;
            #endregion
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

        protected override void OnTriggerToggleAction(string l_actionName)
        {
            Debug.Log($"SPRINTER_CONTROLLER: Custom trigger on child class: {l_actionName}!");
        }
    }
}
