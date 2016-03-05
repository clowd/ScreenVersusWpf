using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ScreenVersusWpf
{
    public static class ScreenTools
    {
        static ScreenTools()
        {
            // sourced from caesay\Clowd
            IntPtr dc = WinAPI.GetDC(IntPtr.Zero);
            DpiX = (double) WinAPI.GetDeviceCaps(dc, WinAPI.DEVICECAP.LOGPIXELSX);
            DpiY = (double) WinAPI.GetDeviceCaps(dc, WinAPI.DEVICECAP.LOGPIXELSY);
            DpiZoom = DpiX / 96.0;
            WinAPI.ReleaseDC(IntPtr.Zero, dc);
        }

        public static double DpiZoom { get; private set; }
        public static double DpiX { get; private set; }
        public static double DpiY { get; private set; }

        public static int WpfToScreen(double wpfUnits)
        {
            return (int) Math.Round(wpfUnits * DpiZoom);
        }

        public static double ScreenToWpf(double screenPixels)
        {
            return screenPixels / DpiZoom;
        }

        public static double WpfSnapToPixels(double wpfUnits)
        {
            return Math.Round(wpfUnits * DpiZoom) / DpiZoom;
        }

        public static double WpfSnapToPixelsCeil(double wpfUnits)
        {
            return Math.Ceiling(wpfUnits * DpiZoom) / DpiZoom;
        }

        public static double WpfSnapToPixelsFloor(double wpfUnits)
        {
            return Math.Floor(wpfUnits * DpiZoom) / DpiZoom;
        }

        public static ScreenPoint GetMousePosition()
        {
            return ScreenPoint.FromSystem(Cursor.Position);
        }

        public static ScreenRect GetVirtualScreen()
        {
            return ScreenRect.FromSystem(SystemInformation.VirtualScreen);
        }

        public static ScreenRect GetBoundsOfScreenContaining(ScreenPoint point, bool returnWorkingAreaOnly = false)
        {
            // sourced from caesay\Clowd
            var hMonitor = WinAPI.MonitorFromPoint(point.ToSystem(), WinAPI.MonitorOptions.MONITOR_DEFAULTTONEAREST);
            var mi = new WinAPI.MONITORINFO();
            mi.cbSize = (uint) Marshal.SizeOf(mi);
            if (WinAPI.GetMonitorInfo(hMonitor, ref mi))
            {
                if (mi.rcWork.HasSize() && returnWorkingAreaOnly)
                    return ScreenRect.FromSystem(mi.rcWork);
                return ScreenRect.FromSystem(mi.rcMonitor);
            }
            return GetVirtualScreen();
        }

        public static ScreenRect GetBoundsOfScreenContaining(ScreenRect rect, bool returnWorkingAreaOnly = false)
        {
            // sourced from caesay\Clowd
            WinAPI.RECT sr = rect.ToSystem();
            var hMonitor = WinAPI.MonitorFromRect(ref sr, WinAPI.MonitorOptions.MONITOR_DEFAULTTONEAREST);
            var mi = new WinAPI.MONITORINFO();
            mi.cbSize = (uint) Marshal.SizeOf(mi);
            if (WinAPI.GetMonitorInfo(hMonitor, ref mi))
            {
                if (mi.rcWork.HasSize() && returnWorkingAreaOnly)
                    return ScreenRect.FromSystem(mi.rcWork);
                return ScreenRect.FromSystem(mi.rcMonitor);
            }
            return GetVirtualScreen();
        }
    }
}
