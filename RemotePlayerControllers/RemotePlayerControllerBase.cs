using UnityEngine;

namespace BDSM.RemotePlayerControllers
{
    public interface RemotePlayerControllerInterface
    {
        public void TriggerToggleAction(string l_actionName);
    }

    public abstract class RemotePlayerControllerBase : MonoBehaviour, RemotePlayerControllerInterface
    {
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

        void RemotePlayerControllerInterface.TriggerToggleAction(string l_actionName)
        {
            OnTriggerToggleAction(l_actionName);
        }

        public abstract void OnTriggerToggleAction(string l_actionName);
    }
}
