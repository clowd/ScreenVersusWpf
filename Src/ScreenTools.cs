using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;

namespace ScreenVersusWpf
{
    public static class ScreenTools
    {
        /// <summary>X coordinate of the virtual screen left edge in the system coordinate space.</summary>
        internal static int VirtualScreenSystemLeft { get; private set; }
        /// <summary>Y coordinate of the virtual screen top edge in the system coordinate space.</summary>
        internal static int VirtualScreenSystemTop { get; private set; }

        /// <summary>Caches screen information. Reset to null when screen settings change, to be regenerated on first use.</summary>
        private static List<ScreenInfo> _screens;

        /// <summary>Initialises ScreenTools on first use.</summary>
        static ScreenTools()
        {
            SystemEvents.DisplaySettingsChanged += delegate { displaySettingsChanged(); };
            displaySettingsChanged();
        }

        /// <summary>Handles changes to screen settings.</summary>
        private static void displaySettingsChanged()
        {
            _screens = null;

            var vscr = SystemInformation.VirtualScreen;
            VirtualScreenSystemLeft = vscr.Left;
            VirtualScreenSystemTop = vscr.Top;
            VirtualScreen = new ScreenInfo(vscr.Width, vscr.Height);

            if (ScreenSettingsChanged != null)
                ScreenSettingsChanged();
        }

        /// <summary>
        ///     Triggered whenever screen settings change, such as resolution, monitor count or position within the virtual
        ///     screen.</summary>
        public static event Action ScreenSettingsChanged;

        private static double _dpiZoom = 0;
        public static double DpiZoom
        {
            get
            {
                if (_dpiZoom == 0)
                    throw new InvalidOperationException("You must call ScreenTools.InitializeDpi before calling this method.");
                return _dpiZoom;
            }
        }

        public static void InitializeDpi(int screenDpi)
        {
            _dpiZoom = screenDpi / 96.0;
        }

        /// <summary>
        ///     Returns the system DPI value, which is the DPI setting at the time when the user signed in. This value does
        ///     not change when the user modifies DPI and doesn't sign out (as supported on Win8.1+). It is also the same for
        ///     all monitors, even if different monitors have different DPI selected.</summary>
        public static int GetSystemDpi()
        {
            // sourced from caesay\Clowd
            IntPtr dc = WinAPI.GetDC(IntPtr.Zero);
            int dpi = WinAPI.GetDeviceCaps(dc, WinAPI.DEVICECAP.LOGPIXELSX);
            WinAPI.ReleaseDC(IntPtr.Zero, dc);
            return dpi;
        }

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

        /// <summary>
        ///     Gets a read-only collection of all physical screens and their bounds. The primary screen is always the first
        ///     one enumerated. See Remarks.</summary>
        /// <remarks>
        ///     Do not cache what this property returns. Use the property directly whenever you require information about
        ///     system screens. This property is designed to be fast and caches the screen data itself. If system display
        ///     settings change, this internal cache is invalidated automatically and the next call to this property will
        ///     retrieve up-to-date screen information.</remarks>
        public static IEnumerable<ScreenInfo> Screens
        {
            get
            {
                if (_screens == null)
                    _screens = Screen.AllScreens.OrderBy(s => s.Primary ? 0 : 1).Select(s => new ScreenInfo(s)).ToList();
                return _screens.AsReadOnly();
            }
        }

        /// <summary>
        ///     Gets information about the virtual screen. In ScreenVersusWpf, the virtual screen's top left coordinate is
        ///     always 0,0. See Remarks.</summary>
        /// <remarks>
        ///     Do not cache what this property returns. This property is fast, and will return up-to-date data when display
        ///     settings change.</remarks>
        public static ScreenInfo VirtualScreen
        {
            get;
            private set;
        }

        /// <summary>Returns current mouse cursor position in the virtual screen coordinate space.</summary>
        public static ScreenPoint GetMousePosition()
        {
            return ScreenPoint.FromSystem(Cursor.Position);
        }

        /// <summary>
        ///     Returns the physical screen containing the specified point, or null if the point is outside of every screen.</summary>
        public static ScreenInfo GetScreenContaining(ScreenPoint point)
        {
            return Screens.FirstOrDefault(s => s.Bounds.Contains(point));
        }

        /// <summary>
        ///     Returns the physical screen containing the center of the specified rectangle, or null if the center is outside
        ///     of every screen.</summary>
        public static ScreenInfo GetScreenContaining(ScreenRect rect)
        {
            var center = new ScreenPoint(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
            return Screens.FirstOrDefault(s => s.Bounds.Contains(center));
        }
    }
}
