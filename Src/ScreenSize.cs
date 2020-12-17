#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using D = System.Drawing;
using W = System.Windows;

namespace ScreenVersusWpf
{
    public struct ScreenSize : IScreenStruct<ScreenSize, W.Size>
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public ScreenSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public override string ToString() => $"W={Width}, H={Height}";
        public static bool operator ==(ScreenSize size1, ScreenSize size2) => size1.Width == size2.Width && size1.Height == size2.Height;
        public static bool operator !=(ScreenSize size1, ScreenSize size2) => !(size1 == size2);
        public bool Equals(ScreenSize other) => this == other;
        public override bool Equals(object obj)
        {
            if (obj is ScreenSize sz)
                return Equals(sz);
            return false;
        }

        public override int GetHashCode() => unchecked(Width + 9949 * Height);

        public static explicit operator D.Size(ScreenSize sz) => new D.Size(sz.Width, sz.Height);
        public static explicit operator ScreenSize(D.Size sz) => new ScreenSize(sz.Width, sz.Height);

        //public W.Size ToVisual(W.Media.Visual visual) => DpiContext.FromVisual(visual).ToWorldSize(this);
        //public W.Size ToDisplay(DisplayInfo display) => DpiContext.FromDisplay(display).ToWorldSize(this);
        //public W.Size ToDisplay(IntPtr hMonitor) => DpiContext.FromDisplay(hMonitor).ToWorldSize(this);
        //public W.Size ToPrimaryDisplay() => DpiContext.FromPrimaryDisplay().ToWorldSize(this);

        public static ScreenSize operator -(ScreenSize size) => new ScreenSize(-size.Width, -size.Height);
        public static ScreenSize operator +(ScreenSize size, int add) => new ScreenSize(size.Width + add, size.Height + add);
        public static ScreenSize operator -(ScreenSize size, int sub) => size + (-sub);
        public static ScreenSize operator *(ScreenSize size, int mul) => new ScreenSize(size.Width * mul, size.Height * mul);
        public static ScreenSize operator /(ScreenSize size, int div) => new ScreenSize(size.Width / div, size.Height / div);
        public static ScreenSize operator +(ScreenSize size, ScreenSize add) => new ScreenSize(size.Width + add.Width, size.Height + add.Height);
        public static ScreenSize operator -(ScreenSize size, ScreenSize sub) => size + (-sub);
    }
}
