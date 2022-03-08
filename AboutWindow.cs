using UnityEngine;

namespace BDSM
{
    class AboutWindow : MonoBehaviour
    {
        private GUIStyle _captionTextStyle;
        private GUIStyle _textStyle;

        private bool _showWindow = true;
        private bool _moveMode = false;
        private int _windowPositionX;
        private int _windowPositionY;
        private const int _windowSizeX = 375;
        private const int _windowSizeY = 385;

        private string _aboutText = "This is a small mod for Bus Driver Simulator.\n" +
                                    "So far, it is under active development.\n" +
                                    "There's still not much here, but I'm working on it.\n" +
                                    "This mod uses BepInEx injector,\n" +
                                    "HarmonyX library for hooking,\n" +
                                    "and LiteNetLib networking library.\n\n" +
                                    "Keybindings:\n" +
                                    "  F1 - Client manager.\n" +
                                    "  F2 - Server manager.\n" +
                                    "  F3 - About Window (This window).\n" +
                                    "You can open or close windows by pressing keys.\n" +
                                    "You also can close windows by pressing \"X\" button.\n" +
                                    "To move any window, just press \"M\" button in the top-left\n" +
                                    "corner of window, move it somewhere, and click again.\n\n\n" +
                                    "Mod authors:\n" +
                                    "  Lead developer: RidgyTheFox\n" +
                                    "  QA/Localization: Resident007\n" +
                                    "    -Enjoy! ^_^";

        private void Awake()
        {
            _captionTextStyle = new GUIStyle();
            _captionTextStyle.alignment = TextAnchor.UpperCenter;
            _captionTextStyle.richText = true;
            _captionTextStyle.normal.textColor = Color.white;
            _captionTextStyle.fontSize = 16;
            _captionTextStyle.fontStyle = FontStyle.Bold;

            _textStyle = new GUIStyle();
            _captionTextStyle.alignment = TextAnchor.UpperCenter;
            _textStyle.normal.textColor = Color.white;
            _textStyle.richText = true;

            _windowPositionX = (Screen.currentResolution.width / 2) - (_windowSizeX / 2);
            _windowPositionY = (Screen.currentResolution.height / 2) - (_windowSizeY / 2);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F3))
                _showWindow = !_showWindow;
        }

        private void FixedUpdate()
        {
            if (_moveMode)
            {
                _windowPositionX = (int)Input.mousePosition.x - 10;
                // Cursor coordinates are taken from the bottom left corner.
                // And the coordinates of the windows come from the top left corner.
                // Therefore, you need to invert the coordinates along the Y axis.
                _windowPositionY = Screen.currentResolution.height - (int)Input.mousePosition.y - 10;
            }
        }

        private void OnGUI()
        {
            if (_showWindow)
            {
                GUI.Box(new Rect(_windowPositionX, _windowPositionY, _windowSizeX, _windowSizeY), "Bus Driver Simulator Mod | About Window");
                if (GUI.Button(new Rect(_windowPositionX+1, _windowPositionY+1, 23, 21), "M"))
                    _moveMode = !_moveMode;
                if (GUI.Button(new Rect(_windowPositionX + _windowSizeX - 24, _windowPositionY + 1, 23, 21), "X"))
                    _showWindow = !_showWindow;

                GUI.Label(new Rect(_windowPositionX + 5, _windowPositionY + 26, _windowSizeX - 10, 20), "Bus Driver Simulator Multiplayer", _captionTextStyle);
                GUI.Label(new Rect(_windowPositionX + 20, _windowPositionY + 47, _windowSizeX - 40, 60), _aboutText, _textStyle);
            }
        }
    }
}
