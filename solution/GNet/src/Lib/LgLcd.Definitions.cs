using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using DWORD = System.Int32;
using WCHAR = System.String;
using BYTE = System.Byte;
using PVOID = System.IntPtr;
using LPCWSTR = System.String;
using BOOL = System.Boolean;

namespace GNet.Lib
{
    public partial class LgLcd
    {
        public const int MAX_PATH = 260;

        public const int LGLCD_INVALID_CONNECTION = -1;
        public const int LGLCD_INVALID_DEVICE = -1;


        // Common Soft-Buttons available through the SDK
        public const uint LGLCDBUTTON_LEFT = 0x00000100;
        public const uint LGLCDBUTTON_RIGHT = 0x00000200;
        public const uint LGLCDBUTTON_OK = 0x00000400;
        public const uint LGLCDBUTTON_CANCEL = 0x00000800;
        public const uint LGLCDBUTTON_UP = 0x00001000;
        public const uint LGLCDBUTTON_DOWN = 0x00002000;
        public const uint LGLCDBUTTON_MENU = 0x00004000;

        // Soft-Button masks. Kept for backwards compatibility
        public const uint LGLCDBUTTON_BUTTON0 = 0x00000001;
        public const uint LGLCDBUTTON_BUTTON1 = 0x00000002;
        public const uint LGLCDBUTTON_BUTTON2 = 0x00000004;
        public const uint LGLCDBUTTON_BUTTON3 = 0x00000008;
        public const uint LGLCDBUTTON_BUTTON4 = 0x00000010;
        public const uint LGLCDBUTTON_BUTTON5 = 0x00000020;
        public const uint LGLCDBUTTON_BUTTON6 = 0x00000040;
        public const uint LGLCDBUTTON_BUTTON7 = 0x00000080;

        //************************************************************************
        // lgLcdDeviceDesc
        //************************************************************************
        [StructLayout(LayoutKind.Sequential)]
        public struct lgLcdDeviceDesc
        {
            public DWORD Width;
            public DWORD Height;
            public DWORD Bpp;
            public DWORD NumSoftButtons;
        } ;


        //************************************************************************
        // lgLcdDeviceDescEx
        //************************************************************************
        [StructLayout(LayoutKind.Sequential)]
        public struct lgLcdDeviceDescExW
        {
            public DWORD deviceFamilyId;
            public WCHAR deviceDisplayName;
            public DWORD Width;            // # of pixels (horizontally) on the LCD
            public DWORD Height;           // # of pixels (lines) on the LCD
            public DWORD Bpp;              // # of bits per pixel (1,8,16,24,...)
            public DWORD NumSoftButtons;
            public DWORD Reserved1;
            public DWORD Reserved2;
        }

        //************************************************************************
        // lgLcdBitmap
        //************************************************************************

        public const int LGLCD_BMP_FORMAT_160x43x1 = 0x00000001;
        public const int LGLCD_BMP_FORMAT_QVGAx32 = 0x00000003;
        public const int LGLCD_BMP_WIDTH = 160;
        public const int LGLCD_BMP_HEIGHT = 43;
        public const int LGLCD_BMP_BPP = 1;
        public const int LGLCD_BW_BMP_WIDTH = 160;
        public const int LGLCD_BW_BMP_HEIGHT = 43;
        public const int LGLCD_BW_BMP_BPP = 1;
        public const int LGLCD_QVGA_BMP_WIDTH = 320;
        public const int LGLCD_QVGA_BMP_HEIGHT = 240;
        public const int LGLCD_QVGA_BMP_BPP = 4;

        [StructLayout(LayoutKind.Sequential)]
        public struct lgLcdBitmapHeader
        {
            public DWORD Format;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct lgLcdBitmap160x43x1
        {
            public lgLcdBitmapHeader hdr;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = LGLCD_BMP_WIDTH * LGLCD_BMP_HEIGHT * LGLCD_BMP_BPP)]
            public BYTE[] pixels;

