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
        public int WorldOffsetX { get; }

        /// <summary>
        /// Gets the amount, in screen units, that a unit will be translated when converting to and from screen coordinates
        /// </summary>
        public int WorldOffsetY { get; }

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
            if (dpiX <= 0 || dpiY <= 0)
                throw new ArgumentException("DPI must be greater than zero.");

            if (dpiX != dpiY)
                throw new ArgumentException("DPI X must equal DPI Y.");

            // TODO THROW IF PROCESS IS NOT DPI AWARE

            DpiX = dpiX;
            DpiY = dpiY;
            WorldOffsetX = offsetX;
            WorldOffsetY = offsetY;
        }

        enum PossibleIdeaForOriginConfiguration
        {
            PrimaryScreenTopLeft,
            VirtualScreenTopLeft,
            WindowTopLeft,
        }

        /// <summary>
        /// Creates a DpiContext using the current transformation matrix of the specified visual. This can only be used if the CompositionTarget already exists, 
        /// so is likely to throw if used from within a window constructor before it is rendered.
        /// </summary>
        public static DpiContext FromVisual(Visual visual, PossibleIdeaForOriginConfiguration origin = PossibleIdeaForOriginConfiguration.VirtualScreenTopLeft)
        {
            PresentationSource source = PresentationSource.FromVisual(visual);
            var dx = (int)(Math.Round(96.0d * source.CompositionTarget.TransformToDevice.M11));
            var dy = (int)(Math.Round(96.0d * source.CompositionTarget.TransformToDevice.M22));
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

        private Func<double, double> GetRoundingFn(WorldRoundingMode roundingMode)
        {
            switch (roundingMode)
            {
                case WorldRoundingMode.Midpoint: return Math.Round;
                case WorldRoundingMode.Floor: return Math.Floor;
                case WorldRoundingMode.Ceiling: return Math.Ceiling;
                default: throw new ArgumentOutOfRangeException(nameof(roundingMode));
            }
        }

        public int ToScreenWH(double worldWH, WorldRoundingMode roundingMode = WorldRoundingMode.Midpoint) => (int)GetRoundingFn(roundingMode)(worldWH * DpiScaleX);

        public int ToScreenX(double worldX, WorldRoundingMode roundingMode = WorldRoundingMode.Midpoint) => ToScreenWH(worldX, roundingMode) + WorldOffsetX;

        public int ToScreenY(double worldY, WorldRoundingMode roundingMode = WorldRoundingMode.Midpoint) => ToScreenWH(worldY, roundingMode) + WorldOffsetY;

        public ScreenPoint ToScreenPoint(double worldX, double worldY, WorldRoundingMode roundingMode = WorldRoundingMode.Midpoint) => new ScreenPoint(ToScreenX(worldX, roundingMode), ToScreenY(worldY, roundingMode));

        public ScreenPoint ToScreenPoint(Point worldPoint, WorldRoundingMode roundingMode = WorldRoundingMode.Midpoint) => ToScreenPoint(worldPoint.X, worldPoint.Y, roundingMode);

        public ScreenSize ToScreenSize(double worldW, double worldH, WorldRoundingMode roundingMode = WorldRoundingMode.Midpoint) => new ScreenSize(ToScreenWH(worldW, roundingMode), ToScreenWH(worldH, roundingMode));

        public ScreenSize ToScreenSize(Size worldSize, WorldRoundingMode roundingMode = WorldRoundingMode.Midpoint) => ToScreenSize(worldSize.Width, worldSize.Height, roundingMode);

        public ScreenRect ToScreenRect(double worldX, double worldY, double worldW, double worldH, WorldRoundingMode roundingMode = WorldRoundingMode.Midpoint) => new ScreenRect(ToScreenX(worldX, roundingMode), ToScreenY(worldY, roundingMode), ToScreenWH(worldW, roundingMode), ToScreenWH(worldH, roundingMode));

        public ScreenRect ToScreenRect(Rect worldRect, WorldRoundingMode roundingMode = WorldRoundingMode.Midpoint) => ToScreenRect(worldRect.X, worldRect.Y, worldRect.Width, worldRect.Height, roundingMode);

        public double ToWorldWH(int screenWH) => screenWH / DpiScaleX;

        public double ToWorldX(int screenX) => ToWorldWH(screenX - WorldOffsetX);

        public double ToWorldY(int screenY) => ToWorldWH(screenY - WorldOffsetY);

        public Point ToWorldPoint(int screenX, int screenY) => new Point(ToWorldX(screenX), ToWorldY(screenY));

        public Point ToWorldPoint(ScreenPoint screenPoint) => ToWorldPoint(screenPoint.X, screenPoint.Y);

        public Size ToWorldSize(int screenW, int screenH) => new Size(ToWorldWH(screenW), ToWorldWH(screenH));

        public Size ToWorldSize(ScreenSize screenSize) => ToWorldSize(screenSize.Width, screenSize.Height);

        public Rect ToWorldRect(int screenX, int screenY, int screenW, int screenH) => new Rect(ToWorldX(screenX), ToWorldY(screenY), ToWorldWH(screenW), ToWorldWH(screenH));

        public Rect ToWorldRect(ScreenRect screenRect) => ToWorldRect(screenRect.Left, screenRect.Top, screenRect.Width, screenRect.Height);

        public double Round(double worldWH, WorldRoundingMode roundingMode = WorldRoundingMode.Midpoint) => ToWorldWH(ToScreenWH(worldWH, roundingMode));

        public Point Round(Point worldPoint, WorldRoundingMode roundingMode = WorldRoundingMode.Midpoint) => ToWorldPoint(ToScreenPoint(worldPoint, roundingMode));

        public Rect Round(Rect worldRect, WorldRoundingMode roundingMode = WorldRoundingMode.Midpoint) => ToWorldRect(ToScreenRect(worldRect, roundingMode));

        public Size Round(Size worldSize, WorldRoundingMode roundingMode = WorldRoundingMode.Midpoint) => ToWorldSize(ToScreenSize(worldSize, roundingMode));
    }
}
