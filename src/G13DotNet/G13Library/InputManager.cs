using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;

namespace G13Library
{
    public class InputManager
    {
        //[DllImport("user32.dll", SetLastError = true)]
        //private static extern uint SendInput(uint numberInputs, [MarshalAs(UnmanagedType.LPArray)]InputWrapper[] inputs, int sizeOfStructure);

        #region pinvoke

        [StructLayout(LayoutKind.Sequential)]
        public struct Win32Point
        {
            public Int32 X;
            public Int32 Y;

            public override string ToString()
            {
                return "Win32Point: " + X + ", " + Y;
            }
        };

        #region GetSytemMetrics
        // http://msdn.microsoft.com/en-us/library/ms724385(v=vs.85).aspx
        // http://www.pinvoke.net/default.aspx/user32.getsystemmetrics
        // http://pinvoke.net/default.aspx/Enums.SystemMetric
        [DllImport("user32.dll")]
        static extern int GetSystemMetrics(SystemMetric smIndex);

        /// <summary>
        /// Flags used with the Windows API (User32.dll):GetSystemMetrics(SystemMetric smIndex)
        ///   
        /// This Enum and declaration signature was written by Gabriel T. Sharp
        /// ai_productions@verizon.net or osirisgothra@hotmail.com
        /// Obtained on pinvoke.net, please contribute your code to support the wiki!
        /// </summary>
        public enum SystemMetric : int
        {
            /// <summary>
            ///  Width of the screen of the primary display monitor, in pixels. This is the same values obtained by calling GetDeviceCaps as follows: GetDeviceCaps( hdcPrimaryMonitor, HORZRES).
            /// </summary>
            SM_CXSCREEN = 0,
            /// <summary>
            /// Height of the screen of the primary display monitor, in pixels. This is the same values obtained by calling GetDeviceCaps as follows: GetDeviceCaps( hdcPrimaryMonitor, VERTRES).
            /// </summary>
            SM_CYSCREEN = 1,
            /// <summary>
            /// Height of the arrow bitmap on a vertical scroll bar, in pixels.
            /// </summary>
            SM_CYVSCROLL = 20,
            /// <summary>
            /// Width of a vertical scroll bar, in pixels.
            /// </summary>
            SM_CXVSCROLL = 2,
            /// <summary>
            /// Height of a caption area, in pixels.
            /// </summary>
            SM_CYCAPTION = 4,
            /// <summary>
            /// Width of a window border, in pixels. This is equivalent to the SM_CXEDGE value for windows with the 3-D look. 
            /// </summary>
            SM_CXBORDER = 5,
            /// <summary>
            /// Height of a window border, in pixels. This is equivalent to the SM_CYEDGE value for windows with the 3-D look. 
            /// </summary>
            SM_CYBORDER = 6,
            /// <summary>
            /// Thickness of the frame around the perimeter of a window that has a caption but is not sizable, in pixels. SM_CXFIXEDFRAME is the height of the horizontal border and SM_CYFIXEDFRAME is the width of the vertical border. 
            /// </summary>
            SM_CXDLGFRAME = 7,
            /// <summary>
            /// Thickness of the frame around the perimeter of a window that has a caption but is not sizable, in pixels. SM_CXFIXEDFRAME is the height of the horizontal border and SM_CYFIXEDFRAME is the width of the vertical border. 
            /// </summary>
            SM_CYDLGFRAME = 8,
            /// <summary>
            /// Height of the thumb box in a vertical scroll bar, in pixels
            /// </summary>
            SM_CYVTHUMB = 9,
            /// <summary>
            /// Width of the thumb box in a horizontal scroll bar, in pixels.
            /// </summary>
            SM_CXHTHUMB = 10,
            /// <summary>
            /// Default width of an icon, in pixels. The LoadIcon function can load only icons with the dimensions specified by SM_CXICON and SM_CYICON
            /// </summary>
            SM_CXICON = 11,
            /// <summary>
            /// Default height of an icon, in pixels. The LoadIcon function can load only icons with the dimensions SM_CXICON and SM_CYICON.
            /// </summary>
            SM_CYICON = 12,
            /// <summary>
            /// Width of a cursor, in pixels. The system cannot create cursors of other sizes.
            /// </summary>
            SM_CXCURSOR = 13,
            /// <summary>
            /// Height of a cursor, in pixels. The system cannot create cursors of other sizes.
            /// </summary>
            SM_CYCURSOR = 14,
            /// <summary>
            /// Height of a single-line menu bar, in pixels.
            /// </summary>
            SM_CYMENU = 15,
            /// <summary>
            /// Width of the client area for a full-screen window on the primary display monitor, in pixels. To get the coordinates of the portion of the screen not obscured by the system taskbar or by application desktop toolbars, call the SystemParametersInfo function with the SPI_GETWORKAREA value.
            /// </summary>
            SM_CXFULLSCREEN = 16,
            /// <summary>
            /// Height of the client area for a full-screen window on the primary display monitor, in pixels. To get the coordinates of the portion of the screen not obscured by the system taskbar or by application desktop toolbars, call the SystemParametersInfo function with the SPI_GETWORKAREA value.
            /// </summary>
            SM_CYFULLSCREEN = 17,
            /// <summary>
            /// For double byte character set versions of the system, this is the height of the Kanji window at the bottom of the screen, in pixels
            /// </summary>
            SM_CYKANJIWINDOW = 18,
            /// <summary>
            /// Nonzero if a mouse with a wheel is installed; zero otherwise
            /// </summary>
            SM_MOUSEWHEELPRESENT = 75,
            /// <summary>
            /// Height of a horizontal scroll bar, in pixels.
            /// </summary>
            SM_CYHSCROLL = 3,
            /// <summary>
            /// Width of the arrow bitmap on a horizontal scroll bar, in pixels.
            /// </summary>
            SM_CXHSCROLL = 21,
            /// <summary>
            /// Nonzero if the debug version of User.exe is installed; zero otherwise.
            /// </summary>
            SM_DEBUG = 22,
            /// <summary>
            /// Nonzero if the left and right mouse buttons are reversed; zero otherwise.
            /// </summary>
            SM_SWAPBUTTON = 23,
            /// <summary>
            /// Reserved for future use
            /// </summary>
            SM_RESERVED1 = 24,
            /// <summary>
            /// Reserved for future use
            /// </summary>
            SM_RESERVED2 = 25,
            /// <summary>
            /// Reserved for future use
            /// </summary>
            SM_RESERVED3 = 26,
            /// <summary>
            /// Reserved for future use
            /// </summary>
            SM_RESERVED4 = 27,
            /// <summary>
            /// Minimum width of a window, in pixels.
            /// </summary>
            SM_CXMIN = 28,
            /// <summary>
            /// Minimum height of a window, in pixels.
            /// </summary>
            SM_CYMIN = 29,
            /// <summary>
            /// Width of a button in a window's caption or title bar, in pixels.
            /// </summary>
            SM_CXSIZE = 30,
            /// <summary>
            /// Height of a button in a window's caption or title bar, in pixels.
            /// </summary>
            SM_CYSIZE = 31,
            /// <summary>
            /// Thickness of the sizing border around the perimeter of a window that can be resized, in pixels. SM_CXSIZEFRAME is the width of the horizontal border, and SM_CYSIZEFRAME is the height of the vertical border. 
            /// </summary>
            SM_CXFRAME = 32,
            /// <summary>
            /// Thickness of the sizing border around the perimeter of a window that can be resized, in pixels. SM_CXSIZEFRAME is the width of the horizontal border, and SM_CYSIZEFRAME is the height of the vertical border. 
            /// </summary>
            SM_CYFRAME = 33,
            /// <summary>
            /// Minimum tracking width of a window, in pixels. The user cannot drag the window frame to a size smaller than these dimensions. A window can override this value by processing the WM_GETMINMAXINFO message.
            /// </summary>
            SM_CXMINTRACK = 34,
            /// <summary>
            /// Minimum tracking height of a window, in pixels. The user cannot drag the window frame to a size smaller than these dimensions. A window can override this value by processing the WM_GETMINMAXINFO message
            /// </summary>
            SM_CYMINTRACK = 35,
            /// <summary>
            /// Width of the rectangle around the location of a first click in a double-click sequence, in pixels. The second click must occur within the rectangle defined by SM_CXDOUBLECLK and SM_CYDOUBLECLK for the system to consider the two clicks a double-click
            /// </summary>
            SM_CXDOUBLECLK = 36,
            /// <summary>
            /// Height of the rectangle around the location of a first click in a double-click sequence, in pixels. The second click must occur within the rectangle defined by SM_CXDOUBLECLK and SM_CYDOUBLECLK for the system to consider the two clicks a double-click. (The two clicks must also occur within a specified time.) 
            /// </summary>
            SM_CYDOUBLECLK = 37,
            /// <summary>
            /// Width of a grid cell for items in large icon view, in pixels. Each item fits into a rectangle of size SM_CXICONSPACING by SM_CYICONSPACING when arranged. This value is always greater than or equal to SM_CXICON
            /// </summary>
            SM_CXICONSPACING = 38,
            /// <summary>
            /// Height of a grid cell for items in large icon view, in pixels. Each item fits into a rectangle of size SM_CXICONSPACING by SM_CYICONSPACING when arranged. This value is always greater than or equal to SM_CYICON.
            /// </summary>
            SM_CYICONSPACING = 39,
            /// <summary>
            /// Nonzero if drop-down menus are right-aligned with the corresponding menu-bar item; zero if the menus are left-aligned.
            /// </summary>
            SM_MENUDROPALIGNMENT = 40,
            /// <summary>
            /// Nonzero if the Microsoft Windows for Pen computing extensions are installed; zero otherwise.
            /// </summary>
            SM_PENWINDOWS = 41,
            /// <summary>
            /// Nonzero if User32.dll supports DBCS; zero otherwise. (WinMe/95/98): Unicode
            /// </summary>
            SM_DBCSENABLED = 42,
            /// <summary>
            /// Number of buttons on mouse, or zero if no mouse is installed.
            /// </summary>
            SM_CMOUSEBUTTONS = 43,
            /// <summary>
            /// Identical Values Changed After Windows NT 4.0  
            /// </summary>
            SM_CXFIXEDFRAME = SM_CXDLGFRAME,
            /// <summary>
            /// Identical Values Changed After Windows NT 4.0
            /// </summary>
            SM_CYFIXEDFRAME = SM_CYDLGFRAME,
            /// <summary>
            /// Identical Values Changed After Windows NT 4.0
            /// </summary>
            SM_CXSIZEFRAME = SM_CXFRAME,
            /// <summary>
            /// Identical Values Changed After Windows NT 4.0
            /// </summary>
            SM_CYSIZEFRAME = SM_CYFRAME,
            /// <summary>
            /// Nonzero if security is present; zero otherwise.
            /// </summary>
            SM_SECURE = 44,
            /// <summary>
            /// Width of a 3-D border, in pixels. This is the 3-D counterpart of SM_CXBORDER
            /// </summary>
            SM_CXEDGE = 45,
            /// <summary>
            /// Height of a 3-D border, in pixels. This is the 3-D counterpart of SM_CYBORDER
            /// </summary>
            SM_CYEDGE = 46,
            /// <summary>
            /// Width of a grid cell for a minimized window, in pixels. Each minimized window fits into a rectangle this size when arranged. This value is always greater than or equal to SM_CXMINIMIZED.
            /// </summary>
            SM_CXMINSPACING = 47,
            /// <summary>
            /// Height of a grid cell for a minimized window, in pixels. Each minimized window fits into a rectangle this size when arranged. This value is always greater than or equal to SM_CYMINIMIZED.
            /// </summary>
            SM_CYMINSPACING = 48,
            /// <summary>
            /// Recommended width of a small icon, in pixels. Small icons typically appear in window captions and in small icon view
            /// </summary>
            SM_CXSMICON = 49,
            /// <summary>
            /// Recommended height of a small icon, in pixels. Small icons typically appear in window captions and in small icon view.
            /// </summary>
            SM_CYSMICON = 50,
            /// <summary>
            /// Height of a small caption, in pixels
            /// </summary>
            SM_CYSMCAPTION = 51,
            /// <summary>
            /// Width of small caption buttons, in pixels.
            /// </summary>
            SM_CXSMSIZE = 52,
            /// <summary>
            /// Height of small caption buttons, in pixels.
            /// </summary>
            SM_CYSMSIZE = 53,
            /// <summary>
            /// Width of menu bar buttons, such as the child window close button used in the multiple document interface, in pixels.
            /// </summary>
            SM_CXMENUSIZE = 54,
            /// <summary>
            /// Height of menu bar buttons, such as the child window close button used in the multiple document interface, in pixels.
            /// </summary>
            SM_CYMENUSIZE = 55,
            /// <summary>
            /// Flags specifying how the system arranged minimized windows
            /// </summary>
            SM_ARRANGE = 56,
            /// <summary>
            /// Width of a minimized window, in pixels.
            /// </summary>
            SM_CXMINIMIZED = 57,
            /// <summary>
            /// Height of a minimized window, in pixels.
            /// </summary>
            SM_CYMINIMIZED = 58,
            /// <summary>
            /// Default maximum width of a window that has a caption and sizing borders, in pixels. This metric refers to the entire desktop. The user cannot drag the window frame to a size larger than these dimensions. A window can override this value by processing the WM_GETMINMAXINFO message.
            /// </summary>
            SM_CXMAXTRACK = 59,
            /// <summary>
            /// Default maximum height of a window that has a caption and sizing borders, in pixels. This metric refers to the entire desktop. The user cannot drag the window frame to a size larger than these dimensions. A window can override this value by processing the WM_GETMINMAXINFO message.
            /// </summary>
            SM_CYMAXTRACK = 60,
            /// <summary>
            /// Default width, in pixels, of a maximized top-level window on the primary display monitor.
            /// </summary>
            SM_CXMAXIMIZED = 61,
            /// <summary>
            /// Default height, in pixels, of a maximized top-level window on the primary display monitor.
            /// </summary>
            SM_CYMAXIMIZED = 62,
            /// <summary>
            /// Least significant bit is set if a network is present; otherwise, it is cleared. The other bits are reserved for future use
            /// </summary>
            SM_NETWORK = 63,
            /// <summary>
            /// Value that specifies how the system was started: 0-normal, 1-failsafe, 2-failsafe /w net
            /// </summary>
            SM_CLEANBOOT = 67,
            /// <summary>
            /// Width of a rectangle centered on a drag point to allow for limited movement of the mouse pointer before a drag operation begins, in pixels. 
            /// </summary>
            SM_CXDRAG = 68,
            /// <summary>
            /// Height of a rectangle centered on a drag point to allow for limited movement of the mouse pointer before a drag operation begins. This value is in pixels. It allows the user to click and release the mouse button easily without unintentionally starting a drag operation.
            /// </summary>
            SM_CYDRAG = 69,
            /// <summary>
            /// Nonzero if the user requires an application to present information visually in situations where it would otherwise present the information only in audible form; zero otherwise. 
            /// </summary>
            SM_SHOWSOUNDS = 70,
            /// <summary>
            /// Width of the default menu check-mark bitmap, in pixels.
            /// </summary>
            SM_CXMENUCHECK = 71,
            /// <summary>
            /// Height of the default menu check-mark bitmap, in pixels.
            /// </summary>
            SM_CYMENUCHECK = 72,
            /// <summary>
            /// Nonzero if the computer has a low-end (slow) processor; zero otherwise
            /// </summary>
            SM_SLOWMACHINE = 73,
            /// <summary>
            /// Nonzero if the system is enabled for Hebrew and Arabic languages, zero if not.
            /// </summary>
            SM_MIDEASTENABLED = 74,
            /// <summary>
            /// Nonzero if a mouse is installed; zero otherwise. This value is rarely zero, because of support for virtual mice and because some systems detect the presence of the port instead of the presence of a mouse.
            /// </summary>
            SM_MOUSEPRESENT = 19,
            /// <summary>
            /// Windows 2000 (v5.0+) Coordinate of the top of the virtual screen
            /// </summary>
            SM_XVIRTUALSCREEN = 76,
            /// <summary>
            /// Windows 2000 (v5.0+) Coordinate of the left of the virtual screen
            /// </summary>
            SM_YVIRTUALSCREEN = 77,
            /// <summary>
            /// Windows 2000 (v5.0+) Width of the virtual screen
            /// </summary>
            SM_CXVIRTUALSCREEN = 78,
            /// <summary>
            /// Windows 2000 (v5.0+) Height of the virtual screen
            /// </summary>
            SM_CYVIRTUALSCREEN = 79,
            /// <summary>
            /// Number of display monitors on the desktop
            /// </summary>
            SM_CMONITORS = 80,
            /// <summary>
            /// Windows XP (v5.1+) Nonzero if all the display monitors have the same color format, zero otherwise. Note that two displays can have the same bit depth, but different color formats. For example, the red, green, and blue pixels can be encoded with different numbers of bits, or those bits can be located in different places in a pixel's color value. 
            /// </summary>
            SM_SAMEDISPLAYFORMAT = 81,
            /// <summary>
            /// Windows XP (v5.1+) Nonzero if Input Method Manager/Input Method Editor features are enabled; zero otherwise
            /// </summary>
            SM_IMMENABLED = 82,
            /// <summary>
            /// Windows XP (v5.1+) Width of the left and right edges of the focus rectangle drawn by DrawFocusRect. This value is in pixels. 
            /// </summary>
            SM_CXFOCUSBORDER = 83,
            /// <summary>
            /// Windows XP (v5.1+) Height of the top and bottom edges of the focus rectangle drawn by DrawFocusRect. This value is in pixels. 
            /// </summary>
            SM_CYFOCUSBORDER = 84,
            /// <summary>
            /// Nonzero if the current operating system is the Windows XP Tablet PC edition, zero if not.
            /// </summary>
            SM_TABLETPC = 86,
            /// <summary>
            /// Nonzero if the current operating system is the Windows XP, Media Center Edition, zero if not.
            /// </summary>
            SM_MEDIACENTER = 87,
            /// <summary>
            /// Metrics Other
            /// </summary>
            SM_CMETRICS_OTHER = 76,
            /// <summary>
            /// Metrics Windows 2000
            /// </summary>
            SM_CMETRICS_2000 = 83,
            /// <summary>
            /// Metrics Windows NT
            /// </summary>
            SM_CMETRICS_NT = 88,
            /// <summary>
            /// Windows XP (v5.1+) This system metric is used in a Terminal Services environment. If the calling process is associated with a Terminal Services client session, the return value is nonzero. If the calling process is associated with the Terminal Server console session, the return value is zero. The console session is not necessarily the physical console - see WTSGetActiveConsoleSessionId for more information. 
            /// </summary>
            SM_REMOTESESSION = 0x1000,
            /// <summary>
            /// Windows XP (v5.1+) Nonzero if the current session is shutting down; zero otherwise
            /// </summary>
            SM_SHUTTINGDOWN = 0x2000,
            /// <summary>
            /// Windows XP (v5.1+) This system metric is used in a Terminal Services environment. Its value is nonzero if the current session is remotely controlled; zero otherwise
            /// </summary>
            SM_REMOTECONTROL = 0x2001,
        }

