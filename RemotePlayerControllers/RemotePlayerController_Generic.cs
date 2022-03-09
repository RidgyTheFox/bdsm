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
    }
}
