using System;
using System.Collections.Generic;
using UnityEngine;

namespace BDSM.BDSMGUI
{
    class PlayersList : MonoBehaviour
    {
        internal class _PlayerListElement
        {
            private string _nickname = "Test";
            private uint _playerPid = 999;

            public void OnGUI()
            {

            }
        }

        private bool _isInitialized = false;
        private bool _showWindow = false;
        private uint _maxPlayers;
        private uint _amountOfPlayers;
        private string _serverName;

        private int _windowPositionX = 0;
        private int _windowPositionY = 0;

        private void FixedUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F3))
                _showWindow = true;
        }

        private void OnGUI()
        {
            if (_isInitialized)
            {
                if (_showWindow)
                {
                    GUI.Box(new Rect(_windowPositionX, _windowPositionX, 500, 500), $"{_serverName} | Players list ({_amountOfPlayers} of {_maxPlayers}) | F3");
                }
            }
        }

        public void Init(string l_serverName, uint l_maxPlayers)
        {
            if (!_isInitialized)
            {
                _maxPlayers = l_maxPlayers;
                _serverName = l_serverName;
                _isInitialized = true;
            }
        }

        public void AddPlayer(string l_nickname, uint l_playerPid)
        {

        }

        public void SetPlayersAmount(uint l_playersAmount)
        {
            _amountOfPlayers = l_playersAmount;
        }

        public void SwitchVisibility()
        {
            _showWindow = !_showWindow;
        }
    }
}
