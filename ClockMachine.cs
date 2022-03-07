using System;
using System.Timers;
using UnityEngine;

namespace BDSM
{
    public class ClockMachine : MonoBehaviour
    {
        public struct DateAndTime
        {
            public uint days;
            public uint hours;
            public uint minutes;
            public uint seconds;
        }

        private static System.Timers.Timer _mainTimer;

        private static uint _currentDay;
        private static uint _currentHour;
        private static uint _currentMinute;
        private static uint _currentSecond;

        private bool _isClockStopped;

        #region Properties.
        public uint currentDay { get { return _currentDay; } }
        public uint currentHour { get { return _currentHour; } }
        public uint currentMinute { get { return _currentMinute; } }
        public uint currentSecond { get { return _currentSecond; } }

        public bool isClockStopped { get { return _isClockStopped; } }
        #endregion

        private void Awake()
        {
            _currentDay = 0;
            _currentHour = 0;
            _currentMinute = 0;
            _currentSecond = 0;

            _mainTimer = new Timer(1000);
            _mainTimer.AutoReset = true;
            _mainTimer.Elapsed += OnClockMachineTimerElapsed;

            StaticData.clockMachine = this;
        }

        private static void OnClockMachineTimerElapsed(object source, ElapsedEventArgs e)
        {
            if (_currentSecond < 59)
                _currentSecond++;
            else
            {
                _currentSecond = 0;
                if (_currentMinute < 59)
                    _currentMinute++;
                else
                {
                    _currentMinute = 0;
                    if (_currentHour < 23)
                        _currentHour++;
                    else
                    {
                        _currentHour = 0;
                        _currentDay++;
                    }
                }
            }
        }

        public void SetTime(uint l_day, uint l_hour, uint l_minute, uint l_second)
        {
            _currentDay = l_day;

            if (l_hour < 24)
                _currentHour = l_hour;
            else
                _currentHour = 24;

            if (l_minute < 60)
                _currentMinute = l_minute;
            else
                _currentMinute = 0;

            if (l_second < 60)
                _currentSecond = l_second;
            else
                _currentSecond = 0;

            Debug.Log($"CLOCK_MACHINE: Time was set to {_currentDay} Day, {_currentHour}:{_currentMinute}:{_currentSecond} (HH:MM:SS).");
        }

        public DateAndTime GetTime()
        {
            return new DateAndTime { days = _currentDay, hours = _currentHour, minutes = _currentMinute, seconds = _currentSecond };
        }

        public void StartTime()
        {
            _mainTimer.Start();
            _isClockStopped = false;
            Debug.Log("CLOCK_MACHIME: Clock machine started!");
        }

        public void StopTime()
        {
            _mainTimer.Stop();
            _isClockStopped = true;
            Debug.Log("CLOCK_MACHINE: Clock machine stopped!");
        }
    }
}
