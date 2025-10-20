using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using SendSequenceCL.Infrastructure;

namespace SendSequenceCL.Core
{
    /// <summary>
    /// Implements virtual mouse automation operations.
    /// </summary>
    internal class MouseController : IVirtualMouse
    {
        private readonly HidDeviceManager _deviceManager;
        private readonly HidCommunicator _communicator;
        private byte _currentButtonState = 0;

        public MouseController(HidDeviceManager deviceManager, HidCommunicator communicator)
        {
            _deviceManager = deviceManager ?? throw new ArgumentNullException(nameof(deviceManager));
            _communicator = communicator ?? throw new ArgumentNullException(nameof(communicator));
        }

        /// <inheritdoc/>
        public Point GetPosition()
        {
            return ScreenUtilities.GetCursorPosition();
        }

        /// <inheritdoc/>
        public void MoveTo(int x, int y)
        {
            ScreenUtilities.ValidateScreenCoordinates(x, y);
            var (driverX, driverY) = ScreenUtilities.ScreenToDriverCoordinates(x, y);
            _communicator.SendMouseAbsolute(driverX, driverY, _currentButtonState);
        }

        /// <inheritdoc/>
        public void MoveHuman(int x, int y, int? durationMs = null)
        {
            ScreenUtilities.ValidateScreenCoordinates(x, y);

            int duration = durationMs ?? Configuration.MouseMovementDuration;
            if (duration <= 0)
            {
                // Instant movement
                MoveTo(x, y);
                return;
            }

            // Get current position as starting point
            Point start = GetPosition();

            // Generate Bézier curve points
            int steps = Math.Max(10, duration / 10); // At least 10 steps, roughly 10ms per step
            var points = BezierCurveGenerator.Generate(
                start.X, start.Y,
                x, y,
                steps,
                Configuration.CurveRandomization);

            // Move along curve
            int delayPerStep = duration / steps;
            foreach (var point in points)
            {
                var (driverX, driverY) = ScreenUtilities.ScreenToDriverCoordinates(point.X, point.Y);
                _communicator.SendMouseAbsolute(driverX, driverY, _currentButtonState);

                if (delayPerStep > 0)
                    Thread.Sleep(delayPerStep);
            }
        }

        /// <inheritdoc/>
        public void Click(MouseButton button)
        {
            if (!Enum.IsDefined(typeof(MouseButton), button))
                throw new ArgumentException($"Invalid MouseButton value: {button}", nameof(button));

            Down(button);
            Thread.Sleep(Configuration.ClickDuration);
            Up(button);
        }

        /// <inheritdoc/>
        public void DoubleClick(MouseButton button)
        {
            if (!Enum.IsDefined(typeof(MouseButton), button))
                throw new ArgumentException($"Invalid MouseButton value: {button}", nameof(button));

            Click(button);
            Thread.Sleep(Configuration.ClickDuration);
            Click(button);
        }

        /// <inheritdoc/>
        public void Down(MouseButton button)
        {
            if (!Enum.IsDefined(typeof(MouseButton), button))
                throw new ArgumentException($"Invalid MouseButton value: {button}", nameof(button));

            byte buttonFlag = GetButtonFlag(button);
            _currentButtonState |= buttonFlag;

            Point pos = GetPosition();
            var (driverX, driverY) = ScreenUtilities.ScreenToDriverCoordinates(pos.X, pos.Y);
            _communicator.SendMouseAbsolute(driverX, driverY, _currentButtonState);
        }

        /// <inheritdoc/>
        public void Up(MouseButton button)
        {
            if (!Enum.IsDefined(typeof(MouseButton), button))
                throw new ArgumentException($"Invalid MouseButton value: {button}", nameof(button));

            byte buttonFlag = GetButtonFlag(button);
            _currentButtonState &= (byte)~buttonFlag;

            Point pos = GetPosition();
            var (driverX, driverY) = ScreenUtilities.ScreenToDriverCoordinates(pos.X, pos.Y);
            _communicator.SendMouseAbsolute(driverX, driverY, _currentButtonState);
        }

        /// <inheritdoc/>
        public void Drag(int startX, int startY, int endX, int endY, MouseButton button = MouseButton.Left, int? durationMs = null)
        {
            if (!Enum.IsDefined(typeof(MouseButton), button))
                throw new ArgumentException($"Invalid MouseButton value: {button}", nameof(button));

            // Move to start position
            MoveTo(startX, startY);
            Thread.Sleep(Configuration.DragPauseMs);

            // Press button down
            Down(button);
            Thread.Sleep(Configuration.ClickDuration);

            // Move to end position with button held
            MoveHuman(endX, endY, durationMs);
            Thread.Sleep(Configuration.DragPauseMs);

            // Release button
            Up(button);
        }

        /// <summary>
        /// Converts MouseButton enum to HID button bit flag.
        /// </summary>
        private byte GetButtonFlag(MouseButton button)
        {
            return button switch
            {
                MouseButton.Left => 1,    // Bit 0
                MouseButton.Right => 2,   // Bit 1
                MouseButton.Middle => 4,  // Bit 2
                _ => 0
            };
        }

        // Async versions of mouse operations

        /// <inheritdoc/>
        public async Task MoveHumanAsync(int x, int y, int? durationMs = null, CancellationToken cancellationToken = default)
        {
            ScreenUtilities.ValidateScreenCoordinates(x, y);

            int duration = durationMs ?? Configuration.MouseMovementDuration;
            if (duration <= 0)
            {
                // Instant movement
                MoveTo(x, y);
                return;
            }

            // Get current position as starting point
            Point start = GetPosition();

            // Generate Bézier curve points
            int steps = Math.Max(10, duration / 10);
            var points = BezierCurveGenerator.Generate(
                start.X, start.Y,
                x, y,
                steps,
                Configuration.CurveRandomization);

            // Move along curve
            int delayPerStep = duration / steps;
            foreach (var point in points)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var (driverX, driverY) = ScreenUtilities.ScreenToDriverCoordinates(point.X, point.Y);
                _communicator.SendMouseAbsolute(driverX, driverY, _currentButtonState);

                if (delayPerStep > 0)
                    await Task.Delay(delayPerStep, cancellationToken);
            }
        }

        /// <inheritdoc/>
        public async Task DragAsync(int startX, int startY, int endX, int endY, MouseButton button = MouseButton.Left, int? durationMs = null, CancellationToken cancellationToken = default)
        {
            if (!Enum.IsDefined(typeof(MouseButton), button))
                throw new ArgumentException($"Invalid MouseButton value: {button}", nameof(button));

            // Move to start position
            MoveTo(startX, startY);
            await Task.Delay(Configuration.DragPauseMs, cancellationToken);

            // Press button down
            Down(button);
            await Task.Delay(Configuration.ClickDuration, cancellationToken);

            // Move to end position with button held
            await MoveHumanAsync(endX, endY, durationMs, cancellationToken);
            await Task.Delay(Configuration.DragPauseMs, cancellationToken);

            // Release button
            Up(button);
        }
    }
}
