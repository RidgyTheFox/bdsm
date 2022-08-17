using LiteNetLib;

namespace BDSM
{
    /// <summary>
    /// This class can be used to get acces to different parts of game or mod. Be careful with it...
    /// </summary>
    public static class StaticData
    {
        #region Fields.
        private static Client _clientInstance;              // Client class reference.
        private static NetManager _clientNetManInstance;    // Client LiteNetLib NetManager.
        private static NetManager _serverNetManInstance;    // Server LiteNetLib NetManager.
        private static ClockMachine _clockMachine;
        private static DiscordIntegration _discordIntegrationInstance;

        private static FreeMode.TimeKeeper _timeKeeper;

        private static MainMenu _mainMenuInstsance;
        #endregion


        #region Properties.
        public static Client clientInstance { get { return _clientInstance; } set { _clientInstance = value; } }
        public static NetManager clientNetManInstance { get { return _clientNetManInstance; } set { _clientNetManInstance = value; } }
        public static NetManager serverNetManInstance { get { return _serverNetManInstance; } set { _serverNetManInstance = value; } }
        public static ClockMachine clockMachine { get { return _clockMachine; } set { _clockMachine = value; } }
        public static DiscordIntegration discordIntegrationInstance { get { return _discordIntegrationInstance; } set { _discordIntegrationInstance = value; } }

        public static FreeMode.TimeKeeper timeKeeper { get { return _timeKeeper; } set { _timeKeeper = value; } }

        public static MainMenu mainMenuInstance { get { return _mainMenuInstsance; } set { _mainMenuInstsance = value; } }
        #endregion
    }
}
