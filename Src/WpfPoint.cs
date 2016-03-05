using System;

namespace ScreenVersusWpf
{
    public struct WpfPoint
    {
        public double X { get; set; }
        public double Y { get; set; }

        public WpfPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"X={X:0.####}, Y={Y:0.####}";
        }

        #region Equality

        public static bool operator ==(WpfPoint point1, WpfPoint point2)
        {
            return point1.X == point2.X && point1.Y == point2.Y;
        }

        public static bool operator !=(WpfPoint point1, WpfPoint point2)
        {
            return !(point1 == point2);
        }

        public bool Equals(WpfPoint other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj == null ? false : !(obj is WpfPoint) ? false : (this == (WpfPoint) obj);
        }

        public override int GetHashCode()
        {
            return unchecked(X.GetHashCode() + 9949 * Y.GetHashCode());
        }

        #endregion Equality

        #region Conversions

        public ScreenPoint ToScreenPoint()
        {
            return new ScreenPoint(
                x: (int) Math.Round(X * ScreenTools.DpiZoom),
                y: (int) Math.Round(Y * ScreenTools.DpiZoom)
            );
        }

        public WpfSize ToWpfSize()
        {
            return new WpfSize(X, Y);
        }

        #endregion

        #region Math

        public static WpfPoint operator -(WpfPoint point)
        {
            return new WpfPoint(-point.X, -point.Y);
        }

        public static WpfPoint operator +(WpfPoint point, double add)
        {
            return new WpfPoint(point.X + add, point.Y + add);
        }

        public static WpfPoint operator -(WpfPoint point, double sub)
        {
            return point + (-sub);
        }

        public static WpfPoint operator *(WpfPoint point, double mul)
        {
            return new WpfPoint(point.X * mul, point.Y * mul);
        }

        public static WpfPoint operator /(WpfPoint point, double div)
        {
            return new WpfPoint(point.X / div, point.Y / div);
        }

        public static WpfPoint operator +(WpfPoint point, WpfPoint add)
        {
            return new WpfPoint(point.X + add.X, point.Y + add.Y);
        }

        public static WpfPoint operator -(WpfPoint point, WpfPoint sub)
        {
            return point + (-sub);
        }

        public static WpfPoint operator +(WpfPoint point, WpfSize add)
        {
            return point + add.ToWpfPoint();
        }

        public static WpfPoint operator -(WpfPoint point, WpfSize sub)
        {
            return point + (-sub);
        }

        #endregion Math
    }
}
