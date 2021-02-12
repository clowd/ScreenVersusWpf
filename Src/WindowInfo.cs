using ScreenVersusWpf.Interop;
using System;
using System.Windows;
using System.Windows.Interop;

namespace ScreenVersusWpf
{
    /// <summary>
    /// Represents a single native window with DPI correct bounds
    /// </summary>
    public class WindowInfo : IEquatable<WindowInfo>
    {
        private readonly IntPtr _hWnd;

        private WindowInfo(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
                throw new ArgumentNullException(nameof(hWnd));
            _hWnd = hWnd;
        }

        /// <summary>
        /// Creates a new instance of WindowInfo from the specified WPF window
        /// </summary>
        public static WindowInfo FromWindow(Window hWnd) => FromWindow(new WindowInteropHelper(hWnd).Handle);

        /// <summary>
        /// Creates a new instance of WindowInfo from the specified WPF window, and if set, also ensures the window handle has been created
        /// </summary>
        public static WindowInfo FromWindow(Window hWnd, bool ensureHandle)
            => ensureHandle ? FromWindow(new WindowInteropHelper(hWnd).EnsureHandle()) : FromWindow(hWnd);

        /// <summary>
        /// Creates a new instance of WindowInfo from the specified window handle
        /// </summary>
        public static WindowInfo FromWindow(IntPtr hWnd) => new WindowInfo(hWnd);

        /// <summary>
        /// Gets the native handle of this window
        /// </summary>
        public IntPtr Handle => _hWnd;

        /// <summary>
        /// Gets the window title / caption for this window
        /// </summary>
        public string Caption => WinAPI.GetWindowCaption(_hWnd);

        /// <summary>
        /// Gets or sets the outer bounds of the window. This includes the non-client area (title bar, scroll bars, resize handles, etc), 
        /// and as of Windows Vista and later, this also includes the area outside of the window occupied by the drop shadow.
        /// See <see cref="RenderArea"/> for window bounds which exclude the drop shadow.
        /// </summary>
        public ScreenRect Bounds
        {
            get => WinAPI.GetWindowRect(_hWnd);
            set => WinAPI.SetWindowPos(_hWnd, SWP_HWND.HWND_TOP, value.X, value.Y, value.Width, value.Height, SWP.ASYNCWINDOWPOS | SWP.NOZORDER | SWP.NOACTIVATE);
        }

        /// <summary>
        /// Gets the client area of this window, which is typically the content bounds excluding any shadow, title bar, resize handles, and scroll bars.
        /// </summary>
        public ScreenRect ClientArea => WinAPI.GetWindowClientRect(_hWnd);

        /// <summary>
        /// Gets the render area of this window. On Windows XP and earlier, this is identical to <see cref="Bounds"/>. On Windows Vista and
        /// later, this value is the bounds of the window excluding the drop shadow rendered by the DWM.
        /// </summary>
        public ScreenRect RenderArea => WinAPI.GetWindowExtendedBoundsRect(_hWnd);

        /// <summary>
        /// Gets a value indicating whether the specified object is logically equal to this object.
        /// </summary>
        public bool Equals(WindowInfo other) => _hWnd == other._hWnd;

        /// <summary>
        /// Gets a value indicating whether the specified object is logically equal to this object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is WindowInfo wnd)
                return Equals(wnd);
            return false;
        }

        /// <summary>
        /// Computes and retrieves a hash code for an object.
        /// </summary>
        public override int GetHashCode() => unchecked((int)_hWnd);

        /// <summary>
        /// Retrieves a human-readable string identifying this window's name and it's position
        /// </summary>
        public override string ToString()
        {
            var handle = IntPtr.Size == 4 ? _hWnd.ToInt32().ToString("X8") : _hWnd.ToInt64().ToString("X8");
            return $"Window 0x{handle} - {Caption}, {Bounds}";
        }
    }
}
