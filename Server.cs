using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using Newtonsoft.Json.Linq;

using BDSM.Network;

namespace BDSM
{
    /// <summary>
    /// This class is repsonsible for all server stuff: handling clients, syncing players and etc...
    /// </summary>
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

        /// <summary>
        /// This function warming up everything for work.
        /// </summary>
        private void Awake()
        {
            Debug.Log("SERVER: Initializing...");

            _server = new NetManager(this) { AutoRecycle = true, EnableStatistics = true };
            StaticData.serverNetManInstance = _server;
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

        /// <summary>
        /// Update function is used for handling keyboard input.
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F2))
                _isMainWindowOpened = !_isMainWindowOpened;
        }

        /// <summary>
        /// Fixed function is used for everything else that you cannot place in Update function.
        /// </summary>
        private void FixedUpdate()
        {
            _server.PollEvents();

            // Sending array with positions and rotations of all players to all client.
            if (_players.Count > 1)
            {
                int i = 0;
                Network.NestedTypes.PlayerState[] states = new Network.NestedTypes.PlayerState[_players.Count];
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

        /// <summary>
        /// This function launching server.
        /// </summary>
        private void LaunchServer()
        {
            if (!_isServerLaunched)
            {
                StaticData.clockMachine.StartTime();
                _server.Statistics.Reset();
                _server.Start(_serverPort);
                _isServerLaunched = true;
                Debug.Log($"SERVER: Server started on port {_serverPort}.");
            }
        }

        /// <summary>
        /// This function stops server.
        /// </summary>
        private void StopServer()
        {
            if (_isServerLaunched)
            {
                StaticData.clockMachine.StopTime();
                _server.Stop();
                _server.Statistics.Reset();
                _isServerLaunched = false;
                Debug.Log("SERVER: Server stopped!");
            }
        }

        /// <summary>
        /// This funciton reading ServerSettings.json and parsing settings into class variables. (See "Server data" region.)
        /// </summary>
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
                Debug.Log($"SERVER: Server name has been set to \"{_serverName}\".");
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
                Debug.Log($"SERVER: Server port has been set to \"{_serverPort}\".");
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
                Debug.Log($"SERVER: \"passwordRequired\" has been set to \"{_isPasswordRequired.ToString()}\".");
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
                    Debug.Log("SERVER: Password set.");
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
                        Debug.Log("SERVER: Map has been changed to \"Serpukhov\".");
                        break;
                    case "serpukhovWinter":
                        _selectedMap = Enums.AvailableMaps.SERPUKHOV_WINTER;
                        Debug.Log("SERVER: Map has been changed to \"Serpukhov Winter\".");
                        break;
                    case "keln":
                        _selectedMap = Enums.AvailableMaps.KELN;
                        Debug.Log("SERVER: Map has been changed to \"Keln\".");
                        break;
                    case "murom":
                        _selectedMap = Enums.AvailableMaps.MUROM;
                        Debug.Log("SERVER: Map has been chnged to \"Murom\".");
                        break;
                    case "muromWinter":
                        _selectedMap = Enums.AvailableMaps.MUROM_WINTER;
                        Debug.Log("SERVER: Map has been chnged to \"Murom Winter\".");
                        break;
                    case "solnechnogorsk":
                        _selectedMap = Enums.AvailableMaps.SOLNECHNOGORSK;
                        Debug.Log("SERVER: Map has been changed to \"Solnechnogorsk\".");
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
                Debug.Log($"SERVER: \"playersLimit\" has been set to \"{_playersLimit}\".");
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
                Debug.LogError("SERVER: Cannot read value \"startAtDay\"! Default value \"True\" will be used...");
            }

            Debug.Log("SERVER: Settings loaded!");
        }

        /// <summary>
        /// This is a small fucntion that helps you with sending packets to a specific peer.
        /// </summary>
        /// <typeparam name="T">Packet class that was registered in LiteNetLib.</typeparam>
        /// <param name="l_packet">Packet.</param>
        /// <param name="l_peer">Receiver of packet.</param>
        /// <param name="l_deliveryMethod">Packet delivering type.</param>
        public void SendPacket<T>(T l_packet, NetPeer l_peer, DeliveryMethod l_deliveryMethod) where T : class, new()
        {
            if (l_peer != null)
            {
                _writer.Reset();
                _packetProcessor.Write(_writer, l_packet);
                l_peer.Send(_writer, l_deliveryMethod);
            }
        }

        #region Callbacks.
        /// <summary>
        /// When player was connected on socket, he sending JoinRequest packet with information about him. This callback for this packet.
        /// </summary>
        /// <param name="l_packet">Packet with information about player.</param>
        /// <param name="l_peer">Sender class.</param>
        public void OnJoinRequest(Network.ServerPackets.RequestJoin l_packet, NetPeer l_peer)
        {
            // Lets if player with this nickname already on server.
            foreach(Network.ServerPackets.ServerPlayer player in _players.Values)
            {
                // Lets disconnect new player if nickname already taken.
                if (player.nickname == l_packet.nickname)
                {
                    Debug.LogWarning($"SERVER: Join request has been rejected because player with this nickname already exists.");
                    SendPacket(new Network.ClientPackets.OnJoinDeclined { message = "Nickname already taken!"}, l_peer, DeliveryMethod.ReliableOrdered);
                    l_peer.Disconnect();
                    return;
                }
            }

            // Creating some stuff for that new player.
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

            // Lets create a ServerPlayer class for that player, assign data, save it in _players dictionary and lets send a packet that will tell to the player that we accepted him join request.
            Network.ServerPackets.ServerPlayer l_newPlayer = new Network.ServerPackets.ServerPlayer { nickname = l_packet.nickname, busState = l_newPlayerBusState, peer = l_peer, state = l_newPlayerState };
            _players.Add(l_newPlayer.state.pid, l_newPlayer);
            SendPacket(new Network.ClientPackets.OnJoinAccepted { pid = l_newPlayer.state.pid }, l_peer, DeliveryMethod.ReliableOrdered);
            
            // Lets let to know to others that a new player has joined on server.
            foreach(Network.ServerPackets.ServerPlayer l_player in _players.Values)
            {
                if (l_player.state.pid != l_newPlayer.state.pid)
                {
                    SendPacket(new Network.ClientPackets.AddRemotePlayer { nickname = l_newPlayer.nickname, busState = l_newPlayerBusState, state = l_newPlayerState }, l_player.peer, DeliveryMethod.ReliableOrdered);
                    SendPacket(new Network.ClientPackets.AddRemotePlayer { nickname = l_player.nickname, busState = l_player.busState, state = l_player.state }, l_newPlayer.peer, DeliveryMethod.ReliableOrdered);
                }
            }

            Debug.Log($"SERVER: new player {l_newPlayer.nickname}[{l_newPlayer.state.pid}] connected! Their bus is {l_newPlayer.state.selectedBusShortName}.");
        }

        /// <summary>
        /// This is a callback for answering on server state requests.
        /// </summary>
        /// <param name="l_packet">Packet with PID who requested server state.</param>
        public void OnServerStateRequest(Network.ServerPackets.RequestServerState l_packet)
        {
            // If this player not in _players dictionary.
            if (_players[l_packet.pid] == null)
            {
                Debug.LogError($"SERVER: User with PID {l_packet.pid} does not exist!");
                return;
            }

            // If that player exist.
            Network.ClientPackets.ReceiveServerState l_newServerState = new Network.ClientPackets.ReceiveServerState {
                serverName = _serverName,
                     currentAmountOfPlayers = (uint)_players.Count,
                     currentMap = (uint)_selectedMap,
                     playersLimit = (uint)_playersLimit
            };
            SendPacket(l_newServerState, _players[l_packet.pid].peer, DeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// This is a callback for answeting on server time and date requests.
        /// </summary>
        /// <param name="l_packet">Packet with PID who requested server time and date.</param>
        public void OnRequestServerDateAndTime(Network.ServerPackets.RequestServerDateAndTime l_packet)
        {
            if (_players[l_packet.pid] == null)
            {
                Debug.LogError($"SERVER: Player with PID {l_packet.pid} requested server date and time, but server cannot find him in players!");
                return;
            }

            Network.ClientPackets.ReceiveServerDateAndTime l_newPacket = new Network.ClientPackets.ReceiveServerDateAndTime {
                currentServerDateAndTime = new Network.NestedTypes.NetDateAndTime {
                    day = StaticData.clockMachine.currentDay,
                    hours = StaticData.clockMachine.currentHour,
                    minutes = StaticData.clockMachine.currentMinute,
                    seconds = StaticData.clockMachine.currentSecond
                 }};

            SendPacket(l_newPacket, _players[l_packet.pid].peer, DeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// When player changing bus, he will send ChangeBus packet to the server. This packet contains information about bus. We should change osme data on our side and dispatch that event to other clients.
        /// </summary>
        /// <param name="l_packet"></param>
        public void OnPlayerChangedBus(Network.ServerPackets.ChangeBus l_packet)
        {
            // TODO: Refactor that function. (I`m using dictionary in really strange way...)
            Debug.Log($"SERVER: Player with PID {l_packet.pid} has changed bus. Searching for player...");
            Network.ServerPackets.ServerPlayer l_playerForEdit;

            // Lets try to find that player in _players dictionary and write that instance to l_playerForEdit variable.
            _players.TryGetValue(l_packet.pid, out l_playerForEdit);

            if (l_playerForEdit != null)
            {
                // Assigning new data to the local variables.
                if (l_playerForEdit.state.selectedBusShortName == l_packet.busShortName)
                {
                    Debug.LogWarning($"SERVER: {l_playerForEdit.nickname}[{l_playerForEdit.state.pid}] changed bus to \"{l_packet.busShortName}\" but they are already driving this bus...");
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

                // Lets dispatch that event to other clients.
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

        /// <summary>
        /// Player every his frame sending new him position and rotation. This callback will apply new data to the ServerPlayer.
        /// </summary>
        /// <param name="l_packet">Packet with player state.</param>
        public void OnUpdatePlayerState(Network.ServerPackets.UpdatePlayerState l_packet)
        {
            _players[l_packet.pid].state = l_packet.state;
        }

        /// <summary>
        /// This callback handling packet with new bus state from player and dispatching new data to others.
        /// </summary>
        /// <param name="l_packet">Packet with new player bus state.</param>
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
                Debug.LogError($"SERVER: New bus state has been received from player with PID {l_packet.pid}, but server cannot find them in players list!");
        }

        /// <summary>
        /// When player doing triggering something on his bus (turning on blinkers or headlights and etc..), he will send TriggerAction packet. So, we should dispatch this action to others players.
        /// </summary>
        /// <param name="l_packet">Packet with action name and state.</param>
        public void OnDispatchBusAction(Network.ServerPackets.DispatchBusAction l_packet)
        {
            Network.NestedTypes.BusState l_newBusState = _players[l_packet.pid].busState;

            // Just checking that this action is exist.
            switch (l_packet.actionName)
            {
                case "engine":
                    l_newBusState.isEngineTurnedOn = l_packet.actionState;
                    break;
                case "blinkersOff":
                    l_newBusState.isLeftBlinkerBlinking = false;
                    l_newBusState.isRightBlinkerBlinking = false;
                    l_newBusState.isBothBlinkersBlinking = false;
                    break;
                case "blinkerLeft":
                    l_newBusState.isLeftBlinkerBlinking = true;
                    l_newBusState.isRightBlinkerBlinking = false;
                    l_newBusState.isBothBlinkersBlinking = false;
                    break;
                case "blinkerRight":
                    l_newBusState.isLeftBlinkerBlinking = false;
                    l_newBusState.isRightBlinkerBlinking = true;
                    l_newBusState.isBothBlinkersBlinking = false;
                    break;
                case "blinkersBoth":
                    l_newBusState.isLeftBlinkerBlinking = false;
                    l_newBusState.isRightBlinkerBlinking = false;
                    l_newBusState.isBothBlinkersBlinking = true;
                    break;
                case "highBeamLights":
                    l_newBusState.isHighBeamTurnedOn = l_packet.actionState;
                    break;
                case "braking":
                    l_newBusState.isBraking = l_packet.actionState;
                    break;
                case "reverseGear":
                    l_newBusState.isReverseGear = l_packet.actionState;
                    break;
                case "insideLights":
                    l_newBusState.isInsideLightsTurnedOn = l_packet.actionState;
                    break;
                case "driverLights":
                    l_newBusState.isDriverLightsTurnedOn = l_packet.actionState;
                    break;
                case "frontDoor":
                    l_newBusState.isFrontDoorOpened = l_packet.actionState;
                    break;
                case "middleDoor":
                    l_newBusState.isMiddleDoorOpened = l_packet.actionState;
                    break;
                case "rearDoor":
                    l_newBusState.isRearDoorOpened = l_packet.actionState;
                    break;
                default:
                    Debug.LogError($"SERVER: Unknown trigger: \"{l_packet.actionName}\"! Aborting...");
                    return;
            }

            // Dispatching new bus state.
            _players[l_packet.pid].busState = l_newBusState;
            foreach(Network.ServerPackets.ServerPlayer l_player in _players.Values)
            {
                if (l_player.state.pid!= l_packet.pid)
                    SendPacket(new Network.ClientPackets.ReceiveRemotePlayerBusAction { pid = l_packet.pid, actionName = l_packet.actionName, actionState = l_packet.actionState }, l_player.peer, DeliveryMethod.ReliableOrdered);
            }
            Debug.Log($"SERVER: Action \"{l_packet.actionName}\" from {_players[l_packet.pid].nickname}[{_players[l_packet.pid]}] has been dispatched to other players!");
        }
        #endregion

        #region INetListener Interface.
        /// <summary>
        /// This callback handling inbound connections from clients, checking some stuff and accepting connection if everything is ok.
        /// </summary>
        /// <param name="l_request">Inbound connection.</param>
        public void OnConnectionRequest(ConnectionRequest l_request)
        {
            if (_players.Count == _playersLimit)
            {
                l_request.Reject();
                Debug.LogWarning($"SERVER: Inbound connection from {l_request.RemoteEndPoint.Address} has been rejected because user limit was reached.");
            }
            else
            {
                if (_isPasswordRequired)
                {
                    l_request.AcceptIfKey(_password);
                    Debug.Log($"SERVER: Inbound connection from {l_request.RemoteEndPoint.Address} will be accepted if client has password.");
                }
                else
                {
                    l_request.Accept();
                    Debug.Log($"SERVER: Inbound connection from {l_request.RemoteEndPoint.Address} was accepted.");
                }
            }
        }

        /// <summary>
        /// When error in LiteNetLib happens, this functions will be called.
        /// </summary>
        /// <param name="l_endPoint">EndPoint where error occured.</param>
        /// <param name="l_socketError">Error class.</param>
        public void OnNetworkError(IPEndPoint l_endPoint, SocketError l_socketError)
        {
        }

        /// <summary>
        /// This function when latency with client changing. So, you can get some useful info. But i`m personally using Statistics in LiteNetLib.NetManager (_client variable).
        /// </summary>
        /// <param name="l_peer">Peer class.</param>
        /// <param name="l_latency">Peer latency.</param>
        public void OnNetworkLatencyUpdate(NetPeer l_peer, int l_latency)
        {
        }

        /// <summary>
        /// When we getting new packet, we should read it for triggering callbacks.
        /// </summary>
        /// <param name="l_peer">Packet sender.</param>
        /// <param name="l_reader">Reader for that packet.</param>
        /// <param name="l_channelNumber">Channel where server got this packet.</param>
        /// <param name="l_deliveryMethod">What delivery method was used for this packet.</param>
        public void OnNetworkReceive(NetPeer l_peer, NetPacketReader l_reader, byte l_channelNumber, DeliveryMethod l_deliveryMethod)
        {
            _packetProcessor.ReadAllPackets(l_reader, l_peer);
        }

        /// <summary>
        /// Tbh, idk wtf is it.
        /// </summary>
        /// <param name="l_remoteEndPoint">Wtf?</param>
        /// <param name="l_reader">Wtf?</param>
        /// <param name="l_messageType">Wtf?</param>
        public void OnNetworkReceiveUnconnected(IPEndPoint l_remoteEndPoint, NetPacketReader l_reader, UnconnectedMessageType l_messageType)
        {
        }

        /// <summary>
        /// This function will be called when we accepting new connection.
        /// </summary>
        /// <param name="l_peer">Peer class.</param>
        public void OnPeerConnected(NetPeer l_peer)
        {
            Debug.Log($"SERVER: Peer {l_peer.EndPoint.Address} has connected!");
        }

        /// <summary>
        /// If somebody disconnecting for some reason, this function will be called. Its will do some important stuff for it: removing ServerPlayer for that peer, letting know about it to others and etc...
        /// </summary>
        /// <param name="l_peer"></param>
        /// <param name="l_disconnectInfo"></param>
        public void OnPeerDisconnected(NetPeer l_peer, DisconnectInfo l_disconnectInfo)
        {
            Debug.Log($"SERVER: Peer {l_peer.EndPoint.Address} disconnected! Trying to find and remove server player...");
            Network.ServerPackets.ServerPlayer l_playerToRemove;
            _players.TryGetValue((uint)l_peer.Id, out l_playerToRemove);

            if (l_playerToRemove == null)
                Debug.LogError($"SERVER: Cannot find player with PID {l_peer.Id}!");
            else
            {
                _players.Remove((uint)l_peer.Id);
                Debug.Log($"SERVER: Player has been found and removed!");
            }

            foreach (Network.ServerPackets.ServerPlayer l_player in _players.Values)
            {
                SendPacket(new Network.ClientPackets.RemoveRemotePlayer { pid = (uint)l_peer.Id }, l_player.peer, DeliveryMethod.ReliableOrdered);
            }
        }
        #endregion
    }
}
