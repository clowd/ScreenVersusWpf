using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
