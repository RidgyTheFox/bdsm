using BDSM.Network.NestedTypes;
using UnityEngine;

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

        public abstract void OnTriggerToggleAction(string l_actionName);
    }
}
