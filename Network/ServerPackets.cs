using UnityEngine;
using LiteNetLib;

namespace BDSM.Network.ServerPackets
{
    public class ServerPlayer
    {
        public NetPeer peer { get; set; }
        public string nickname { get; set; }
        public NestedTypes.PlayerState state { get; set; }
        public BusState currentBusState { get; set; }
    }

    public class BusState
    {
        public bool isEngineTurnedOn { get; set; }
        public bool isHighBeamTurnedOn { get; set; }
        public bool isLeftBlinkerBlinking { get; set; }
        public bool isRightBlinkerBlinking { get; set; }
        public bool isBothBlinkersBlinking { get; set; }
        public bool isReverseGear { get; set; }
        public bool isBraking { get; set; }
        public bool isDriverLightsTurnedOn { get; set; }
        public bool isInsideLightsTurnedOn { get; set; }
        public bool isFrontDoorOpened { get; set; }
        public bool isMiddleDoorOpened { get; set; }
        public bool isRearDoorOpened { get; set; }
    }

    public class UpgradesState
    {
        public string[] installedUpgrades { get; set; }
    }

    public class RequestJoin
    {
        public string nickname { get; set; }
        public NestedTypes.PlayerState state { get; set; }
    }

    public class RequestServerState
    {
        public uint pid { get; set; }
    }

    public class RequestServerDateAndTime
    {
        public uint pid { get; set; }
    }

    public class ChangeBus
    {
        public uint pid { get; set; }
        public string busShortName { get; set; }
    }

    public class UpdatePlayerState
    {
        public uint pid { get; set; }
        public NestedTypes.PlayerState state { get; set; }
    }

    public class SetBusState
    {
        public uint pid;
        public BusState newBusState { get; set; }
    }
}
