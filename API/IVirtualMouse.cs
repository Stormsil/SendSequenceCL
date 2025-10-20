using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace SendSequenceCL
{
    /// <summary>
    /// Defines contract for virtual mouse automation operations.
    /// </summary>
    public interface IVirtualMouse
    {
        /// <summary>
        /// Gets the current mouse cursor position on screen.
        /// </summary>
        /// <returns>The current cursor position as a <see cref="Point"/> with X and Y coordinates.</returns>
        /// <exception cref="DriverCommunicationException">Thrown if unable to query cursor position.</exception>
        Point GetPosition();

        /// <summary>
        /// Moves mouse cursor instantly to specified screen coordinates (teleport).
        /// </summary>
        /// <param name="x">Target X coordinate in pixels (0 = left edge).</param>
        /// <param name="y">Target Y coordinate in pixels (0 = top edge).</param>
        /// <exception cref="InvalidCoordinateException">Thrown if coordinates are outside screen bounds.</exception>
        /// <exception cref="DriverNotFoundException">Thrown if HID driver not found.</exception>
        /// <exception cref="DriverCommunicationException">Thrown if communication with driver fails.</exception>
        void MoveTo(int x, int y);

        /// <summary>
        /// Moves mouse cursor to specified coordinates using human-like curved motion.
        /// </summary>
        /// <param name="x">Target X coordinate in pixels (0 = left edge).</param>
        /// <param name="y">Target Y coordinate in pixels (0 = top edge).</param>
        /// <param name="durationMs">Optional override for movement duration in milliseconds. If null, uses <see cref="Configuration.MouseMovementDuration"/>.</param>
        /// <exception cref="InvalidCoordinateException">Thrown if coordinates are outside screen bounds.</exception>
        /// <exception cref="DriverNotFoundException">Thrown if HID driver not found.</exception>
        /// <exception cref="DriverCommunicationException">Thrown if communication with driver fails.</exception>
        void MoveHuman(int x, int y, int? durationMs = null);

        /// <summary>
        /// Clicks a mouse button at the current cursor position.
        /// </summary>
        /// <param name="button">The mouse button to click.</param>
        /// <exception cref="DriverNotFoundException">Thrown if HID driver not found.</exception>
        /// <exception cref="DriverCommunicationException">Thrown if communication with driver fails.</exception>
        void Click(MouseButton button);

        /// <summary>
        /// Double-clicks a mouse button at the current cursor position.
        /// </summary>
        /// <param name="button">The mouse button to double-click.</param>
        /// <exception cref="DriverNotFoundException">Thrown if HID driver not found.</exception>
        /// <exception cref="DriverCommunicationException">Thrown if communication with driver fails.</exception>
        void DoubleClick(MouseButton button);

        /// <summary>
        /// Presses a mouse button down (without releasing).
        /// </summary>
        /// <param name="button">The mouse button to press down.</param>
        /// <exception cref="DriverNotFoundException">Thrown if HID driver not found.</exception>
        /// <exception cref="DriverCommunicationException">Thrown if communication with driver fails.</exception>
        void Down(MouseButton button);

        /// <summary>
        /// Releases a previously pressed mouse button.
        /// </summary>
        /// <param name="button">The mouse button to release.</param>
        /// <exception cref="DriverNotFoundException">Thrown if HID driver not found.</exception>
        /// <exception cref="DriverCommunicationException">Thrown if communication with driver fails.</exception>
        void Up(MouseButton button);

        /// <summary>
        /// Drags mouse from start position to end position with button held down.
        /// </summary>
        /// <param name="startX">Starting X coordinate in pixels.</param>
        /// <param name="startY">Starting Y coordinate in pixels.</param>
        /// <param name="endX">Ending X coordinate in pixels.</param>
        /// <param name="endY">Ending Y coordinate in pixels.</param>
        /// <param name="button">Optional mouse button to hold during drag (default: Left).</param>
        /// <param name="durationMs">Optional override for drag duration in milliseconds. If null, uses <see cref="Configuration.MouseMovementDuration"/>.</param>
        /// <exception cref="InvalidCoordinateException">Thrown if any coordinates are outside screen bounds.</exception>
        /// <exception cref="DriverNotFoundException">Thrown if HID driver not found.</exception>
        /// <exception cref="DriverCommunicationException">Thrown if communication with driver fails.</exception>
        void Drag(int startX, int startY, int endX, int endY, MouseButton button = MouseButton.Left, int? durationMs = null);

        // Async versions of mouse operations

        /// <summary>
        /// Asynchronously moves mouse cursor to specified coordinates using human-like curved motion.
        /// </summary>
        /// <param name="x">Target X coordinate in pixels (0 = left edge).</param>
        /// <param name="y">Target Y coordinate in pixels (0 = top edge).</param>
        /// <param name="durationMs">Optional override for movement duration in milliseconds. If null, uses <see cref="Configuration.MouseMovementDuration"/>.</param>
        /// <param name="cancellationToken">Optional token to cancel the movement operation.</param>
        /// <exception cref="InvalidCoordinateException">Thrown if coordinates are outside screen bounds.</exception>
        /// <exception cref="DriverNotFoundException">Thrown if HID driver not found.</exception>
        /// <exception cref="DriverCommunicationException">Thrown if communication with driver fails.</exception>
        /// <exception cref="OperationCanceledException">Thrown if operation is canceled via token.</exception>
        Task MoveHumanAsync(int x, int y, int? durationMs = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously drags mouse from start position to end position with button held down.
        /// </summary>
        /// <param name="startX">Starting X coordinate in pixels.</param>
        /// <param name="startY">Starting Y coordinate in pixels.</param>
        /// <param name="endX">Ending X coordinate in pixels.</param>
        /// <param name="endY">Ending Y coordinate in pixels.</param>
        /// <param name="button">Optional mouse button to hold during drag (default: Left).</param>
        /// <param name="durationMs">Optional override for drag duration in milliseconds. If null, uses <see cref="Configuration.MouseMovementDuration"/>.</param>
        /// <param name="cancellationToken">Optional token to cancel the drag operation.</param>
        /// <exception cref="InvalidCoordinateException">Thrown if any coordinates are outside screen bounds.</exception>
        /// <exception cref="DriverNotFoundException">Thrown if HID driver not found.</exception>
        /// <exception cref="DriverCommunicationException">Thrown if communication with driver fails.</exception>
        /// <exception cref="OperationCanceledException">Thrown if operation is canceled via token.</exception>
        Task DragAsync(int startX, int startY, int endX, int endY, MouseButton button = MouseButton.Left, int? durationMs = null, CancellationToken cancellationToken = default);
    }
}