            public static lgLcdBitmap160x43x1 New
            {
                get
                {
                    return new lgLcdBitmap160x43x1()
                    {
                        hdr = new lgLcdBitmapHeader() { Format = LGLCD_BMP_FORMAT_160x43x1 },
                        pixels = new BYTE[LGLCD_BMP_WIDTH * LGLCD_BMP_HEIGHT * LGLCD_BMP_BPP]
                    };
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct lgLcdBitmapQVGAx32
        {
            public lgLcdBitmapHeader hdr;  // Format = LGLCD_BMP_FORMAT_QVGAx32
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = LGLCD_QVGA_BMP_WIDTH * LGLCD_QVGA_BMP_HEIGHT * LGLCD_QVGA_BMP_BPP)]
            public BYTE[] pixels;

            public static lgLcdBitmapQVGAx32 New
            {
                get
                {
                    return new lgLcdBitmapQVGAx32()
                    {
                        hdr = new lgLcdBitmapHeader() { Format = LGLCD_BMP_FORMAT_QVGAx32 },
                        pixels = new BYTE[LGLCD_QVGA_BMP_WIDTH * LGLCD_QVGA_BMP_HEIGHT * LGLCD_QVGA_BMP_BPP]
                    };
                }
            }
        }
        ////
        //// Generic bitmap for use by both color and BW applets
        ////
        //typedef union
        //{
        //    lgLcdBitmapHeader hdr;          // provides easy access to the header
        //    lgLcdBitmap160x43x1 bmp_mono;   // B/W bitmap data
        //    lgLcdBitmapQVGAx32 bmp_qvga32;  // Color bitmap data
        //} lgLcdBitmap;

        // Priorities
        public const int LGLCD_PRIORITY_IDLE_NO_SHOW = 0;
        public const int LGLCD_PRIORITY_BACKGROUND = 64;
        public const int LGLCD_PRIORITY_NORMAL = 128;
        public const int LGLCD_PRIORITY_ALERT = 255;

        public static uint LGLCD_SYNC_UPDATE(uint priority) { return 0x80000000 | priority; }
        public static uint LGLCD_SYNC_COMPLETE_WITHIN_FRAME(uint priority) { return 0xC0000000 | priority; }
        public static uint LGLCD_ASYNC_UPDATE(uint priority) { return priority; }

        // Foreground mode for client applications
        public const uint LGLCD_LCD_FOREGROUND_APP_NO = 0;
        public const uint LGLCD_LCD_FOREGROUND_APP_YES = 1;

        // Device family definitions
        public const uint LGLCD_DEVICE_FAMILY_BW_160x43_GAMING = 0x00000001;
        public const uint LGLCD_DEVICE_FAMILY_KEYBOARD_G15 = 0x00000001;
        public const uint LGLCD_DEVICE_FAMILY_BW_160x43_AUDIO = 0x00000002;
        public const uint LGLCD_DEVICE_FAMILY_SPEAKERS_Z10 = 0x00000002;
        public const uint LGLCD_DEVICE_FAMILY_JACKBOX = 0x00000004;
        public const uint LGLCD_DEVICE_FAMILY_BW_160x43_BASIC = 0x00000008;
        public const uint LGLCD_DEVICE_FAMILY_LCDEMULATOR_G15 = 0x00000008;
        public const uint LGLCD_DEVICE_FAMILY_RAINBOW = 0x00000010;
        public const uint LGLCD_DEVICE_FAMILY_QVGA_BASIC = 0x00000020;
        public const uint LGLCD_DEVICE_FAMILY_QVGA_GAMING = 0x00000040;
        public const uint LGLCD_DEVICE_FAMILY_GAMEBOARD_G13 = 0x00000080;
        public const uint LGLCD_DEVICE_FAMILY_KEYBOARD_G510 = 0x00000100;
        public const uint LGLCD_DEVICE_FAMILY_OTHER = 0x80000000;

        // Combinations of device families (device clans?)
        public const uint LGLCD_DEVICE_FAMILY_ALL_BW_160x43 = LGLCD_DEVICE_FAMILY_BW_160x43_GAMING
                                                           | LGLCD_DEVICE_FAMILY_BW_160x43_AUDIO
                                                           | LGLCD_DEVICE_FAMILY_JACKBOX
                                                           | LGLCD_DEVICE_FAMILY_BW_160x43_BASIC
                                                           | LGLCD_DEVICE_FAMILY_RAINBOW
                                                           | LGLCD_DEVICE_FAMILY_GAMEBOARD_G13
                                                           | LGLCD_DEVICE_FAMILY_KEYBOARD_G510;

        public const uint LGLCD_DEVICE_FAMILY_ALL_QVGA = LGLCD_DEVICE_FAMILY_QVGA_BASIC
                                                           | LGLCD_DEVICE_FAMILY_QVGA_GAMING;

        public const uint LGLCD_DEVICE_FAMILY_ALL = LGLCD_DEVICE_FAMILY_ALL_BW_160x43
                                                           | LGLCD_DEVICE_FAMILY_ALL_QVGA;


        // Capabilities of applets connecting to LCD Manager.
        public const uint LGLCD_APPLET_CAP_BASIC = 0x00000000;
        public const uint LGLCD_APPLET_CAP_BW = 0x00000001;
        public const uint LGLCD_APPLET_CAP_QVGA = 0x00000002;

