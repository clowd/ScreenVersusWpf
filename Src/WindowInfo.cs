using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ScreenVersusWpf
{
    /// <summary>
    /// Represents a single native window with DPI correct bounds
    /// </summary>
    public class WindowInfo : IEquatable<WindowInfo>
    {
        private readonly IntPtr _hWnd;

        static WindowInfo()
        {
            Sys.EnsureProcessDPIAwareness();
        }

        private WindowInfo(IntPtr hWnd)
        {
            _hWnd = hWnd;
        }

        public static WindowInfo FromWindow(System.Windows.Window window)
        {
            var handle = new System.Windows.Interop.WindowInteropHelper(window).Handle;
            return FromWindow(handle);
        }

        public static WindowInfo FromWindow(IntPtr hWnd)
        {
            return new WindowInfo(hWnd);
        }

        /// <summary>
        /// Gets the native handle of this window
        /// </summary>
        public IntPtr Handle => _hWnd;

        /// <summary>
        /// Gets the window title / caption for this window
        /// </summary>
        public string Caption
        {
            get
            {
                int length = Sys.GetWindowTextLength(_hWnd);
                if (length == 0) return String.Empty;
                StringBuilder builder = new StringBuilder(length);
                Sys.GetWindowText(_hWnd, builder, length + 1);
                return builder.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the outer bounds of the window. This includes the non-client area (title bar, scroll bars, resize handles, etc), 
        /// and as of Windows Vista and later, this also includes the area outside of the window occupied by the drop shadow.
        /// See <see cref="RenderArea"/> for window bounds which exclude the drop shadow.
        /// </summary>
        public ScreenRect Bounds
        {
            get
            {
                if (!Sys.GetWindowRect(_hWnd, out var rect))
                    throw new Win32Exception();
                return ScreenRect.FromLTRB(rect);
            }
            set
            {
                Sys.SetWindowPos(_hWnd, Sys.SWP_HWND.HWND_TOP, value.X, value.Y, value.Width, value.Height, Sys.SWP.ASYNCWINDOWPOS | Sys.SWP.NOZORDER | Sys.SWP.NOACTIVATE);
            }
        }

        /// <summary>
        /// Gets the client area of this window, which is typiaclly the content bounds excluding any shadow, title bar, resize handles, and scroll bars.
        /// </summary>
        public ScreenRect ClientArea
        {
            get
            {
                if (!Sys.GetClientRect(_hWnd, out var rect))
                    throw new Win32Exception();
                return ScreenRect.FromLTRB(rect);
            }
        }

        /// <summary>
        /// Gets the render area of this window. On Windows XP and earlier, this is identical to <see cref="Bounds"/>. On Windows Vista and
        /// later, this value is the bounds of the window excluding the drop shadow rendered by the DWM.
        /// </summary>
        public ScreenRect RenderArea
        {
            get
            {
                try
                {
                    if (Sys.IsWindowsVistaOrLater && 0 == Sys.DwmIsCompositionEnabled(out var dwmIsEnabled) && dwmIsEnabled)
                    {
                        var result = Sys.DwmGetWindowAttribute(_hWnd, Sys.DWMWINDOWATTRIBUTE.DWMWA_EXTENDED_FRAME_BOUNDS, out var rect, Marshal.SizeOf(typeof(Sys.RECT)));
                        if (result != 0)
                            throw new Win32Exception();
                        return ScreenRect.FromLTRB(rect);
                    }
                }
                catch (DllNotFoundException) { }

                // fallback to old style calls if we can't query the DWM
                return Bounds;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the specified object is logically equal to this object.
        /// </summary>
        public bool Equals(WindowInfo other)
        {
            return _hWnd == other._hWnd;
        }

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
        public override int GetHashCode()
        {
            return unchecked((int)_hWnd);
        }

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
