using System;
using System.Runtime.InteropServices;

namespace ScreenVersusWpf.Interop
{
    internal enum MONITOR_FLAGS : int
    {
        MONITOR_DEFAULTTONULL = 0,
        MONITOR_DEFAULTTOPRIMARY = 1,
        MONITOR_DEFAULTTONEAREST = 2,
    }


    [Flags]
    internal enum SWP : uint
    {
        NOSIZE = 0x0001,
        NOMOVE = 0x0002,
        NOZORDER = 0x0004,
        NOREDRAW = 0x0008,
        NOACTIVATE = 0x0010,
        DRAWFRAME = 0x0020,
        FRAMECHANGED = 0x0020,
        SHOWWINDOW = 0x0040,
        HIDEWINDOW = 0x0080,
        NOCOPYBITS = 0x0100,
        NOOWNERZORDER = 0x0200,
        NOREPOSITION = 0x0200,
        NOSENDCHANGING = 0x0400,
        DEFERERASE = 0x2000,
        ASYNCWINDOWPOS = 0x4000
    }

    internal enum SWP_HWND : int
    {
        /// <summary>
        /// Places the window at the bottom of the Z order. If the hWnd parameter identifies a topmost window, the window loses its topmost status and is placed at the bottom of all other windows.
        /// </summary>
        HWND_BOTTOM = 1,
        /// <summary>
        /// Places the window above all non-topmost windows (that is, behind all topmost windows). This flag has no effect if the window is already a non-topmost window.
        /// </summary>
        HWND_NOTOPMOST = -2,
        /// <summary>
        /// Places the window at the top of the Z order.
        /// </summary>
        HWND_TOP = 0,
        /// <summary>
        /// Places the window above all non-topmost windows. The window maintains its topmost position even when it is deactivated.
        /// </summary>
        HWND_TOPMOST = -1,
    }

    internal enum DWMWINDOWATTRIBUTE
    {
        DWMWA_NCRENDERING_ENABLED = 1,
        DWMWA_NCRENDERING_POLICY = 2,
        DWMWA_TRANSITIONS_FORCEDISABLED = 3,
        DWMWA_ALLOW_NCPAINT = 4,
        DWMWA_CAPTION_BUTTON_BOUNDS = 5,
        DWMWA_NONCLIENT_RTL_LAYOUT = 6,
        DWMWA_FORCE_ICONIC_REPRESENTATION = 7,
        DWMWA_FLIP3D_POLICY = 8,
        DWMWA_EXTENDED_FRAME_BOUNDS = 9,
        DWMWA_HAS_ICONIC_BITMAP = 10,
        DWMWA_DISALLOW_PEEK = 11,
        DWMWA_EXCLUDED_FROM_PEEK = 12,
        DWMWA_LAST = 13
    }

    internal enum MONITOR_DPI_TYPE
    {
        MDT_EFFECTIVE_DPI = 0,
        MDT_ANGULAR_DPI = 1,
        MDT_RAW_DPI = 2,
        MDT_DEFAULT = MDT_EFFECTIVE_DPI
    }

    internal enum PROCESS_DPI_AWARENESS
    {
        PROCESS_DPI_UNAWARE = 0,
        PROCESS_SYSTEM_DPI_AWARE = 1,
        PROCESS_PER_MONITOR_DPI_AWARE = 2
    }

    internal enum MonitorOptions : uint
    {
        MONITOR_DEFAULTTONULL = 0x00000000,
        MONITOR_DEFAULTTOPRIMARY = 0x00000001,
        MONITOR_DEFAULTTONEAREST = 0x00000002
    }

    internal enum DEVICECAP
    {
        LOGPIXELSX = 88,
        LOGPIXELSY = 90,
        VERTRES = 10,
        DESKTOPVERTRES = 117,
        DESKTOPHORZRES = 118,
    }

    internal enum SYSTEM_METRIC : int
    {
        SM_CXSCREEN = 0,
        SM_CYSCREEN = 1,
        SM_XVIRTUALSCREEN = 76,
        SM_YVIRTUALSCREEN = 77,
        SM_CXVIRTUALSCREEN = 78,
        SM_CYVIRTUALSCREEN = 79,
        SM_CMONITORS = 80,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct POINT
    {
        public int x;
        public int y;

        public static implicit operator ScreenPoint(POINT pt) => new ScreenPoint(pt.x, pt.y);
        public static implicit operator POINT(ScreenPoint pt) => new POINT { x = pt.X, y = pt.Y };

        //public static implicit operator D.Point(POINT pt) { return new D.Point(pt.x, pt.y); }
        //public static implicit operator POINT(D.Point pt) { return new POINT { x = pt.X, y = pt.Y }; }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;

        public bool HasSize()
        {
            return right - left > 0 && bottom - top > 0;
        }

        public static implicit operator ScreenRect(RECT rect) => ScreenRect.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
        public static implicit operator RECT(ScreenRect rect) => new RECT
        {
            left = rect.X,
            top = rect.Y,
            right = rect.X + rect.Width,
            bottom = rect.Y + rect.Height
        };

        //public static implicit operator D.Rectangle(RECT rect)
        //{
        //    return new D.Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
        //}
        //public static implicit operator RECT(D.Rectangle rect)
        //{
        //    return new RECT
        //    {
        //        left = rect.X,
        //        top = rect.Y,
        //        right = rect.X + rect.Width,
        //        bottom = rect.Y + rect.Height
        //    };
        //}
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MONITORINFO
    {
        public uint cbSize;
        public RECT rcMonitor;
        public RECT rcWork;
        public uint dwFlags;
    }
}
