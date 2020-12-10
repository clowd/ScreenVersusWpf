﻿using System;
using System.Runtime.InteropServices;
using D = System.Drawing;

// Large parts of this file have been sourced from caesay\Clowd

namespace ScreenVersusWpf
{
    public static class WinAPI
    {
        #region Functions

        /// <summary>
        ///     Retrieves the cursor's position, in screen coordinates.</summary>
        /// <param name="lpPoint">
        ///     A pointer to a POINT structure that receives the screen coordinates of the cursor.</param>
        /// <returns>
        ///     Returns nonzero if successful or zero otherwise. To get extended error information, call GetLastError.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("gdi32.dll")]
        public static extern int GetDeviceCaps(IntPtr hdc, DEVICECAP nIndex);

        [DllImport("user32.dll")]
        public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRectangle);

        #endregion Functions

        #region Enums / constants

        public enum MonitorOptions : uint
        {
            MONITOR_DEFAULTTONULL = 0x00000000,
            MONITOR_DEFAULTTOPRIMARY = 0x00000001,
            MONITOR_DEFAULTTONEAREST = 0x00000002
        }

        public enum DEVICECAP
        {
            LOGPIXELSX = 88,
            LOGPIXELSY = 90,
            VERTRES = 10,
            DESKTOPVERTRES = 117,
            DESKTOPHORZRES = 118,
        }

        #endregion Enums / constants

        #region Structs

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;

            public static implicit operator D.Point(POINT pt) { return new D.Point(pt.x, pt.y); }
            public static implicit operator POINT(D.Point pt) { return new POINT { x = pt.X, y = pt.Y }; }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public bool HasSize()
            {
                return right - left > 0 && bottom - top > 0;
            }

            public static implicit operator D.Rectangle(RECT rect)
            {
                return new D.Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
            }

            public static implicit operator RECT(D.Rectangle rect)
            {
                return new RECT
                {
                    left = rect.X,
                    top = rect.Y,
                    right = rect.X + rect.Width,
                    bottom = rect.Y + rect.Height
                };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MONITORINFO
        {
            /// <summary>The size of the structure, in bytes.</summary>
            public uint cbSize;

            /// <summary>
            ///     A RECT structure that specifies the display monitor rectangle, expressed in virtual-screen coordinates.
            ///     Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be
            ///     negative values.</summary>
            public RECT rcMonitor;

            /// <summary>
            ///     A RECT structure that specifies the work area rectangle of the display monitor, expressed in
            ///     virtual-screen coordinates. Note that if the monitor is not the primary display monitor, some of the
            ///     rectangle's coordinates may be negative values.</summary>
            public RECT rcWork;

            /// <summary>A set of flags that represent attributes of the display monitor.</summary>
            public uint dwFlags;
        }

        #endregion Structs
    }
}
