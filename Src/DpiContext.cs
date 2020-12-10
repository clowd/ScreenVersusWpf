using System;
using System.Windows;
using System.Windows.Media;

namespace ScreenVersusWpf
{
    /// <summary>
    /// A DpiContext is used to translate coordinates from a specified virtual DPI/PPI into typical screen coordinates @ 96 DPI. 
    /// If translating to/from a window, this should be short lived, and re-created each time a conversion needs to be done, since 
    /// the window DPI depends on the monitor in which it's center point is located and can change at any time.
    /// </summary>
    public struct DpiContext
    {
        /// <summary>
        /// Gets the amount, in screen units, that a unit will be translated when converting to and from screen coordinates
        /// </summary>
        public int VirtualWorldOffsetX { get; }

        /// <summary>
        /// Gets the amount, in screen units, that a unit will be translated when converting to and from screen coordinates
        /// </summary>
        public int VirtualWorldOffsetY { get; }

        /// <summary>
        /// Gets the DPI on the X axis. When monitor zoom is 100%, this is 96. 
        /// </summary>
        public int DpiX { get; }

        /// <summary>
        /// Gets the DPI on the Y axis. When monitor zoom is 100%, this is 96. On Windows Desktop, this value is always the same as <see cref="DpiX"/>
        /// </summary>
        public int DpiY { get; }

        /// <summary>
        /// Gets the DPI scale on the X axis. When DPI is 96, <see cref="DpiScaleX"/> is 1. 
        /// </summary>
        public double DpiScaleX => DpiX / 96.0d;

        /// <summary>
        /// Gets the DPI scale on the Y axis. When DPI is 96, <see cref="DpiScaleY"/> is 1. On Windows Desktop, this value is always the same as <see cref="DpiScaleX"/>
        /// </summary>
        public double DpiScaleY => DpiY / 96.0d;

        /// <summary>
        /// Creates a DpiContext with the specified X and Y DPI, and relative top left coordinate. When translating to/from virutal coordinates, the points
        /// will be shifted by offset which allows you to specify a world view which is based on the location of your window, for example.
        /// See also the static methods for ways to instantiate this type.
        /// </summary>
        public DpiContext(int dpiX, int dpiY, int offsetX, int offsetY)
        {
            if (dpiX == 0 || dpiY == 0)
                throw new ArgumentException("DPI of '0' is invalid.");

            DpiX = dpiX;
            DpiY = dpiY;
            VirtualWorldOffsetX = offsetX;
            VirtualWorldOffsetY = offsetY;
        }

        /// <summary>
        /// Creates a DpiContext using the current transformation matrix of the specified visual. This can only be used if the CompositionTarget already exists, 
        /// so is likely to throw if used from within a window constructor before it is rendered.
        /// </summary>
        public static DpiContext FromVisual(Visual visual)
        {
            PresentationSource source = PresentationSource.FromVisual(visual);
            var dx = (int)(96.0d * source.CompositionTarget.TransformToDevice.M11);
            var dy = (int)(96.0d * source.CompositionTarget.TransformToDevice.M22);
            var vscr = System.Windows.Forms.SystemInformation.VirtualScreen;
            return new DpiContext(dx, dy, vscr.Left, vscr.Top);
        }

        /// <summary>
        /// Creates a DpiContext using the System DPI, which is based on the DPI of the primary display.
        /// </summary>
        public static DpiContext FromPrimaryScreen()
        {
            IntPtr dc = WinAPI.GetDC(IntPtr.Zero);
            int dx = WinAPI.GetDeviceCaps(dc, WinAPI.DEVICECAP.LOGPIXELSX);
            int dy = WinAPI.GetDeviceCaps(dc, WinAPI.DEVICECAP.LOGPIXELSY);
            WinAPI.ReleaseDC(IntPtr.Zero, dc);
            var vscr = System.Windows.Forms.SystemInformation.VirtualScreen;
            return new DpiContext(dx, dy, vscr.Left, vscr.Top);
        }

