using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

using BDSM.Network;

namespace BDSM
{
    /// <summary>
    /// This class is repsonsible for all client stuff: connection, settings, syncing local and remote players, changing bus, map loading, creating models for remote players and etc...
    /// </summary>
    public class Client : MonoBehaviour, LiteNetLib.INetEventListener
    {
        private NetManager _client;
        private NetDataWriter _writer;
        private NetPacketProcessor _packetProcessor;

        #region Client connection info.
        // AKA Client settings.
        private string _nickname = "test";
        private string _serverIp = "127.0.0.1";
        private int _serverPort = 2022;
        private bool _usePassword = true;
        private string _password = "bdsmIsCool";
        #endregion

        #region Player bus data.
        public Quaternion localWheelFLRotation;
        public Quaternion localWheelFRRotation;
        public Quaternion localWheelRLRotation;
        public Quaternion localWheelRRRotation;
        public GameObject localPlayerFrontWheel;
        public GameObject localPlayerBus;
        #endregion

        #region Player data.
        private NetPeer _server;
        private Network.NestedTypes.PlayerState _localPlayerState;
        private Network.NestedTypes.ServerState _serverState;
        private Dictionary<uint, Network.ClientPackets.RemotePlayer> _remotePlayers;
        #endregion

        #region Current client state.
        public bool _isConnected = false;
        private bool _isAuthorized = false;
        public bool isTimeSynced = false;
        public bool isPlayerOnMap = false;
        private bool _hidePlayerNicknames = false;
        #endregion

        #region Client GUI data;
        private bool _isMainWindowOpened = true;
        private bool _mainWindowMoveMode = false;

        private uint _mainWindowPosX = 0;
        private uint _mainWindowPosY = 57;

        private GUIStyle _netStatsTextStyle;
        #endregion

        // This variable define distance limit for flying nicknames render.
        private const float _maxDistanceForNicknames = 60.0f;

