using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

using BDSM.Network;

namespace BDSM
{
    class DummyClient : MonoBehaviour, LiteNetLib.INetEventListener
    {
        private NetManager _client;
        private NetDataWriter _writer;
        private NetPacketProcessor _packetProcessor;

        private NetPeer _server;
        private bool _isConnected = false;
        private bool _isAuthorized = false;
        private bool _isMoving = false;

        private Network.NestedTypes.PlayerState _localPlayerState;
        private Network.NestedTypes.ServerState _serverState;
        //-207.7788 12.4482 -492.7207 - Serpukhov spawn.
        private Dictionary<uint, Network.ClientPackets.RemotePlayer> _remotePlayers;

        private void Awake()
        {
            Debug.Log("DUMMY: Initializing...");
            _client = new NetManager(this) { AutoRecycle = true };
            _writer = new NetDataWriter();
            
            _localPlayerState = new Network.NestedTypes.PlayerState { selectedBusShortName = "LZ", position = new Vector3(-207.7788f, 12.4482f, -492.7207f), rotation = new Quaternion(0.0f, 180.0f, 0.0f, 1.0f)};
            _remotePlayers = new Dictionary<uint, Network.ClientPackets.RemotePlayer>();

            Debug.Log("DUMMY: Registering nested types...");
            _packetProcessor = new NetPacketProcessor();
            _packetProcessor.RegisterNestedType((w, v) => w.Put(v), reader => reader.GetQuaternion());
            _packetProcessor.RegisterNestedType((w, v) => w.Put(v), reader => reader.GetVector2());
            _packetProcessor.RegisterNestedType((w, v) => w.Put(v), reader => reader.GetVector2Int());
            _packetProcessor.RegisterNestedType((w, v) => w.Put(v), reader => reader.GetVector3());
            _packetProcessor.RegisterNestedType((w, v) => w.Put(v), reader => reader.GetVector3Int());
            _packetProcessor.RegisterNestedType<BDSM.Network.NestedTypes.PlayerState>();
            _packetProcessor.RegisterNestedType<BDSM.Network.NestedTypes.ServerState>();
            Debug.Log("DUMMY: Registering callbacks...");
            _packetProcessor.SubscribeReusable<BDSM.Network.ClientPackets.OnJoinAccepted>(OnJoinRequestAccepted);
            _packetProcessor.SubscribeReusable<BDSM.Network.ClientPackets.OnJoinDeclined>(OnJoinRequestDeclined);
            _packetProcessor.SubscribeReusable<BDSM.Network.ClientPackets.ReceiveServerState>(OnReceiveServerState);
            _packetProcessor.SubscribeReusable<BDSM.Network.ClientPackets.AddRemotePlayer>(OnAddRemotePlayer);
            _packetProcessor.SubscribeReusable<BDSM.Network.ClientPackets.RemoveRemotePlayer>(OnRemoveRemotePlayer);
            _packetProcessor.SubscribeReusable<BDSM.Network.ClientPackets.RemotePlayerChangedBus>(OnRemotePlayerChangedBus);
            _packetProcessor.SubscribeReusable<BDSM.Network.ClientPackets.UpdateRemotePlayers>(OnUpdateRemotePlayers);
            Debug.Log("DUMMY: Initialized!");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.PageUp))
                Connect();
            if (Input.GetKeyDown(KeyCode.PageDown))
                Disconnect();

