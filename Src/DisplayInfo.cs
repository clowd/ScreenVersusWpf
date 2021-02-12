using ScreenVersusWpf.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Interop;

namespace ScreenVersusWpf
{
    /// <summary>
    /// Represents a display device or multiple display devices on a single system.
    /// </summary>
    public class DisplayInfo : IEquatable<DisplayInfo>
    {
        // References:
        // http://referencesource.microsoft.com/#System.Windows.Forms/ndp/fx/src/winforms/Managed/System/WinForms/Screen.cs
        // http://msdn.microsoft.com/en-us/library/windows/desktop/dd145072.aspx
        // http://msdn.microsoft.com/en-us/library/windows/desktop/dd183314.aspx
        // https://github.com/micdenny/WpfScreenHelper
        // https://raw.githubusercontent.com/micdenny/WpfScreenHelper/master/src/WpfScreenHelper/Screen.cs

        private readonly IntPtr _hMonitor;

        private const int MONITORINFOF_PRIMARY = 0x00000001;

        private DisplayInfo()
        {
            this.IsVirtual = true;
        }

        private DisplayInfo(IntPtr hMonitor)
        {
            if (hMonitor == IntPtr.Zero)
                throw new ArgumentNullException(nameof(hMonitor));

            var info = WinAPI.GetMonitorInfo(hMonitor);
            this.IsPrimary = ((info.dwFlags & MONITORINFOF_PRIMARY) != 0);
            _hMonitor = hMonitor;
        }

        /// <summary>
        /// Gets the primary display.
        /// </summary>
        public static DisplayInfo PrimaryScreen => AllScreens.FirstOrDefault(t => t.IsPrimary);

        /// <summary>
        /// Gets the virtual display bounds. On a single monitor system, this will have the same bounds as PrimaryScreen.
        /// On a multi-monitor system, this will be a rectangle that contains the bounds of all displays.
        /// </summary>
        public static DisplayInfo VirtualScreen => new DisplayInfo();

        /// <summary>
        /// Gets an enumeration of all displays on the system.
        /// </summary>
        public static IEnumerable<DisplayInfo> AllScreens => WinAPI.EnumDisplayMonitors().Select(d => new DisplayInfo(d));

        /// <summary>
        /// Retrieves a <see cref="DisplayInfo"/> for the display that contains the specified point.
        /// </summary>
        public static DisplayInfo FromPoint(ScreenPoint point) => new DisplayInfo(WinAPI.MonitorFromPoint(point, MONITOR_FLAGS.MONITOR_DEFAULTTONEAREST));

        /// <summary>
        /// Retrieves a <see cref="DisplayInfo"/> for the display that contains the center point of the specified rect.
        /// </summary>
        public static DisplayInfo FromRect(ScreenRect rect) => FromPoint(rect.Center);

        /// <summary>
        /// Retrieves a <see cref="DisplayInfo"/> for the display that contains the center point of the specified window.
        /// </summary>
        public static DisplayInfo FromWindow(Window hWnd) => FromWindow(new WindowInteropHelper(hWnd).Handle);

        /// <summary>
        /// Retrieves a <see cref="DisplayInfo"/> for the display that contains the center point of the specified window handle.
        /// </summary>
        public static DisplayInfo FromWindow(IntPtr hWnd) => new DisplayInfo(WinAPI.MonitorFromWindow(hWnd, MONITOR_FLAGS.MONITOR_DEFAULTTONEAREST));

        /// <summary>
        /// Gets the native monitor handle to use when executing interop with user32.dll functions
        /// </summary>
        public IntPtr Handle => _hMonitor;

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
        public ScreenRect Bounds => IsVirtual ? WinAPI.GetVirtualBounds() : WinAPI.GetMonitorInfo(_hMonitor).rcMonitor;

        /// <summary>
        /// Gets the working area of the display. The working area is the desktop area of the display, excluding taskbars, docked windows, and docked tool bars.
        /// </summary>
        public ScreenRect WorkingArea => IsVirtual ? WinAPI.GetVirtualWorkArea() : WinAPI.GetMonitorInfo(_hMonitor).rcWork;

        public DpiContext DpiContext => new DisplayDpiContext(_hMonitor);

        //public DpiContext GetDpiContext(WorldOrigin origin) => DpiContext.FromDisplay(this, origin);
        //public DpiContext GetDpiContext(ScreenPoint origin) => DpiContext.FromDisplay(this, origin);
        //public DpiContext GetDpiContext(int originX, int originY) => DpiContext.FromDisplay(this, originX, originY);

        /// <summary>
        /// Gets a value indicating whether the specified object is logically equal to this object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is DisplayInfo display)
                return Equals(display);
            return false;
        }

        /// <summary>
        /// Gets a value indicating whether the specified object is logically equal to this object.
        /// </summary>
        public bool Equals(DisplayInfo other) => _hMonitor == other._hMonitor;

        /// <summary>
        /// Computes and retrieves a hash code for an object.
        /// </summary>
        public override int GetHashCode() => unchecked((int)_hMonitor);

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
