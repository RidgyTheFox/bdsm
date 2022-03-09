using BDSM.Network.NestedTypes;
using UnityEngine;
using System;
using System.Collections;

namespace BDSM.RemotePlayerControllers
{
    public interface RemotePlayerControllerInterface
    {
        public void TriggerToggleAction(string l_actionName);
    }

    public abstract class RemotePlayerControllerBase : MonoBehaviour, RemotePlayerControllerInterface
    {
        private GameObject _flyingNickname;
        private TextMesh _flyingTextMesh;

        private string _playerNickname;

        protected float _blinkersBlinkingPeriod = 14.0f;

        protected Vector3 _vehicleGroundOffset;

        protected Network.NestedTypes.BusState _currentBusState;

        public GameObject _brakingLightsRoot;
        public GameObject _rearLightsRoot;
        public GameObject _backwardLightsRoot;
        public GameObject _rearLeftBlinker;
        public GameObject _rearRightBlinker;
        public GameObject _middleLeftBlinker;
        public GameObject _middleRightBlinker;
        public GameObject _sideLights;
        public GameObject _headlightsLowBeam;
        public GameObject _headLightsHighBeam;
        public GameObject _frontLeftBlinker;
        public GameObject _frontRightBlinker;

        private void Awake()
        {
            _currentBusState = new BusState {
                isBothBlinkersBlinking = false,
                isBraking = false,
                isDriverLightsTurnedOn = false,
                isEngineTurnedOn = false,
                isFrontDoorOpened = false,
                isHighBeamTurnedOn = false,
                isInsideLightsTurnedOn = false,
                isLeftBlinkerBlinking = false,
                isMiddleDoorOpened = false,
                isRearDoorOpened = false,
                isReverseGear = false,
                isRightBlinkerBlinking = false 
            };
        }

        private void Update()
        {
            if (_currentBusState.isEngineTurnedOn)
            {
                if (_headlightsLowBeam != null)
                    _headlightsLowBeam.SetActive(_currentBusState.isEngineTurnedOn);

                if (_sideLights != null)
                    _sideLights.SetActive(_currentBusState.isEngineTurnedOn);

                if (_rearLightsRoot != null)
                    _rearLightsRoot.SetActive(_currentBusState.isEngineTurnedOn);

                if (_brakingLightsRoot != null)
                    _brakingLightsRoot.SetActive(_currentBusState.isBraking);

                if (_backwardLightsRoot != null)
                    _backwardLightsRoot.SetActive(_currentBusState.isReverseGear);

                if (_headLightsHighBeam != null)
                    _headLightsHighBeam.SetActive(_currentBusState.isHighBeamTurnedOn);

                if (_frontLeftBlinker == null || _middleLeftBlinker == null || _rearLeftBlinker == null)
                    return;

                if (_frontRightBlinker == null || _middleRightBlinker == null || _rearRightBlinker == null)
                    return;

                if (_currentBusState.isLeftBlinkerBlinking)
                {
                    _frontLeftBlinker.SetActive(Convert.ToBoolean(Mathf.FloorToInt(Mathf.PingPong(Time.time * _blinkersBlinkingPeriod, 2.0f))));
                    _middleLeftBlinker.SetActive(Convert.ToBoolean(Mathf.FloorToInt(Mathf.PingPong(Time.time * _blinkersBlinkingPeriod, 2.0f))));
                    _rearLeftBlinker.SetActive(Convert.ToBoolean(Mathf.FloorToInt(Mathf.PingPong(Time.time * _blinkersBlinkingPeriod, 2.0f))));
                }
                else if (!_currentBusState.isLeftBlinkerBlinking && !_currentBusState.isBothBlinkersBlinking)
                {
                    _frontLeftBlinker.SetActive(false);
                    _middleLeftBlinker.SetActive(false);
                    _rearLeftBlinker.SetActive(false);
                }

                if (_currentBusState.isRightBlinkerBlinking)
                {
                    _frontRightBlinker.SetActive(Convert.ToBoolean(Mathf.FloorToInt(Mathf.PingPong(Time.time * _blinkersBlinkingPeriod, 2.0f))));
                    _middleRightBlinker.SetActive(Convert.ToBoolean(Mathf.FloorToInt(Mathf.PingPong(Time.time * _blinkersBlinkingPeriod, 2.0f))));
                    _rearRightBlinker.SetActive(Convert.ToBoolean(Mathf.FloorToInt(Mathf.PingPong(Time.time * _blinkersBlinkingPeriod, 2.0f))));
                }
                else if (!_currentBusState.isBothBlinkersBlinking && !_currentBusState.isBothBlinkersBlinking)
                {
                    _frontRightBlinker.SetActive(false);
                    _middleRightBlinker.SetActive(false);
                    _rearRightBlinker.SetActive(false);
                }

                if (_currentBusState.isBothBlinkersBlinking)
                {
                    _frontLeftBlinker.SetActive(Convert.ToBoolean(Mathf.FloorToInt(Mathf.PingPong(Time.time * _blinkersBlinkingPeriod, 2.0f))));
                    _middleLeftBlinker.SetActive(Convert.ToBoolean(Mathf.FloorToInt(Mathf.PingPong(Time.time * _blinkersBlinkingPeriod, 2.0f))));
                    _rearLeftBlinker.SetActive(Convert.ToBoolean(Mathf.FloorToInt(Mathf.PingPong(Time.time * _blinkersBlinkingPeriod, 2.0f))));
                    _frontRightBlinker.SetActive(Convert.ToBoolean(Mathf.FloorToInt(Mathf.PingPong(Time.time * _blinkersBlinkingPeriod, 2.0f))));
                    _middleRightBlinker.SetActive(Convert.ToBoolean(Mathf.FloorToInt(Mathf.PingPong(Time.time * _blinkersBlinkingPeriod, 2.0f))));
                    _rearRightBlinker.SetActive(Convert.ToBoolean(Mathf.FloorToInt(Mathf.PingPong(Time.time * _blinkersBlinkingPeriod, 2.0f))));
                }
                else if (!_currentBusState.isBothBlinkersBlinking && !_currentBusState.isLeftBlinkerBlinking && !_currentBusState.isRightBlinkerBlinking)
                {
                    _frontLeftBlinker.SetActive(false);
                    _middleLeftBlinker.SetActive(false);
                    _rearLeftBlinker.SetActive(false);
                    _frontRightBlinker.SetActive(false);
                    _middleRightBlinker.SetActive(false);
                    _rearRightBlinker.SetActive(false);
                }
            }
            else
            {
                _brakingLightsRoot.SetActive(false);
                _rearLightsRoot.SetActive(false);
                _backwardLightsRoot.SetActive(false);
                _sideLights.SetActive(false);
                _headlightsLowBeam.SetActive(false);
                _headLightsHighBeam.SetActive(false);
                _rearLeftBlinker.SetActive(false);
                _rearRightBlinker.SetActive(false);
                _middleLeftBlinker.SetActive(false);
                _middleRightBlinker.SetActive(false);
                _frontLeftBlinker.SetActive(false);
                _frontRightBlinker.SetActive(false);
            }
        }

