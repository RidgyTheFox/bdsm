using System;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using System.Collections.Generic;
using UnityEngine;

namespace BDSM
{
    /// <summary>
    /// This is a class that provides Statistics window and statistics dumping functions.
    /// </summary>
    class StatisticsWindow : MonoBehaviour
    {
        #region Variables for calculating speeds.
        private long clientOldBytesReceivedPerSecond = 0;
        private long clientOldBytesSendPerSecond = 0;
        private long clientOldPacketsReceivedPerSecond = 0;
        private long clientOldPacketsSendPerSecond = 0;

        private long clientBytesReceivedPerSecond = 0;
        private long clientBytesSendPerSecond = 0;
        private long clientPacketsReceivedPerSecond = 0;
        private long clientPacketsSendPerSecond = 0;
                
        private long serverOldBytesReceivedPerSecond = 0;
        private long serverOldBytesSendPerSecond = 0;
        private long serverOldPacketsReceivedPerSecond = 0;
        private long serverOldPacketsSendPerSecond = 0;
                
        private long serverBytesReceivedPerSecond = 0;
        private long serverBytesSendPerSecond = 0;
        private long serverPacketsReceivedPerSecond = 0;
        private long serverPacketsSendPerSecond = 0;

        private int clientBytesRXSpeed = 0;
        private int clientBytesTXSpeed = 0;
        private int clientPacketsRXSpeed = 0;
        private int clientPacketsTXSpeed = 0;

        private int serverBytesRXSpeed = 0;
        private int serverBytesTXSpeed = 0;
        private int serverPacketsRXSpeed = 0;
        private int serverPacketsTXSpeed = 0;
        #endregion

        private static System.Timers.Timer _speedCountTimer;

        private uint _statisticsWindowPosX = 0;
        private uint _statisticsWindowPosY = 366;

        private bool _isStatisticsWindowOpened = false;
        private bool _statisticsWindowMoveMode = false;

        GUIStyle _fontCaptionStyle;
        GUIStyle _fontContentStyle;

        private void Awake()
        {
            _fontCaptionStyle = new GUIStyle();
            _fontCaptionStyle.normal.textColor = new Color(1.0f, 1.0f, 1.0f);
            _fontCaptionStyle.fontStyle = FontStyle.Bold;
            _fontCaptionStyle.fontSize = 16;

            _fontContentStyle = new GUIStyle();
            _fontContentStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f);
            _fontContentStyle.fontSize = 13;

            // This is a main timer that collects statistics and updating data in window.
            // Timer tick time defines resolution of speed meters.
            // Please keep it at least on 250 ms (4 updates per second) for saving perfomance.
            _speedCountTimer = new System.Timers.Timer(500);
            _speedCountTimer.AutoReset = true;
            _speedCountTimer.Elapsed += OnSpeedCountTimer;
            _speedCountTimer.Start();
        }