        // Notifications sent by LCD Manager to applets connected to it.
        public const uint LGLCD_NOTIFICATION_DEVICE_ARRIVAL = 0x00000001;
        public const uint LGLCD_NOTIFICATION_DEVICE_REMOVAL = 0x00000002;
        public const uint LGLCD_NOTIFICATION_CLOSE_CONNECTION = 0x00000003;
        public const uint LGLCD_NOTIFICATION_APPLET_DISABLED = 0x00000004;
        public const uint LGLCD_NOTIFICATION_APPLET_ENABLED = 0x00000005;
        public const uint LGLCD_NOTIFICATION_TERMINATE_APPLET = 0x00000006;

        // Device types used in notifications
        public const int LGLCD_DEVICE_BW = 0x00000001;
        public const int LGLCD_DEVICE_QVGA = 0x00000002;

        //************************************************************************
        // Callbacks
        //************************************************************************

        // Callback used to notify client of soft button change
        public delegate int lgLcdOnSoftButtonsCB(int device, DWORD dwButtons, PVOID pContext);
        //typedef DWORD (WINAPI *lgLcdOnSoftButtonsCB)(IN int device,
        //                                             IN DWORD dwButtons,
        //                                             IN const PVOID pContext);

        // Callback used to allow client to pop up a "configuration panel"
        public delegate int lgLcdOnConfigureCB(int connection, PVOID pContext);
        //typedef DWORD (WINAPI *lgLcdOnConfigureCB)(IN int connection,
        //                                           IN const PVOID pContext);

        // Callback used to notify client of events, such as device arrival, ...
        // Arrival, removal, applet enable/disable supported as of version 3.0.
        public delegate int lgLcdOnNotificationCB(int connection,
                                                    PVOID pContext,
                                                    DWORD notificationCode,
                                                    DWORD notifyParm1,
                                                    DWORD notifyParm2,
                                                    DWORD notifyParm3,
                                                    DWORD notifyParm4);
        //typedef DWORD (WINAPI *lgLcdOnNotificationCB)(IN int connection,
        //                                              IN const PVOID pContext,
        //                                              IN DWORD notificationCode,
        //                                              IN DWORD notifyParm1,
        //                                              IN DWORD notifyParm2,
        //                                              IN DWORD notifyParm3,
        //                                              IN DWORD notifyParm4);


        //************************************************************************
        // lgLcdConfigureContext
        //************************************************************************
        [StructLayout(LayoutKind.Sequential)]
        public struct lgLcdConfigureContext
        {
            // Set to NULL if not configurable
            public lgLcdOnConfigureCB configCallback;
            public PVOID configContext;
        }

        //************************************************************************
        // lgLcdNotificationContext
        //************************************************************************
        [StructLayout(LayoutKind.Sequential)]
        public struct lgLcdNotificationContext
        {
            // Set to NULL if not notifiable
            public lgLcdOnNotificationCB notificationCallback;
            public PVOID notifyContext;
        }

        //************************************************************************
        // lgLcdConnectContext
        //************************************************************************
        [StructLayout(LayoutKind.Sequential)]
        public struct lgLcdConnectContext
        {
            // "Friendly name" display in the listing
            public LPCWSTR appFriendlyName;
            // isPersistent determines whether this connection persists in the list
            public BOOL isPersistent;
            // isAutostartable determines whether the client can be started by
            // LCDMon
            public BOOL isAutostartable;
            public lgLcdConfigureContext onConfigure;
            // --> Connection handle
            public int connection;
        }

        //************************************************************************
        // lgLcdConnectContextEx
        //************************************************************************
        [StructLayout(LayoutKind.Sequential)]
        public struct lgLcdConnectContextEx
        {
            // "Friendly name" display in the listing
            public LPCWSTR appFriendlyName;
            // isPersistent determines whether this connection persists in the list
            public BOOL isPersistent;
            // isAutostartable determines whether the client can be started by
            // LCDMon
            public BOOL isAutostartable;
            public lgLcdConfigureContext onConfigure;
            // --> Connection handle
            public int connection;
            // New additions added in 1.03 revision
            public DWORD dwAppletCapabilitiesSupported;    // Or'd combination of LGLCD_APPLET_CAP_... defines
            public DWORD dwReserved1;
            public lgLcdNotificationContext onNotify;
        }


        //************************************************************************
        // lgLcdOpenContext & lgLcdOpenByTypeContext
        //************************************************************************