        /// <summary>
        /// This function initializing everything on start.
        /// </summary>
        private void Awake()
        {
            Debug.Log("CLIENT: Initializing...");
            StaticData.clientInstance = this;

            _netStatsTextStyle = new GUIStyle();
            _netStatsTextStyle.normal.textColor = Color.green;
            _netStatsTextStyle.fontStyle = FontStyle.Bold;
            _client = new NetManager(this) { AutoRecycle = true, EnableStatistics = true };
            StaticData.clientNetManInstance = _client;
            _writer = new NetDataWriter();

            try
            {
                _localPlayerState = new Network.NestedTypes.PlayerState { pid = 0, selectedBusShortName = FreeMode.PlayerData.GetCurrentData().SelectedBusData.ShortName, position = new Vector3(1.0f, 2.0f, 3.0f), rotation = new Quaternion(4.0f, 3.0f, 2.0f, 1.0f), wheelFL = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f), wheelFR = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f), wheelRL = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f), wheelRR = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f) };
            }
            catch (ArgumentOutOfRangeException e)
            {
                Debug.LogError($"CLIENT: Exception while creaing local player state. Cannot load save data! Falling back to default...");
                _localPlayerState = new Network.NestedTypes.PlayerState { selectedBusShortName = "SPR", position = new Vector3(0.0f, 0.0f, 0.0f), rotation = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f) };
            }
            
            _remotePlayers = new Dictionary<uint, Network.ClientPackets.RemotePlayer>();

            Debug.Log("CLIENT: Registering nested types...");
            _packetProcessor = new NetPacketProcessor();
            _packetProcessor.RegisterNestedType((w, v) => w.Put(v), reader => reader.GetQuaternion());
            _packetProcessor.RegisterNestedType((w, v) => w.Put(v), reader => reader.GetVector2());
            _packetProcessor.RegisterNestedType((w, v) => w.Put(v), reader => reader.GetVector2Int());
            _packetProcessor.RegisterNestedType((w, v) => w.Put(v), reader => reader.GetVector3());
            _packetProcessor.RegisterNestedType((w, v) => w.Put(v), reader => reader.GetVector3Int());
            _packetProcessor.RegisterNestedType<BDSM.Network.NestedTypes.PlayerState>();
            _packetProcessor.RegisterNestedType<BDSM.Network.NestedTypes.ServerState>();
            _packetProcessor.RegisterNestedType<BDSM.Network.NestedTypes.NetDateAndTime>();
            _packetProcessor.RegisterNestedType<BDSM.Network.NestedTypes.BusState>();
            Debug.Log("CLIENT: Registering callbacks...");
            _packetProcessor.SubscribeReusable<BDSM.Network.ClientPackets.OnJoinAccepted>(OnJoinRequestAccepted);
            _packetProcessor.SubscribeReusable<BDSM.Network.ClientPackets.OnJoinDeclined>(OnJoinRequestDeclined);
            _packetProcessor.SubscribeReusable<BDSM.Network.ClientPackets.ReceiveServerState>(OnReceiveServerState);
            _packetProcessor.SubscribeReusable<BDSM.Network.ClientPackets.ReceiveServerDateAndTime>(OnReceiveServerDateAndTime);
            _packetProcessor.SubscribeReusable<BDSM.Network.ClientPackets.AddRemotePlayer>(OnAddRemotePlayer);
            _packetProcessor.SubscribeReusable<BDSM.Network.ClientPackets.RemoveRemotePlayer>(OnRemoveRemotePlayer);
            _packetProcessor.SubscribeReusable<BDSM.Network.ClientPackets.RemotePlayerChangedBus>(OnRemotePlayerChangedBus);
            _packetProcessor.SubscribeReusable<BDSM.Network.ClientPackets.UpdateRemotePlayers>(OnUpdateRemotePlayers);
            _packetProcessor.SubscribeReusable<BDSM.Network.ClientPackets.ReceiveBusState>(OnReceiveBusState);
            _packetProcessor.SubscribeReusable<BDSM.Network.ClientPackets.ReceiveRemotePlayerBusAction>(OnReceiveRemotePlayerBusAction);

            ReloadSettings();
            Debug.Log("CLIENT: Initialized!");
        }
        
        /// <summary>
        /// Typical Update function responsible for keyboard events tracking. Dont use FixedUpdate because its creating some bugs and lags.
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
                _isMainWindowOpened = !_isMainWindowOpened;

            // This is used for debugging. Just press END on your keyboard to change settings for local connection.
            if (Input.GetKeyDown(KeyCode.End))
            {
                _serverIp = "127.0.0.1";
                _serverPort = 2022;
                _usePassword = true;
                _password = "bdsmIsCool";
            }

            // Syncing brakes. I like hardcoding. xD
            // TODO: Add _onConnected check after commit with documentation for this file.
            if (Input.GetKeyDown(KeyCode.S))
                SendPacket(new Network.ServerPackets.DispatchBusAction { pid = _localPlayerState.pid, actionName = "braking", actionState = true }, DeliveryMethod.ReliableOrdered);
            if (Input.GetKeyUp(KeyCode.S))
                SendPacket(new Network.ServerPackets.DispatchBusAction { pid = _localPlayerState.pid, actionName = "braking", actionState = false }, DeliveryMethod.ReliableOrdered);
        }

        private void FixedUpdate()
        {
            _client.PollEvents();

            if (_isConnected && _isAuthorized && isPlayerOnMap && localPlayerBus != null)
            {
                _localPlayerState.position = localPlayerBus.transform.position;
                _localPlayerState.rotation = localPlayerBus.transform.rotation;
                _localPlayerState.wheelFL = localWheelFLRotation;
                _localPlayerState.wheelFR = localWheelFRRotation;
                _localPlayerState.wheelRL = localWheelRLRotation;
                _localPlayerState.wheelRR = localWheelRRRotation;
                SendPacket(new Network.ServerPackets.UpdatePlayerState { pid = _localPlayerState.pid, state = _localPlayerState }, DeliveryMethod.Unreliable);
            }
            else if (_isConnected && _isAuthorized && !isPlayerOnMap)
            {
                _localPlayerState.position = new Vector3(-999.0f, -999.0f, -999.0f);
                SendPacket(new Network.ServerPackets.UpdatePlayerState { pid = _localPlayerState.pid, state = _localPlayerState }, DeliveryMethod.Unreliable);
            }

            if (_isConnected && _isAuthorized && isPlayerOnMap)
            {
                foreach (Network.ClientPackets.RemotePlayer l_player in _remotePlayers.Values)
                {
                    l_player.remotePlayerController.UpdatePosition(l_player);
                }
            }
        }       

        private void OnGUI()
        {
            if (_isConnected && _isAuthorized)
                GUI.Label(new Rect(5, Screen.currentResolution.height-20, 300, 20), $"Connected to {_serverIp}:{_serverPort} | Ping: {_server.Ping * 2}", _netStatsTextStyle);

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

            GUI.Box(new Rect(_mainWindowPosX, _mainWindowPosY, 250, 155), "BDSM | Client Control Panel (F1)");
            if (GUI.Button(new Rect(_mainWindowPosX + 1, _mainWindowPosY + 1, 23, 21), "M"))
                _mainWindowMoveMode = !_mainWindowMoveMode;
            if (GUI.Button(new Rect(_mainWindowPosX + 226, _mainWindowPosY, 23, 21), "X"))
            {
                _mainWindowMoveMode = false;
                _isMainWindowOpened = !_isMainWindowOpened;
            }
            if (!_isConnected)
            {
                GUI.Label(new Rect(_mainWindowPosX + 5, _mainWindowPosY + 30, 250, 20), $"Nickname: {_nickname}");
                GUI.Label(new Rect(_mainWindowPosX + 5, _mainWindowPosY + 50, 250, 20), $"Server IP: {_serverIp}");
                GUI.Label(new Rect(_mainWindowPosX + 5, _mainWindowPosY + 70, 250, 20), $"Server port: {_serverPort}");
                GUI.Label(new Rect(_mainWindowPosX + 5, _mainWindowPosY + 90, 250, 20), $"Use password: {_usePassword.ToString()}");
            }
            else if (_isAuthorized)
            {
                GUI.Label(new Rect(_mainWindowPosX + 5, _mainWindowPosY + 30, 250, 20), $"Connected as {_nickname}[{_localPlayerState.pid}]");
                GUI.Label(new Rect(_mainWindowPosX + 5, _mainWindowPosY + 50, 250, 20), $"Players {_serverState.currentAmountOfPlayers} of {_serverState.playersLimit}.");
                GUI.Label(new Rect(_mainWindowPosX + 5, _mainWindowPosY + 70, 250, 20), $"Map: {EnumUtils.MapEnumToString(_serverState.currentMap)}.");
                GUI.Label(new Rect(_mainWindowPosX + 5, _mainWindowPosY + 90, 250, 20), $"Connected to {_serverIp}:{_serverPort}");
            }
            if (_isConnected)
            {
                if (GUI.Button(new Rect(_mainWindowPosX + 5, _mainWindowPosY + 110, 200, 20), "Disconnect"))
                    Disconnect();
                if (_hidePlayerNicknames)
                {
                    if (GUI.Button(new Rect(_mainWindowPosX + 5, _mainWindowPosY + 130, 200, 20), "Show player nicknames"))
                    {
                        _hidePlayerNicknames = !_hidePlayerNicknames;
                        SwitchFlyingNicknameVisibilityForRemotePlayers();
                    }
                }
                else
                {
                    if (GUI.Button(new Rect(_mainWindowPosX + 5, _mainWindowPosY + 130, 200, 20), "Hide player nicknames"))
                    {
                        _hidePlayerNicknames = !_hidePlayerNicknames;
                        SwitchFlyingNicknameVisibilityForRemotePlayers();
                    }
                }
            }
            else
            {
                if (GUI.Button(new Rect(_mainWindowPosX + 5, _mainWindowPosY + 110, 200, 20), "Connect"))
                {
                    if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "menu")
                        Connect();
                    else
                        Debug.LogError("CLIENT: Cannot connect! You need to exit to the main menu first.");
                }
                if (GUI.Button(new Rect(_mainWindowPosX + 5, _mainWindowPosY + 130, 200, 20), "Reload settings"))
                    ReloadSettings();
            }
        }

        /// <summary>
        /// This function will try to connect to the server according with the client settings.
        /// </summary>
        private void Connect()
        {
            if (!_isConnected)
            {
                if (_usePassword)
                {
                    isPlayerOnMap = false;
                    _client.Statistics.Reset();
                    _client.Start();
                    _client.Connect(_serverIp, _serverPort, _password);
                    Debug.Log($"CLIENT: Connecting to {_serverIp}:{_serverPort} with password...");
                }
                else
                {
                    isPlayerOnMap = false;
                    _client.Statistics.Reset();
                    _client.Start();
                    _client.Connect(_serverIp, _serverPort, "ass");
                    Debug.Log($"CLIENT: Connecting to {_serverIp}:{_serverPort} without password.");
                }
            }
        }

        /// <summary>
        /// This function taking control over disconnecting and other disconnection-related stuff. (Map unloading, deleting remote players and etc...)
        /// </summary>
        public void Disconnect()
        {
            if (_isConnected)
            {
                StaticData.clockMachine.RestoreSavedDateAndTime();
                StaticData.discordIntegrationInstance.UpdateActivity(AvailbaleActivities.IN_MAIN_MENU);
                foreach (Network.ClientPackets.RemotePlayer l_player in _remotePlayers.Values)
                {
                    if (l_player.remotePlayerBus != null)
                        GameObject.Destroy(l_player.remotePlayerBus);
                }

                _isConnected = false;
                _isAuthorized = false;
                isPlayerOnMap = false;
                isTimeSynced = false;
                _client.DisconnectAll();
                _client.Stop();
                _client.Statistics.Reset();
                _server = null;
                _remotePlayers.Clear();
                _serverState.currentAmountOfPlayers = 0;
                Debug.Log("CLIENT: Disconnected!");

                UnityEngine.SceneManagement.SceneManager.LoadScene("menu");
            }
        }

        /// <summary>
        /// This function loads settings from ClientSettings.JSON and setting-up all client settings related variables.
        /// </summary>
        private void ReloadSettings()
        {
            // We cannot change settings when we connected.
            if (_isConnected)
            {
                Debug.LogError("CLIENT: Unable to reload settings because the client is connected!");
                return;
            }

            // TODO: Refactor settings loading. Newtonsoft.JSON has a slightly easier way to parse data.
            Debug.Log("SERVER: Reloading settings...");

            JObject l_settings = JObject.Parse(System.IO.File.ReadAllText("ClientConfig.json"));
            JToken l_valueToken;

            l_settings.TryGetValue("nickname", out l_valueToken);
            if (l_valueToken != null)
            {
                _nickname = l_valueToken.ToString();
                Debug.Log($"CLIENT: Nickname has been set to \"{_nickname}\".");
            }
            else
            {
                _nickname = "Fool";
                Debug.LogError("CLIENT: Cannot read value \"nickname\"! Default value \"Fool\" will be used...");
            }
            l_valueToken = null;

            l_settings.TryGetValue("serverIp", out l_valueToken);
            if (l_valueToken != null)
            {
                _serverIp = l_valueToken.ToString();
                Debug.Log($"CLIENT: Server IP was set to \"{_serverIp}\".");
            }
            else
            {
                _serverIp = "127.0.0.1";
                Debug.LogError("CLIENT: Cannot read value \"serverIp\"! Default value \"127.0.0.1\" will be used...");
            }
            l_valueToken = null;

            l_settings.TryGetValue("serverPort", out l_valueToken);
            if (l_valueToken != null)
            {
                _serverPort = int.Parse(l_valueToken.ToString());
                Debug.Log($"CLIENT: Server port was set to \"{_serverPort}\".");
            }
            else
            {
                _serverPort = 2022;
                Debug.LogError("CLIENT: Cannot read value \"serverPort\"! Default value \"2022\" will be used...");
            }
            l_valueToken = null;

            l_settings.TryGetValue("usePassword", out l_valueToken);
            if (l_valueToken != null)
            {
                _usePassword = bool.Parse(l_valueToken.ToString());
                Debug.Log($"CLIENT: \"usePassword\" was set to \"{_usePassword.ToString()}\".");
            }
            else
            {
                _usePassword = false;
                Debug.LogError("CLIENT: Cannot read value \"usePassword\"! Default value \"False\" will be used...");
            }
            l_valueToken = null;

            if (_usePassword)
            {
                l_settings.TryGetValue("password", out l_valueToken);
                if (l_valueToken != null)
                {
                    _password = l_valueToken.ToString();
                    Debug.Log("CLIENT: Password has been set.");
                }
                else
                {
                    _usePassword = false;
                    Debug.LogError("CLIENT: Cannot read value \"password\"! Connection by password will be disabled!");
                }
            }

            l_valueToken = null;
            Debug.Log("CLIENT: Settings loaded!");
        }

        /// <summary>
        /// This function will load and add to the world 3D models and classes (controllers for remote buses) for each remote player in _remotePlayers IF you`re on map..
        /// </summary>
        public void CreateRemotePlayersModels()
        {
            foreach (Network.ClientPackets.RemotePlayer l_player in _remotePlayers.Values)
            {
                if (l_player.remotePlayerBus == null)
                {
                    // This game uses FreeMode.Garage.GaragePrefabStorage for saving prefabs. So, we just can use bus short name from our player state.
                    _remotePlayers[l_player.state.pid].remotePlayerBus = GameObject.Instantiate(FreeMode.Garage.GaragePrefabStorage.GetSingleton().GetPrefab(l_player.state.selectedBusShortName, true));
                    CreateAssociatedControolerForBus(l_player.state.pid);
                    // Dont forget about bus state.
                    _remotePlayers[l_player.state.pid].remotePlayerController.AssignBuState(_remotePlayers[l_player.state.pid].busState);
                }
            }
        }

        /// <summary>
        /// This function turns on and off flying nicknames above each remote player.
        /// </summary>
        private void SwitchFlyingNicknameVisibilityForRemotePlayers()
        {
            foreach(Network.ClientPackets.RemotePlayer l_player in _remotePlayers.Values)
            {
                // We should check that controller is not null, because contorllers have flying nickname feature.
                if (l_player.remotePlayerController != null)
                    l_player.remotePlayerController.SetFlyingNicknameVisibility();
            }
        }

        /// <summary>
        /// This function will create class for controlling remote bus according.
        /// </summary>
        /// <param name="pid">PID for creating controller.</param>
        private void CreateAssociatedControolerForBus(uint pid)
        {
            switch (_remotePlayers[pid].state.selectedBusShortName)
            {
                case "SPR":
                    _remotePlayers[pid].remotePlayerController = _remotePlayers[pid].remotePlayerBus.AddComponent<RemotePlayerControllers.RemotePlayerController_Sprinter>();
                    _remotePlayers[pid].remotePlayerController.SetNickname(_remotePlayers[pid].nickname, pid);
                    Debug.Log($"CLIENT: Created SPR controller for {_remotePlayers[pid].nickname}[{_remotePlayers[pid].state.pid}].");
                    break;
                case "LZ":
                    _remotePlayers[pid].remotePlayerController = _remotePlayers[pid].remotePlayerBus.AddComponent<RemotePlayerControllers.RemotePlayerController_LIAZ677>();
                    _remotePlayers[pid].remotePlayerController.SetNickname(_remotePlayers[pid].nickname, pid);
                    Debug.Log($"CLIENT: Created LZ controller for {_remotePlayers[pid].nickname}[{_remotePlayers[pid].state.pid}].");
                    break;
                // If specific not coded yet, generic will be used. (Generic controller have only syncing functions and basic flying nickname feature. Specific controllers also have lights and wheels syncing.)
                default:
                    Debug.LogError($"CLIENT: Controller for \"{_remotePlayers[pid].state.selectedBusShortName}\" not found! Generic class will be used...");
                    _remotePlayers[pid].remotePlayerController = _remotePlayers[pid].remotePlayerBus.AddComponent<RemotePlayerControllers.RemotePlayerController_Generic>();
                    _remotePlayers[pid].remotePlayerController.SetNickname(_remotePlayers[pid].nickname, pid);
                    break;
            }
        }

        /// <summary>
        /// This function taking controls over map loading according with client settings.
        /// </summary>
        private void ProceedMapLoading()
        {
            switch (_serverState.currentMap)
            {
                case Enums.AvailableMaps.UNKNOWN:
                    Debug.LogError("CLIENT: Unable to load map! Disconnecting...");
                    Disconnect();
                    break;
                case Enums.AvailableMaps.SERPUKHOV:
                    FileBasedPrefs.SetInt("winter", 0);
                    FileBasedPrefs.SetBool("IsAutomatic", true);
                    StaticData.discordIntegrationInstance.currentMap = "Serpukhov";
                    StaticData.discordIntegrationInstance.UpdateActivity(AvailbaleActivities.IN_MULTIPLAYER);
                    UnityEngine.SceneManagement.SceneManager.LoadScene("FreeModeRoutes");
                    break;
                case Enums.AvailableMaps.SERPUKHOV_WINTER:
                    FileBasedPrefs.SetInt("winter", 1);
                    FileBasedPrefs.SetBool("IsAutomatic", true);
                    StaticData.discordIntegrationInstance.currentMap = "Serpukhov Winter";
                    StaticData.discordIntegrationInstance.UpdateActivity(AvailbaleActivities.IN_MULTIPLAYER);
                    UnityEngine.SceneManagement.SceneManager.LoadScene("FreeModeRoutes");
                    break;
                case Enums.AvailableMaps.KELN:
                    FileBasedPrefs.SetInt("winter", 1);
                    FileBasedPrefs.SetBool("IsAutomatic", true);
                    StaticData.discordIntegrationInstance.currentMap = "Keln";
                    StaticData.discordIntegrationInstance.UpdateActivity(AvailbaleActivities.IN_MULTIPLAYER);
                    UnityEngine.SceneManagement.SceneManager.LoadScene("FreeModeKeln");
                    break;
                case Enums.AvailableMaps.MUROM:
                    StaticData.mainMenuInstance.winterToggleMurom.isOn = false;
                    StaticData.discordIntegrationInstance.currentMap = "Murom";
                    StaticData.discordIntegrationInstance.UpdateActivity(AvailbaleActivities.IN_MULTIPLAYER);
                    StaticData.mainMenuInstance.StartCampain(3);
                    break;
                case Enums.AvailableMaps.MUROM_WINTER:
                    StaticData.mainMenuInstance.winterToggleMurom.isOn = true;
                    StaticData.discordIntegrationInstance.currentMap = "Murom Winter";
                    StaticData.discordIntegrationInstance.UpdateActivity(AvailbaleActivities.IN_MULTIPLAYER);
                    StaticData.mainMenuInstance.StartCampain(3);
                    break;
                case Enums.AvailableMaps.SOLNECHNOGORSK:
                    StaticData.discordIntegrationInstance.currentMap = "Solnechnogorst";
                    StaticData.discordIntegrationInstance.UpdateActivity(AvailbaleActivities.IN_MULTIPLAYER);
                    StaticData.mainMenuInstance.StartCampain(4);
                    break;
            }
        }

        #region Public functions for other parts of mod.
        /// <summary>
        /// This function responsible for requesting time syncing with server.
        /// </summary>
        public void RequestTimeUpdate()
        {
            if (!_isConnected || !_isAuthorized)
                return;
            
            SendPacket(new Network.ServerPackets.RequestServerDateAndTime { pid = _localPlayerState.pid }, DeliveryMethod.ReliableOrdered);
            Debug.Log("CLIENT: Server date and time requested...");
        }

        /// <summary>
        /// Use this function for triggering blinkers on your bus FOR OTHERS. This will not affect on local bus.
        /// </summary>
        /// <param name="l_blinkerSelect">Blinker for trigger. 0 All  blinkers disabled. -1 Left. 1 Right. 2 Both blinkers.</param>
        public void TriggerBlinkers(int l_blinkerSelect)
        {
            switch (l_blinkerSelect)
            {
                case 0: // All blinkers are disabled.
                    SendPacket(new Network.ServerPackets.DispatchBusAction { pid = _localPlayerState.pid, actionName = "blinkersOff", actionState = false }, DeliveryMethod.ReliableOrdered);
                    break;
                case -1: // Left blinker.
                    SendPacket(new Network.ServerPackets.DispatchBusAction { pid = _localPlayerState.pid, actionName = "blinkerLeft", actionState = true }, DeliveryMethod.ReliableOrdered);
                    break;
                case 1: // Right blinker.
                    SendPacket(new Network.ServerPackets.DispatchBusAction { pid = _localPlayerState.pid, actionName = "blinkerRight", actionState = true }, DeliveryMethod.ReliableOrdered);
                    break;
                case 2: // Both blinkers.
                    SendPacket(new Network.ServerPackets.DispatchBusAction { pid = _localPlayerState.pid, actionName = "blinkersBoth", actionState = true }, DeliveryMethod.ReliableOrdered);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// This function have same functionality as TriggerBlinkers, but for other bus systems like engine, lights, doors (doors not implemented yet)and etc.
        /// </summary>
        /// <param name="l_actionName">There is should be a name of action. Frist time, i wanted to use a ENUM, but its not so soft and universal as text.</param>
        /// <param name="l_actionState">Current state of aciton.</param>
        public void TriggerBusAction(string l_actionName, bool l_actionState)
        {
            // This is just a checking. Nothing important...
            switch(l_actionName)
            {
                case "engine":
                    break;
                case "highBeamLights":
                    break;
                case "reverseGear":
                    break;
                case "insideLights":
                    break;
                case "driverLights":
                    break;
                case "frontDoor":
                    break;
                case "middleDoor":
                    break;
                case "rearDoor":
                    break;
                default:
                    Debug.LogError($"CLIENT: Unknown action \"{l_actionName}\"! Aborting...");
                    return;
            }

            SendPacket(new Network.ServerPackets.DispatchBusAction { pid = _localPlayerState.pid, actionName = l_actionName, actionState = l_actionState }, DeliveryMethod.ReliableOrdered);
        }
        #endregion

        #region Callbacks.
        /// <summary>
        /// When server accepting our request, he will send OnJoinRequestAccepted packet with PID. This function creates states and assigning PID to the local player state.
        /// </summary>
        /// <param name="l_packet">A packet from server with our PID.</param>
        public void OnJoinRequestAccepted(Network.ClientPackets.OnJoinAccepted l_packet)
        {
            _localPlayerState.pid = l_packet.pid;
            _isAuthorized = true;
            Debug.Log($"CLIENT: Join request was accepted! Assigned PID is {l_packet.pid}. Requesting server state...");
            _localPlayerState.pid = l_packet.pid;
            // Lets request server state.
            SendPacket(new Network.ServerPackets.RequestServerState { pid = l_packet.pid }, DeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// If server for some reason declined our request, he will send OnJoinDeclined packet with reason.
        /// </summary>
        /// <param name="l_packet">Packet with disconnection reason.</param>
        public void OnJoinRequestDeclined(Network.ClientPackets.OnJoinDeclined l_packet)
        {
            _isAuthorized = false;
            _isConnected = false;
            _client.DisconnectAll();
            _client.Stop();
            _server = null;
            _remotePlayers.Clear();
            Debug.LogError($"CLIENT: Cannot join server! Reason: {l_packet.message}.");
        }
        
        /// <summary>
        /// When our join request was accepted, we requesting server state. So, this function ctaching up server request and process packet.
        /// </summary>
        /// <param name="l_packet">Packet with current server state.</param>
        public void OnReceiveServerState(Network.ClientPackets.ReceiveServerState l_packet)
        {
            Debug.Log($"CLIENT: Server state has been received! Server name: {l_packet.serverName}. Current map: {EnumUtils.MapUintToEnum(l_packet.currentMap)}. Players limit: {l_packet.playersLimit}. Amount of players: {l_packet.currentAmountOfPlayers}.");
            _serverState = new Network.NestedTypes.ServerState { serverName = l_packet.serverName, currentMap = EnumUtils.MapUintToEnum(l_packet.currentMap), playersLimit = l_packet.playersLimit, currentAmountOfPlayers = l_packet.currentAmountOfPlayers };
            // Now, when we have server state with map, we can load map according with server state.
            ProceedMapLoading();
        }

        /// <summary>
        /// This function saving somewhere our current game time from singleplater and asigning new, server time.
        /// </summary>
        /// <param name="l_packet">Packet with current server time.</param>
        public void OnReceiveServerDateAndTime(Network.ClientPackets.ReceiveServerDateAndTime l_packet)
        {
            if (StaticData.timeKeeper != null)
            {
                StaticData.clockMachine.SaveCurrentDateAndTime();
                StaticData.timeKeeper.Day = (int)l_packet.currentServerDateAndTime.day;
                StaticData.timeKeeper.Hour = (int)l_packet.currentServerDateAndTime.hours;
                StaticData.timeKeeper.Minute = (int)l_packet.currentServerDateAndTime.minutes;
                StaticData.timeKeeper.Second = (int)l_packet.currentServerDateAndTime.seconds;
                StaticData.timeKeeper.UpdateSky(); // This is very important thing. For some reason, its fixes lighting on map and main menu after disconnecting.
                isTimeSynced = true;
                Debug.Log($"CLIENT: Time has been set to day {l_packet.currentServerDateAndTime.day}, {l_packet.currentServerDateAndTime.hours}:{l_packet.currentServerDateAndTime.minutes}:{l_packet.currentServerDateAndTime.seconds}!");
            }
            else
                Debug.LogError("CLIENT: Cannot find FreeMode.TimeKeeper!");
        }

        /// <summary>
        /// When somebody joining on server, server will send OnAddRemotePlayer packet with player state for all clients (except new client) with player state for new player.
        /// </summary>
        /// <param name="l_packet">Packet with player state for a new player.</param>
        public void OnAddRemotePlayer(Network.ClientPackets.AddRemotePlayer l_packet)
        {
            Network.NestedTypes.BusState l_newBusState = l_packet.busState;
            Network.NestedTypes.PlayerState l_newPlayerState = new Network.NestedTypes.PlayerState {
                pid = l_packet.state.pid,
                selectedBusShortName = l_packet.state.selectedBusShortName,
                position = l_packet.state.position,
                rotation = l_packet.state.rotation,
                wheelFL = l_packet.state.wheelFL,
                wheelFR = l_packet.state.wheelFR,
                wheelRL = l_packet.state.wheelRL,
                wheelRR = l_packet.state.wheelRR};

            Network.ClientPackets.RemotePlayer l_newPlayer = new Network.ClientPackets.RemotePlayer { nickname = l_packet.nickname, remotePlayerBus = null, remotePlayerController = null, busState = l_newBusState, state = l_newPlayerState };
            _remotePlayers.Add(l_newPlayer.state.pid, l_newPlayer);
            _serverState.currentAmountOfPlayers++; // We taking care about amount of players on server in our instance of server state to not to spam packets.

            // If we on map, lets create a gameobject with model and controller for this player.
            if (isPlayerOnMap)
            {
                _remotePlayers[l_newPlayer.state.pid].remotePlayerBus = GameObject.Instantiate(FreeMode.Garage.GaragePrefabStorage.GetSingleton().GetPrefab(l_newPlayer.state.selectedBusShortName, true));
                CreateAssociatedControolerForBus(l_newPlayer.state.pid);
                _remotePlayers[l_newPlayer.state.pid].remotePlayerController.AssignBuState(l_newBusState);
            }

            Debug.Log($"CLIENT: Remote player with bus \"{l_newPlayer.state.selectedBusShortName}\" has been created for {l_newPlayer.nickname}[{l_newPlayer.state.pid}]!");
        }

        /// <summary>
        /// When somebody leaving from server, server will send OnRemoveRemotePlayer for each client. That packet contains PID for deleting.
        /// </summary>
        /// <param name="l_packet">Packet with PID.</param>
        public void OnRemoveRemotePlayer(Network.ClientPackets.RemoveRemotePlayer l_packet)
        {
            // Dont forget to delete a gameobject for that player.
            if (_remotePlayers[l_packet.pid].remotePlayerBus != null)
                GameObject.Destroy(_remotePlayers[l_packet.pid].remotePlayerBus);

            _remotePlayers.Remove(l_packet.pid);
            _serverState.currentAmountOfPlayers--; // Dont roget about server state...
            Debug.Log($"CLIENT: Remote player with PID {l_packet.pid} has been removed.");
        }

        /// <summary>
        /// When somebody changing bus, server will send OnRemotePlayerChanged bus.
        /// </summary>
        /// <param name="l_packet">Packet with PID and selected bus shortname.</param>
        public void OnRemotePlayerChangedBus(Network.ClientPackets.RemotePlayerChangedBus l_packet)
        {
            Network.NestedTypes.BusState l_newBusState = new Network.NestedTypes.BusState {
                isBothBlinkersBlinking = false,
                isBraking = false,
                isDriverLightsTurnedOn = false,
                isEngineTurnedOn = false,
                isFrontDoorOpened = false,
                isHighBeamTurnedOn = false,
                isInsideLightsTurnedOn = false,
                isLeftBlinkerBlinking = false,
                isMiddleDoorOpened = false,
                isRearDoorOpened = false,
                isReverseGear = false,
                isRightBlinkerBlinking = false };

            if (isPlayerOnMap)
            {
                GameObject.Destroy(_remotePlayers[l_packet.pid].remotePlayerBus);
                Network.NestedTypes.PlayerState l_newPlayerState = new Network.NestedTypes.PlayerState { pid = _remotePlayers[l_packet.pid].state.pid, selectedBusShortName = l_packet.busShortName, position = _remotePlayers[l_packet.pid].state.position, rotation = _remotePlayers[l_packet.pid].state.rotation };

                _remotePlayers[l_packet.pid].busState = l_newBusState;
                _remotePlayers[l_packet.pid].state = l_newPlayerState;
                _remotePlayers[l_packet.pid].remotePlayerBus = GameObject.Instantiate(FreeMode.Garage.GaragePrefabStorage.GetSingleton().GetPrefab(l_packet.busShortName, true));
                CreateAssociatedControolerForBus(l_packet.pid);
                _remotePlayers[l_packet.pid].remotePlayerController.AssignBuState(l_newBusState);

                Debug.Log($"CLIENT: Bus for {_remotePlayers[l_packet.pid].nickname}[{l_packet.pid}] has been changed to \"{l_packet.busShortName}\".");
            }
            else
            {
                Network.NestedTypes.PlayerState l_newPlayerState = new Network.NestedTypes.PlayerState { pid = _remotePlayers[l_packet.pid].state.pid, selectedBusShortName = l_packet.busShortName, position = _remotePlayers[l_packet.pid].state.position, rotation = _remotePlayers[l_packet.pid].state.rotation };
                _remotePlayers[l_packet.pid].state = l_newPlayerState;

                Debug.Log($"CLIENT: Bus for {_remotePlayers[l_packet.pid].nickname}[{l_packet.pid}] has been changed to \"{l_packet.busShortName}\".");
            }
        }

        /// <summary>
        /// This packet contains an array with player states for all players. Client receiving this packet each frame. It uses for sync.
        /// </summary>
        /// <param name="l_packet">Packet with array of player states.</param>
        public void OnUpdateRemotePlayers(Network.ClientPackets.UpdateRemotePlayers l_packet)
        {
            foreach (Network.NestedTypes.PlayerState l_receivedState in l_packet.states)
            {
                if (l_receivedState.selectedBusShortName != null)
                {
                    if (l_receivedState.pid != _localPlayerState.pid) // If this is player state not with our information.
                    {
                        Network.NestedTypes.PlayerState l_newState = new Network.NestedTypes.PlayerState {
                            pid = l_receivedState.pid,
                            selectedBusShortName = l_receivedState.selectedBusShortName,
                            position = l_receivedState.position,
                            rotation = l_receivedState.rotation,
                            wheelFL = l_receivedState.wheelFL,
                            wheelFR = l_receivedState .wheelFR,
                            wheelRL = l_receivedState.wheelRL,
                            wheelRR = l_receivedState.wheelRR};

                        _remotePlayers[l_receivedState.pid].state = l_newState;
                    }
                }
            }
        }

        /// <summary>
        /// THis is not a callback. This function will be called when you changed your bus in garage. Its will send a packet to server to let him know that you`re changed the bus.
        /// </summary>
        public void OnPlayerChangedBusInGarage()
        {
            // If we not on server - we dont care.
            if (!_isConnected || !_isAuthorized)
                return;

            _localPlayerState.selectedBusShortName = FreeMode.PlayerData.GetCurrentData().SelectedBusData.ShortName;

            SendPacket( new Network.ServerPackets.ChangeBus { pid = _localPlayerState.pid, busShortName = _localPlayerState.selectedBusShortName }, DeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// This is a fucntion for assigning bus states for remote players.
        /// </summary>
        /// <param name="l_newState">Packet with new bus state and PID.</param>
        public void OnReceiveBusState(Network.ClientPackets.ReceiveBusState l_newState)
        {
            _remotePlayers[l_newState.pid].remotePlayerController.AssignBuState(l_newState.newBusState);
        }

        /// <summary>
        /// When somebody triggering action, server sends OnReceiveRemotePlayerBusAction with action name and state.
        /// </summary>
        /// <param name="l_packet">Packet with PID, action name and action state.</param>
        public void OnReceiveRemotePlayerBusAction(Network.ClientPackets.ReceiveRemotePlayerBusAction l_packet)
        {
            if (_remotePlayers[l_packet.pid].remotePlayerController == null)
                return;

            _remotePlayers[l_packet.pid].remotePlayerController.TriggerStandartAction(l_packet.actionName, l_packet.actionState);
        }
        #endregion

        /// <summary>
        /// Small function that helps you with sending packets to server.
        /// </summary>
        /// <typeparam name="T">Packet class for sending (BDSM.Network...).</typeparam>
        /// <param name="l_packet">Packet for sending.</param>
        /// <param name="l_deliveryMethod">Actually, delivery method. Use ReliableOrdered for actions, and Unreliable for syncing.</param>
        public void SendPacket<T>(T l_packet, DeliveryMethod l_deliveryMethod) where T : class, new()
        {
            if (_server != null)
            {
                _writer.Reset();
                _packetProcessor.Write(_writer, l_packet);
                _server.Send(_writer, l_deliveryMethod);
            }
        }

        #region INetListener Interface.

        /// <summary>
        /// Defauflt callback for INetSocketListener. We should reject any incoming connections. We are client, not a server.
        /// </summary>
        /// <param name="request">Incoming request class.</param>
        public void OnConnectionRequest(ConnectionRequest request)
        {
            // TODO: Make refusing all incoming connections in next commit after documentation in client.cs. We are`nt a server! :C
        }

        /// <summary>
        /// When error in LiteNetLib happens, this functions will be called.
        /// </summary>
        /// <param name="l_endPoint">EndPoint where error occured.</param>
        /// <param name="l_socketError">Error class.</param>
        public void OnNetworkError(IPEndPoint l_endPoint, SocketError l_socketError)
        {
            Debug.LogError($"CLIENT: Network error has occured! {l_socketError.ToString()}.");
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
        /// This functions will be called if we get any sort of a packet from server. We just re-routing these packets into our NetPacketProcessor for processing.
        /// </summary>
        /// <param name="l_peer">Sender.</param>
        /// <param name="l_reader">Created NetPacketReader for this packet.</param>
        /// <param name="l_channelNumber">Channel where we got this packet.</param>
        /// <param name="l_deliveryMethod">What delivery type was used for delivering.</param>
        public void OnNetworkReceive(NetPeer l_peer, NetPacketReader l_reader, byte l_channelNumber, DeliveryMethod l_deliveryMethod)
        {
            _packetProcessor.ReadAllPackets(l_reader, l_peer);
        }

        /// <summary>
        /// Tbh, idk wtf is it.
        /// </summary>
        /// <param name="l_remoteEndPoint"></param>
        /// <param name="l_reader"></param>
        /// <param name="l_messageType"></param>
        public void OnNetworkReceiveUnconnected(IPEndPoint l_remoteEndPoint, NetPacketReader l_reader, UnconnectedMessageType l_messageType)
        {
        }

        /// <summary>
        /// If someone or we successfully connected to the server, then this function will be called.
        /// </summary>
        /// <param name="l_peer">Peer that was connected.</param>
        public void OnPeerConnected(NetPeer l_peer)
        {
            _server = l_peer;
            _isConnected = true;
            Debug.Log("CLIENT: Connected!");
            SendPacket(new Network.ServerPackets.RequestJoin { nickname = _nickname, state = _localPlayerState }, DeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// If someone or we successfully connected to the server, then this function will be called.
        /// </summary>
        /// <param name="l_peer">Peer class.</param>
        /// <param name="l_disconnectInfo">Some additional info.</param>
        public void OnPeerDisconnected(NetPeer l_peer, DisconnectInfo l_disconnectInfo)
        {
            if (l_peer == _server)
            {
                StaticData.clockMachine.RestoreSavedDateAndTime();
                _isAuthorized = false;
                _isConnected = false;
                isPlayerOnMap = false;
                isTimeSynced = false;
                _server = null;
                _client.Stop();
                _remotePlayers.Clear();
                _serverState.currentAmountOfPlayers = 0;
                UnityEngine.SceneManagement.SceneManager.LoadScene("menu");
                Debug.Log($"CLIENT: Server disconnected!");
            }
        }
        #endregion
    }
}
