using System;

namespace ScreenVersusWpf
{
    public struct WpfSize
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public WpfSize(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public override string ToString()
        {
            return $"W={Width:0.####}, H={Height:0.####}";
        }

        #region Equality

        public static bool operator ==(WpfSize size1, WpfSize size2)
        {
            return size1.Width == size2.Width && size1.Height == size2.Height;
        }

        public static bool operator !=(WpfSize size1, WpfSize size2)
        {
            return !(size1 == size2);
        }

        public bool Equals(WpfSize other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj == null ? false : !(obj is WpfSize) ? false : (this == (WpfSize) obj);
        }

        public override int GetHashCode()
        {
            return unchecked(Width.GetHashCode() + 9949 * Height.GetHashCode());
        }

        #endregion Equality

        #region Conversions

        public ScreenSize ToScreenSize()
        {
            return new ScreenSize(
                width: (int) Math.Round(Width * ScreenTools.DpiZoom),
                height: (int) Math.Round(Height * ScreenTools.DpiZoom)
            );
        }

        public WpfPoint ToWpfPoint()
        {
            return new WpfPoint(Width, Height);
        }

        #endregion

        #region Math

        public static WpfSize operator -(WpfSize size)
        {
            return new WpfSize(-size.Width, -size.Height);
        }

        public static WpfSize operator +(WpfSize size, double add)
        {
            return new WpfSize(size.Width + add, size.Height + add);
        }

        public static WpfSize operator -(WpfSize size, double sub)
        {
            return size + (-sub);
        }

        public static WpfSize operator *(WpfSize size, double mul)
        {
            return new WpfSize(size.Width * mul, size.Height * mul);
        }

        public static WpfSize operator /(WpfSize size, double div)
        {
            return new WpfSize(size.Width / div, size.Height / div);
        }

        public static WpfSize operator +(WpfSize size, WpfSize add)
        {
            return new WpfSize(size.Width + add.Width, size.Height + add.Height);
        }

        public static WpfSize operator -(WpfSize size, WpfSize sub)
        {
            return size + (-sub);
        }

        public static WpfSize operator +(WpfSize size, WpfPoint add)
        {
            return size + add.ToWpfSize();
        }

        public static WpfSize operator -(WpfSize size, WpfPoint sub)
        {
            return size + (-sub);
        }

        #endregion Math
    }
}
