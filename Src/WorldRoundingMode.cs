using System;

namespace ScreenVersusWpf
{
    /// <summary>
    /// Specifies which rounding mode to use when converting from a floating-point world coordinate to an integer screen coordinate.
    /// </summary>
    public enum WorldRoundingMode
    {
        /// <summary>
        /// Round world coordinates using Math.Round(double)
        /// </summary>
        Midpoint = 0,
        /// <summary>
        /// Round world coordinates using Math.Ceiling(double)
        /// </summary>
        Ceiling = 1,
        /// <summary>
        /// Round world coordinates using Math.Floor(double)
        /// </summary>
        Floor = 2,
        /// <summary>
        /// Round up for .75 and above, floor for below.
        /// </summary>
        RoundPreferFloor = 3,
    }
}
