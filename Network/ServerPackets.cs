using UnityEngine;
using LiteNetLib;

/// <summary>
/// This namespace conatins server-related classes: some local data types, requests from clients and etc...
/// </summary>
namespace BDSM.Network.ServerPackets
{
    /// <summary>
    /// This class not for packets. Its represents player on server side.
    /// </summary>
    public class ServerPlayer
    {
        public NetPeer peer { get; set; } // Class that represents our connection to this client, so we can communicate with him.
        public string nickname { get; set; }
        public NestedTypes.PlayerState state { get; set; }
        public NestedTypes.BusState busState { get; set; }
    }

    /// <summary>
    /// This class for syncing upgrades on buses. But its not in use yet...
    /// </summary>
    public class UpgradesState
    {
        public string[] installedUpgrades { get; set; }
    }

    /// <summary>
    /// This is a request for joining on server from client. We can accept it or decline.
    /// </summary>
    public class RequestJoin
    {
        public string nickname { get; set; }
        public NestedTypes.BusState busState { get; set; }
        public NestedTypes.PlayerState state { get; set; }
    }

    // This is a request on getting current server state from client. We should provide a server state, and send it to specified client. We can find specified client by him PID and using this PID as a key in dictionary with players.
    public class RequestServerState
    {
        public uint pid { get; set; }
    }

    /// <summary>
    /// This is a request on getting current server date and time.
    /// </summary>
    public class RequestServerDateAndTime
    {
        public uint pid { get; set; }
    }

    /// <summary>
    /// When player changing bus on him side, he will send this packet, so whe should let know to everybody (except sender of this packet) that this player changed him bus.
    /// </summary>
    public class ChangeBus
    {
        public uint pid { get; set; }
        public string busShortName { get; set; }
    }

    /// <summary>
    /// Players sending this packet with their positions and rotations every their frame.
    /// </summary>
    public class UpdatePlayerState
    {
        public uint pid { get; set; }
        public NestedTypes.PlayerState state { get; set; }
    }

    /// <summary>
    /// This packet is for updating bus state on server.
    /// </summary>
    public class SetBusState
    {
        public uint pid;
        public NestedTypes.BusState newBusState { get; set; }
    }

    /// <summary>
    /// When client pressing brake, turning on headlights or blinkers, he will send this packet to us and we should dispatch it to other clients.
    /// </summary>
    public class DispatchBusAction
    {
        public uint pid { get; set; }
        public string actionName { get; set; }
        public bool actionState { get; set; }
    }
}
