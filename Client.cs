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
        private bool _isConnected = false;
        private bool _isAuthorized = false;

        private string _nickname = "test";
        private string _serverIp = "127.0.0.1";
        private int _serverPort = 2022;
        private bool _usePassword = true;
        private string _password = "bdsmIsCool";

        private Network.NestedTypes.PlayerState _localPlayerState;
        private Network.NestedTypes.ServerState _serverState;
        private Dictionary<uint, Network.ClientPackets.RemotePlayer> _remotePlayers;

        #region Client GUI data;
        private bool _isMainWindowOpened = true;
        private bool _mainWindowMoveMode = false;

        private uint _mainWindowPosX = 0;
        private uint _mainWindowPosY = 0;
        #endregion

        private void Awake()
        {
            Debug.Log("CLIENT: Initializing...");
            StaticData.clientInstance = this;
            _client = new NetManager(this) { AutoRecycle = true };
            _writer = new NetDataWriter();

            _localPlayerState = new Network.NestedTypes.PlayerState { isBusHided = true, position = new Vector3(1.0f, 2.0f, 3.0f), rotation = new Quaternion(4.0f, 3.0f, 2.0f, 1.0f) };
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
            Debug.Log("CLIENT: Registering callbacks...");
            _packetProcessor.SubscribeReusable<BDSM.Network.ClientPackets.OnJoinAccepted>(OnJoinRequestAccepted);
            _packetProcessor.SubscribeReusable<BDSM.Network.ClientPackets.OnJoinDeclined>(OnJoinRequestDeclined);
            _packetProcessor.SubscribeReusable<BDSM.Network.ClientPackets.ReceiveServerState>(OnReceiveServerState);
            _packetProcessor.SubscribeReusable<BDSM.Network.ClientPackets.AddRemotePlayer>(OnAddRemotePlayer);
            _packetProcessor.SubscribeReusable<BDSM.Network.ClientPackets.RemoveRemotePlayer>(OnRemoveRemotePlayer);

            ReloadSettings();
            Debug.Log("CLIENT: Initialized!");
        }

        private void FixedUpdate()
        {
            _client.PollEvents();

            if (Input.GetKeyDown(KeyCode.F1))
                _isMainWindowOpened = !_isMainWindowOpened;
        }

        private void OnGUI()
        {
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
                    {
                        UnityEngine.SceneManagement.SceneManager.LoadScene("menu");
                        Disconnect();
                    }
                }
                else
                {
                    if (GUI.Button(new Rect(_mainWindowPosX + 5, _mainWindowPosY + 110, 200, 20), "Connect"))
                    {
                        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "menu")
                        {
                            Connect();
                        }
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
                    _client.Start();
                    _client.Connect(_serverIp, _serverPort, _password);
                    Debug.Log($"CLIENT: Connecting to {_serverIp}:{_serverPort} with password...");
                }
                else
                {
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
                case Enums.AvailableMaps.SERPUHOV:
                    FileBasedPrefs.SetInt("winter", 0);
                    FileBasedPrefs.SetBool("IsAutomatic", true);
                    UnityEngine.SceneManagement.SceneManager.LoadScene("FreeModeRoutes");
                    break;
                case Enums.AvailableMaps.SERPUHOV_WINTER:
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
            Network.NestedTypes.PlayerState l_newPlaterState = new Network.NestedTypes.PlayerState { pid = l_packet.state.pid, isBusHided = l_packet.state.isBusHided, position = l_packet.state.position, rotation = l_packet.state.rotation };
            Network.ClientPackets.RemotePlayer l_newPlayer = new Network.ClientPackets.RemotePlayer { nickname = l_packet.nickname, remotePlayerBus = null, selectedBus = Enums.AvailableBuses.UNKNOWN, state = l_newPlaterState };
            _remotePlayers.Add(l_newPlayer.state.pid, l_newPlayer);
            _serverState.currentAmountOfPlayers++;
            Debug.Log($"CLIENT: Remote player for {l_newPlayer.nickname}[{l_newPlayer.state.pid}] was created.");
        }

        public void OnRemoveRemotePlayer(Network.ClientPackets.RemoveRemotePlayer l_packet)
        {
            Debug.Log($"CLIENT: Searching remote player with PID {l_packet.pid} for removing...");
            Network.ClientPackets.RemotePlayer l_playerToRemove;
            _remotePlayers.TryGetValue(l_packet.pid, out l_playerToRemove);

            if (l_playerToRemove != null)
            {
                _remotePlayers.Remove(l_packet.pid);
                _serverState.currentAmountOfPlayers--;
                Debug.Log($"CLIENT: Remote player {l_playerToRemove.nickname}[{l_playerToRemove.state.pid}] was removed.");
            }
            else
                Debug.LogError($"CLIENT: Cannot find remote player with PID {l_packet.pid}!");
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
                _server = null;
                _client.Stop();
                _remotePlayers.Clear();
                Debug.Log($"CLIENT: Server disconnected!");
            }
        }

    }
}
