using System;
using W = System.Windows;

namespace ScreenVersusWpf
{
    public struct WpfRect : IEquatable<WpfRect>
    {
        public double Left { get; set; }
        public double Top { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public double Right { get { return Left + Width; } set { Width = value - Left; } }
        public double Bottom { get { return Top + Height; } set { Height = value - Top; } }

        public static WpfRect Empty => new WpfRect(0, 0, 0, 0);

        public WpfRect(double left, double top, double width, double height)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }

        public WpfRect(W.Rect rect) : this(rect.Left, rect.Top, rect.Width, rect.Height) { }

        public override string ToString()
        {
            return $"L={Left:0.####}, T={Top:0.####}, W={Width:0.####}, H={Height:0.####}";
        }

        #region Equality

        public static bool operator ==(WpfRect rect1, WpfRect rect2)
        {
            return rect1.Left == rect2.Left && rect1.Top == rect2.Top && rect1.Width == rect2.Width && rect1.Height == rect2.Height;
        }

        public static bool operator !=(WpfRect rect1, WpfRect rect2)
        {
            return !(rect1 == rect2);
        }

        public bool Equals(WpfRect other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj == null ? false : !(obj is WpfRect) ? false : (this == (WpfRect)obj);
        }

        public override int GetHashCode()
        {
            return unchecked(Left.GetHashCode() + 997 * (Top.GetHashCode() + 997 * (Width.GetHashCode() + 997 * Height.GetHashCode())));
        }

        #endregion Equality

        #region Conversions

        public static implicit operator W.Rect(WpfRect rect)
        {
            return new W.Rect(rect.Left, rect.Top, rect.Width, rect.Height);
        }

        public ScreenRect ToScreenRect()
        {
            return new ScreenRect(
                x: (int)Math.Round(Left * ScreenTools.DpiZoom),
                y: (int)Math.Round(Top * ScreenTools.DpiZoom),
                width: (int)Math.Round(Width * ScreenTools.DpiZoom),
                height: (int)Math.Round(Height * ScreenTools.DpiZoom)
            );
        }

        #endregion Conversions

        #region Utility

        public bool Contains(WpfPoint pt)
        {
            return pt.X >= Left && pt.X < Right && pt.Y >= Top && pt.Y < Bottom;
        }

        public bool IntersectsWith(WpfRect rect)
        {
            // Touching WpfRects intersect (different to ScreenRect)
            return !IsEmpty() && !rect.IsEmpty() && Left <= rect.Right && rect.Left <= Right && Top <= rect.Bottom && rect.Top <= Bottom;
        }

        public bool IsEmpty()
        {
            return Width == 0 && Height == 0;
        }

        public WpfRect Grow(double amount)
        {
            return new WpfRect(Left - amount, Top - amount, Width + 2 * amount, Height + 2 * amount);
        }

        public WpfRect Intersect(WpfRect rect)
        {
            var result = new WpfRect();
            result.Left = Math.Max(Left, rect.Left);
            result.Top = Math.Max(Top, rect.Top);
            result.Right = Math.Min(Left + Width, rect.Left + rect.Width);
            result.Bottom = Math.Min(Top + Height, rect.Top + rect.Height);
            if (result.Width < 0 || result.Height < 0)
                return WpfRect.Empty;
            return result;
        }

        #endregion Utility
    }
}
