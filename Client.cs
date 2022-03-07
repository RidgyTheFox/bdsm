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
    public class Client : MonoBehaviour, LiteNetLib.INetEventListener
    {
        private NetManager _client;
        private NetDataWriter _writer;
        private NetPacketProcessor _packetProcessor;

        private NetPeer _server;
        public bool _isConnected = false;
        private bool _isAuthorized = false;

        private string _nickname = "test";
        private string _serverIp = "127.0.0.1";
        private int _serverPort = 2022;
        private bool _usePassword = true;
        private string _password = "bdsmIsCool";

        private Vector3 _remotePlayerPositionOffset = new Vector3(0.0f, -0.39f, 0.0f);

        private Network.NestedTypes.PlayerState _localPlayerState;
        private Network.NestedTypes.ServerState _serverState;
        private Dictionary<uint, Network.ClientPackets.RemotePlayer> _remotePlayers;

        public bool isSceneLoaded = false;
        private bool isBusesForRemoteClientsWasCreated = false;
        public GameObject localPlayerBus;

        #region Client GUI data;
        private bool _isMainWindowOpened = true;
        private bool _mainWindowMoveMode = false;

        private uint _mainWindowPosX = 0;
        private uint _mainWindowPosY = 57;

        private GUIStyle _netStatsTextStyle;
        #endregion

        private void Awake()
        {
            Debug.Log("CLIENT: Initializing...");
            StaticData.clientInstance = this;

            _netStatsTextStyle = new GUIStyle();
            _netStatsTextStyle.normal.textColor = Color.green;
            _netStatsTextStyle.fontStyle = FontStyle.Bold;

            _client = new NetManager(this) { AutoRecycle = true };
            _writer = new NetDataWriter();

            _localPlayerState = new Network.NestedTypes.PlayerState { selectedBusShortName = FreeMode.PlayerData.GetCurrentData().boughtBuses[FreeMode.PlayerData.GetCurrentData().selectedBus].ShortName, position = new Vector3(1.0f, 2.0f, 3.0f), rotation = new Quaternion(4.0f, 3.0f, 2.0f, 1.0f) };
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
            Debug.Log("CLIENT: Registering callbacks...");
            _packetProcessor.SubscribeReusable<BDSM.Network.ClientPackets.OnJoinAccepted>(OnJoinRequestAccepted);
            _packetProcessor.SubscribeReusable<BDSM.Network.ClientPackets.OnJoinDeclined>(OnJoinRequestDeclined);
            _packetProcessor.SubscribeReusable<BDSM.Network.ClientPackets.ReceiveServerState>(OnReceiveServerState);
            _packetProcessor.SubscribeReusable<BDSM.Network.ClientPackets.AddRemotePlayer>(OnAddRemotePlayer);
            _packetProcessor.SubscribeReusable<BDSM.Network.ClientPackets.RemoveRemotePlayer>(OnRemoveRemotePlayer);
            _packetProcessor.SubscribeReusable<BDSM.Network.ClientPackets.RemotePlayerChangedBus>(OnRemotePlayerChangedBus);
            _packetProcessor.SubscribeReusable<BDSM.Network.ClientPackets.UpdateRemotePlayers>(OnUpdateRemotePlayers);

            ReloadSettings();
            Debug.Log("CLIENT: Initialized!");
        }

        private void FixedUpdate()
        {
            _client.PollEvents();

            if (Input.GetKeyDown(KeyCode.F1))
                _isMainWindowOpened = !_isMainWindowOpened;

            if (Input.GetKeyDown(KeyCode.End))
            {
                _serverIp = "127.0.0.1";
                _serverPort = 2022;
                _usePassword = true;
                _password = "bdsmIsCool";
            }
            
            /* Example how to send commands to bus controllers.
            if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                Network.ClientPackets.RemotePlayer l_player;
                _remotePlayers.TryGetValue(1, out l_player);
                if (l_player != null)
                {
                    Debug.Log("CLIENT: Calling...");
                    l_player.remotePlayerController.OnTriggerToggleAction("Fucking test");
                }
                else
                    Debug.LogError("CLIENT:Cannot find player!");
            }*/

            if (_isConnected && _isAuthorized && isSceneLoaded && !isBusesForRemoteClientsWasCreated)
                CreateBusesForRemotePlayers();

            if (_isConnected && _isAuthorized && isSceneLoaded && isBusesForRemoteClientsWasCreated)
            {
                foreach (Network.ClientPackets.RemotePlayer l_player in _remotePlayers.Values)
                {
                    if (l_player.remotePlayerBus != null)
                    {
                        l_player.remotePlayerBus.transform.position = l_player.state.position + _remotePlayerPositionOffset;
                        l_player.remotePlayerBus.transform.rotation = l_player.state.rotation;
                    }
                }

                if (localPlayerBus == null)
                {
                    _localPlayerState.position = new Vector3(-999.0f, -999.0f, -999.0f);
                    _localPlayerState.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
                }
                else
                {
                    _localPlayerState.position = localPlayerBus.transform.position;
                    _localPlayerState.rotation = localPlayerBus.transform.rotation;
                }

                SendPacket( new Network.ServerPackets.UpdatePlayerState { pid = _localPlayerState.pid, state = _localPlayerState }, DeliveryMethod.Unreliable);
            }
        }

        private void OnGUI()
        {
            if (_isConnected && _isAuthorized)
                GUI.Label(new Rect(5, Screen.currentResolution.height-20, 300, 20), $"Connected to {_serverIp}:{_serverPort} | Ping: {_server.Ping}", _netStatsTextStyle);

            if (_isMainWindowOpened)
            {
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
                }
                else
                {
                    if (GUI.Button(new Rect(_mainWindowPosX + 5, _mainWindowPosY + 110, 200, 20), "Connect"))
                    {
                        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "menu")
                            Connect();
                        else
                            Debug.LogError("CLIENT: Cannot connect! First you need to exit to the main menu.");
                    }
                    if (GUI.Button(new Rect(_mainWindowPosX + 5, _mainWindowPosY + 130, 200, 20), "Reload settings"))
                        ReloadSettings();
                }
            }
        }

        private void Connect()
        {
            if (!_isConnected)
            {
                if (_usePassword)
                {
                    isSceneLoaded = false;
                    _client.Start();
                    _client.Connect(_serverIp, _serverPort, _password);
                    Debug.Log($"CLIENT: Connecting to {_serverIp}:{_serverPort} with password...");
                }
                else
                {
                    isSceneLoaded = false;
                    _client.Start();
                    _client.Connect(_serverIp, _serverPort, "ass");
                    Debug.Log($"CLIENT: Connecting to {_serverIp}:{_serverPort} without password.");
                }
            }
        }

        public void Disconnect()
        {
            if (_isConnected)
            {
                foreach (Network.ClientPackets.RemotePlayer l_player in _remotePlayers.Values)
                {
                    if (l_player.remotePlayerBus != null)
                        GameObject.Destroy(l_player.remotePlayerBus);
                }

                isSceneLoaded = false;
                isBusesForRemoteClientsWasCreated = false;
                UnityEngine.SceneManagement.SceneManager.LoadScene("menu");

                _isConnected = false;
                _isAuthorized = false;
                _client.DisconnectAll();
                _client.Stop();
                _server = null;
                _remotePlayers.Clear();
                Debug.Log("CLIENT: Disconnected!");
            }
        }

        private void ReloadSettings()
        {
            if (_isConnected)
            {
                Debug.LogError("CLIENT: Unable to reload settings because the client is connected!");
                return;
            }

            Debug.Log("SERVER: Reloading settings...");

            JObject l_settings = JObject.Parse(System.IO.File.ReadAllText("ClientConfig.json"));
            JToken l_valueToken;

            l_settings.TryGetValue("nickname", out l_valueToken);
            if (l_valueToken != null)
            {
                _nickname = l_valueToken.ToString();
                Debug.Log($"CLIENT: Nickname was set to \"{_nickname}\".");
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
                    Debug.Log("CLIENT: Password was set..");
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
                    UnityEngine.SceneManagement.SceneManager.LoadScene("FreeModeRoutes");
                    break;
                case Enums.AvailableMaps.SERPUKHOV_WINTER:
                    FileBasedPrefs.SetInt("winter", 1);
                    FileBasedPrefs.SetBool("IsAutomatic", true);
                    UnityEngine.SceneManagement.SceneManager.LoadScene("FreeModeRoutes");
                    break;
                case Enums.AvailableMaps.KELN:
                    FileBasedPrefs.SetInt("winter", 1);
                    FileBasedPrefs.SetBool("IsAutomatic", true);
                    UnityEngine.SceneManagement.SceneManager.LoadScene("FreeModeKeln");
                    break;
                case Enums.AvailableMaps.MUROM:
                    StaticData.mainMenuInstance.winterToggleMurom.isOn = false;
                    StaticData.mainMenuInstance.StartCampain(3);
                    break;
                case Enums.AvailableMaps.MUROM_WINTER:
                    StaticData.mainMenuInstance.winterToggleMurom.isOn = true;
                    StaticData.mainMenuInstance.StartCampain(3);
                    break;
                case Enums.AvailableMaps.SOLNECHNOGORSK:
                    StaticData.mainMenuInstance.StartCampain(4);
                    break;
            }
        }

        private void CreateBusesForRemotePlayers()
        {
            if (!isBusesForRemoteClientsWasCreated)
            {
                foreach (Network.ClientPackets.RemotePlayer l_player in _remotePlayers.Values)
                {
                    if (l_player.remotePlayerBus == null)
                    {
                        l_player.remotePlayerBus = GameObject.Instantiate(FreeMode.Garage.GaragePrefabStorage.GetSingleton().GetPrefab(l_player.state.selectedBusShortName, true));
                        CreateAssociatedControolerForBus(l_player);
                    }
                }
                isBusesForRemoteClientsWasCreated = true;
            }
        }

        private void CreateAssociatedControolerForBus(Network.ClientPackets.RemotePlayer l_player)
        {
            switch (l_player.state.selectedBusShortName)
            {
                case "SPR": l_player.remotePlayerController = l_player.remotePlayerBus.AddComponent<RemotePlayerControllers.RmeotePlayerController_Sprinter>(); break;
                default: Debug.LogError($"CLIENT: Controller for \"{l_player.state.selectedBusShortName}\" nor found!"); break;
            }
        }

        public void OnJoinRequestAccepted(Network.ClientPackets.OnJoinAccepted l_packet)
        {
            _localPlayerState.pid = l_packet.pid;
            _isAuthorized = true;
            Debug.Log($"CLIENT: Join request was accepted! Given PID is {l_packet.pid}. Requesting server state...");
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
            Debug.LogError($"CLIENT: Cannot join on server! Reason: {l_packet.message}.");
        }

        public void OnReceiveServerState(Network.ClientPackets.ReceiveServerState l_packet)
        {
            Debug.Log($"CLIENT: Server state was received! Server name: {l_packet.serverName}. Current map: {EnumUtils.MapUintToEnum(l_packet.currentMap)}. Players limit: {l_packet.playersLimit}. Amount of players: {l_packet.currentAmountOfPlayers}.");
            _serverState = new Network.NestedTypes.ServerState { serverName = l_packet.serverName, currentMap = EnumUtils.MapUintToEnum(l_packet.currentMap), playersLimit = l_packet.playersLimit, currentAmountOfPlayers = l_packet.currentAmountOfPlayers };
            ProceedMapLoading();
        }

        public void OnAddRemotePlayer(Network.ClientPackets.AddRemotePlayer l_packet)
        {
            Network.NestedTypes.PlayerState l_newPlayerState = new Network.NestedTypes.PlayerState { pid = l_packet.state.pid, selectedBusShortName = l_packet.state.selectedBusShortName, position = l_packet.state.position, rotation = l_packet.state.rotation };
            Network.ClientPackets.RemotePlayer l_newPlayer = new Network.ClientPackets.RemotePlayer { nickname = l_packet.nickname, remotePlayerBus = null, state = l_newPlayerState };
            
            if (isSceneLoaded && isBusesForRemoteClientsWasCreated)
            {
                l_newPlayer.remotePlayerBus = GameObject.Instantiate(FreeMode.Garage.GaragePrefabStorage.GetSingleton().GetPrefab(l_newPlayer.state.selectedBusShortName, true));
                CreateAssociatedControolerForBus(l_newPlayer);
            }

            _remotePlayers.Add(l_newPlayer.state.pid, l_newPlayer);
            _serverState.currentAmountOfPlayers++;

            Debug.Log($"CLIENT: Remote player for {l_newPlayer.nickname}[{l_newPlayer.state.pid}] was created. His bus is {l_newPlayer.state.selectedBusShortName}.");
        }

        public void OnRemoveRemotePlayer(Network.ClientPackets.RemoveRemotePlayer l_packet)
        {
            Debug.Log($"CLIENT: Searching remote player with PID {l_packet.pid} for removing...");
            Network.ClientPackets.RemotePlayer l_playerToRemove;
            _remotePlayers.TryGetValue(l_packet.pid, out l_playerToRemove);

            if (l_playerToRemove != null)
            {
                if (l_playerToRemove.remotePlayerBus != null)
                    GameObject.Destroy(l_playerToRemove.remotePlayerBus);

                _remotePlayers.Remove(l_packet.pid);
                _serverState.currentAmountOfPlayers--;

                Debug.Log($"CLIENT: Remote player {l_playerToRemove.nickname}[{l_playerToRemove.state.pid}] was removed.");
            }
            else
                Debug.LogError($"CLIENT: Cannot find remote player with PID {l_packet.pid}!");
        }

        public void OnRemotePlayerChangedBus(Network.ClientPackets.RemotePlayerChangedBus l_packet)
        {
            Debug.Log($"CLIENT: Player with PID {l_packet.pid} changed bus. Searching...");
            Network.ClientPackets.RemotePlayer l_playerForEdit;
            _remotePlayers.TryGetValue(l_packet.pid, out l_playerForEdit);
            if (l_playerForEdit != null)
            {
                Network.NestedTypes.PlayerState l_newPlayerState = new Network.NestedTypes.PlayerState { pid = l_playerForEdit.state.pid, selectedBusShortName = l_packet.busShortName, position = l_playerForEdit.state.position, rotation = l_playerForEdit.state.rotation };
                l_playerForEdit.state = l_newPlayerState;

                if (l_playerForEdit.remotePlayerBus != null)
                    GameObject.Destroy(l_playerForEdit.remotePlayerBus);

                l_playerForEdit.remotePlayerBus = GameObject.Instantiate(FreeMode.Garage.GaragePrefabStorage.GetSingleton().GetPrefab(l_playerForEdit.state.selectedBusShortName, true));
                CreateAssociatedControolerForBus(l_playerForEdit);
                

                _remotePlayers.Remove(l_packet.pid);
                _remotePlayers.Add(l_packet.pid, l_playerForEdit);
                Debug.Log($"CLIENT: Bus for {l_playerForEdit.nickname}[{l_playerForEdit.state.pid}] was changed to {l_playerForEdit.state.selectedBusShortName}.");
            }
            else
                Debug.LogError($"CLIENT: Cannot find remote player for {l_packet.pid}!");
        }

        public void OnUpdateRemotePlayers(Network.ClientPackets.UpdateRemotePlayers l_packet)
        {
            foreach(Network.NestedTypes.PlayerState l_newState in l_packet.states)
            {
                Network.ClientPackets.RemotePlayer l_playerToEdit;

                if (l_newState.selectedBusShortName != null)
                {
                    if (l_newState.pid != _localPlayerState.pid)
                    {
                        _remotePlayers.TryGetValue(l_newState.pid, out l_playerToEdit);
                        if (l_playerToEdit != null)
                        {
                            l_playerToEdit.state = l_newState;
                            _remotePlayers.Remove(l_newState.pid);
                            _remotePlayers.Add(l_newState.pid, l_playerToEdit);
                        }
                    }
                }
            }
        }

        public void OnPlayerChangedBusInGarage()
        {
            _localPlayerState.selectedBusShortName = FreeMode.PlayerData.GetCurrentData().boughtBuses[FreeMode.PlayerData.GetCurrentData().selectedBus].ShortName;
            SendPacket( new Network.ServerPackets.ChangeBus { pid = _localPlayerState.pid, busShortName = _localPlayerState.selectedBusShortName }, DeliveryMethod.ReliableOrdered);
            Debug.Log($"CLIENT: Bus was changed to {_localPlayerState.selectedBusShortName}.");
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

        public void OnConnectionRequest(ConnectionRequest request)
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
            Debug.Log("CLIENT: Connected!");
            SendPacket(new Network.ServerPackets.RequestJoin { nickname = _nickname, state = _localPlayerState }, DeliveryMethod.ReliableOrdered);
        }

        public void OnPeerDisconnected(NetPeer l_peer, DisconnectInfo l_disconnectInfo)
        {
            if (l_peer == _server)
            {
                _isAuthorized = false;
                _isConnected = false;
                isSceneLoaded = false;
                isBusesForRemoteClientsWasCreated = false;
                _server = null;
                _client.Stop();
                _remotePlayers.Clear();
                UnityEngine.SceneManagement.SceneManager.LoadScene("menu");
                Debug.Log($"CLIENT: Server disconnected!");
            }
        }

    }
}