            if (Input.GetKeyDown(KeyCode.KeypadPlus))
                _isMoving = !_isMoving;
            if (Input.GetKeyDown(KeyCode.KeypadMinus))
                _localPlayerState.position = new Vector3(-207.7788f, 12.4482f, - 492.7207f);
        }

        private void FixedUpdate()
        {
            _client.PollEvents();

            if (_isMoving)
                _localPlayerState.position += new Vector3(0.1f * Time.deltaTime, 0.1f * Time.deltaTime, 0.1f * Time.deltaTime);

            if (_isConnected && _isAuthorized)
                SendPacket( new Network.ServerPackets.UpdatePlayerState { pid = _localPlayerState.pid, state = _localPlayerState }, DeliveryMethod.Sequenced);
        }

        private void OnGUI()
        {

        }

        public void Connect()
        {
            if (!_isConnected)
            {
                _remotePlayers.Clear();
                _client.Start();
                Debug.Log("DUMMY: Connecting to localhost...");
                _client.Connect("127.0.0.1", 2022, "bdsmIsCool");
            }
        }

        public void Disconnect()
        {
            if (_isConnected)
            {
                _isConnected = false;
                _client.DisconnectAll();
                _server.Disconnect();
                _client.Stop();
                _server = null;
                _remotePlayers.Clear();
                Debug.Log("DUMMY: Disconnected!");
            }
        }

        public void OnJoinRequestAccepted(Network.ClientPackets.OnJoinAccepted l_packet)
        {
            _localPlayerState.pid = l_packet.pid;
            _isAuthorized = true;
            Debug.Log($"DUMMY: Join request was accepted! Given PID is {l_packet.pid}. Requesting server state...");
            _localPlayerState.pid = l_packet.pid;
            SendPacket(new Network.ServerPackets.RequestServerState { pid = l_packet.pid }, DeliveryMethod.ReliableOrdered);
        }

        public void OnJoinRequestDeclined(Network.ClientPackets.OnJoinDeclined l_packet)
        {
            _isAuthorized = false;
            _isConnected = false;
            _client.DisconnectAll();
            _client.Stop();
            _server = null;
            _remotePlayers.Clear();
            Debug.LogError($"DUMMY: Cannot join on server! Reason: {l_packet.message}.");
        }

        public void OnReceiveServerState(Network.ClientPackets.ReceiveServerState l_packet)
        {
            Debug.Log($"DUMMY: Server state was received! Server name: {l_packet.serverName}. Current map: {EnumUtils.MapUintToEnum(l_packet.currentMap)}. Players limit: {l_packet.playersLimit}. Amount of players: {l_packet.currentAmountOfPlayers}.");
            _serverState = new Network.NestedTypes.ServerState { serverName = l_packet.serverName, currentMap = EnumUtils.MapUintToEnum(l_packet.currentMap), playersLimit = l_packet.playersLimit, currentAmountOfPlayers = l_packet.currentAmountOfPlayers };
        }

        public void OnAddRemotePlayer(Network.ClientPackets.AddRemotePlayer l_packet)
        {
            Network.NestedTypes.PlayerState l_newPlayerState = new Network.NestedTypes.PlayerState { pid = l_packet.state.pid, position = l_packet.state.position, rotation = l_packet.state.rotation };
            Network.ClientPackets.RemotePlayer l_newPlayer = new Network.ClientPackets.RemotePlayer { nickname = l_packet.nickname, remotePlayerBus = null, state = l_newPlayerState };
            _remotePlayers.Add(l_newPlayer.state.pid, l_newPlayer);
            _serverState.currentAmountOfPlayers++;

            Debug.Log($"DUMMY: Remote player for {l_newPlayer.nickname}[{l_newPlayer.state.pid}] was created.");
        }

        public void OnRemoveRemotePlayer(Network.ClientPackets.RemoveRemotePlayer l_packet)
        {
            Network.ClientPackets.RemotePlayer l_playerToRemove;
            _remotePlayers.TryGetValue(l_packet.pid, out l_playerToRemove);

            if (l_playerToRemove == null)
            {
                Debug.LogError($"DUMMY: Cannot find remote player with PID {l_packet.pid} for removing!");
            }
            else
            {
                Debug.Log($"DUMMY: Remote player for {l_playerToRemove.nickname}[{l_playerToRemove.state.pid}] was removed.");
                _remotePlayers.Remove(l_packet.pid);
            }
        }

        public void OnRemotePlayerChangedBus(Network.ClientPackets.RemotePlayerChangedBus l_packet)
        {
            Debug.Log($"DUMMY: Player with PID {l_packet.pid} changed bus. Searching...");
            Network.ClientPackets.RemotePlayer l_playerForEdit;
            _remotePlayers.TryGetValue(l_packet.pid, out l_playerForEdit);
            if (l_playerForEdit != null)
            {
                Network.NestedTypes.PlayerState l_newPlayerState = new Network.NestedTypes.PlayerState { pid = l_playerForEdit.state.pid, selectedBusShortName = l_packet.busShortName, position = l_playerForEdit.state.position, rotation = l_playerForEdit.state.rotation };
                l_playerForEdit.state = l_newPlayerState;
                _remotePlayers.Remove(l_packet.pid);
                _remotePlayers.Add(l_packet.pid, l_playerForEdit);
                Debug.Log($"DUMMY: Bus for {l_playerForEdit.nickname}[{l_playerForEdit.state.pid}] was changed to {l_playerForEdit.state.selectedBusShortName}.");
            }
            else
                Debug.LogError($"DUMMY: Cannot find remote player for {l_packet.pid}!");
        }

        public void OnUpdateRemotePlayers(Network.ClientPackets.UpdateRemotePlayers l_packet)
        {
            foreach (Network.NestedTypes.PlayerState l_newState in l_packet.states)
            {
                Network.ClientPackets.RemotePlayer l_playerToEdit;
                if (l_newState.pid != _localPlayerState.pid)
                {
                    _remotePlayers.TryGetValue(l_newState.pid, out l_playerToEdit);
                    if (l_playerToEdit != null)
                        l_playerToEdit.state = l_newState;
                }
            }
        }

        public void SendPacket<T>(T l_packet, DeliveryMethod l_deliveryMethod) where T : class, new()
        {
            if (_server != null)
            {
                _writer.Reset();
                _packetProcessor.Write(_writer, l_packet);
                _server.Send(_writer, l_deliveryMethod);
            }
        }

        public void OnConnectionRequest(ConnectionRequest l_request)
        {
        }

        public void OnNetworkError(IPEndPoint l_endPoint, SocketError l_socketError)
        {
        }

        public void OnNetworkLatencyUpdate(NetPeer l_peer, int l_latency)
        {
        }

        public void OnNetworkReceive(NetPeer l_peer, NetPacketReader l_reader, byte l_channelNumber, DeliveryMethod l_deliveryMethod)
        {
            _packetProcessor.ReadAllPackets(l_reader, l_peer);
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint l_remoteEndPoint, NetPacketReader l_reader, UnconnectedMessageType l_messageType)
        {
        }

        public void OnPeerConnected(NetPeer l_peer)
        {
            _server = l_peer;
            _isConnected = true;
            Debug.Log("DUMMY: Connected!");
            SendPacket( new Network.ServerPackets.RequestJoin { nickname = "Dummy", state = _localPlayerState }, DeliveryMethod.ReliableOrdered);
        }

        public void OnPeerDisconnected(NetPeer l_peer, DisconnectInfo l_disconnectInfo)
        {
            if (l_peer == _server)
            {
                _isAuthorized = false;
                _isConnected = false;
                _server = null;
                _client.Stop();
                Debug.Log($"DUMMY: Server disconnected!");
            }
        }
    }
}
