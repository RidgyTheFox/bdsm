using UnityEngine;

namespace BDSM.RemotePlayerControllers
{
    class RemotePlayerController_Generic : RemotePlayerControllerBase
    {
        public void Awake()
        {
            SetFlyingNicknameYOffset(3.0f);
        }

        protected override void OnTriggerToggleAction(string l_actionName)
        {

        }

        protected override void OnWheelUpdate(Quaternion l_wheelFL, Quaternion l_wheelFR, Quaternion l_wheelRL, Quaternion l_wheelRR)
        {

        }
    }
}