        [StructLayout(LayoutKind.Sequential)]
        public struct lgLcdSoftbuttonsChangedContext
        {
            // Set to NULL if no softbutton notifications are needed
            public lgLcdOnSoftButtonsCB softbuttonsChangedCallback;
            public PVOID softbuttonsChangedContext;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct lgLcdOpenContext
        {
            public int connection;
            // Device index to open
            public int index;
            public lgLcdSoftbuttonsChangedContext onSoftbuttonsChanged;
            // --> Device handle
            public int device;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct lgLcdOpenByTypeContext
        {
            public int connection;
            // Device type to open (either LGLCD_DEVICE_BW or LGLCD_DEVICE_QVGA)
            public int deviceType;
            public lgLcdSoftbuttonsChangedContext onSoftbuttonsChanged;
            // --> Device handle
            public int device;
        }




        //************************************************************************
        // Prototypes
        //************************************************************************

        // Initialize the library by calling this function.
        [DllImport("LgLcd.dll")]
        public static extern
            DWORD lgLcdInit();

        // Must be called to release the library and free all allocated structures.
        [DllImport("LgLcd.dll")]
        public static extern
            DWORD lgLcdDeInit();

        // Connect as a client to the LCD subsystem. Provide name to be
        // displayed for user when viewing the user interface of the LCD module,
        // as well as a configuration callback and context, and a flag that states
        // whether this client is startable by LCDMon
        [DllImport("LgLcd.dll", EntryPoint = "lgLcdConnectW")]
        public static extern
            DWORD lgLcdConnect(ref lgLcdConnectContext ctx);
        [DllImport("LgLcd.dll", EntryPoint = "lgLcdConnectExW")]
        public static extern
            DWORD lgLcdConnectEx(ref lgLcdConnectContextEx ctx);

        // Must be called to release the connection and free all allocated resources
        [DllImport("LgLcd.dll")]
        public static extern
            DWORD lgLcdDisconnect(int connection);

        // New additions added in 1.03 revision of API. Call this method to setup which device families the applet
        // is interested in. After this call, the applet can use lgLcdEnumerateEx to determine
        // if a device from the device family wanted is found.
        [DllImport("LgLcd.dll")]
        public static extern
            DWORD lgLcdSetDeviceFamiliesToUse(int connection,
                DWORD dwDeviceFamiliesSupported,    // Or'd combination of LGLCD_DEVICE_FAMILY_... defines
                DWORD dwReserved1);

        // To determine all connected LCD devices supported by this library, and
        // their capabilities. Start with index 0, and increment by one, until
        // the library returns an error (WHICH?).
        [DllImport("LgLcd.dll")]
        public static extern
            DWORD lgLcdEnumerate(int connection, int index,
            out lgLcdDeviceDesc description);

        // To determine all connected LCD devices supported by this library, and
        // their capabilities. Start with 0, and increment by one, until
        // the library returns an error (WHICH?).
        [DllImport("LgLcd.dll")]
        public static extern
            DWORD lgLcdEnumerateExW(int connection, int index,
            out lgLcdDeviceDescExW description);

        // Opens the LCD at position=index. Library sets the device parameter to
        // its internal reference to the device. Calling application provides the
        // device handle in all calls that access the LCD.
        [DllImport("LgLcd.dll")]
        public static extern
            DWORD lgLcdOpen(ref lgLcdOpenContext ctx);

        // Opens an LCD of the specified type. If no such device is available, returns
        // an error.
        [DllImport("LgLcd.dll")]
        public static extern
            DWORD lgLcdOpenByType(ref lgLcdOpenByTypeContext ctx);

        // Closes the LCD. Must be paired with lgLcdOpen()/lgLcdOpenByType().
        [DllImport("LgLcd.dll")]
        public static extern
            DWORD lgLcdClose(int device);

        // Reads the state of the soft buttons for the device.
        [DllImport("LgLcd.dll")]
        public static extern
            DWORD lgLcdReadSoftButtons(int device, out DWORD buttons);

        // Provides a bitmap to be displayed on the LCD. The priority field
        // further describes the way in which the bitmap is to be applied.
        [DllImport("LgLcd.dll", EntryPoint = "lgLcdUpdateBitmap")]
        public static extern
            DWORD lgLcdUpdateBitmapBW(int device,
            ref lgLcdBitmap160x43x1 bitmap,
            DWORD priority);
        [DllImport("LgLcd.dll", EntryPoint = "lgLcdUpdateBitmap")]
        public static extern
            DWORD lgLcdUpdateBitmapQVGA32(int device,
            ref lgLcdBitmapQVGAx32 bitmap,
            DWORD priority);

        // Sets the calling application as the shown application on the LCD, and stops
        // any type of rotation among other applications on the LCD.
        [DllImport("LgLcd.dll")]
        public static extern
            DWORD lgLcdSetAsLCDForegroundApp(int device, int foregroundYesNoFlag);



        // windows error codes
        public const int ERROR_SUCCESS = 0; //   The action completed successfully.
        public const int ERROR_INVALID_DATA = 13; // The data is invalid.
        public const int ERROR_INVALID_PARAMETER = 87; // One of the parameters was invalid.
    }
}
