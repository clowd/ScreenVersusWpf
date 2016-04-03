using System;
using D = System.Drawing;

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

        public override string ToString()
        {
            return $"X={X}, Y={Y}";
        }

        #region Equality

        public static bool operator ==(ScreenPoint point1, ScreenPoint point2)
        {
            return point1.X == point2.X && point1.Y == point2.Y;
        }

        public static bool operator !=(ScreenPoint point1, ScreenPoint point2)
        {
            return !(point1 == point2);
        }

        public bool Equals(ScreenPoint other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj == null ? false : !(obj is ScreenPoint) ? false : (this == (ScreenPoint) obj);
        }

        public override int GetHashCode()
        {
            return unchecked(X + 9949 * Y);
        }

        #endregion Equality

        #region Conversions

        public WpfPoint ToWpfPoint()
        {
            return new WpfPoint(
                X / ScreenTools.DpiZoom,
                Y / ScreenTools.DpiZoom
            );
        }

        public ScreenSize ToScreenSize()
        {
            return new ScreenSize(X, Y);
        }

        public static ScreenPoint FromSystem(D.Point point)
        {
            return new ScreenPoint(point.X - ScreenTools.VirtualScreenSystemLeft, point.Y - ScreenTools.VirtualScreenSystemTop);
        }

        public D.Point ToSystem()
        {
            return new D.Point(X + ScreenTools.VirtualScreenSystemLeft, Y + ScreenTools.VirtualScreenSystemTop);
        }

        #endregion Conversions

        #region Math

        public static ScreenPoint operator -(ScreenPoint point)
        {
            return new ScreenPoint(-point.X, -point.Y);
        }

        public static ScreenPoint operator +(ScreenPoint point, int add)
        {
            return new ScreenPoint(point.X + add, point.Y + add);
        }

        public static ScreenPoint operator -(ScreenPoint point, int sub)
        {
            return point + (-sub);
        }

        public static ScreenPoint operator *(ScreenPoint point, int mul)
        {
            return new ScreenPoint(point.X * mul, point.Y * mul);
        }

        public static ScreenPoint operator /(ScreenPoint point, int div)
        {
            return new ScreenPoint(point.X / div, point.Y / div);
        }

        public static ScreenPoint operator +(ScreenPoint point, ScreenPoint add)
        {
            return new ScreenPoint(point.X + add.X, point.Y + add.Y);
        }

        public static ScreenPoint operator -(ScreenPoint point, ScreenPoint sub)
        {
            return point + (-sub);
        }

        public static ScreenPoint operator +(ScreenPoint point, ScreenSize add)
        {
            return point + add.ToScreenPoint();
        }

        public static ScreenPoint operator -(ScreenPoint point, ScreenSize sub)
        {
            return point + (-sub);
        }

        #endregion Math
    }
}