        private int v2sX(double virtualUnit) => ((int)Math.Round(virtualUnit * DpiScaleX)) + VirtualWorldOffsetX;
        private int v2sY(double virtualUnit) => ((int)Math.Round(virtualUnit * DpiScaleY)) + VirtualWorldOffsetY;
        private double s2vX(int screenUnit) => (screenUnit - VirtualWorldOffsetX) / DpiScaleX;
        private double s2vY(int screenUnit) => (screenUnit - VirtualWorldOffsetY) / DpiScaleY;

        /// <summary>
        /// Converts a fractional unit in virtual space to it's corresponding horizontal unit in standard screen space
        /// </summary>
        public int VirtualUnitToScreenX(double virtualUnit) => (int)Math.Round(virtualUnit * DpiScaleX);

        /// <summary>
        /// Converts a fractional unit in virtual space to it's corresponding vertical unit in standard screen space
        /// </summary>
        public int VirtualUnitToScreenY(double virtualUnit) => (int)Math.Round(virtualUnit * DpiScaleY);

        /// <summary>
        /// Converts a screen unit to it's corresponding horizontal unit in fractional virtual space
        /// </summary>
        public double ScreenToVirtualUnitX(int screenUnit) => screenUnit / DpiScaleX;

        /// <summary>
        /// Converts a screen unit to it's corresponding vertical unit in fractional virtual space
        /// </summary>
        public double ScreenToVirtualUnitY(int screenUnit) => screenUnit / DpiScaleY;

        public ScreenPoint VirtualPointToScreen(double x, double y) => new ScreenPoint(v2sX(x), v2sY(y));

        public ScreenPoint VirtualPointToScreen(Point virtualPoint) => VirtualPointToScreen(virtualPoint.X, virtualPoint.Y);

        public ScreenSize VirtualSizeToScreen(double w, double h) => new ScreenSize(VirtualUnitToScreenX(w), VirtualUnitToScreenY(h));

        public ScreenSize VirtualSizeToScreen(Size virtualSize) => VirtualSizeToScreen(virtualSize.Width, virtualSize.Height);

        public ScreenRect VirtualRectToScreen(double x, double y, double w, double h) => new ScreenRect(v2sX(x), v2sY(y), VirtualUnitToScreenX(w), VirtualUnitToScreenY(h));

        public ScreenRect VirtualRectToScreen(Rect virtualRect) => VirtualRectToScreen(virtualRect.X, virtualRect.Y, virtualRect.Width, virtualRect.Height);

        public Point ScreenPointToVirtual(int x, int y) => new Point(s2vX(x), s2vY(y));

        public Point ScreenPointToVirtual(ScreenPoint screenPoint) => ScreenPointToVirtual(screenPoint.X, screenPoint.Y);

        public Size ScreenSizeToVirtual(int w, int h) => new Size(ScreenToVirtualUnitX(w), ScreenToVirtualUnitY(h));

        public Size ScreenSizeToVirtual(ScreenSize screenSize) => ScreenSizeToVirtual(screenSize.Width, screenSize.Height);

        public Rect ScreenRectToVirtual(int x, int y, int w, int h) => new Rect(s2vX(x), s2vY(y), ScreenToVirtualUnitX(w), ScreenToVirtualUnitY(h));

        public Rect ScreenRectToVirtual(ScreenRect screenRect) => ScreenRectToVirtual(screenRect.Left, screenRect.Top, screenRect.Width, screenRect.Height);


        //public double WpfSnapToPixels(double wpfUnits)
        //{
        //    return Math.Round(wpfUnits * DpiZoom) / DpiZoom;
        //}

        //public double WpfSnapToPixelsCeil(double wpfUnits)
        //{
        //    return Math.Ceiling(wpfUnits * DpiZoom) / DpiZoom;
        //}

        //public double WpfSnapToPixelsFloor(double wpfUnits)
        //{
        //    return Math.Floor(wpfUnits * DpiZoom) / DpiZoom;
        //}
    }
}