        #endregion

        [DllImport("user32.dll")]
        static extern ushort VkKeyScan(char ch);

        [DllImport("user32.dll")]
        static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(ref Win32Point pt);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint numberInputs, InputWrapper[] inputs, int sizeOfStructure);

        [DllImport("user32.dll")]
        static extern short GetKeyState(VirtualKeyStates nVirtKey);

        public static bool IsKeyPressed(VirtualKeyStates testKey)
        {
            bool keyPressed = false;
            short result = GetKeyState(testKey);

            switch (result)
            {
                case 0:
                    // Not pressed and not toggled on.
                    keyPressed = false;
                    break;

                case 1:
                    // Not pressed, but toggled on
                    keyPressed = false;
                    break;

                default:
                    // Pressed (and may be toggled on)
                    keyPressed = true;
                    break;
            }

            return keyPressed;
        }

        #region VirtualKeyStates
        public enum VirtualKeyStates : int
        {
            VK_LBUTTON = 0x01,
            VK_RBUTTON = 0x02,
            VK_CANCEL = 0x03,
            VK_MBUTTON = 0x04,
            //
            VK_XBUTTON1 = 0x05,
            VK_XBUTTON2 = 0x06,
            //
            VK_BACK = 0x08,
            VK_TAB = 0x09,
            //
            VK_CLEAR = 0x0C,
            VK_RETURN = 0x0D,
            //
            VK_SHIFT = 0x10,
            VK_CONTROL = 0x11,
            VK_MENU = 0x12,
            VK_PAUSE = 0x13,
            VK_CAPITAL = 0x14,
            //
            VK_KANA = 0x15,
            VK_HANGEUL = 0x15, /* old name - should be here for compatibility */
            VK_HANGUL = 0x15,
            VK_JUNJA = 0x17,
            VK_FINAL = 0x18,
            VK_HANJA = 0x19,
            VK_KANJI = 0x19,
            //
            VK_ESCAPE = 0x1B,
            //
            VK_CONVERT = 0x1C,
            VK_NONCONVERT = 0x1D,
            VK_ACCEPT = 0x1E,
            VK_MODECHANGE = 0x1F,
            //
            VK_SPACE = 0x20,
            VK_PRIOR = 0x21,
            VK_NEXT = 0x22,
            VK_END = 0x23,
            VK_HOME = 0x24,
            VK_LEFT = 0x25,
            VK_UP = 0x26,
            VK_RIGHT = 0x27,
            VK_DOWN = 0x28,
            VK_SELECT = 0x29,
            VK_PRINT = 0x2A,
            VK_EXECUTE = 0x2B,
            VK_SNAPSHOT = 0x2C,
            VK_INSERT = 0x2D,
            VK_DELETE = 0x2E,
            VK_HELP = 0x2F,
            //
            VK_LWIN = 0x5B,
            VK_RWIN = 0x5C,
            VK_APPS = 0x5D,
            //
            VK_SLEEP = 0x5F,
            //
            VK_NUMPAD0 = 0x60,
            VK_NUMPAD1 = 0x61,
            VK_NUMPAD2 = 0x62,
            VK_NUMPAD3 = 0x63,
            VK_NUMPAD4 = 0x64,
            VK_NUMPAD5 = 0x65,
            VK_NUMPAD6 = 0x66,
            VK_NUMPAD7 = 0x67,
            VK_NUMPAD8 = 0x68,
            VK_NUMPAD9 = 0x69,
            VK_MULTIPLY = 0x6A,
            VK_ADD = 0x6B,
            VK_SEPARATOR = 0x6C,
            VK_SUBTRACT = 0x6D,
            VK_DECIMAL = 0x6E,
            VK_DIVIDE = 0x6F,
            VK_F1 = 0x70,
            VK_F2 = 0x71,
            VK_F3 = 0x72,
            VK_F4 = 0x73,
            VK_F5 = 0x74,
            VK_F6 = 0x75,
            VK_F7 = 0x76,
            VK_F8 = 0x77,
            VK_F9 = 0x78,
            VK_F10 = 0x79,
            VK_F11 = 0x7A,
            VK_F12 = 0x7B,
            VK_F13 = 0x7C,
            VK_F14 = 0x7D,
            VK_F15 = 0x7E,
            VK_F16 = 0x7F,
            VK_F17 = 0x80,
            VK_F18 = 0x81,
            VK_F19 = 0x82,
            VK_F20 = 0x83,
            VK_F21 = 0x84,
            VK_F22 = 0x85,
            VK_F23 = 0x86,
            VK_F24 = 0x87,
            //
            VK_NUMLOCK = 0x90,
            VK_SCROLL = 0x91,
            //
            VK_OEM_NEC_EQUAL = 0x92, // '=' key on numpad
            //
            VK_OEM_FJ_JISHO = 0x92, // 'Dictionary' key
            VK_OEM_FJ_MASSHOU = 0x93, // 'Unregister word' key
            VK_OEM_FJ_TOUROKU = 0x94, // 'Register word' key
            VK_OEM_FJ_LOYA = 0x95, // 'Left OYAYUBI' key
            VK_OEM_FJ_ROYA = 0x96, // 'Right OYAYUBI' key
            //
            VK_LSHIFT = 0xA0,
            VK_RSHIFT = 0xA1,
            VK_LCONTROL = 0xA2,
            VK_RCONTROL = 0xA3,
            VK_LMENU = 0xA4,
            VK_RMENU = 0xA5,
            //
            VK_BROWSER_BACK = 0xA6,
            VK_BROWSER_FORWARD = 0xA7,
            VK_BROWSER_REFRESH = 0xA8,
            VK_BROWSER_STOP = 0xA9,
            VK_BROWSER_SEARCH = 0xAA,
            VK_BROWSER_FAVORITES = 0xAB,
            VK_BROWSER_HOME = 0xAC,
            //
            VK_VOLUME_MUTE = 0xAD,
            VK_VOLUME_DOWN = 0xAE,
            VK_VOLUME_UP = 0xAF,
            VK_MEDIA_NEXT_TRACK = 0xB0,
            VK_MEDIA_PREV_TRACK = 0xB1,
            VK_MEDIA_STOP = 0xB2,
            VK_MEDIA_PLAY_PAUSE = 0xB3,
            VK_LAUNCH_MAIL = 0xB4,
            VK_LAUNCH_MEDIA_SELECT = 0xB5,
            VK_LAUNCH_APP1 = 0xB6,
            VK_LAUNCH_APP2 = 0xB7,
            //
            VK_OEM_1 = 0xBA, // ';:' for US
            VK_OEM_PLUS = 0xBB, // '+' any country
            VK_OEM_COMMA = 0xBC, // ',' any country
            VK_OEM_MINUS = 0xBD, // '-' any country
            VK_OEM_PERIOD = 0xBE, // '.' any country
            VK_OEM_2 = 0xBF, // '/?' for US
            VK_OEM_3 = 0xC0, // '`~' for US
            //
            VK_OEM_4 = 0xDB, // '[{' for US
            VK_OEM_5 = 0xDC, // '\|' for US
            VK_OEM_6 = 0xDD, // ']}' for US
            VK_OEM_7 = 0xDE, // ''"' for US
            VK_OEM_8 = 0xDF,
            //
            VK_OEM_AX = 0xE1, // 'AX' key on Japanese AX kbd
            VK_OEM_102 = 0xE2, // "<>" or "\|" on RT 102-key kbd.
            VK_ICO_HELP = 0xE3, // Help key on ICO
            VK_ICO_00 = 0xE4, // 00 key on ICO
            //
            VK_PROCESSKEY = 0xE5,
            //
            VK_ICO_CLEAR = 0xE6,
            //
            VK_PACKET = 0xE7,
            //
            VK_OEM_RESET = 0xE9,
            VK_OEM_JUMP = 0xEA,
            VK_OEM_PA1 = 0xEB,
            VK_OEM_PA2 = 0xEC,
            VK_OEM_PA3 = 0xED,
            VK_OEM_WSCTRL = 0xEE,
            VK_OEM_CUSEL = 0xEF,
            VK_OEM_ATTN = 0xF0,
            VK_OEM_FINISH = 0xF1,
            VK_OEM_COPY = 0xF2,
            VK_OEM_AUTO = 0xF3,
            VK_OEM_ENLW = 0xF4,
            VK_OEM_BACKTAB = 0xF5,
            //
            VK_ATTN = 0xF6,
            VK_CRSEL = 0xF7,
            VK_EXSEL = 0xF8,
            VK_EREOF = 0xF9,
            VK_PLAY = 0xFA,
            VK_ZOOM = 0xFB,
            VK_NONAME = 0xFC,
            VK_PA1 = 0xFD,
            VK_OEM_CLEAR = 0xFE
        }

