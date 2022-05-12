using UnityEngine;

/// <summary>
/// This namespace conatins clients-related classes: some local data types, answers from servers and etc...
/// </summary>
namespace BDSM.Network.ClientPackets
{
    /// <summary>
    /// This class represents remote player on client side.
    /// </summary>
    public class RemotePlayer
    {
        public string nickname { get; set; }
        public GameObject remotePlayerBus { get; set; } // GameObject with bus for player.
        public RemotePlayerControllers.RemotePlayerControllerBase remotePlayerController { get; set; }
        public NestedTypes.BusState busState { get; set; }
        public NestedTypes.PlayerState state { get; set; }
    }

    /// <summary>
    /// If your join request will be accepted by server, you will get this packet with your own PID. (We should save it for next requests to server.)
    /// </summary>
    public class OnJoinAccepted
    {
        public uint pid { get; set; }
    }

    /// <summary>
    /// If for some reason your join request will be declined, you will get this packet with reason and will be disconnected from server.
    /// </summary>
    public class OnJoinDeclined
    {
        public string message { get; set; }
    }

    /// <summary>
    /// This packet is an answer on request server state.
    /// </summary>
    public class ReceiveServerState
    {
        public string serverName { get; set; }
        public uint currentMap { get; set; }
        public uint playersLimit { get; set; }
        public uint currentAmountOfPlayers { get; set; }
    }

    /// <summary>
    /// This packet for time syncing.
    /// </summary>
    public class ReceiveServerDateAndTime
    {
        public NestedTypes.NetDateAndTime currentServerDateAndTime { get; set; }
    }

    /// <summary>
    /// When somebody joining on server, we will get this packet with information about new player.
    /// </summary>
    public class AddRemotePlayer
    {
        public string nickname { get; set; }
        public NestedTypes.PlayerState state { get; set; }
        public NestedTypes.BusState busState { get; set; }
    }

    /// <summary>
    /// When somebody leave, we getting this packet with him PID, so whe can delete him remote player on our side.
    /// </summary>
    public class RemoveRemotePlayer
    {
        public uint pid { get; set; }
    }

    /// <summary>
    /// When somebody changing bus, we will get this packet with PID and bus short name.
    /// </summary>
    public class RemotePlayerChangedBus
    {
        public uint pid { get; set; }
        public NestedTypes.BusState busState { get; set; }
        public string busShortName { get; set; }
    }

    /// <summary>
    /// This is a packet that we receiving every server frame. Its contains array of player states, so we can update positions and rotations of remote clients on our side. IMPORTANT: This array also contains our position and rotation: dont forget to check it before trying to search for remote player in dictionary!
    /// </summary>
    public class UpdateRemotePlayers
    {
        public NestedTypes.PlayerState[] states { get; set; }
    }

    /// <summary>
    /// This packet contains current remote player bus state.
    /// </summary>
    public class ReceiveBusState
    {
        public uint pid { get; set; }
        public NestedTypes.BusState newBusState { get; set; }
    }

    /// <summary>
    /// When somebody pressing brake, turning on blinker or headlights and etc, we will get this packet with action name and state.
    /// </summary>
    public class ReceiveRemotePlayerBusAction
    {
        public uint pid { get; set; }
        public string actionName { get; set; }
        public bool actionState { get; set; }
    }
}
