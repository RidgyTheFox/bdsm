namespace BDSM
{
    public static class StaticData
    {
        #region Fields.
        private static Client _clientInstance;
        private static ClockMachine _clockMachine;

        private static FreeMode.TimeKeeper _timeKeeper;

        private static MainMenu _mainMenuInstsance;
        #endregion


        #region Properties.
        public static Client clientInstance { get { return _clientInstance; } set { _clientInstance = value; } }
        public static ClockMachine clockMachine { get { return _clockMachine; } set { _clockMachine = value; } }

        public static FreeMode.TimeKeeper timeKeeper { get { return _timeKeeper; } set { _timeKeeper = value; } }

        public static MainMenu mainMenuInstance { get { return _mainMenuInstsance; } set { _mainMenuInstsance = value; } }
        #endregion
    }
}