        #endregion

        [StructLayout(LayoutKind.Sequential)]
        internal struct InputWrapper
        {
            public SendInputType Type;
            public MouseKeyboardHardwareUnion MKH;
        }

        internal enum SendInputType : int
        {
            Mouse = 0,
            Keyboard,
            Hardware
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct MouseKeyboardHardwareUnion
        {
            [FieldOffset(0)]
            public MouseInputData Mouse;
            [FieldOffset(0)]
            public KeyboardInputData Keyboard;
            [FieldOffset(0)]
            public HardwareInputData Hardware;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MouseInputData
        {
            public int X;
            public int Y;
            public int MouseData;
            public MouseEventFlags Flags;
            public uint Time;
            public IntPtr ExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct KeyboardInputData
        {
            public ushort Key;
            public ScanCode Scan;
            public KeyboardFlags Flags;
            public uint Time;
            public IntPtr ExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HardwareInputData
        {
            public int Msg;
            public short ParamL;
            public short ParamH;
        }

        [Flags]
        internal enum MouseDataFlags : uint
        {
            XButton1 = 0x0001,
            XButton2 = 0x0002
        }

        [Flags]
        internal enum KeyboardFlags : uint
        {
            ExtendedKey = 0x1,
            KeyUp = 0x2,
            Unicode = 0x4,
            ScanCode = 0x8
        }

        [Flags]
        internal enum MouseEventFlags : uint
        {
            Move = 0x0001,
            LeftDown = 0x0002,
            LeftUp = 0x0004,
            RightDown = 0x0008,
            RightUp = 0x0010,
            MiddleDown = 0x0020,
            MiddleUp = 0x0040,
            XDown = 0x0080,
            XUp = 0x0100,
            Wheel = 0x0800,
            VirtualDesktop = 0x4000,
            Absolute = 0x8000
        }

        [Flags]
        public enum MouseDownFlags : uint
        {
            LeftDown = 0x0002,
            RightDown = 0x0008,
            MiddleDown = 0x0020,
            XDown = 0x0080
        }

        [Flags]
        public enum MouseUpFlags : uint
        {
            LeftUp = 0x0004,
            RightUp = 0x0010,
            MiddleUp = 0x0040,
            XUp = 0x0100
        }

        [Flags]
        public enum MouseTapFlags : uint
        {
            LeftTap = 0x0002,
            RightTap = 0x0008,
            MiddleTap = 0x0020,
            XTap = 0x0080
        }

        public enum ScanCode : ushort
        {
            Escape = 0x01,

            k1 = 0x02,
            k2 = 0x03,
            k3 = 0x04,
            k4 = 0x05,
            k5 = 0x06,
            k6 = 0x07,
            k7 = 0x08,
            k8 = 0x09,
            k9 = 0x0a,
            k0 = 0x0b,

            minus = 0x0c,
            equals = 0x0d,
            backspace = 0x0e,
            tab = 0x0f,

            q = 0x10,
            w = 0x11,
            e = 0x12,
            r = 0x13,
            t = 0x14,
            y = 0x15,
            u = 0x16,
            i = 0x17,
            o = 0x18,
            p = 0x19,
            lbracket = 0x1a,
            rbracket = 0x1b,
            enter = 0x1c,
            lcontrol = 0x1d,
            a = 0x1e,
            s = 0x1f,
            d = 0x20,
            f = 0x21,
            g = 0x22,
            h = 0x23,
            j = 0x24,
            k = 0x25,
            l = 0x26,
            semicolon = 0x27,
            apostrophe = 0x28,
            accentGrave = 0x29,
            lshift = 0x2a,
            backslash = 0x2b,
            z = 0x2c,
            x = 0x2d,
            c = 0x2e,
            v = 0x2f,
            b = 0x30,
            n = 0x31,
            m = 0x32,
            comma = 0x33,
            period = 0x34,
            forwardSlash = 0x35,
            rshift = 0x36,
            asterisk = 0x37,
            lalt = 0x38,
            space = 0x39,
            capital = 0x3a,
            f1 = 0x3b,
            f2 = 0x3c,
            f3 = 0x3d,
            f4 = 0x3e,
            f5 = 0x3f,
            f6 = 0x40,
            f7 = 0x41,
            f8 = 0x42,
            f9 = 0x43,
            f10 = 0x44,
            numlock = 0x45,
            scrollLock = 0x46,
            numpad7 = 0x47,
            numpad8 = 0x48,
            numpad9 = 0x49,
            numpadSubtract = 0x4a,
            numpad4 = 0x4b,
            numpad5 = 0x4c,
            numpad6 = 0x4d,
            numpadAdd = 0x4e,
            numpad1 = 0x4f,
            numpad2 = 0x50,
            numpad3 = 0x51,
            numpad0 = 0x52,
            numpadPeriod = 0x53,
            f11 = 0x57,
            f12 = 0x58,
        }

        //http://www.quadibloc.com/comp/scan.htm
        public enum ScanCodeExtended : ushort
        {
            rcontrol = 0x1d,
            ralt = 0x38,
            home = 0x47,
            up = 0x48,
            pgup = 0x49,
            left = 0x4b,
            right = 0x4d,
            end = 0x4f,
            down = 0x50,
            pgdn = 0x51,
            ins = 0x52,
            del = 0x53,
            mute = 0x20,
            volup = 0x30,
            voldown = 0x2e
        }

        

        #endregion

        static int InputWrapperSize;

        static BackgroundWorker keyRepeater;
        //static Queue<RepeatingKey> repeatingKeyQueue;
        static RepeatingKey rkey;

        struct RepeatingKey
        {
            public ScanCode Code;
            public bool Extended;
            public int InitialPauseTime;
            public int RepeatPauseTime;
            public bool IsRunning;
            public bool IsEmpty;

            public void Clear()
            {
                IsRunning = false;
                IsEmpty = true;
            }

            public static RepeatingKey Empty = new RepeatingKey() { IsEmpty = true };
        }

        static InputManager()
        {
            InputWrapperSize = Marshal.SizeOf(typeof(InputWrapper));

            keyRepeater = new BackgroundWorker() { WorkerSupportsCancellation = true };
            keyRepeater.DoWork += new DoWorkEventHandler(keyRepeater_DoWork);
            keyRepeater.RunWorkerCompleted += new RunWorkerCompletedEventHandler(keyRepeater_RunWorkerCompleted);

            //repeatingKeyQueue = new Queue<RepeatingKey>();
            rkey = RepeatingKey.Empty;
        }

        static void keyRepeater_DoWork(object sender, DoWorkEventArgs e)
        {
            if (rkey.IsEmpty)
                return;

            while (!rkey.IsEmpty)
            {
                rkey.IsRunning = true;
                var code = rkey.Code;
                var ext = rkey.Extended;
                var initialPauseTime = rkey.InitialPauseTime;
                var repeatPauseTime = rkey.RepeatPauseTime;

                Thread.Sleep(initialPauseTime);
                while (rkey.IsRunning)
                {
                    DoKeyUp(code, ext);
                    if (rkey.IsRunning)
                        Thread.Sleep(repeatPauseTime);
                    if (rkey.IsRunning)
                        DoKeyDown(code, ext);
                }
            }

            //RepeatingKey args;

            //if (e.Argument is RepeatingKey)
            //    args = (RepeatingKey)e.Argument;
            //else
            //    return;

            //DoKeyDown(args.Code, args.Extended);
            //Thread.Sleep(args.InitialPauseTime);
            //DoKeyUp(args.Code, args.Extended);

            //while (!keyRepeater.CancellationPending)
            //{
            //    DoKeyDown(args.Code, args.Extended);
            //    Thread.Sleep(args.RepeatPauseTime);
            //    DoKeyUp(args.Code, args.Extended);
            //}
        }

        static void keyRepeater_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (rkey.IsEmpty == false)
                keyRepeater.RunWorkerAsync();

            rkey = RepeatingKey.Empty;
        }

        static void RepeatKey(RepeatingKey keyInfo)
        {
            rkey = keyInfo;
            rkey.IsRunning = false;
            rkey.IsEmpty = false;

            if (!keyRepeater.IsBusy)
                keyRepeater.RunWorkerAsync();
        }

        static InputWrapper KeyWrapper(char c, bool keyUp, bool extended)
        {
            var vKCode = VkKeyScan(c);
            var scanCode = (ScanCode)MapVirtualKey((uint)(vKCode & 0x00ff), 0);

            return new InputWrapper
            {
                Type = SendInputType.Keyboard,
                MKH = new MouseKeyboardHardwareUnion
                {
                    Keyboard = new KeyboardInputData
                    {
                        Key = vKCode,
                        Scan = scanCode,
                        Flags =
                            (keyUp ? KeyboardFlags.KeyUp : 0) |
                            (extended ? KeyboardFlags.ExtendedKey : 0)
                    }
                }
            };
        }

        static InputWrapper KeyWrapper(ScanCode scanCode, bool keyUp, bool extended)
        {
            return new InputWrapper
            {
                Type = SendInputType.Keyboard,
                MKH = new MouseKeyboardHardwareUnion
                {
                    Keyboard = new KeyboardInputData
                    {
                        Scan = scanCode,
                        Flags =
                            KeyboardFlags.ScanCode |
                            (keyUp ? KeyboardFlags.KeyUp : 0) |
                            (extended ? KeyboardFlags.ExtendedKey : 0)
                    }
                }
            };
        }

        static InputWrapper KeyWrapper(KeyboardInputData data)
        {
            return new InputWrapper
            {
                Type = SendInputType.Keyboard,
                MKH = new MouseKeyboardHardwareUnion
                {
                    Keyboard = data
                }
            };
        }

        static InputWrapper MouseWrapper(MouseEventFlags flags, int data)
        {
            return new InputWrapper
            {
                Type = SendInputType.Mouse,
                MKH = new MouseKeyboardHardwareUnion
                {
                    Mouse = new MouseInputData
                    {
                        Flags = flags,
                        MouseData = data
                    }
                }
            };
        }

        static uint DoKeyDown(ScanCode scanCode, bool extended)
        {
            var inputData = new InputWrapper[]
            {
                KeyWrapper(scanCode, false, extended)
            };

            return SendInput(1, inputData, Marshal.SizeOf(typeof(InputWrapper)));
        }

        static uint DoKeyUp(ScanCode scanCode, bool extended)
        {
            var inputData = new InputWrapper[]
            {
                KeyWrapper(scanCode, true, extended)
            };

            return SendInput(1, inputData, Marshal.SizeOf(typeof(InputWrapper)));
        }

        static uint DoKeyDown(char c, bool extended)
        {
            var inputData = new InputWrapper[]
            {
                KeyWrapper(c, false, extended)
            };

            return SendInput(1, inputData, Marshal.SizeOf(typeof(InputWrapper)));
        }

        static uint DoKeyUp(char c, bool extended)
        {
            var inputData = new InputWrapper[]
            {
                KeyWrapper(c, true, extended)
            };

            return SendInput(1, inputData, Marshal.SizeOf(typeof(InputWrapper)));
        }

        public static uint KeyDown(ScanCode scanCode, bool extended = false, bool repeat = false, int initialPauseTime = 500, int repeatPauseTime = 30)
        {
            uint retval;
            if (repeat)
            {
                retval = DoKeyDown(scanCode, extended);
                RepeatKey(new RepeatingKey { Code = scanCode, Extended = extended, InitialPauseTime = initialPauseTime, RepeatPauseTime = repeatPauseTime });
            }
            else
                retval = DoKeyDown(scanCode, extended);

            return retval;
        }


        public static uint KeyUp(ScanCode scanCode, bool extended = false)
        {
            uint retval;
            if (scanCode == rkey.Code && extended == rkey.Extended)
            {
                retval = DoKeyUp(scanCode, extended);
                rkey.Clear();
            }
            else
                retval = DoKeyUp(scanCode, extended);

            return retval;
        }

        public static uint KeyTap(ScanCode scanCode, bool extended = false)
        {
            var inputData = new InputWrapper[]
            { 
                KeyWrapper(scanCode, false, extended),
                KeyWrapper(scanCode, true, extended)
            };

            return SendInput(2, inputData, Marshal.SizeOf(typeof(InputWrapper)));
        }

        public static uint KeyDown(char c, bool extended = false)
        {
            return DoKeyDown(c, extended);
        }


        public static uint KeyUp(char c, bool extended = false)
        {
            return DoKeyUp(c, extended);
        }

        public static uint KeyTap(char c, bool extended = false)
        {
            var inputData = new InputWrapper[]
            { 
                KeyWrapper(c, false, extended),
                KeyWrapper(c, true, extended)
            };

            return SendInput(2, inputData, Marshal.SizeOf(typeof(InputWrapper)));
        }

        public static uint MouseDown(MouseDownFlags flags, int xButton = 0)
        {
            var inputData = new InputWrapper[]
            {
                MouseWrapper((MouseEventFlags)flags, xButton)
            };

            return SendInput(1, inputData, Marshal.SizeOf(typeof(InputWrapper)));
        }

        public static uint MouseUp(MouseUpFlags flags, int xButton = 0)
        {
            var inputData = new InputWrapper[]
            {
                MouseWrapper((MouseEventFlags)flags, xButton)
            };

            return SendInput(1, inputData, Marshal.SizeOf(typeof(InputWrapper)));
        }

        public static uint MouseTap(MouseTapFlags flags, int xButton = 0)
        {
            var inputData = new InputWrapper[]
            {
                MouseWrapper((MouseEventFlags)flags, xButton),
                MouseWrapper((MouseEventFlags)((int)flags << 1), xButton)
            };

            return SendInput(2, inputData, Marshal.SizeOf(typeof(InputWrapper)));
        }

        /// <summary>
        /// Simulate a mouse wheel event
        /// </summary>
        /// <param name="amount">The amount of wheel movement (120 == one click)</param>
        /// <returns>1 if successful, 0 if SendInput failed</returns>
        public static uint MouseWheel(int amount)
        {
            var inputData = new InputWrapper[]
            {
                MouseWrapper(MouseEventFlags.Wheel, amount)
            };

            return SendInput(1, inputData, Marshal.SizeOf(typeof(InputWrapper)));
        }

        public static uint MouseMoveTo(int x, int y, bool virtualDesk = false)
        {
            var inputData = new InputWrapper[]
            {
                new InputWrapper
                {
                    Type = SendInputType.Mouse,
                    MKH = new MouseKeyboardHardwareUnion
                    {
                        Mouse = new MouseInputData
                        {
                            X = x,
                            Y = y,
                            Flags = MouseEventFlags.Move | MouseEventFlags.Absolute | (virtualDesk ? MouseEventFlags.VirtualDesktop : 0)
                        }
                    }
                }
            };

            return SendInput(1, inputData, Marshal.SizeOf(typeof(InputWrapper)));
        }

        public static uint MouseMoveTo(Win32Point p, bool virtualDesk = false)
        {
            return MouseMoveTo(p.X, p.Y, virtualDesk);
        }

        public static uint MouseMoveToPixel(int x, int y)
        {
            var size = ScreenSize;

            var inputData = new InputWrapper[]
            {
                new InputWrapper
                {
                    Type = SendInputType.Mouse,
                    MKH = new MouseKeyboardHardwareUnion
                    {
                        Mouse = new MouseInputData
                        {
                            X = x * 65536 / size.X + 1,
                            Y = y * 65536 / size.Y + 1,
                            Flags = MouseEventFlags.Move | MouseEventFlags.Absolute
                        }
                    }
                }
            };

            return SendInput(1, inputData, Marshal.SizeOf(typeof(InputWrapper)));
        }

        public static uint MouseMoveToPixel(Win32Point p)
        {
            return MouseMoveToPixel(p.X, p.Y);
        }

        public static uint MouseMoveBy(int x, int y)
        {
            var inputData = new InputWrapper[]
            {
                new InputWrapper
                {
                    Type = SendInputType.Mouse,
                    MKH = new MouseKeyboardHardwareUnion
                    {
                        Mouse = new MouseInputData
                        {
                            X = x,
                            Y = y,
                            Flags = MouseEventFlags.Move
                        }
                    }
                }
            };

            return SendInput(1, inputData, Marshal.SizeOf(typeof(InputWrapper)));
        }

        public static uint MouseMoveBy(Win32Point p)
        {
            return MouseMoveBy(p.X, p.Y);
        }

        public static Win32Point MouseScreenPos
        {
            get
            {
                Win32Point p = new Win32Point();
                if (!GetCursorPos(ref p))
                    System.Diagnostics.Debug.WriteLine("GetCursorPos returned false");
                return p;
            }
        }

        public static Win32Point ScreenSize
        {
            get
            {
                return new Win32Point
                {
                    X = GetSystemMetrics(SystemMetric.SM_CXSCREEN),
                    Y = GetSystemMetrics(SystemMetric.SM_CYSCREEN)
                };
            }
        }

        /// <summary>
        /// Gets the absolute position on the primary monitor
        /// </summary>
        public static Win32Point MouseAbsolutePos
        {
            get
            {
                var pos = MouseScreenPos;
                var scr = ScreenSize;

                return new Win32Point
                {
                    X = (int)Math.Ceiling(pos.X * 65536 / (double)scr.X),
                    Y = (int)Math.Ceiling(pos.Y * 65536 / (double)scr.Y)
                };
            }
        }

        public static uint Write(string text)
        {
            List<InputWrapper> input = new List<InputWrapper>();
            for (int i = 0; i < text.Length; i++)
            {
                var sci = AsciiToScanCode(text[i]);

                if (sci.IsShifted)
                    input.Add(KeyWrapper(new KeyboardInputData { Scan = ScanCode.lshift, Flags = KeyboardFlags.ScanCode }));
                input.Add(KeyWrapper(new KeyboardInputData { Key = sci.VkKey, Scan = sci.ScanCode }));
                input.Add(KeyWrapper(new KeyboardInputData { Key = sci.VkKey, Scan = sci.ScanCode, Flags = KeyboardFlags.KeyUp }));
                if (sci.IsShifted)
                    input.Add(KeyWrapper(new KeyboardInputData { Scan = ScanCode.lshift, Flags = KeyboardFlags.ScanCode | KeyboardFlags.KeyUp }));
            }

            return SendInput((uint)input.Count, input.ToArray(), Marshal.SizeOf(typeof(InputWrapper)));
        }

        public struct ScanCodeInfo
        {
            public ushort VkKey;
            public ScanCode ScanCode;
            public bool IsShifted;

            public override string ToString()
            {
                var str = ScanCode.ToString("x");
                if (IsShifted)
                    str += " : shifted";

                return str;
            }
        }

        public static ScanCodeInfo AsciiToScanCode(char ascii)
        {
            var VKCode = VkKeyScan(ascii);

            return new ScanCodeInfo
            {
                VkKey = VKCode,
                ScanCode = (ScanCode)MapVirtualKey((uint)(VKCode & 0x00ff), 0),
                IsShifted = VKCode > 0x00ff
            };
        }
    }
}
