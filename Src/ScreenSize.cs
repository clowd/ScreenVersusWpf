using System;

namespace ScreenVersusWpf
{
    public struct ScreenSize : IEquatable<ScreenSize>
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public ScreenSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public override string ToString()
        {
            return $"W={Width}, H={Height}";
        }

        #region Equality

        public static bool operator ==(ScreenSize size1, ScreenSize size2)
        {
            return size1.Width == size2.Width && size1.Height == size2.Height;
        }

        public static bool operator !=(ScreenSize size1, ScreenSize size2)
        {
            return !(size1 == size2);
        }

        public bool Equals(ScreenSize other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj == null ? false : !(obj is ScreenSize) ? false : (this == (ScreenSize)obj);
        }

        public override int GetHashCode()
        {
            return unchecked(Width + 9949 * Height);
        }

        #endregion Equality

        //#region Conversions

        //public WpfSize ToWpfSize()
        //{
        //    return new WpfSize(
        //        Width / ScreenTools.DpiZoom,
        //        Height / ScreenTools.DpiZoom
        //    );
        //}

        //public ScreenPoint ToScreenPoint()
        //{
        //    return new ScreenPoint(Width, Height);
        //}

        //#endregion Conversions

        #region Math

        public static ScreenSize operator -(ScreenSize size)
        {
            return new ScreenSize(-size.Width, -size.Height);
        }

        public static ScreenSize operator +(ScreenSize size, int add)
        {
            return new ScreenSize(size.Width + add, size.Height + add);
        }

        public static ScreenSize operator -(ScreenSize size, int sub)
        {
            return size + (-sub);
        }

        public static ScreenSize operator *(ScreenSize size, int mul)
        {
            return new ScreenSize(size.Width * mul, size.Height * mul);
        }

        public static ScreenSize operator /(ScreenSize size, int div)
        {
            return new ScreenSize(size.Width / div, size.Height / div);
        }

        public static ScreenSize operator +(ScreenSize size, ScreenSize add)
        {
            return new ScreenSize(size.Width + add.Width, size.Height + add.Height);
        }

        public static ScreenSize operator -(ScreenSize size, ScreenSize sub)
        {
            return size + (-sub);
        }

        //public static ScreenSize operator +(ScreenSize size, ScreenPoint add)
        //{
        //    return size + add.ToScreenSize();
        //}

        //public static ScreenSize operator -(ScreenSize size, ScreenPoint sub)
        //{
        //    return size + (-sub);
        //}

        #endregion
    }
}
