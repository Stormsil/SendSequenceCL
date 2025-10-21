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

    /// <summary>
    /// HID feature report structure for relative mouse positioning.
    /// Corresponds to Tetherscript Mouse Relative driver report format.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct SetFeatureMouseRel
    {
        /// <summary>
        /// Report ID (must be 1 for mouse relative driver).
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
        /// Relative X movement (-127 to 127).
        /// Negative = left, Positive = right.
        /// </summary>
        public sbyte X;

        /// <summary>
        /// Relative Y movement (-127 to 127).
        /// Negative = up, Positive = down.
        /// </summary>
        public sbyte Y;
    }

    /// <summary>
    /// HID feature report structure for joystick input.
    /// Corresponds to Tetherscript Joystick driver report format.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct SetFeatureJoy
    {
        /// <summary>
        /// Report ID (must be 1 for joystick driver).
        /// </summary>
        public byte ReportID;

        /// <summary>
        /// Command code (2 = send data).
        /// </summary>
        public byte CommandCode;

        /// <summary>
        /// Joystick X axis (0-32767, center = 16384).
        /// </summary>
        public ushort X;

        /// <summary>
        /// Joystick Y axis (0-32767, center = 16384).
        /// </summary>
        public ushort Y;

        /// <summary>
        /// Joystick Z axis (0-32767, center = 16384).
        /// </summary>
        public ushort Z;

        /// <summary>
        /// Joystick rotation X axis (0-32767, center = 16384).
        /// </summary>
        public ushort rX;

        /// <summary>
        /// Joystick rotation Y axis (0-32767, center = 16384).
        /// </summary>
        public ushort rY;

        /// <summary>
        /// Joystick rotation Z axis (0-32767, center = 16384).
        /// </summary>
        public ushort rZ;

        /// <summary>
        /// Slider control (0-32767, center = 16384).
        /// </summary>
        public ushort slider;

        /// <summary>
        /// Dial control (0-32767, center = 16384).
        /// </summary>
        public ushort dial;

        /// <summary>
        /// Wheel control (0-32767, center = 16384).
        /// Used for mouse scroll emulation.
        /// </summary>
        public ushort wheel;

        /// <summary>
        /// Hat switch position (0-255).
        /// </summary>
        public byte hat;

        /// <summary>Button 0 state (bit flags).</summary>
        public byte btn0;
        /// <summary>Button 1 state (bit flags).</summary>
        public byte btn1;
        /// <summary>Button 2 state (bit flags).</summary>
        public byte btn2;
        /// <summary>Button 3 state (bit flags).</summary>
        public byte btn3;
        /// <summary>Button 4 state (bit flags).</summary>
        public byte btn4;
        /// <summary>Button 5 state (bit flags).</summary>
        public byte btn5;
        /// <summary>Button 6 state (bit flags).</summary>
        public byte btn6;
        /// <summary>Button 7 state (bit flags).</summary>
        public byte btn7;
        /// <summary>Button 8 state (bit flags).</summary>
        public byte btn8;
        /// <summary>Button 9 state (bit flags).</summary>
        public byte btn9;
        /// <summary>Button 10 state (bit flags).</summary>
        public byte btn10;
        /// <summary>Button 11 state (bit flags).</summary>
        public byte btn11;
        /// <summary>Button 12 state (bit flags).</summary>
        public byte btn12;
        /// <summary>Button 13 state (bit flags).</summary>
        public byte btn13;
        /// <summary>Button 14 state (bit flags).</summary>
        public byte btn14;
        /// <summary>Button 15 state (bit flags).</summary>
        public byte btn15;
    }
}
