using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenVersusWpf
{
    /// <summary>
    /// Specifies where the world origin (0,0) is located.
    /// </summary>
    public enum WorldOrigin
    {
        /// <summary>
        /// The world origin will be in the top left of the virtual screen
        /// </summary>
        VirtualTopLeft = 0,

        /// <summary>
        /// The world origin will be in the top left of the primary screen
        /// </summary>
        PrimaryTopLeft = 1,
    }
}
