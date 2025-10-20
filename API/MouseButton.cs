using System;

namespace SendSequenceCL
{
    /// <summary>
    /// Represents mouse button types for click and drag operations.
    /// </summary>
    public enum MouseButton : byte
    {
        /// <summary>
        /// Left mouse button (primary button).
        /// </summary>
        Left = 1,

        /// <summary>
        /// Right mouse button (secondary button, context menu).
        /// </summary>
        Right = 2,

        /// <summary>
        /// Middle mouse button (scroll wheel button).
        /// </summary>
        Middle = 3
    }
}
