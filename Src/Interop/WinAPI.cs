using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace ScreenVersusWpf.Interop
{
    internal static class WinAPI
    {
        static WinAPI()
        {
            EnsureProcessDPIAwareness();
        }

        private delegate bool EnumMonitorsProc(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(SYSTEM_METRIC nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool SystemParametersInfo(int nAction, int nParam, out RECT rc, int nUpdate);

        public static ScreenPoint GetMouseScreenPosition()
        {
            var w32Mouse = new POINT();
            var success = GetCursorPos(ref w32Mouse);
            if (!success)
                throw new Win32Exception(Marshal.GetLastWin32Error());
            return new ScreenPoint(w32Mouse.x, w32Mouse.y);
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(ref POINT pt);

        private static void EnsureProcessDPIAwareness()
        {
            bool IsWindowsVistaOrLater = Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version >= new Version(6, 0, 0);
            bool IsDesignMode = DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()) || LicenseManager.UsageMode == LicenseUsageMode.Designtime;

            if (!IsWindowsVistaOrLater)
                return;

            if (IsDesignMode) // don't throw if we're at design time (initialized by WPF or WinForms designer)
                return;

            // https://stackoverflow.com/a/2819962
            // XP and BELOW: font/control scaling - no DPI scaling
            // WIN VISTA to WIN 8.0 - Global desktop DPI scaling / virtualization
            // WIN 8.1 and up - Per-monitor DPI scaling. shcore.dll introduced.

            bool isAware = false;

            try
            {
                // https://stackoverflow.com/questions/4172850/isprocessdpiaware-always-returns-true
                // we could check if the windows version is >= 8.1 first, but windows will lie to us if the application author
                // has not created the correct manifest files, so lets just try an shcore.dll function and see if it works.

                PROCESS_DPI_AWARENESS awareness = PROCESS_DPI_AWARENESS.PROCESS_DPI_UNAWARE;
                GetProcessDpiAwareness(IntPtr.Zero, ref awareness);
                isAware = awareness == PROCESS_DPI_AWARENESS.PROCESS_PER_MONITOR_DPI_AWARE;
            }
            catch (Exception ex) when (ex is DllNotFoundException || ex is EntryPointNotFoundException)
            {
                try
                {
                    // shcore.dll does not exist, lets fall back to a user32 version
                    isAware = IsProcessDPIAware();
                }
                catch (Exception ex2) when (ex2 is DllNotFoundException || ex2 is EntryPointNotFoundException)
                {
                    // it's possible this method also does not exist
                    isAware = true;
                }
            }

            if (!isAware)
                throw new NotSupportedException("To execute this function, the current process must be DPI-Aware (Vista-8.0) or Per-Monitor DPI aware (> 8.1) and .Net 4.8 or above.");
        }

        [DllImport("user32.dll")]
        private static extern bool IsProcessDPIAware();

        [DllImport("shcore.dll")]
        private static extern bool GetProcessDpiAwareness(IntPtr hProcess, ref PROCESS_DPI_AWARENESS value);

        public static List<IntPtr> EnumDisplayMonitors()
        {
            List<IntPtr> displays = new List<IntPtr>();
            bool Callback(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData)
            {
                displays.Add(hMonitor);
                return true;
            }

            bool success = EnumDisplayMonitors_UM(IntPtr.Zero, IntPtr.Zero, Callback, IntPtr.Zero);
            if (!success)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            return displays;
        }

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "EnumDisplayMonitors")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumDisplayMonitors_UM(IntPtr hdc, IntPtr lprcClip, EnumMonitorsProc lpfnEnum, IntPtr dwData);

        public static MONITORINFO GetMonitorInfo(IntPtr hMonitor)
        {
            if (hMonitor == IntPtr.Zero)
                throw new ArgumentNullException(nameof(hMonitor));

            var info = new MONITORINFO();
            info.cbSize = (uint)Marshal.SizeOf(typeof(MONITORINFO));

            var success = GetMonitorInfo_UM(hMonitor, ref info);
            if (!success)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            return info;
        }

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "GetMonitorInfo")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetMonitorInfo_UM(IntPtr hMonitor, ref MONITORINFO lpmi);

        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(IntPtr hWnd, MONITOR_FLAGS flags);

        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromPoint(POINT pt, MONITOR_FLAGS flags);

        public static RECT GetVirtualBounds()
        {
            var x = GetSystemMetrics(SYSTEM_METRIC.SM_XVIRTUALSCREEN);
            var y = GetSystemMetrics(SYSTEM_METRIC.SM_YVIRTUALSCREEN);
            var cx = GetSystemMetrics(SYSTEM_METRIC.SM_CXVIRTUALSCREEN);
            var cy = GetSystemMetrics(SYSTEM_METRIC.SM_CYVIRTUALSCREEN);
            return new RECT
            {
                left = x,
                top = y,
                right = x + cx,
                bottom = y + cy,
            };
        }

        public static RECT GetVirtualWorkArea()
        {
            var SPI_GETWORKAREA = 48;
            SystemParametersInfo(SPI_GETWORKAREA, 0, out var rect, 0);
            return rect;
        }

        public static (int dx, int dy) GetDpiForMonitor(IntPtr hMonitor)
        {
            if (hMonitor == IntPtr.Zero)
                throw new ArgumentNullException(nameof(hMonitor));

            try
            {
                var hresult = GetDpiForMonitor_UM(hMonitor, MONITOR_DPI_TYPE.MDT_EFFECTIVE_DPI, out var dx, out var dy);
                if (hresult != 0)
                    throw new Win32Exception(hresult, "The handle, DPI type, or pointers passed in are not valid.");
                return ((int)dx, (int)dy);
            }
            catch (Exception ex) when (ex is DllNotFoundException || ex is EntryPointNotFoundException)
            {
                return GetDpiForSystem();
            }
        }

        [DllImport("shcore.dll", EntryPoint = "GetDpiForMonitor")]
        private static extern int GetDpiForMonitor_UM(IntPtr hMonitor, MONITOR_DPI_TYPE dpiType, out uint dpiX, out uint dpiY);

        public static (int dx, int dy) GetDpiForSystem()
        {
            IntPtr dc = GetDC(IntPtr.Zero);
            if (dc == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            int dx = GetDeviceCaps(dc, DEVICECAP.LOGPIXELSX);
            int dy = GetDeviceCaps(dc, DEVICECAP.LOGPIXELSY);
            ReleaseDC(IntPtr.Zero, dc);
            return (dx, dy);
        }

        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, DEVICECAP nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        public static RECT GetWindowRect(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
                throw new ArgumentNullException(nameof(hWnd));

            bool success = GetWindowRect_UM(hWnd, out var rect);
            if (!success)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            return rect;
        }

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "GetWindowRect")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect_UM(IntPtr hWnd, out RECT lpRectangle);

        public static RECT GetWindowClientRect(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
                throw new ArgumentNullException(nameof(hWnd));

            bool success = GetClientRect_UM(hWnd, out var rect);
            if (!success)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            return rect;
        }

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "GetClientRect")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetClientRect_UM(IntPtr hWnd, out RECT lpRectangle);

        public static RECT GetWindowExtendedBoundsRect(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
                throw new ArgumentNullException(nameof(hWnd));

            try
            {
                int hresult;
                hresult = DwmIsCompositionEnabled(out var enabled);
                if (hresult != 0)
                    throw new Win32Exception(hresult);

                if (!enabled)
                    return GetWindowRect(hWnd);

                hresult = DwmGetWindowAttribute(hWnd, DWMWINDOWATTRIBUTE.DWMWA_EXTENDED_FRAME_BOUNDS, out var rect, Marshal.SizeOf(typeof(RECT)));
                if (hresult != 0)
                    throw new Win32Exception(hresult);

                return rect;
            }
            catch (Exception ex) when (ex is DllNotFoundException || ex is EntryPointNotFoundException)
            {
                return GetWindowRect(hWnd);
            }
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmGetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE dwAttribute, out RECT pvAttribute, int cbAttribute);

        [DllImport("dwmapi.dll")]
        private static extern int DwmIsCompositionEnabled(out bool enabled);

        public static string GetWindowCaption(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
                throw new ArgumentNullException(nameof(hWnd));

            int length = GetWindowTextLength(hWnd);
            if (length == 0) return String.Empty;
            StringBuilder builder = new StringBuilder(length);
            length = GetWindowText(hWnd, builder, length + 1);
            if (length == 0) return String.Empty;
            return builder.ToString();
        }

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        public static void SetWindowPos(IntPtr hWnd, SWP_HWND hWndInsertAfter, int x, int y, int cx, int cy, SWP wFlags)
        {
            if (hWnd == IntPtr.Zero)
                throw new ArgumentNullException(nameof(hWnd));

            bool success = SetWindowPos_UM(hWnd, hWndInsertAfter, x, y, cx, cy, wFlags);
            if (!success)
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "SetWindowPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos_UM(IntPtr hWnd, SWP_HWND hWndInsertAfter, int x, int y, int cx, int cy, SWP wFlags);
    }
}
