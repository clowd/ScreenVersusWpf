using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using D = System.Drawing;

namespace ScreenVersusWpf
{
    internal static class Sys
    {
        // STATIC HELPERS
        private static bool _isAware;

        public static bool IsDesignMode => DesignerProperties.GetIsInDesignMode(new DependencyObject()) || LicenseManager.UsageMode == LicenseUsageMode.Designtime;

        public static bool IsWindowsVistaOrLater => Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version >= new Version(6, 0, 0);

        public static void EnsureProcessDPIAwareness()
        {
            if (_isAware)
                return;

            if (!Sys.IsWindowsVistaOrLater)
                return;

            if (Sys.IsDesignMode) // don't throw if we're at design time (initialized by WPF or WinForms designer)
                return;

            // https://stackoverflow.com/a/2819962
            // XP and BELOW: font/control scaling - no DPI scaling
            // WIN VISTA to WIN 8.0 - Global desktop DPI scaling / virtualization
            // WIN 8.1 and up - Per-monitor DPI scaling. shcore.dll introduced.

            try
            {
                // https://stackoverflow.com/questions/4172850/isprocessdpiaware-always-returns-true
                // we could check if the windows version is >= 8.1 first, but windows will lie to us if the application author
                // has not created the correct manifest files, so lets just try an shcore.dll function and see if it works.

                Sys.PROCESS_DPI_AWARENESS awareness = Sys.PROCESS_DPI_AWARENESS.PROCESS_DPI_UNAWARE;
                Sys.GetProcessDpiAwareness(IntPtr.Zero, ref awareness);
                _isAware = awareness == Sys.PROCESS_DPI_AWARENESS.PROCESS_PER_MONITOR_DPI_AWARE;
            }
            catch (DllNotFoundException)
            {
                // shcore.dll does not exist, lets fall back to a user32 version
                _isAware = Sys.IsProcessDPIAware();
            }

            if (!_isAware)
                throw new NotSupportedException("To execute this function, the current process must be DPI-Aware (Vista-8.0) or Per-Monitor DPI aware (> 8.1) and .Net 4.8 or above.");
        }

        // DELEGATES
        public delegate bool EnumMonitorsProc(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

        // IMPORTS
        [DllImport("dwmapi.dll", SetLastError = true)]
        public static extern int DwmGetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE dwAttribute, out RECT pvAttribute, int cbAttribute);

        [DllImport("dwmapi.dll")]
        public static extern int DwmIsCompositionEnabled(out bool enabled);

        [DllImport("gdi32.dll")]
        public static extern int GetDeviceCaps(IntPtr hdc, DEVICECAP nIndex);

        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(SYSTEM_METRIC nIndex);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, EnumMonitorsProc lpfnEnum, IntPtr dwData);

        [DllImport("user32.dll")]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromPoint(ref POINT pt, int flags);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRectangle);

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRectangle);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, SWP_HWND hWndInsertAfter, int x, int Y, int cx, int cy, SWP wFlags);

        [DllImport("user32.dll")]
        public static extern bool SetProcessDPIAware();

        [DllImport("user32.dll")]
        public static extern bool IsProcessDPIAware();

        [DllImport("shcore.dll")]
        public static extern bool GetDpiForMonitor(IntPtr hMonitor, MONITOR_DPI_TYPE dpiType, ref uint dpiX, ref uint dpiY);

        [DllImport("shcore.dll")]
        public static extern bool SetProcessDpiAwareness(PROCESS_DPI_AWARENESS value);

        [DllImport("shcore.dll")]
        public static extern bool GetProcessDpiAwareness(IntPtr hProcess, ref PROCESS_DPI_AWARENESS value);

        // ENUMS
        [Flags]
        public enum SWP : uint
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

        public enum SWP_HWND : int
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

        public enum DWMWINDOWATTRIBUTE
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

        public enum MONITOR_DPI_TYPE
        {
            MDT_EFFECTIVE_DPI = 0,
            MDT_ANGULAR_DPI = 1,
            MDT_RAW_DPI = 2,
            MDT_DEFAULT = MDT_EFFECTIVE_DPI
        }

        public enum PROCESS_DPI_AWARENESS
        {
            PROCESS_DPI_UNAWARE = 0,
            PROCESS_SYSTEM_DPI_AWARE = 1,
            PROCESS_PER_MONITOR_DPI_AWARE = 2
        }

        public enum MonitorOptions : uint
        {
            MONITOR_DEFAULTTONULL = 0x00000000,
            MONITOR_DEFAULTTOPRIMARY = 0x00000001,
            MONITOR_DEFAULTTONEAREST = 0x00000002
        }

        public enum DEVICECAP
        {
            LOGPIXELSX = 88,
            LOGPIXELSY = 90,
            VERTRES = 10,
            DESKTOPVERTRES = 117,
            DESKTOPHORZRES = 118,
        }

        public enum SYSTEM_METRIC : int
        {
            SM_CXSCREEN = 0,
            SM_CYSCREEN = 1,
            SM_CMONITORS = 80,
        }

        // STRUCTURES
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;

            public static implicit operator D.Point(POINT pt) { return new D.Point(pt.x, pt.y); }
            public static implicit operator POINT(D.Point pt) { return new POINT { x = pt.X, y = pt.Y }; }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public bool HasSize()
            {
                return right - left > 0 && bottom - top > 0;
            }

            public static implicit operator D.Rectangle(RECT rect)
            {
                return new D.Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
            }
            public static implicit operator RECT(D.Rectangle rect)
            {
                return new RECT
                {
                    left = rect.X,
                    top = rect.Y,
                    right = rect.X + rect.Width,
                    bottom = rect.Y + rect.Height
                };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MONITORINFO
        {
            public uint cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
        }
    }
}
