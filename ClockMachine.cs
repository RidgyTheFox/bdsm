using System;
using System.Timers;
using UnityEngine;

namespace BDSM
{
    /// <summary>
    /// This class can save current in-game time, restore it from saved, and count time itself.
    /// </summary>
    public class ClockMachine : MonoBehaviour
    {
        /// <summary>
        /// This struct contain time.
        /// </summary>
        public struct DateAndTime
        {
            public uint days;
            public uint hours;
            public uint minutes;
            public uint seconds;
        }

        private DateAndTime _savedDateAndTime;
        private bool _isDateAndTimeWasSaved = false;

        private static System.Timers.Timer _mainTimer; // This timer will count time.

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

            // Dont change timer time!
            _mainTimer = new Timer(1000);
            _mainTimer.AutoReset = true;
            _mainTimer.Elapsed += OnClockMachineTimerElapsed;

            // Lets add our current instance to StaticData so other parts of plugin can use us.
            StaticData.clockMachine = this;
        }

        private void Update()
        {
        }

        /// <summary>
        /// This is a callback for timer. It counts time! "Time machine!" :P
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private static void OnClockMachineTimerElapsed(object source, ElapsedEventArgs e)
        {
            if (_currentSecond < 59)
            {
                _currentSecond++;
                return;
            }

            _currentSecond = 0;
            if (_currentMinute < 59)
            {
                _currentMinute++;
                return;
            }

            _currentMinute = 0;
            if (_currentHour < 23)
                _currentHour++;
            else
            {
                _currentHour = 0;
                _currentDay++;
            }
        }

        /// <summary>
        /// This function allows you to change internal (in ClockMachine, not in game) time.
        /// </summary>
        /// <param name="l_day">Current day.</param>
        /// <param name="l_hour">Current hour.</param>
        /// <param name="l_minute">Current minute.</param>
        /// <param name="l_second">Current second.</param>
        public void SetTime(uint l_day, uint l_hour, uint l_minute, uint l_second)
        {
            _currentDay = l_day;
            _currentHour = l_hour;
            _currentMinute = l_minute;
            _currentSecond = l_second;

            Debug.Log($"CLOCK_MACHINE: Time has been set to {_currentDay} Day, {_currentHour}:{_currentMinute}:{_currentSecond} (HH:MM:SS).");
        }

        /// <summary>
        /// This function will return you current ClockMachine time.
        /// </summary>
        /// <returns>Current ClockMachine time.</returns>
        public DateAndTime GetTime()
        {
            return new DateAndTime { days = _currentDay, hours = _currentHour, minutes = _currentMinute, seconds = _currentSecond };
        }

        /// <summary>
        /// This function starts the passage of time in the ClockMachine.
        /// </summary>
        public void StartTime()
        {
            _mainTimer.Start();
            _isClockStopped = false;
            Debug.Log("CLOCK_MACHIME: Clock machine started!");
        }

        /// <summary>
        /// This function starts the passage of time in the ClockMachine.
        /// </summary>
        public void StopTime()
        {
            _mainTimer.Stop();
            _isClockStopped = true;
            Debug.Log("CLOCK_MACHINE: Clock machine stopped!");
        }

        /// <summary>
        /// This function can save current in-game time. Be carefull, this time will be saved until game is running.
        /// </summary>
        public void SaveCurrentDateAndTime()
        {
            if (StaticData.timeKeeper == null)
            {
                Debug.LogError("CLOCK_MACHINE: timeKeeper is null! Cannot save time!");
                _isDateAndTimeWasSaved = false;
                return;
            }

            _savedDateAndTime = new DateAndTime {
                days = (uint)StaticData.timeKeeper.Day,
                hours = (uint)StaticData.timeKeeper.Hour,
                minutes = (uint)StaticData.timeKeeper.Minute,
                seconds = (uint)StaticData.timeKeeper.Second
            };
            _isDateAndTimeWasSaved = true;
            Debug.Log($"CLOCK_MACHINE: Time {_savedDateAndTime.days}:{_savedDateAndTime.hours}:{_savedDateAndTime.minutes}:{_savedDateAndTime.seconds} has been saved.");
        }

        /// <summary>
        /// This function can restore in-game time from variable if you already saved it some time ago.
        /// </summary>
        public void RestoreSavedDateAndTime()
        {
            if (!_isDateAndTimeWasSaved)
            {
                Debug.LogError("CLOCK_MACHINE: Cannot restore time because it wasn't saved!");
                return;
            }
            if (StaticData.timeKeeper == null)
            {
                Debug.LogError("CLOCK_MACHINE: timeKeeper is null! Cannot restore time!");
                _isDateAndTimeWasSaved = false;
                return;
            }

            StaticData.timeKeeper.Day = (int)_savedDateAndTime.days;
            StaticData.timeKeeper.Hour = (int)_savedDateAndTime.hours;
            StaticData.timeKeeper.Minute = (int)_savedDateAndTime.minutes;
            StaticData.timeKeeper.Second = (int)_savedDateAndTime.seconds;

            _isDateAndTimeWasSaved = false;
            Debug.Log($"CLOCK_MACHINE: Time {_savedDateAndTime.days}:{_savedDateAndTime.hours}:{_savedDateAndTime.minutes}:{_savedDateAndTime.seconds} has been restored.");
        }
    }
}
