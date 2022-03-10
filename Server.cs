﻿using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using Newtonsoft.Json.Linq;

using BDSM.Network;

namespace BDSM
{
    public class Server : MonoBehaviour, LiteNetLib.INetEventListener
    {
        private NetManager _server;
        private NetDataWriter _writer;
        private NetPacketProcessor _packetProcessor;

        #region Server data.
        private string _serverName = "Some Server";
        private int _serverPort = 2022;
        private bool _isPasswordRequired = true;
        private string _password = "somePassowrd";
        private Enums.AvailableMaps _selectedMap = Enums.AvailableMaps.SERPUKHOV;
        private int _playersLimit = 10;
        public Dictionary<uint, Network.ServerPackets.ServerPlayer> _players;

        private bool _isServerLaunched = false;
        #endregion

        #region Server GUI data;

        private bool _isMainWindowOpened = false;
        private bool _mainWindowMoveMode = false;

        private uint _mainWindowPosX = 0;
        private uint _mainWindowPosY = 211;
        #endregion

        private void Awake()
        {
            Debug.Log("SERVER: Initializing...");

            _server = new NetManager(this) { AutoRecycle = true };
            _writer = new NetDataWriter();
            _packetProcessor = new NetPacketProcessor();
            _players = new Dictionary<uint, Network.ServerPackets.ServerPlayer>();

            Debug.Log("SERVER: Registering nested types...");
            _packetProcessor.RegisterNestedType((w, v) => w.Put(v), reader => reader.GetQuaternion());
            _packetProcessor.RegisterNestedType((w, v) => w.Put(v), reader => reader.GetVector2());
            _packetProcessor.RegisterNestedType((w, v) => w.Put(v), reader => reader.GetVector2Int());
            _packetProcessor.RegisterNestedType((w, v) => w.Put(v), reader => reader.GetVector3());
            _packetProcessor.RegisterNestedType((w, v) => w.Put(v), reader => reader.GetVector3Int());
            _packetProcessor.RegisterNestedType<BDSM.Network.NestedTypes.PlayerState>();
            _packetProcessor.RegisterNestedType<BDSM.Network.NestedTypes.ServerState>();
            _packetProcessor.RegisterNestedType<BDSM.Network.NestedTypes.NetDateAndTime>();
            _packetProcessor.RegisterNestedType<BDSM.Network.NestedTypes.BusState>();
            Debug.Log("SERVER: Registering callbacks...");
            _packetProcessor.SubscribeReusable<BDSM.Network.ServerPackets.RequestJoin, NetPeer>(OnJoinRequest);
            _packetProcessor.SubscribeReusable<BDSM.Network.ServerPackets.RequestServerState>(OnServerStateRequest);
            _packetProcessor.SubscribeReusable<BDSM.Network.ServerPackets.RequestServerDateAndTime>(OnRequestServerDateAndTime);
            _packetProcessor.SubscribeReusable<BDSM.Network.ServerPackets.ChangeBus>(OnPlayerChangedBus);
            _packetProcessor.SubscribeReusable<Network.ServerPackets.UpdatePlayerState>(OnUpdatePlayerState);
            _packetProcessor.SubscribeReusable<Network.ServerPackets.SetBusState>(OnSetBusState);
            _packetProcessor.SubscribeReusable<Network.ServerPackets.DispatchBusAction>(OnDispatchBusAction);

            ReloadSettings();

            Debug.Log("SERVER: Initialized!");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F2))
                _isMainWindowOpened = !_isMainWindowOpened;
        }

        private void FixedUpdate()
        {
            _server.PollEvents();

            if (_players.Count > 1)
            {
                int i = 0;
                Network.NestedTypes.PlayerState[] states = new Network.NestedTypes.PlayerState[_playersLimit];
                foreach(Network.ServerPackets.ServerPlayer l_player in _players.Values)
                {
                    states[i] = l_player.state;
                    i++;
                }
                foreach (Network.ServerPackets.ServerPlayer l_player in _players.Values)
                {
                    SendPacket( new Network.ClientPackets.UpdateRemotePlayers { states = states } , l_player.peer, DeliveryMethod.Unreliable);
                }
            }
        }

        private void OnGUI()
        {
            if (!_isMainWindowOpened)
                return;

            if (_mainWindowMoveMode)
            {
                _mainWindowPosX = (uint)Input.mousePosition.x - 10;
                // Cursor coordinates are taken from the bottom left corner.
                // And the coordinates of the windows come from the top left corner.
                // Therefore, you need to invert the coordinates along the Y axis.
                _mainWindowPosY = (uint)Screen.currentResolution.height - (uint)Input.mousePosition.y - 10;
            }

            GUI.Box(new Rect(_mainWindowPosX, _mainWindowPosY, 250, 155), "BDSM | Server Control Panel (F2)");
            if (GUI.Button(new Rect(_mainWindowPosX + 1, _mainWindowPosY + 1, 23, 21), "M"))
                _mainWindowMoveMode = !_mainWindowMoveMode;
            if (GUI.Button(new Rect(_mainWindowPosX + 226, _mainWindowPosY, 23, 21), "X"))
            {
                _isMainWindowOpened = !_isMainWindowOpened;
                _mainWindowMoveMode = false;
            }

            if (!_isServerLaunched)
            {
                GUI.Label(new Rect(_mainWindowPosX + 5, _mainWindowPosY + 30, 250, 20), "State: Stopped.");
                GUI.Label(new Rect(_mainWindowPosX + 5, _mainWindowPosY + 50, 250, 20), $"Password protection: {_isPasswordRequired}");
                GUI.Label(new Rect(_mainWindowPosX + 5, _mainWindowPosY + 70, 250, 20), $"Map: {EnumUtils.MapEnumToString(_selectedMap)}.");
                GUI.Label(new Rect(_mainWindowPosX + 5, _mainWindowPosY + 90, 250, 20), $"Players {_players.Count} of {_playersLimit}.");
                if (GUI.Button(new Rect(_mainWindowPosX + 5, _mainWindowPosY + 110, 200, 20), "Launch server"))
                    LaunchServer();
                if (GUI.Button(new Rect(_mainWindowPosX + 5, _mainWindowPosY + 130, 200, 20), "Reload settings"))
                    ReloadSettings();
            }
            else
            {
                GUI.Label(new Rect(_mainWindowPosX + 5, _mainWindowPosY + 30, 250, 20), $"State: Launched on port {_serverPort}!");
                GUI.Label(new Rect(_mainWindowPosX + 5, _mainWindowPosY + 50, 250, 20), $"Password protection: {_isPasswordRequired}");
                GUI.Label(new Rect(_mainWindowPosX + 5, _mainWindowPosY + 70, 250, 20), $"Map: {EnumUtils.MapEnumToString(_selectedMap)}.");
                GUI.Label(new Rect(_mainWindowPosX + 5, _mainWindowPosY + 90, 250, 20), $"Players {_players.Count} of {_playersLimit}.");
                if (GUI.Button(new Rect(_mainWindowPosX + 5, _mainWindowPosY + 110, 200, 20), "Stop server"))
                    StopServer();
            }
        }

        private void LaunchServer()
        {
            if (!_isServerLaunched)
            {
                StaticData.clockMachine.StartTime();
                _server.Start(_serverPort);
                _isServerLaunched = true;
                Debug.Log($"SERVER: Server launched on port {_serverPort}.");
            }
        }

        private void StopServer()
        {
            if (_isServerLaunched)
            {
                StaticData.clockMachine.StopTime();
                _server.Stop();
                _isServerLaunched = false;
                Debug.Log("SERVER: Server stopped!");
            }
        }

        private void ReloadSettings()
        {
            if (_isServerLaunched)
            {
                Debug.LogError("SERVER: Unable to reload settings because the server is running!");
                return;
            }

            Debug.Log("SERVER: Reloading settings...");

            JObject l_settings = JObject.Parse(System.IO.File.ReadAllText("ServerConfig.json"));
            JToken l_valueToken;

            l_settings.TryGetValue("serverName", out l_valueToken);
            if (l_valueToken != null)
            {
                _serverName = l_valueToken.ToString();
                Debug.Log($"SERVER: Server name was set to \"{_serverName}\".");
            }
            else
            {
                _serverName = "BDSM Server";
                Debug.LogError("SERVER: Cannot read value \"serverName\"! Default value \"BDSM Server\" will be used...");
            }
            l_valueToken = null;

            l_settings.TryGetValue("serverPort", out l_valueToken);
            if (l_valueToken != null)
            {
                _serverPort = int.Parse(l_valueToken.ToString());
                Debug.Log($"SERVER: Server port was set to \"{_serverPort}\".");
            }
            else
            {
                _serverPort = 2022;
                Debug.LogError("SERVER: Cannot read value \"serverPort\"! Default value \"2022\" will be used...");
            }
            l_valueToken = null;

            l_settings.TryGetValue("passwordRequired", out l_valueToken);
            if (l_valueToken != null)
            {
                _isPasswordRequired = bool.Parse(l_valueToken.ToString());
                Debug.Log($"SERVER: \"passwordRequired\" was set to \"{_isPasswordRequired.ToString()}\".");
            }
            else
            {
                _isPasswordRequired = false;
                Debug.LogError("SERVER: Cannot read value \"passwordRequired\"! Default value \"False\" will be used...");
            }
            l_valueToken = null;

            if (_isPasswordRequired)
            {
                l_settings.TryGetValue("password", out l_valueToken);
                if (l_valueToken != null)
                {
                    _password = l_valueToken.ToString();
                    Debug.Log("SERVER: Password was set..");
                }
                else
                {
                    _isPasswordRequired = false;
                    Debug.LogError("SERVER: Cannot read value \"password\"! Passwords will be disabled!");
                }
            }
            l_valueToken = null;

            l_settings.TryGetValue("map", out l_valueToken);
            if (l_valueToken != null)
            {
                switch(l_valueToken.ToString())
                {
                    case "serpukhov":
                        _selectedMap = Enums.AvailableMaps.SERPUKHOV;
                        Debug.Log("SERVER: Map was changed to \"Serpukhov\".");
                        break;
                    case "serpukhovWinter":
                        _selectedMap = Enums.AvailableMaps.SERPUKHOV_WINTER;
                        Debug.Log("SERVER: Map was changed to \"Serpukhov Winter\".");
                        break;
                    case "keln":
                        _selectedMap = Enums.AvailableMaps.KELN;
                        Debug.Log("SERVER: Map was changed to \"Keln\".");
                        break;
                    case "murom":
                        _selectedMap = Enums.AvailableMaps.MUROM;
                        Debug.Log("SERVER: Map was chnged to \"Murom\".");
                        break;
                    case "muromWinter":
                        _selectedMap = Enums.AvailableMaps.MUROM_WINTER;
                        Debug.Log("SERVER: Map was chnged to \"Murom Winter\".");
                        break;
                    case "solnechnogorsk":
                        _selectedMap = Enums.AvailableMaps.SOLNECHNOGORSK;
                        Debug.Log("SERVER: Map was changed to \"Solnechnogorsk\".");
                        break;
                    default:
                        _selectedMap = Enums.AvailableMaps.SERPUKHOV;
                        Debug.LogWarning($"Undefined map: {l_valueToken.ToString()}! \"Serpukhov\" will be used...");
                        break;
                }
            }
            else
            {
                _selectedMap = Enums.AvailableMaps.SERPUKHOV;
                Debug.LogError("SERVER: Cannot read value \"map\". Default value \"serpukhov\" will be used...");
            }
            l_valueToken = null;

            l_settings.TryGetValue("playersLimit", out l_valueToken);
            if (l_valueToken != null)
            {
                _playersLimit = int.Parse(l_valueToken.ToString());
                Debug.Log($"SERVER: \"playersLimit\" was set to \"{_playersLimit}\".");
            }
            else
            {
                _playersLimit = 10;
                Debug.LogError("SERVER: Cannot read value \"playersLimit\"! Default value \"10\" will be used...");
            }
            l_valueToken = null;

            l_settings.TryGetValue("startAtNight", out l_valueToken);
            if (l_valueToken != null)
            {
                if (!bool.Parse(l_valueToken.ToString()))
                {
                    StaticData.clockMachine.SetTime(1, 12, 0, 0);
                    Debug.Log("SERVER: Time set to Day 1 12:00:00.");
                }
                else
                {
                    StaticData.clockMachine.SetTime(1, 0, 0, 0);
                    Debug.Log("SERVER: Time set to Day 1 00:00:00.");
                }
            }
            else
            {
                StaticData.clockMachine.SetTime(1, 12, 0, 0);
                Debug.LogError("SERVER: Cannot read valoe \"startAtDay\"! Default value \"True\" will be used...");
            }

            Debug.Log("SERVER: Settings loaded!");
        }

        public void SendPacket<T>(T l_packet, NetPeer l_peer, DeliveryMethod l_deliveryMethod) where T : class, new()
        {
            if (l_peer != null)
            {
                _writer.Reset();
                _packetProcessor.Write(_writer, l_packet);
                l_peer.Send(_writer, l_deliveryMethod);
            }
        }

        public void OnJoinRequest(Network.ServerPackets.RequestJoin l_packet, NetPeer l_peer)
        {
            foreach(Network.ServerPackets.ServerPlayer player in _players.Values)
            {
                if (player.nickname == l_packet.nickname)
                {
                    Debug.LogWarning($"SERVER: Join request was declined because player with this nikcname already exist.");
                    SendPacket(new Network.ClientPackets.OnJoinDeclined { message = "Nickname already taken!"}, l_peer, DeliveryMethod.ReliableOrdered);
                    l_peer.Disconnect();
                    return;
                }
            }

            Network.NestedTypes.PlayerState l_newPlayerState = new Network.NestedTypes.PlayerState {
                pid = (uint)l_peer.Id,
                selectedBusShortName = l_packet.state.selectedBusShortName,
                position = l_packet.state.position,
                rotation = l_packet.state.rotation,
                wheelFL = l_packet.state.wheelFL,
                wheelFR = l_packet.state.wheelFR,
                wheelRL = l_packet.state.wheelRL,
                wheelRR = l_packet.state.wheelRR };

            Network.NestedTypes.BusState l_newPlayerBusState = new Network.NestedTypes.BusState { 
                isRearDoorOpened = l_packet.busState.isRearDoorOpened,
                isBothBlinkersBlinking = l_packet.busState.isBothBlinkersBlinking,
                isBraking = l_packet.busState.isBraking,
                isDriverLightsTurnedOn = l_packet.busState.isDriverLightsTurnedOn,
                isEngineTurnedOn = l_packet.busState.isEngineTurnedOn,
                isFrontDoorOpened = l_packet.busState.isFrontDoorOpened,
                isHighBeamTurnedOn = l_packet.busState.isHighBeamTurnedOn,
                isInsideLightsTurnedOn = l_packet.busState.isInsideLightsTurnedOn,
                isLeftBlinkerBlinking = l_packet.busState.isLeftBlinkerBlinking,
                isMiddleDoorOpened = l_packet.busState.isMiddleDoorOpened,
                isReverseGear = l_packet.busState.isReverseGear,
                isRightBlinkerBlinking = l_packet.busState.isRightBlinkerBlinking};

            Network.ServerPackets.ServerPlayer l_newPlayer = new Network.ServerPackets.ServerPlayer { nickname = l_packet.nickname, busState = l_newPlayerBusState, peer = l_peer, state = l_newPlayerState };
            _players.Add(l_newPlayer.state.pid, l_newPlayer);
            SendPacket(new Network.ClientPackets.OnJoinAccepted { pid = l_newPlayer.state.pid }, l_peer, DeliveryMethod.ReliableOrdered);

            foreach(Network.ServerPackets.ServerPlayer l_player in _players.Values)
            {
                if (l_player.state.pid != l_newPlayer.state.pid)
                {
                    SendPacket(new Network.ClientPackets.AddRemotePlayer { nickname = l_newPlayer.nickname, busState = l_newPlayerBusState, state = l_newPlayerState }, l_player.peer, DeliveryMethod.ReliableOrdered);
                    SendPacket(new Network.ClientPackets.AddRemotePlayer { nickname = l_player.nickname, busState = l_player.busState, state = l_player.state }, l_newPlayer.peer, DeliveryMethod.ReliableOrdered);
                }
            }

            Debug.Log($"SERVER: new player {l_newPlayer.nickname}[{l_newPlayer.state.pid}] connected! His bus is {l_newPlayer.state.selectedBusShortName}.");
        }

        public void OnServerStateRequest(Network.ServerPackets.RequestServerState l_packet)
        {
            Network.ServerPackets.ServerPlayer l_player;
            _players.TryGetValue(l_packet.pid, out l_player);

            if (l_player == null)
            {
                Debug.LogError($"SERVER: Player with PID {l_packet.pid} requested player state, but he doesn't exist on the server!");
                return;
            }
            else
            {
                Network.ClientPackets.ReceiveServerState l_serverState = new Network.ClientPackets.ReceiveServerState { currentAmountOfPlayers = (uint)_players.Count, currentMap = EnumUtils.MapEnumToUint(_selectedMap), playersLimit = (uint)_playersLimit, serverName = _serverName};
                SendPacket(l_serverState, l_player.peer, DeliveryMethod.ReliableOrdered);
                Debug.Log($"SERVER: Server state was sent to {l_player.nickname}[{l_player.state.pid}].");
            }
        }

        public void OnRequestServerDateAndTime(Network.ServerPackets.RequestServerDateAndTime l_packet)
        {
            Debug.Log($"SERVER: Requst date and time was received from player with PID {l_packet.pid}. Searching for player...");
            Network.ServerPackets.ServerPlayer l_player;
            _players.TryGetValue(l_packet.pid, out l_player);
            if (l_player != null)
            {
                SendPacket( new Network.ClientPackets.ReceiveServerDateAndTime { currentServerDateAndTime = new Network.NestedTypes.NetDateAndTime { day = StaticData.clockMachine.currentDay, hours = StaticData.clockMachine.currentHour, minutes = StaticData.clockMachine.currentMinute, seconds = StaticData.clockMachine.currentSecond } }, l_player.peer, DeliveryMethod.ReliableOrdered);
                Debug.Log($"SERVER: Server date and time was sent to {l_player.nickname}[{l_player.state.pid}].");
            }
            else
                Debug.LogError($"SERVER: Cannot find player with PID {l_packet.pid}!");
        }

        public void OnPlayerChangedBus(Network.ServerPackets.ChangeBus l_packet)
        {
            Debug.Log($"SERVER: Player with PID {l_packet.pid} changed bus. Searching for player...");
            Network.ServerPackets.ServerPlayer l_playerForEdit;
            _players.TryGetValue(l_packet.pid, out l_playerForEdit);

            if (l_playerForEdit != null)
            {
                if (l_playerForEdit.state.selectedBusShortName == l_packet.busShortName)
                {
                    Debug.LogWarning($"SERVER: {l_playerForEdit.nickname}[{l_playerForEdit.state.pid}] changed bus to \"{l_packet.busShortName}\" but already driving this bus...");
                    return;
                }
                Network.NestedTypes.BusState l_newBusState = new Network.NestedTypes.BusState {
                    isRightBlinkerBlinking = false,
                    isReverseGear = false,
                    isMiddleDoorOpened = false,
                    isLeftBlinkerBlinking = false,
                    isInsideLightsTurnedOn = false,
                    isBothBlinkersBlinking = false,
                    isBraking = false,
                    isDriverLightsTurnedOn = false,
                    isEngineTurnedOn = false,
                    isFrontDoorOpened = false,
                    isHighBeamTurnedOn = false,
                    isRearDoorOpened = false};

                Network.NestedTypes.PlayerState l_newPlayerState = new Network.NestedTypes.PlayerState {
                    pid = l_playerForEdit.state.pid,
                    selectedBusShortName = l_packet.busShortName,
                    position = l_playerForEdit.state.position,
                    rotation = l_playerForEdit.state.rotation,
                    wheelFL = l_playerForEdit.state.wheelFL,
                    wheelFR = l_playerForEdit.state.wheelFR,
                    wheelRL = l_playerForEdit.state.wheelRL,
                    wheelRR = l_playerForEdit.state.wheelRR};

                l_playerForEdit.busState = l_newBusState;
                l_playerForEdit.state = l_newPlayerState;
                _players.Remove(l_packet.pid);
                _players.Add(l_packet.pid, l_playerForEdit);

                foreach(Network.ServerPackets.ServerPlayer l_player in _players.Values)
                {
                    if (l_player.state.pid != l_packet.pid)
                        SendPacket( new Network.ClientPackets.RemotePlayerChangedBus { pid = l_packet.pid, busState = l_newBusState, busShortName = l_packet.busShortName }, l_player.peer, DeliveryMethod.ReliableOrdered);
                }

                Debug.Log($"SERVER: Bus for {l_playerForEdit.nickname}[{l_playerForEdit.state.pid}] was changed to {l_packet.busShortName} and synced with other players.");
            }
            else
                Debug.LogError($"SERVER: Cannot find player with PID {l_packet.pid}!");
        }

        public void OnUpdatePlayerState(Network.ServerPackets.UpdatePlayerState l_packet)
        {
            _players[l_packet.pid].state = l_packet.state;
        }

        public void OnSetBusState(Network.ServerPackets.SetBusState l_packet)
        {
            Network.ServerPackets.ServerPlayer l_player;
            _players.TryGetValue(l_packet.pid, out l_player);
            if (l_player != null)
            {
                l_player.busState = l_packet.newBusState;
                foreach(Network.ServerPackets.ServerPlayer l_playerToSend in _players.Values)
                {
                    if (l_playerToSend.state.pid != l_packet.pid)
                        SendPacket( new Network.ClientPackets.ReceiveBusState { pid = l_packet.pid, newBusState = l_packet.newBusState }, l_playerToSend.peer, DeliveryMethod.ReliableOrdered);
                }
                Debug.Log($"SERVER: Bus state for {l_player.nickname}[{l_player.state.pid}] was updated and synced!");
            }
            else
                Debug.LogError($"SERVER: New bus state was received from player with PID {l_packet.pid}, but server cannot find him in players list!");
        }

        public void OnDispatchBusAction(Network.ServerPackets.DispatchBusAction l_packet)
        {
            Network.NestedTypes.BusState l_newBusState = _players[l_packet.pid].busState;

            switch (l_packet.actionName)
            {
                case "triggerEngine":
                    l_newBusState.isEngineTurnedOn = !l_newBusState.isEngineTurnedOn;
                    break;
                case "triggerLeftBlinker":
                    l_newBusState.isLeftBlinkerBlinking = !l_newBusState.isLeftBlinkerBlinking;
                    break;
                case "triggerRightBlinker":
                    l_newBusState.isRightBlinkerBlinking = !l_newBusState.isRightBlinkerBlinking;
                    break;
                case "triggerBothBlinkers":
                    l_newBusState.isBothBlinkersBlinking = !l_newBusState.isBothBlinkersBlinking;
                    break;
                case "triggerHighBeamLights":
                    l_newBusState.isHighBeamTurnedOn = !l_newBusState.isHighBeamTurnedOn;
                    break;
                case "triggerBraking":
                    l_newBusState.isBraking = !l_newBusState.isBraking;
                    break;
                case "triggerReverse":
                    l_newBusState.isReverseGear = !l_newBusState.isReverseGear;
                    break;
                case "triggerInsideLights":
                    l_newBusState.isInsideLightsTurnedOn = !l_newBusState.isInsideLightsTurnedOn;
                    break;
                case "triggerDriverLights":
                    l_newBusState.isDriverLightsTurnedOn = !l_newBusState.isDriverLightsTurnedOn;
                    break;
                case "triggerFrontDoor":
                    l_newBusState.isFrontDoorOpened = !l_newBusState.isFrontDoorOpened;
                    break;
                case "triggerMiddleDoor":
                    l_newBusState.isMiddleDoorOpened = l_newBusState.isMiddleDoorOpened;
                    break;
                case "triggerRearDoor":
                    l_newBusState.isRearDoorOpened = !l_newBusState.isRearDoorOpened;
                    break;
                default:
                    Debug.LogError($"SERVER: Unknown trigger: \"{l_packet.actionName}\"! Aborting...");
                    return;
            }

            _players[l_packet.pid].busState = l_newBusState;
            foreach(Network.ServerPackets.ServerPlayer l_player in _players.Values)
            {
                if (l_player.state.pid!= l_packet.pid)
                    SendPacket(new Network.ClientPackets.ReceiveRemotePlayerBusAction { pid = l_packet.pid, actionName = l_packet.actionName }, l_player.peer, DeliveryMethod.ReliableOrdered);
            }
            Debug.Log($"SERVER: Action \"{l_packet.actionName}\" from {_players[l_packet.pid].nickname}[{_players[l_packet.pid]}] was dispatched to other players!");
        }

        public void OnConnectionRequest(ConnectionRequest l_request)
        {
            if (_players.Count == _playersLimit)
            {
                l_request.Reject();
                Debug.LogWarning($"SERVER: Inboung connection from {l_request.RemoteEndPoint.Address} was rejected because uer limit was reached.");
            }
            else
            {
                if (_isPasswordRequired)
                {
                    l_request.AcceptIfKey(_password);
                    Debug.Log($"SERVER: Inbound connection from {l_request.RemoteEndPoint.Address} will be accepted if client have password.");
                }
                else
                {
                    l_request.Accept();
                    Debug.Log($"SERVER: Inbound connection from {l_request.RemoteEndPoint.Address} was accepted.");
                }
            }
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
            Debug.Log($"SERVER: Peer {l_peer.EndPoint.Address} was connected!");
        }

        public void OnPeerDisconnected(NetPeer l_peer, DisconnectInfo l_disconnectInfo)
        {
            Debug.Log($"SERVER: Peer {l_peer.EndPoint.Address} disconnected! Trying to find and remove server player...");
            Network.ServerPackets.ServerPlayer l_playerToRemove;
            _players.TryGetValue((uint)l_peer.Id, out l_playerToRemove);

            if (l_playerToRemove == null)
                Debug.LogError($"SERVER: Cannot find player with PID {l_peer.Id}!");
            else
                Debug.Log($"SERVER: Player was found and removed!");
            _players.Remove((uint)l_peer.Id);

            foreach (Network.ServerPackets.ServerPlayer l_player in _players.Values)
            {
                SendPacket(new Network.ClientPackets.RemoveRemotePlayer { pid = (uint)l_peer.Id }, l_player.peer, DeliveryMethod.ReliableOrdered);
            }
        }
    }
}
