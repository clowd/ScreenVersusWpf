#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using D = System.Drawing;
using W = System.Windows;

namespace ScreenVersusWpf
{
    public struct ScreenRect : IEquatable<ScreenRect>
    {
        public int X { get => Left; set => Left = value; }
        public int Y { get => Top; set => Top = value; }
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Right { get { return Left + Width; } set { Width = value - Left; } }
        public int Bottom { get { return Top + Height; } set { Height = value - Top; } }
        public ScreenPoint TopLeft => new ScreenPoint(Left, Top);
        public ScreenPoint TopRight => new ScreenPoint(Right, Top);
        public ScreenPoint BottomRight => new ScreenPoint(Right, Bottom);
        public ScreenPoint BottomLeft => new ScreenPoint(Left, Bottom);

        public static ScreenRect Empty => new ScreenRect(0, 0, 0, 0);

        public ScreenRect(D.Rectangle rect) : this(rect.X, rect.Y, rect.Width, rect.Height) { }

        public ScreenRect(int x, int y, int width, int height)
        {
            Left = x;
            Top = y;
            Width = width;
            Height = height;
        }

        public override string ToString() => $"L={Left}, T={Top}, W={Width}, H={Height}";

        public static bool operator ==(ScreenRect rect1, ScreenRect rect2)
        {
            return rect1.Left == rect2.Left && rect1.Top == rect2.Top && rect1.Width == rect2.Width && rect1.Height == rect2.Height;
        }

        public static bool operator !=(ScreenRect rect1, ScreenRect rect2) => !(rect1 == rect2);

        public bool Equals(ScreenRect other) => this == other;

        public override bool Equals(object obj) => obj == null ? false : !(obj is ScreenRect) ? false : (this == (ScreenRect)obj);

        public override int GetHashCode() => unchecked(Left + 997 * (Top + 997 * (Width + 997 * Height)));

        public static implicit operator W.Int32Rect(ScreenRect rect) => new W.Int32Rect(rect.Left, rect.Top, rect.Width, rect.Height);
        public static implicit operator ScreenRect(W.Int32Rect rect) => new ScreenRect(rect.X, rect.Y, rect.Width, rect.Height);
        public static explicit operator D.Rectangle(ScreenRect rect) => new D.Rectangle(rect.Left, rect.Top, rect.Width, rect.Height);
        public static explicit operator ScreenRect(D.Rectangle rect) => new ScreenRect(rect.X, rect.Y, rect.Width, rect.Height);
        public static ScreenRect FromLTRB(int left, int top, int right, int bottom) => new ScreenRect(left, top, right - left, bottom - top);
        internal static ScreenRect FromLTRB(Sys.RECT rect) => FromLTRB(rect.left, rect.top, rect.right, rect.bottom);

        public W.Rect ToVisual(W.Media.Visual visual) => DpiContext.FromVisual(visual).ToWorldRect(this);
        public W.Rect ToScreen(ScreenInfo screen) => DpiContext.FromScreen(screen).ToWorldRect(this);
        public W.Rect ToScreen(IntPtr hMonitor) => DpiContext.FromScreen(hMonitor).ToWorldRect(this);
        public W.Rect ToPrimaryScreen() => DpiContext.FromPrimaryScreen().ToWorldRect(this);

        public bool Contains(ScreenPoint pt) => pt.X >= Left && pt.X < Right && pt.Y >= Top && pt.Y < Bottom;

        public bool IntersectsWith(ScreenRect rect)
        {
            // Touching ScreenRects do not intersect (different to WpfRect)
            return !IsEmpty() && !rect.IsEmpty() && Left < rect.Right && rect.Left < Right && Top < rect.Bottom && rect.Top < Bottom;
        }

        public bool IsEmpty() => Width == 0 && Height == 0;

        public ScreenRect Grow(int amount) => new ScreenRect(Left - amount, Top - amount, Width + 2 * amount, Height + 2 * amount);

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
    }
}