        /// <summary>
        /// This function is called every time the timer expires. Its collecting all data and calcualtes speeds.
        /// </summary>
        /// <param name="source">Not in use. Idk what is this.</param>
        /// <param name="e">Not in use. Idk what is this.</param>
        private void OnSpeedCountTimer(object source, ElapsedEventArgs e)
        {
            clientBytesReceivedPerSecond = StaticData.clientNetManInstance.Statistics.BytesReceived;
            clientBytesSendPerSecond = StaticData.clientNetManInstance.Statistics.BytesSent;
            clientPacketsReceivedPerSecond = StaticData.clientNetManInstance.Statistics.PacketsReceived;
            clientPacketsSendPerSecond = StaticData.clientNetManInstance.Statistics.PacketsSent;

            serverBytesReceivedPerSecond = StaticData.serverNetManInstance.Statistics.BytesReceived;
            serverBytesSendPerSecond = StaticData.serverNetManInstance.Statistics.BytesSent;
            serverPacketsReceivedPerSecond = StaticData.serverNetManInstance.Statistics.PacketsReceived;
            serverPacketsSendPerSecond = StaticData.serverNetManInstance.Statistics.PacketsSent;

            clientBytesRXSpeed = (int)(clientBytesReceivedPerSecond - clientOldBytesReceivedPerSecond) / 1024;
            clientBytesTXSpeed = (int)(clientBytesSendPerSecond - clientOldBytesSendPerSecond) / 1024;
            clientPacketsRXSpeed = (int)(clientPacketsReceivedPerSecond - clientOldPacketsReceivedPerSecond);
            clientPacketsTXSpeed = (int)(clientPacketsSendPerSecond - clientOldPacketsSendPerSecond);

            serverBytesRXSpeed = (int)(serverBytesReceivedPerSecond - serverOldBytesReceivedPerSecond) / 1024;
            serverBytesTXSpeed = (int)(serverBytesSendPerSecond - serverOldBytesSendPerSecond) / 1024;
            serverPacketsRXSpeed = (int)(serverPacketsReceivedPerSecond - serverOldPacketsReceivedPerSecond);
            serverPacketsTXSpeed = (int)(serverPacketsSendPerSecond - serverOldPacketsSendPerSecond);

            clientOldBytesReceivedPerSecond = clientBytesReceivedPerSecond;
            clientOldBytesSendPerSecond = clientBytesSendPerSecond;
            clientOldPacketsReceivedPerSecond = clientPacketsReceivedPerSecond;
            clientOldPacketsSendPerSecond = clientPacketsSendPerSecond;

            serverOldBytesReceivedPerSecond = serverBytesReceivedPerSecond;
            serverOldBytesSendPerSecond = serverBytesSendPerSecond;
            serverOldPacketsReceivedPerSecond = serverPacketsReceivedPerSecond;
            serverOldPacketsSendPerSecond = serverPacketsSendPerSecond;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F4))
                _isStatisticsWindowOpened = !_isStatisticsWindowOpened;
        }

        private void OnGUI()
        {
            if (!_isStatisticsWindowOpened)
                return;

            if (_statisticsWindowMoveMode)
            {
                _statisticsWindowPosX = (uint)Input.mousePosition.x - 10;
                // Cursor coordinates are taken from the bottom left corner.
                // And the coordinates of the windows come from the top left corner.
                // Therefore, you need to invert the coordinates along the Y axis.
                _statisticsWindowPosY = (uint)Screen.currentResolution.height - (uint)Input.mousePosition.y - 10;
            }

            GUI.Box(new Rect(_statisticsWindowPosX, _statisticsWindowPosY, 250, 375), "BDSM | Network Statistics (F4)");
            if (GUI.Button(new Rect(_statisticsWindowPosX + 1, _statisticsWindowPosY + 1, 23, 21), "M"))
                _statisticsWindowMoveMode = !_statisticsWindowMoveMode;
            if (GUI.Button(new Rect(_statisticsWindowPosX + 226, _statisticsWindowPosY, 23, 21), "X"))
            {
                _isStatisticsWindowOpened = !_isStatisticsWindowOpened;
                _statisticsWindowMoveMode = false;
            }

            GUI.Label(new Rect(_statisticsWindowPosX + 20, _statisticsWindowPosY + 30, 100, 20), "Client", _fontCaptionStyle);
            GUI.Label(new Rect(_statisticsWindowPosX + 5, _statisticsWindowPosY + 50, 100, 20), $"TX (bytes):\t{StaticData.clientNetManInstance.Statistics.BytesSent} ({clientBytesTXSpeed} Kb\\S)", _fontContentStyle);
            GUI.Label(new Rect(_statisticsWindowPosX + 5, _statisticsWindowPosY + 70, 100, 20), $"RX (bytes):\t{StaticData.clientNetManInstance.Statistics.BytesReceived} ({clientBytesRXSpeed} Kb\\S)", _fontContentStyle);
            GUI.Label(new Rect(_statisticsWindowPosX + 5, _statisticsWindowPosY + 90, 100, 20), $"TX (packets):\t{StaticData.clientNetManInstance.Statistics.PacketsSent} ({clientPacketsTXSpeed} P\\S)", _fontContentStyle);
            GUI.Label(new Rect(_statisticsWindowPosX + 5, _statisticsWindowPosY + 110, 100, 20), $"RX (packets):\t{StaticData.clientNetManInstance.Statistics.PacketsReceived} ({clientPacketsRXSpeed} P\\S)", _fontContentStyle);
            GUI.Label(new Rect(_statisticsWindowPosX + 5, _statisticsWindowPosY + 130, 100, 20), $"Lost packets:\t{StaticData.clientNetManInstance.Statistics.PacketLoss}", _fontContentStyle);
            GUI.Label(new Rect(_statisticsWindowPosX + 5, _statisticsWindowPosY + 150, 100, 20), $"Lost packets (%):\t{StaticData.clientNetManInstance.Statistics.PacketLossPercent}", _fontContentStyle);
            if (GUI.Button(new Rect(_statisticsWindowPosX + 5, _statisticsWindowPosY + 170, 240, 20), "Dump to file"))
                DumpClientStatistics();

            GUI.Label(new Rect(_statisticsWindowPosX + 20, _statisticsWindowPosY + 210, 100, 20), "Server", _fontCaptionStyle);
            GUI.Label(new Rect(_statisticsWindowPosX + 5, _statisticsWindowPosY + 230, 100, 20), $"TX (bytes):\t{StaticData.serverNetManInstance.Statistics.BytesSent} ({serverBytesTXSpeed} Kb\\S)", _fontContentStyle);
            GUI.Label(new Rect(_statisticsWindowPosX + 5, _statisticsWindowPosY + 250, 100, 20), $"RX (bytes):\t{StaticData.serverNetManInstance.Statistics.BytesReceived} ({serverBytesRXSpeed} Kb\\S)", _fontContentStyle);
            GUI.Label(new Rect(_statisticsWindowPosX + 5, _statisticsWindowPosY + 270, 100, 20), $"TX (packets):\t{StaticData.serverNetManInstance.Statistics.PacketsSent} ({serverPacketsTXSpeed} P\\S)", _fontContentStyle);
            GUI.Label(new Rect(_statisticsWindowPosX + 5, _statisticsWindowPosY + 290, 100, 20), $"RX (packets):\t{StaticData.serverNetManInstance.Statistics.PacketsReceived} ({serverPacketsRXSpeed} P\\S)", _fontContentStyle);
            GUI.Label(new Rect(_statisticsWindowPosX + 5, _statisticsWindowPosY + 310, 100, 20), $"Lost packets:\t{StaticData.serverNetManInstance.Statistics.PacketLoss}", _fontContentStyle);
            GUI.Label(new Rect(_statisticsWindowPosX + 5, _statisticsWindowPosY + 330, 100, 20), $"Lost packets (%):\t{StaticData.serverNetManInstance.Statistics.PacketLossPercent}", _fontContentStyle);
            if (GUI.Button(new Rect(_statisticsWindowPosX + 5, _statisticsWindowPosY + 350, 240, 20), "Dump to file"))
                DumpServerStatistics();
        }

        /// <summary>
        /// This function will collect and then dump client network statistics to the file that will be called as yyyy_mm_dd_hh_MM_ss_clientStatistics.dump Dont look at format, its just a plain text.
        /// </summary>
        public void DumpClientStatistics()
        {
            DateTime timeNow = DateTime.Now;
            string l_fileName = $"{timeNow.Year}_{timeNow.Month}_{timeNow.Day}_{timeNow.Hour}{timeNow.Minute}{timeNow.Second}_clientStatistics.dump";

            List<string> l_lines = new List<string>();
            l_lines.Add("BDSM Client Statistics Dump");
            l_lines.Add($"{timeNow.Year}\\{timeNow.Month}\\{timeNow.Day}  {timeNow.Hour}:{timeNow.Minute}:{timeNow.Second}");
            l_lines.Add($"Bytes transmitted:        {StaticData.clientNetManInstance.Statistics.BytesSent}");
            l_lines.Add($"Bytes received:           {StaticData.clientNetManInstance.Statistics.BytesReceived}");
            l_lines.Add($"Packets transmitted:      {StaticData.clientNetManInstance.Statistics.PacketsSent}");
            l_lines.Add($"Packets received:         {StaticData.clientNetManInstance.Statistics.PacketsReceived}");
            l_lines.Add($"Packets lost:             {StaticData.clientNetManInstance.Statistics.PacketLoss}");
            l_lines.Add($"Packets lost percentage:  {StaticData.clientNetManInstance.Statistics.PacketLossPercent}");
            l_lines.Add("-----END-----");

            WriteTextToFile(l_fileName, l_lines, false);
        }

        /// <summary>
        /// This function will collect and then dump server network statistics to the file that will be called as yyyy_mm_dd_hh_MM_ss_serverStatistics.dump Dont look at format, its just a plain text.
        /// </summary>
        private void DumpServerStatistics()
        {
            DateTime timeNow = DateTime.Now;
            string l_fileName = $"{timeNow.Year}_{timeNow.Month}_{timeNow.Day}_{timeNow.Hour}{timeNow.Minute}{timeNow.Second}_serverStatistics.dump";

            List<string> l_lines = new List<string>();
            l_lines.Add("BDSM Server Statistics Dump");
            l_lines.Add($"{timeNow.Year}\\{timeNow.Month}\\{timeNow.Day}  {timeNow.Hour}:{timeNow.Minute}:{timeNow.Second}");
            l_lines.Add($"Bytes transmitted:        {StaticData.serverNetManInstance.Statistics.BytesSent}");
            l_lines.Add($"Bytes received:           {StaticData.serverNetManInstance.Statistics.BytesReceived}");
            l_lines.Add($"Packets transmitted:      {StaticData.serverNetManInstance.Statistics.PacketsSent}");
            l_lines.Add($"Packets received:         {StaticData.serverNetManInstance.Statistics.PacketsReceived}");
            l_lines.Add($"Packets lost:             {StaticData.serverNetManInstance.Statistics.PacketLoss}");
            l_lines.Add($"Packets lost percentage:  {StaticData.serverNetManInstance.Statistics.PacketLossPercent}");
            l_lines.Add("-----END-----");

            WriteTextToFile(l_fileName, l_lines, false);
        }

        /// <summary>
        /// This is an async function for writing data into file.
        /// </summary>
        /// <param name="file">Filename for writing.</param>
        /// <param name="lines">Array of lines for writing.</param>
        /// <param name="append">Should function append new string to the old file if he exist and have same name.</param>
        public async void WriteTextToFile(string file, List<string> lines, bool append)
        {
            if (!append && File.Exists(file))
                File.Delete(file);

            using (var writer = File.OpenWrite(file))
            {
                using (var streamWriter = new StreamWriter(writer))
                    foreach (var line in lines)
                        await streamWriter.WriteLineAsync(line);
            }
        }
    }
}
