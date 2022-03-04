namespace BDSM
{
    public static class StaticData
    {
        #region Fields.
        private static Client _clientInstance;

        private static MainMenu _mainMenuInstsance;
        #endregion


        #region Properties.
        public static Client clientInstance { get { return _clientInstance; } set { _clientInstance = value; } }

        public static MainMenu mainMenuInstance { get { return _mainMenuInstsance; } set { _mainMenuInstsance = value; } }
        #endregion
    }
}
