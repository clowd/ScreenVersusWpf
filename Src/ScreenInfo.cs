using F = System.Windows.Forms;

namespace ScreenVersusWpf
{
    public class ScreenInfo
    {
        /// <summary>Constructs a new instance for a physical screen.</summary>
        internal ScreenInfo(F.Screen screen)
        {
            IsPrimary = screen.Primary;
            IsVirtual = false;
            Bounds = ScreenRect.FromSystem(screen.Bounds);
            WorkingArea = ScreenRect.FromSystem(screen.WorkingArea);
            DeviceName = screen.DeviceName;
        }

        /// <summary>Constructs a new instance for the virtual screen with the specified size.</summary>
        internal ScreenInfo(int virtualScreenWidth, int virtualScreenHeight)
        {
            IsPrimary = false;
            IsVirtual = true;
            Bounds = new ScreenRect(0, 0, virtualScreenWidth, virtualScreenHeight);
            WorkingArea = Bounds;
            DeviceName = "Virtual Screen";
        }

        public bool IsPrimary { get; }
        public bool IsVirtual { get; }
        public ScreenRect Bounds { get; }
        public ScreenRect WorkingArea { get; }
        public string DeviceName { get; }

        public override string ToString()
        {
            if (IsVirtual)
                return $"{Bounds.Width}x{Bounds.Height}, virtual screen";
            var workingArea = WorkingArea == Bounds ? "" : $", working area {WorkingArea.Width}x{WorkingArea.Height} at ({WorkingArea.Left}, {WorkingArea.Top})";
            return $"{Bounds.Width}x{Bounds.Height} at ({Bounds.Left}, {Bounds.Top}){(IsPrimary ? ", primary" : "")}{workingArea}, {DeviceName}";
        }
    }
}
