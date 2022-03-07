using UnityEngine;
using LiteNetLib;

namespace BDSM.Network.ServerPackets
{
    public class ServerPlayer
    {
        public NetPeer peer { get; set; }
        public string nickname { get; set; }
        public NestedTypes.PlayerState state { get; set; }
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
}
