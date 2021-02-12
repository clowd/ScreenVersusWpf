using ScreenVersusWpf.Interop;
using System;

namespace ScreenVersusWpf
{
    public class DisplayDpiContext : DpiContext
    {
        private readonly IntPtr hMonitor;

        public override int WorldOffsetX => 0;

        public override int WorldOffsetY => 0;

        public override int DpiX => WinAPI.GetDpiForMonitor(hMonitor).dy;

        public override int DpiY => WinAPI.GetDpiForMonitor(hMonitor).dx;

        public DisplayDpiContext(IntPtr hMonitor)
        {
            this.hMonitor = hMonitor;
        }
    }
}
