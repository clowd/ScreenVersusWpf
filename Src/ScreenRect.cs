using System;
using D = System.Drawing;
using W = System.Windows;

namespace ScreenVersusWpf
{
    public struct ScreenRect : IEquatable<ScreenRect>
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public int Right { get { return Left + Width; } set { Width = value - Left; } }
        public int Bottom { get { return Top + Height; } set { Height = value - Top; } }

        public static ScreenRect Empty => new ScreenRect(0, 0, 0, 0);

        public ScreenRect(int x, int y, int width, int height)
        {
            Left = x;
            Top = y;
            Width = width;
            Height = height;
        }

        public override string ToString()
        {
            return $"L={Left}, T={Top}, W={Width}, H={Height}";
        }

        #region Equality

        public static bool operator ==(ScreenRect rect1, ScreenRect rect2)
        {
            return rect1.Left == rect2.Left && rect1.Top == rect2.Top && rect1.Width == rect2.Width && rect1.Height == rect2.Height;
        }

        public static bool operator !=(ScreenRect rect1, ScreenRect rect2)
        {
            return !(rect1 == rect2);
        }

        public bool Equals(ScreenRect other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj == null ? false : !(obj is ScreenRect) ? false : (this == (ScreenRect) obj);
        }

        public override int GetHashCode()
        {
            return unchecked(Left + 997 * (Top + 997 * (Width + 997 * Height)));
        }

        #endregion

        #region Conversions

        public static implicit operator W.Int32Rect(ScreenRect rect)
        {
            return new W.Int32Rect(rect.Left, rect.Top, rect.Width, rect.Height);
        }

        public WpfRect ToWpfRect()
        {
            return new WpfRect(
                Left / ScreenTools.DpiZoom,
                Top / ScreenTools.DpiZoom,
                Width / ScreenTools.DpiZoom,
                Height / ScreenTools.DpiZoom
            );
        }

        public static ScreenRect FromSystem(D.Rectangle rect)
        {
            return new ScreenRect(rect.X - ScreenTools.VirtualScreenSystemLeft, rect.Y - ScreenTools.VirtualScreenSystemTop, rect.Width, rect.Height);
        }

        public D.Rectangle ToSystem()
        {
            return new D.Rectangle(Left + ScreenTools.VirtualScreenSystemLeft, Top + ScreenTools.VirtualScreenSystemTop, Width, Height);
        }

        #endregion

        #region Utility

        public bool Contains(ScreenPoint pt)
        {
            return pt.X >= Left && pt.X < Right && pt.Y >= Top && pt.Y < Bottom;
        }

        public bool IntersectsWith(ScreenRect rect)
        {
            // Touching ScreenRects do not intersect (different to WpfRect)
            return !IsEmpty() && !rect.IsEmpty() && Left < rect.Right && rect.Left < Right && Top < rect.Bottom && rect.Top < Bottom;
        }

        public bool IsEmpty()
        {
            return Width == 0 && Height == 0;
        }

        public ScreenRect Grow(int amount)
        {
            return new ScreenRect(Left - amount, Top - amount, Width + 2 * amount, Height + 2 * amount);
        }

        public ScreenRect Intersect(ScreenRect rect)
        {
            var result = new ScreenRect();
            result.Left = Math.Max(Left, rect.Left);
            result.Top = Math.Max(Top, rect.Top);
            result.Right = Math.Min(Left + Width, rect.Left + rect.Width);
            result.Bottom = Math.Min(Top + Height, rect.Top + rect.Height);
            if (result.Width < 0 || result.Height < 0)
                return ScreenRect.Empty;
            return result;
        }

        #endregion
    }
}
