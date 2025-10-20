using System;
using System.Runtime.InteropServices;

namespace SendSequenceCL.Infrastructure
{
    /// <summary>
    /// HID feature report structure for absolute mouse positioning.
    /// Corresponds to Tetherscript Mouse Absolute driver report format.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct SetFeatureMouseAbs
    {
        /// <summary>
        /// Report ID (must be 1 for mouse absolute driver).
        /// </summary>
        public byte ReportID;

        /// <summary>
        /// Command code (2 = send data).
        /// </summary>
        public byte CommandCode;

        /// <summary>
        /// Mouse button states as bit flags.
        /// Bit 0: Left button (1)
        /// Bit 1: Right button (2)
        /// Bit 2: Middle button (4)
        /// </summary>
        public byte Buttons;

        /// <summary>
        /// Absolute X coordinate (0-32767).
        /// 0 = left edge, 16384 = center, 32767 = right edge.
        /// </summary>
        public ushort X;

        /// <summary>
        /// Absolute Y coordinate (0-32767).
        /// 0 = top edge, 16384 = center, 32767 = bottom edge.
        /// </summary>
        public ushort Y;
    }

    /// <summary>
    /// HID feature report structure for keyboard input.
    /// Corresponds to Tetherscript Keyboard driver report format.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct SetFeatureKeyboard
    {
        /// <summary>
        /// Report ID (must be 1 for keyboard driver).
        /// </summary>
        public byte ReportID;

        /// <summary>
        /// Command code:
        /// 2 = send key data
        /// 3 = ping (keep-alive to prevent stuck keys)
        /// </summary>
        public byte CommandCode;

        /// <summary>
        /// Timeout in milliseconds (divided by 5 for driver).
        /// Driver resets if no ping received within timeout period.
        /// </summary>
        public uint Timeout;

        /// <summary>
        /// Modifier key states as bit flags:
        /// Bit 0: Left Control (1)
        /// Bit 1: Left Shift (2)
        /// Bit 2: Left Alt (4)
        /// Bit 3: Left Windows (8)
        /// Bit 4: Right Control (16)
        /// Bit 5: Right Shift (32)
        /// Bit 6: Right Alt (64)
        /// Bit 7: Right Windows (128)
        /// </summary>
        public byte Modifier;

        /// <summary>
        /// Padding byte (must be 0).
        /// </summary>
        public byte Padding;

        /// <summary>
        /// Key slot 0 (HID Usage ID).
        /// </summary>
        public byte Key0;

        /// <summary>
        /// Key slot 1 (HID Usage ID).
        /// </summary>
        public byte Key1;

        /// <summary>
        /// Key slot 2 (HID Usage ID).
        /// </summary>
        public byte Key2;

        /// <summary>
        /// Key slot 3 (HID Usage ID).
        /// </summary>
        public byte Key3;

        /// <summary>
        /// Key slot 4 (HID Usage ID).
        /// </summary>
        public byte Key4;

        /// <summary>
        /// Key slot 5 (HID Usage ID).
        /// </summary>
        public byte Key5;
    }
}
