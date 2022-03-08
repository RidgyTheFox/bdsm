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
            _flyingNickname = new GameObject("FlyingNickname");
            _flyingNickname.transform.parent = this.transform;
            _flyingTextMesh.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            _flyingNickname.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            _flyingTextMesh = _flyingNickname.AddComponent<TextMesh>();
            _flyingTextMesh.fontSize = 20;
            _flyingTextMesh.text = "Test [999]";
        }

        void RemotePlayerControllerInterface.TriggerToggleAction(string l_actionName)
        {
            OnTriggerToggleAction(l_actionName);
        }

        public void SetFlyingNicknameVisibility()
        {
            _flyingNickname.SetActive(!_flyingNickname.activeSelf);
        }

        public void SetNickname(string l_playerNickname, uint l_playerPid)
        {
            _playerNickname = l_playerNickname;
            _flyingTextMesh.text = $"{l_playerNickname} [{l_playerPid}]";
        }

        public void SetFlyingNicknameYOffset(float l_newOffset)
        {
            _flyingNickname.transform.localPosition = new Vector3(0.0f, l_newOffset, 0.0f);
        }

        public abstract void OnTriggerToggleAction(string l_actionName);
    }
}
