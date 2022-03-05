using UnityEngine;

namespace BDSM.Network.ClientPackets
{
    public class RemotePlayer
    {
        public string nickname { get; set; }
        public uint busId { get; set; }
        public GameObject remotePlayerBus { get; set; }
        public Enums.AvailableBuses selectedBus { get; set; }
        public NestedTypes.PlayerState state { get; set; }
    }

    public class OnJoinAccepted
    {
        public uint pid { get; set; }
    }
    public class OnJoinDeclined
    {
        public string message { get; set; }
    }

    public class ReceiveServerState
    {
        public string serverName { get; set; }
        public uint currentMap { get; set; }
        public uint playersLimit { get; set; }
        public uint currentAmountOfPlayers { get; set; }
    }

    public class AddRemotePlayer
    {
        public string nickname { get; set; }
        public uint busId { get; set; }
        public NestedTypes.PlayerState state { get; set; }
    }

    public class RemoveRemotePlayer
    {
        public uint pid { get; set; }
    }

    public class RemotePlayerChangedBus
    {
        public uint pid { get; set; }
        public uint busId { get; set; }
    }
}
