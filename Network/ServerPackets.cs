using UnityEngine;
using LiteNetLib;

namespace BDSM.Network.ServerPackets
{
    public class ServerPlayer
    {
        public NetPeer peer { get; set; }
        public string nickname { get; set; }
        public NestedTypes.PlayerState state { get; set; }
        public NestedTypes.BusState busState { get; set; }
    }

    public class UpgradesState
    {
        public string[] installedUpgrades { get; set; }
    }

    public class RequestJoin
    {
        public string nickname { get; set; }
        public NestedTypes.BusState busState { get; set; }
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
        public NestedTypes.BusState newBusState { get; set; }
    }

    public class DispatchBusAction
    {
        public uint pid { get; set; }
        public string actionName { get; set; }
        public bool actionState { get; set; }
    }
}
