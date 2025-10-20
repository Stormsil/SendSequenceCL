using System;
using System.Drawing;

namespace SendSequenceCL.Core
{
    /// <summary>
    /// Utilities for screen coordinate conversion and validation.
    /// </summary>
    internal static class ScreenUtilities
    {
        /// <summary>
        /// Converts screen pixel coordinates to HID driver coordinates (0-32767).
        /// </summary>
        /// <param name="screenX">Screen X coordinate in pixels.</param>
        /// <param name="screenY">Screen Y coordinate in pixels.</param>
        /// <returns>Tuple of (driverX, driverY) in range 0-32767.</returns>
        public static (ushort driverX, ushort driverY) ScreenToDriverCoordinates(int screenX, int screenY)
        {
            var screenSize = GetScreenSize();

            // Linear scaling: driverX = (screenX * 32767) / screenWidth
            // Clamp to valid range
            int clampedX = Math.Max(0, Math.Min(screenX, screenSize.Width - 1));
            int clampedY = Math.Max(0, Math.Min(screenY, screenSize.Height - 1));

            ushort driverX = (ushort)((clampedX * 32767) / Math.Max(1, screenSize.Width - 1));
            ushort driverY = (ushort)((clampedY * 32767) / Math.Max(1, screenSize.Height - 1));

            return (driverX, driverY);
        }

        /// <summary>
        /// Validates that screen coordinates are within screen bounds.
        /// </summary>
        /// <param name="x">Screen X coordinate in pixels.</param>
        /// <param name="y">Screen Y coordinate in pixels.</param>
        /// <exception cref="InvalidCoordinateException">Thrown if coordinates are outside screen bounds.</exception>
        public static void ValidateScreenCoordinates(int x, int y)
        {
            var screenSize = GetScreenSize();

            if (x < 0 || x >= screenSize.Width)
            {
                throw new InvalidCoordinateException(
                    $"X coordinate {x} is outside screen bounds (0-{screenSize.Width - 1}).");
            }

            if (y < 0 || y >= screenSize.Height)
            {
                throw new InvalidCoordinateException(
                    $"Y coordinate {y} is outside screen bounds (0-{screenSize.Height - 1}).");
            }
        }

        /// <summary>
        /// Gets the current screen size (primary monitor).
        /// </summary>
        /// <returns>Screen size in pixels.</returns>
        public static Size GetScreenSize()
        {
            try
            {
                // Use Win32 API to get desktop window size
                IntPtr desktopWindow = Infrastructure.NativeMethods.GetDesktopWindow();
                if (Infrastructure.NativeMethods.GetWindowRect(desktopWindow, out Infrastructure.NativeMethods.RECT rect))
                {
                    int width = rect.Right - rect.Left;
                    int height = rect.Bottom - rect.Top;
                    return new Size(width, height);
                }

                // Fallback to System.Windows.Forms if Win32 fails
                return new Size(1920, 1080); // Common default
            }
            catch
            {
                // Safe fallback
                return new Size(1920, 1080);
            }
        }

        /// <summary>
        /// Gets the current mouse cursor position.
        /// </summary>
        /// <returns>Current cursor position in screen pixels.</returns>
        /// <exception cref="DriverCommunicationException">Thrown if unable to query cursor position.</exception>
        public static Point GetCursorPosition()
        {
            if (Infrastructure.NativeMethods.GetCursorPos(out Infrastructure.NativeMethods.POINT point))
            {
                return new Point(point.X, point.Y);
            }

            throw new DriverCommunicationException("Failed to get cursor position.");
        }
    }
}
