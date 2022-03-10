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

        protected GameObject _brakingLightsRoot;
        protected GameObject _rearLightsRoot;
        protected GameObject _backwardLightsRoot;
        protected GameObject _rearLeftBlinker;
        protected GameObject _rearRightBlinker;
        protected GameObject _middleLeftBlinker;
        protected GameObject _middleRightBlinker;
        protected GameObject _sideLights;
        protected GameObject _headlightsLowBeam;
        protected GameObject _headLightsHighBeam;
        protected GameObject _frontLeftBlinker;
        protected GameObject _frontRightBlinker;
        protected GameObject _insideLightRoot;
        protected GameObject _driverLightRoot;

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
                if (_headlightsLowBeam != null && _sideLights != null && _rearLightsRoot != null)
                {
                    _headlightsLowBeam.SetActive(true);
                    _sideLights.SetActive(true);
                    _rearLightsRoot.SetActive(true);
                }

                if (_currentBusState.isBraking && _brakingLightsRoot != null)
                {
                    _brakingLightsRoot.SetActive(true);
                }
                else if (!_currentBusState.isBraking && _brakingLightsRoot != null)
                {
                    _brakingLightsRoot.SetActive(false);
                }

                if (_currentBusState.isReverseGear && _backwardLightsRoot != null)
                {
                    _backwardLightsRoot.SetActive(true);
                }
                else if (!_currentBusState.isReverseGear && _backwardLightsRoot != null)
                {
                    _backwardLightsRoot.SetActive(false);
                }

                if (_currentBusState.isHighBeamTurnedOn && _headLightsHighBeam != null)
                {
                    _headLightsHighBeam.SetActive(true);
                }
                else if (!_currentBusState.isHighBeamTurnedOn && _headLightsHighBeam != null)
                {
                    _headLightsHighBeam.SetActive(false);
                }

                if (_currentBusState.isInsideLightsTurnedOn && _insideLightRoot != null)
                {
                    _insideLightRoot.SetActive(true);
                }
                else if (!_currentBusState.isInsideLightsTurnedOn && _insideLightRoot != null)
                {
                    _insideLightRoot.SetActive(false);
                }

                if (_currentBusState.isDriverLightsTurnedOn && _driverLightRoot != null)
                {
                    _driverLightRoot.SetActive(true);
                }
                else if (!_currentBusState.isDriverLightsTurnedOn && _driverLightRoot != null)
                {
                    _driverLightRoot.SetActive(false);
                }

                if (_frontLeftBlinker != null && _middleLeftBlinker != null && _rearLeftBlinker != null)
                {
                    if (_currentBusState.isLeftBlinkerBlinking && !_currentBusState.isRightBlinkerBlinking && !_currentBusState.isBothBlinkersBlinking)
                    {
                        _frontLeftBlinker.SetActive(Convert.ToBoolean(Mathf.FloorToInt(Mathf.PingPong(Time.time * _blinkersBlinkingPeriod, 2.0f))));
                        _middleLeftBlinker.SetActive(Convert.ToBoolean(Mathf.FloorToInt(Mathf.PingPong(Time.time * _blinkersBlinkingPeriod, 2.0f))));
                        _rearLeftBlinker.SetActive(Convert.ToBoolean(Mathf.FloorToInt(Mathf.PingPong(Time.time * _blinkersBlinkingPeriod, 2.0f))));
                    }
                    else if (!_currentBusState.isLeftBlinkerBlinking && !_currentBusState.isRightBlinkerBlinking && !_currentBusState.isBothBlinkersBlinking)
                    {
                        _frontLeftBlinker.SetActive(false);
                        _middleLeftBlinker.SetActive(false);
                        _rearLeftBlinker.SetActive(false);
                    }
                }

                if (_frontRightBlinker !=null && _middleRightBlinker != null && _rearRightBlinker)
                {
                    if (_currentBusState.isRightBlinkerBlinking && !_currentBusState.isLeftBlinkerBlinking && !_currentBusState.isBothBlinkersBlinking)
                    {
                        _frontRightBlinker.SetActive(Convert.ToBoolean(Mathf.FloorToInt(Mathf.PingPong(Time.time * _blinkersBlinkingPeriod, 2.0f))));
                        _middleRightBlinker.SetActive(Convert.ToBoolean(Mathf.FloorToInt(Mathf.PingPong(Time.time * _blinkersBlinkingPeriod, 2.0f))));
                        _rearRightBlinker.SetActive(Convert.ToBoolean(Mathf.FloorToInt(Mathf.PingPong(Time.time * _blinkersBlinkingPeriod, 2.0f))));
                    }
                    else if (!_currentBusState.isRightBlinkerBlinking &&!_currentBusState.isRightBlinkerBlinking && !_currentBusState.isBothBlinkersBlinking)
                    {
                        _frontRightBlinker.SetActive(false);
                        _middleRightBlinker.SetActive(false);
                        _rearRightBlinker.SetActive(false);
                    }
                }

                if (_frontLeftBlinker != null && _middleLeftBlinker != null && _rearLeftBlinker != null && _frontRightBlinker != null && _middleRightBlinker != null && _rearRightBlinker)
                {
                    if (_currentBusState.isBothBlinkersBlinking && !_currentBusState.isLeftBlinkerBlinking && !_currentBusState.isRightBlinkerBlinking)
                    {
                        _frontLeftBlinker.SetActive(Convert.ToBoolean(Mathf.FloorToInt(Mathf.PingPong(Time.time * _blinkersBlinkingPeriod, 2.0f))));
                        _middleLeftBlinker.SetActive(Convert.ToBoolean(Mathf.FloorToInt(Mathf.PingPong(Time.time * _blinkersBlinkingPeriod, 2.0f))));
                        _rearLeftBlinker.SetActive(Convert.ToBoolean(Mathf.FloorToInt(Mathf.PingPong(Time.time * _blinkersBlinkingPeriod, 2.0f))));
                        _frontRightBlinker.SetActive(Convert.ToBoolean(Mathf.FloorToInt(Mathf.PingPong(Time.time * _blinkersBlinkingPeriod, 2.0f))));
                        _middleRightBlinker.SetActive(Convert.ToBoolean(Mathf.FloorToInt(Mathf.PingPong(Time.time * _blinkersBlinkingPeriod, 2.0f))));
                        _rearRightBlinker.SetActive(Convert.ToBoolean(Mathf.FloorToInt(Mathf.PingPong(Time.time * _blinkersBlinkingPeriod, 2.0f))));
                    }
                    else if (!_currentBusState.isBothBlinkersBlinking && !_currentBusState.isLeftBlinkerBlinking && _currentBusState.isLeftBlinkerBlinking)
                    {
                        _frontLeftBlinker.SetActive(false);
                        _middleLeftBlinker.SetActive(false);
                        _rearLeftBlinker.SetActive(false);
                        _frontRightBlinker.SetActive(false);
                        _middleRightBlinker.SetActive(false);
                        _rearRightBlinker.SetActive(false);
                    }
                }
            }
            else
            {
                if (_headlightsLowBeam != null && _sideLights != null && _rearLightsRoot != null && _headLightsHighBeam != null)
                {
                    _headLightsHighBeam.SetActive(false);
                    _headlightsLowBeam.SetActive(false);
                    _sideLights.SetActive(false);
                    _rearLightsRoot.SetActive(false);
                }

                if (_insideLightRoot != null && _driverLightRoot != null)
                {
                    _insideLightRoot.SetActive(false);
                    _driverLightRoot.SetActive(false);
                }

                if (_brakingLightsRoot != null && _backwardLightsRoot != null)
                {
                    _brakingLightsRoot.SetActive(false);
                    _backwardLightsRoot.SetActive(false);
                }

                if (_frontLeftBlinker != null && _middleLeftBlinker != null && _rearLeftBlinker != null)
                {
                    _frontLeftBlinker.SetActive(false);
                    _middleLeftBlinker.SetActive(false);
                    _rearLeftBlinker.SetActive(false);
                }
                if (_frontRightBlinker != null && _middleRightBlinker != null && _rearRightBlinker != null)
                {
                    _frontRightBlinker.SetActive(false);
                    _middleRightBlinker.SetActive(false);
                    _rearRightBlinker.SetActive(false);
                }
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
        }

        public void TriggerStandartAction(string l_actionName)
        {
            switch (l_actionName)
            {
                case "triggerEngine":
                    _currentBusState.isEngineTurnedOn = !_currentBusState.isEngineTurnedOn;
                    Debug.Log($"BASE_CONTROLLER: Action \"{l_actionName}\" was processed by {_playerNickname} controller.");
                    break;
                case "triggerHighBeamLights":
                    _currentBusState.isHighBeamTurnedOn = !_currentBusState.isHighBeamTurnedOn;
                    Debug.Log($"BASE_CONTROLLER: Action \"{l_actionName}\" was processed by {_playerNickname} controller.");
                    break;
                case "triggerBraking":
                    _currentBusState.isBraking = !_currentBusState.isBraking;
                    Debug.Log($"BASE_CONTROLLER: Action \"{l_actionName}\" was processed by {_playerNickname} controller.");
                    break;
                case "triggerReverse":
                    _currentBusState.isReverseGear = !_currentBusState.isReverseGear;
                    Debug.Log($"BASE_CONTROLLER: Action \"{l_actionName}\" was processed by {_playerNickname} controller.");
                    break;
                case "triggerLeftBlinker":
                    _currentBusState.isRightBlinkerBlinking = false;
                    _currentBusState.isBothBlinkersBlinking = false;
                    _currentBusState.isLeftBlinkerBlinking = !_currentBusState.isLeftBlinkerBlinking;
                    Debug.Log($"BASE_CONTROLLER: Action \"{l_actionName}\" was processed by {_playerNickname} controller.");
                    break;
                case "triggerRightBlinker":
                    _currentBusState.isLeftBlinkerBlinking = false;
                    _currentBusState.isBothBlinkersBlinking = false;
                    _currentBusState.isRightBlinkerBlinking = !_currentBusState.isRightBlinkerBlinking;
                    Debug.Log($"BASE_CONTROLLER: Action \"{l_actionName}\" was processed by {_playerNickname} controller.");
                    break;
                case "triggerBothBlinkers":
                    _currentBusState.isRightBlinkerBlinking = false;
                    _currentBusState.isLeftBlinkerBlinking = false;
                    _currentBusState.isBothBlinkersBlinking = !_currentBusState.isBothBlinkersBlinking;
                    Debug.Log($"BASE_CONTROLLER: Action \"{l_actionName}\" was processed by {_playerNickname} controller.");
                    break;
                case "triggerInsideLights":
                    _currentBusState.isInsideLightsTurnedOn = !_currentBusState.isInsideLightsTurnedOn;
                    Debug.Log($"BASE_CONTROLLER: Action \"{l_actionName}\" was processed by {_playerNickname} controller.");
                    break;
                case "triggerDriverLights":
                    _currentBusState.isDriverLightsTurnedOn = !_currentBusState.isDriverLightsTurnedOn;
                    Debug.Log($"BASE_CONTROLLER: Action \"{l_actionName}\" was processed by {_playerNickname} controller.");
                    break;
                default:
                    Debug.LogWarning($"REMOTE_PLAYER_CONTROLLER ({_playerNickname}): Cannot find standart action \"{l_actionName}\"! Dispatching trigger to child class...");
                    OnTriggerToggleAction(l_actionName);
                    break;
            }
        }

        protected abstract void OnTriggerToggleAction(string l_actionName);
    }
}