        void RemotePlayerControllerInterface.TriggerToggleAction(string l_actionName)
        {
            OnTriggerToggleAction(l_actionName);
        }

        public void UpdatePosition(Network.ClientPackets.RemotePlayer l_player)
        {
            gameObject.transform.position = l_player.state.position + _vehicleGroundOffset;
            gameObject.transform.rotation = l_player.state.rotation;
        }
        private void CreateFlyingNicknameIfNull()
        {
            if (_flyingNickname == null)
            {
                _flyingNickname = new GameObject("FlyingNickname");
                _flyingNickname.transform.parent = this.transform;
                _flyingNickname.transform.rotation = new Quaternion(0.0f, 180.0f, 0.0f, 1.0f);
                _flyingNickname.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                _flyingNickname.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                _flyingTextMesh = _flyingNickname.AddComponent<TextMesh>();
                _flyingTextMesh.anchor = TextAnchor.MiddleCenter;
                _flyingTextMesh.fontSize = 22;
                _flyingTextMesh.text = "Test [999]";
            }
        }

        public void SetFlyingNicknameVisibility()
        {
            if (_flyingNickname == null)
                CreateFlyingNicknameIfNull();

            _flyingNickname.SetActive(!_flyingNickname.activeSelf);
        }

        public bool GetFlyingNicknameVisibility()
        {
            return _flyingNickname.activeSelf;
        }

        public void SetNickname(string l_playerNickname, uint l_playerPid)
        {
            if (_flyingNickname == null)
                CreateFlyingNicknameIfNull();

            _playerNickname = l_playerNickname;
            _flyingTextMesh.text = $"{l_playerNickname} [{l_playerPid}]";
        }

        public void SetFlyingNicknameYOffset(float l_newOffset)
        {
            if (_flyingNickname == null)
                CreateFlyingNicknameIfNull();

            _flyingNickname.transform.localPosition = new Vector3(0.0f, l_newOffset, 0.0f);
        }

        public void AssignBuState(Network.NestedTypes.BusState l_busState)
        {
            _currentBusState = new BusState();
            _currentBusState = l_busState;

            if (l_busState.isEngineTurnedOn)
            {
                _headlightsLowBeam.SetActive(true);
                _sideLights.SetActive(true);
                _rearLightsRoot.SetActive(true);
            }
        }

        public void TriggerStandartAction(string l_actionName)
        {
            switch (l_actionName)
            {
                case "triggerEngineOn":
                    _currentBusState.isEngineTurnedOn = true;
                    break;
                case "triggerEngineOff":
                    _currentBusState.isEngineTurnedOn = false;
                    break;
                case "triggerHighBeamLights":
                    _currentBusState.isHighBeamTurnedOn = !_currentBusState.isHighBeamTurnedOn;
                    break;
                case "triggerBracking":
                    _currentBusState.isBraking = !_currentBusState.isBraking;
                    break;
                case "triggerReverseGear":
                    _currentBusState.isReverseGear = !_currentBusState.isReverseGear;
                    break;
                case "triggerLeftBlinker":
                    _currentBusState.isRightBlinkerBlinking = false;
                    _currentBusState.isBothBlinkersBlinking = false;
                    _currentBusState.isLeftBlinkerBlinking = !_currentBusState.isLeftBlinkerBlinking;
                    break;
                case "triggerRightBlinker":
                    _currentBusState.isLeftBlinkerBlinking = false;
                    _currentBusState.isBothBlinkersBlinking = false;
                    _currentBusState.isRightBlinkerBlinking = !_currentBusState.isRightBlinkerBlinking;
                    break;
                case "triggerBothBlinkers":
                    _currentBusState.isRightBlinkerBlinking = false;
                    _currentBusState.isLeftBlinkerBlinking = false;
                    _currentBusState.isBothBlinkersBlinking = !_currentBusState.isBothBlinkersBlinking;
                    break;
                default:
                    Debug.LogWarning($"REMOTE_PLAYER_CONTROLLER ({_playerNickname}): Cannot find standart action \"{l_actionName}\"! Dispatching trigger to child class...");
                    OnTriggerToggleAction(l_actionName);
                    break;
            }
        }

        public abstract void OnTriggerToggleAction(string l_actionName);
    }
}
