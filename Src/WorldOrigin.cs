using System;

namespace ScreenVersusWpf
{
    /// <summary>
    /// Specifies where the world origin (0,0) is located.
    /// </summary>
    public enum WorldOrigin
    {
        /// <summary>
        /// The world origin will be in the top left of the primary screen
        /// </summary>
        PrimaryTopLeft = 0,

        /// <summary>
        /// The world origin will be in the top left of the virtual screen
        /// </summary>
        VirtualTopLeft = 1,
    }
}
