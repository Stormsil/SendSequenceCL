using System;

namespace SendSequenceCL.Infrastructure
{
    /// <summary>
    /// Constants for Tetherscript HID virtual driver identification.
    /// </summary>
    internal static class DriverConstants
    {
        /// <summary>
        /// Tetherscript vendor ID for all HID virtual drivers.
        /// </summary>
        public const ushort VendorId = 0xF00F;

        /// <summary>
        /// Product ID for Tetherscript Virtual Mouse Absolute driver.
        /// </summary>
        public const ushort ProductIdMouseAbsolute = 0x0002;

        /// <summary>
        /// Product ID for Tetherscript Virtual Keyboard driver.
        /// </summary>
        public const ushort ProductIdKeyboard = 0x0003;

        /// <summary>
        /// Product ID for Tetherscript Virtual Joystick driver.
        /// </summary>
        public const ushort ProductIdJoystick = 0x0001;

        /// <summary>
        /// Product ID for Tetherscript Virtual Mouse Relative driver.
        /// </summary>
        public const ushort ProductIdMouseRelative = 0x0005;

        /// <summary>
        /// HID Report ID for feature reports (must be 1).
        /// </summary>
        public const byte ReportId = 1;

        /// <summary>
        /// Command code for sending data to driver.
        /// </summary>
        public const byte CommandCodeSend = 2;

        /// <summary>
        /// Command code for ping (keep-alive) to keyboard driver.
        /// </summary>
        public const byte CommandCodePing = 3;

        /// <summary>
        /// Maximum coordinate value for mouse absolute positioning (0-32767).
        /// </summary>
        public const ushort MaxCoordinate = 32767;

        /// <summary>
        /// Center coordinate value for mouse absolute positioning.
        /// </summary>
        public const ushort CenterCoordinate = 16384;

        /// <summary>
        /// Default ping timeout in milliseconds for keyboard driver (prevents stuck keys).
        /// </summary>
        public const uint KeyboardPingTimeout = 5000;
    }
}
