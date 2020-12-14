#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using D = System.Drawing;
using W = System.Windows;

namespace ScreenVersusWpf
{
    public struct ScreenPoint : IEquatable<ScreenPoint>
    {
        public int X { get; set; }
        public int Y { get; set; }

        public ScreenPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString() => $"X={X}, Y={Y}";
        public static bool operator ==(ScreenPoint point1, ScreenPoint point2) => point1.X == point2.X && point1.Y == point2.Y;
        public static bool operator !=(ScreenPoint point1, ScreenPoint point2) => !(point1 == point2);
        public bool Equals(ScreenPoint other) => this == other;
        public override bool Equals(object obj)
        {
            if (obj is ScreenPoint pt)
                return Equals(pt);
            return false;
        }

        public override int GetHashCode() => unchecked(X + 9949 * Y);

        public static explicit operator D.Point(ScreenPoint pt) => new D.Point(pt.X, pt.Y);
        public static explicit operator ScreenPoint(D.Point pt) => new ScreenPoint(pt.X, pt.Y);

        public W.Point ToVisual(W.Media.Visual visual) => DpiContext.FromVisual(visual).ToWorldPoint(this);
        public W.Point ToScreen(ScreenInfo screen) => DpiContext.FromScreen(screen).ToWorldPoint(this);
        public W.Point ToScreen(IntPtr hMonitor) => DpiContext.FromScreen(hMonitor).ToWorldPoint(this);
        public W.Point ToPrimaryScreen() => DpiContext.FromPrimaryScreen().ToWorldPoint(this);

        public static ScreenPoint operator -(ScreenPoint point) => new ScreenPoint(-point.X, -point.Y);
        public static ScreenPoint operator +(ScreenPoint point, int add) => new ScreenPoint(point.X + add, point.Y + add);
        public static ScreenPoint operator -(ScreenPoint point, int sub) => point + (-sub);
        public static ScreenPoint operator *(ScreenPoint point, int mul) => new ScreenPoint(point.X * mul, point.Y * mul);
        public static ScreenPoint operator /(ScreenPoint point, int div) => new ScreenPoint(point.X / div, point.Y / div);
        public static ScreenPoint operator +(ScreenPoint point, ScreenPoint add) => new ScreenPoint(point.X + add.X, point.Y + add.Y);
        public static ScreenPoint operator -(ScreenPoint point, ScreenPoint sub) => point + (-sub);
    }
}
