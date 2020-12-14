using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace ScreenVersusWpf
{
    /// <summary>
    /// Represents a display device or multiple display devices on a single system.
    /// </summary>
    public class ScreenInfo : IEquatable<ScreenInfo>
    {
        // References:
        // http://referencesource.microsoft.com/#System.Windows.Forms/ndp/fx/src/winforms/Managed/System/WinForms/Screen.cs
        // http://msdn.microsoft.com/en-us/library/windows/desktop/dd145072.aspx
        // http://msdn.microsoft.com/en-us/library/windows/desktop/dd183314.aspx
        // https://raw.githubusercontent.com/micdenny/WpfScreenHelper/master/src/WpfScreenHelper/Screen.cs

        private readonly IntPtr _hMonitor;

        private const int MONITORINFOF_PRIMARY = 0x00000001;
        private const int MONITOR_DEFAULTTONEAREST = 0x00000002;

        private static readonly bool SystemHasMultiMonitorSupport;

        static ScreenInfo()
        {
            Sys.EnsureProcessDPIAwareness();
            SystemHasMultiMonitorSupport = Sys.GetSystemMetrics(Sys.SYSTEM_METRIC.SM_CMONITORS) != 0;
        }

        private ScreenInfo()
        {
            this.IsVirtual = true;
            this.IsPrimary = true;
        }

        private ScreenInfo(IntPtr hMonitor)
        {
            var info = new Sys.MONITORINFO();
            info.cbSize = (uint)Marshal.SizeOf(typeof(Sys.MONITORINFO));
            Sys.GetMonitorInfo(hMonitor, ref info);
            this.IsPrimary = ((info.dwFlags & MONITORINFOF_PRIMARY) != 0);
            _hMonitor = hMonitor;
        }

        /// <summary>
        /// Gets the primary display.
        /// </summary>
        public static ScreenInfo PrimaryScreen
        {
            get
            {
                if (!SystemHasMultiMonitorSupport)
                    return new ScreenInfo();

                return AllScreens.FirstOrDefault(t => t.IsPrimary);
            }
        }

        /// <summary>
        /// Gets the virtual display bounds. On a single monitor system, this will have the same bounds as PrimaryScreen.
        /// On a multi-monitor system, this will be a rectangle that contains the bounds of all displays.
        /// </summary>
        public static ScreenInfo VirtualScreen => new ScreenInfo();

        /// <summary>
        /// Gets an enumeration of all displays on the system.
        /// </summary>
        public static IEnumerable<ScreenInfo> AllScreens
        {
            get
            {
                if (!SystemHasMultiMonitorSupport)
                    return new[] { new ScreenInfo() };

                List<ScreenInfo> displays = new List<ScreenInfo>();
                bool Callback(IntPtr hMonitor, IntPtr hdcMonitor, ref Sys.RECT lprcMonitor, IntPtr dwData)
                {
                    displays.Add(new ScreenInfo(hMonitor));
                    return true;
                }
                Sys.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, Callback, IntPtr.Zero);

                return displays;
            }
        }

        /// <summary>
        /// Retrieves a <see cref="ScreenInfo"/> for the display that contains the specified point.
        /// </summary>
        public static ScreenInfo FromPoint(ScreenPoint point)
        {
            if (!SystemHasMultiMonitorSupport)
                return new ScreenInfo();

            var pt = (Sys.POINT)(System.Drawing.Point)point;
            return new ScreenInfo(Sys.MonitorFromPoint(ref pt, MONITOR_DEFAULTTONEAREST));
        }

        /// <summary>
        /// Retrieves a <see cref="ScreenInfo"/> for the display that contains the center point of the specified rect.
        /// </summary>
        public static ScreenInfo FromRect(ScreenRect rect)
        {
            var center = new ScreenPoint(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
            return FromPoint(center);
        }

        /// <summary>
        /// Retrieves a <see cref="ScreenInfo"/> for the display that contains the center point of the specified window.
        /// </summary>
        public static ScreenInfo FromWindow(System.Windows.Window hWnd)
        {
            var h = new System.Windows.Interop.WindowInteropHelper(hWnd);
            return FromWindow(h.Handle);
        }

        /// <summary>
        /// Retrieves a <see cref="ScreenInfo"/> for the display that contains the center point of the specified window handle.
        /// </summary>
        public static ScreenInfo FromWindow(IntPtr hWnd)
        {
            if (!SystemHasMultiMonitorSupport)
                return new ScreenInfo();

            if (hWnd == IntPtr.Zero)
                throw new ArgumentException("Window handle must be non-empty.");

            return new ScreenInfo(Sys.MonitorFromWindow(hWnd, MONITOR_DEFAULTTONEAREST));
        }

        /// <summary>
        /// Gets the native monitor handle to use when executing interop with user32.dll functions
        /// </summary>
        public IntPtr Handle => _hMonitor;

        /// <summary>
        /// Gets the current DpiContext for this monitor.
        /// </summary>
        public DpiContext DpiContext
        {
            get
            {
                if (!SystemHasMultiMonitorSupport)
                    return DpiContext.FromPrimaryScreen();

                return DpiContext.FromScreen(_hMonitor);
            }
        }

        /// <summary>
        /// Gets a value indicating whether a particular display is the primary device.
        /// </summary>
        /// <returns>true if this display is primary; otherwise, false.</returns>
        public bool IsPrimary { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this class represents the virtual screen instead of a physical device.
        /// </summary>
        /// <returns>true if this display is virtual; otherwise, false.</returns>
        public bool IsVirtual { get; private set; }

        /// <summary>
        /// Gets the bounds of the display.
        /// </summary>
        public ScreenRect Bounds
        {
            get
            {
                if (IsVirtual || !SystemHasMultiMonitorSupport)
                    return (ScreenRect)System.Windows.Forms.SystemInformation.VirtualScreen;

                var info = new Sys.MONITORINFO();
                info.cbSize = (uint)Marshal.SizeOf(typeof(Sys.MONITORINFO));
                Sys.GetMonitorInfo(_hMonitor, ref info);
                return ScreenRect.FromLTRB(info.rcMonitor);
            }
        }

        /// <summary>
        /// Gets the working area of the display. The working area is the desktop area of the display, excluding taskbars, docked windows, and docked tool bars.
        /// </summary>
        public ScreenRect WorkingArea
        {
            get
            {
                if (IsVirtual || !SystemHasMultiMonitorSupport)
                    return (ScreenRect)System.Windows.Forms.SystemInformation.WorkingArea;

                var info = new Sys.MONITORINFO();
                info.cbSize = (uint)Marshal.SizeOf(typeof(Sys.MONITORINFO));
                Sys.GetMonitorInfo(_hMonitor, ref info);
                return ScreenRect.FromLTRB(info.rcWork);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the specified object is logically equal to this object.
        /// </summary>
        public override bool Equals(object obj)
        {
            var monitor = obj as ScreenInfo;
            if (monitor != null)
            {
                if (_hMonitor == monitor._hMonitor)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets a value indicating whether the specified object is logically equal to this object.
        /// </summary>
        public bool Equals(ScreenInfo other)
        {
            return _hMonitor == other._hMonitor;
        }

        /// <summary>
        /// Computes and retrieves a hash code for an object.
        /// </summary>
        public override int GetHashCode()
        {
            return unchecked((int)_hMonitor);
        }

        /// <summary>
        /// Retrieves the screen bounds and working area as a human-readable string
        /// </summary>
        public override string ToString()
        {
            if (IsVirtual)
                return $"{Bounds.Width}x{Bounds.Height}, virtual screen";

            var workingArea = WorkingArea == Bounds ? "" : $", working area {WorkingArea.Width}x{WorkingArea.Height} at ({WorkingArea.Left}, {WorkingArea.Top})";
            return $"{Bounds.Width}x{Bounds.Height} at ({Bounds.Left}, {Bounds.Top}){(IsPrimary ? ", primary" : "")}{workingArea}";
        }
    }
}
