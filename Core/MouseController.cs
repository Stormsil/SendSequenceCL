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
        private readonly HidDeviceManager _joystickDeviceManager;
        private readonly HidCommunicator _joystickCommunicator;
        private readonly HidDeviceManager _relativeDeviceManager;
        private readonly HidCommunicator _relativeCommunicator;
        private readonly Lazy<IVirtualKeyboard> _keyboard;
        private byte _currentButtonState = 0;
        private static readonly Random _random = new Random();

        public MouseController(
            HidDeviceManager deviceManager,
            HidCommunicator communicator,
            HidDeviceManager joystickDeviceManager,
            HidCommunicator joystickCommunicator,
            HidDeviceManager relativeDeviceManager,
            HidCommunicator relativeCommunicator,
            Lazy<IVirtualKeyboard> keyboard)
        {
            _deviceManager = deviceManager ?? throw new ArgumentNullException(nameof(deviceManager));
            _communicator = communicator ?? throw new ArgumentNullException(nameof(communicator));
            _joystickDeviceManager = joystickDeviceManager ?? throw new ArgumentNullException(nameof(joystickDeviceManager));
            _joystickCommunicator = joystickCommunicator ?? throw new ArgumentNullException(nameof(joystickCommunicator));
            _relativeDeviceManager = relativeDeviceManager ?? throw new ArgumentNullException(nameof(relativeDeviceManager));
            _relativeCommunicator = relativeCommunicator ?? throw new ArgumentNullException(nameof(relativeCommunicator));
            _keyboard = keyboard ?? throw new ArgumentNullException(nameof(keyboard));
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

            // Random duration from range
            int duration = durationMs ?? RandomInRange(Configuration.MouseMovementDuration);
            if (duration <= 0)
            {
                MoveTo(x, y);
                return;
            }

            Point start = GetPosition();

            // Calculate steps
            int steps = Math.Max(Configuration.MinCurveSteps, duration / Configuration.MillisecondsPerCurveStep);
            int noiseIntensity = RandomInRange(Configuration.PerlinNoiseIntensity);

            // Generate path using configured algorithm
            var points = MousePathGenerator.Generate(
                start.X, start.Y,
                x, y,
                steps,
                Configuration.MotionAlgorithm,
                Configuration.CurveRandomization,
                noiseIntensity);

            // Overshoot simulation
            if (Configuration.EnableMouseOvershoot && _random.NextDouble() < 0.2) // 20% chance
            {
                double overshootPercent = RandomInRange(Configuration.OvershootAmount.Min, Configuration.OvershootAmount.Max);
                int deltaX = x - start.X;
                int deltaY = y - start.Y;
                int overshootX = x + (int)(deltaX * overshootPercent);
                int overshootY = y + (int)(deltaY * overshootPercent);

                // Validate overshoot coordinates
                try
                {
                    ScreenUtilities.ValidateScreenCoordinates(overshootX, overshootY);

                    // Add overshoot points
                    int overshootSteps = Math.Max(2, steps / 10);
                    var overshootPath = MousePathGenerator.Generate(
                        x, y, overshootX, overshootY,
                        overshootSteps,
                        Configuration.MotionAlgorithm,
                        Configuration.CurveRandomization * 0.5,
                        noiseIntensity);

                    points.AddRange(overshootPath);

                    // Correction back to target
                    var correctionPath = MousePathGenerator.Generate(
                        overshootX, overshootY, x, y,
                        overshootSteps,
                        Configuration.MotionAlgorithm,
                        Configuration.CurveRandomization * 0.3,
                        noiseIntensity / 2);

                    points.AddRange(correctionPath);
                }
                catch (InvalidCoordinateException)
                {
                    // Skip overshoot if coordinates invalid
                }
            }

            // Move along path
            int delayPerStep = points.Count > 0 ? duration / points.Count : 0;
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
            Thread.Sleep(RandomInRange(Configuration.ClickDuration));
            Up(button);
        }

        /// <inheritdoc/>
        public void DoubleClick(MouseButton button)
        {
            if (!Enum.IsDefined(typeof(MouseButton), button))
                throw new ArgumentException($"Invalid MouseButton value: {button}", nameof(button));

            Click(button);
            Thread.Sleep(RandomInRange(Configuration.ClickDuration));
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
            Thread.Sleep(RandomInRange(Configuration.DragPauseMs));

            // Press button down
            Down(button);
            Thread.Sleep(RandomInRange(Configuration.ClickDuration));

            // Move to end position with button held
            MoveHuman(endX, endY, durationMs);
            Thread.Sleep(RandomInRange(Configuration.DragPauseMs));

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

            int duration = durationMs ?? RandomInRange(Configuration.MouseMovementDuration);
            if (duration <= 0)
            {
                MoveTo(x, y);
                return;
            }

            Point start = GetPosition();
            int steps = Math.Max(Configuration.MinCurveSteps, duration / Configuration.MillisecondsPerCurveStep);
            int noiseIntensity = RandomInRange(Configuration.PerlinNoiseIntensity);

            var points = MousePathGenerator.Generate(
                start.X, start.Y, x, y, steps,
                Configuration.MotionAlgorithm,
                Configuration.CurveRandomization,
                noiseIntensity);

            int delayPerStep = points.Count > 0 ? duration / points.Count : 0;
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

            MoveTo(startX, startY);
            await Task.Delay(RandomInRange(Configuration.DragPauseMs), cancellationToken);

            Down(button);
            await Task.Delay(RandomInRange(Configuration.ClickDuration), cancellationToken);

            await MoveHumanAsync(endX, endY, durationMs, cancellationToken);
            await Task.Delay(RandomInRange(Configuration.DragPauseMs), cancellationToken);

            Up(button);
        }

        /// <inheritdoc/>
        public void Scroll(int delta)
        {
            // HID driver doesn't support mouse wheel scrolling
            // Use keyboard arrow keys instead: Up = scroll up, Down = scroll down
            int count = Math.Abs(delta);
            VirtualKey key = delta < 0 ? VirtualKey.Up : VirtualKey.Down;

            for (int i = 0; i < count; i++)
            {
                _keyboard.Value.KeyPress(key);
                if (i < count - 1)
                    Thread.Sleep(10); // Small delay between key presses
            }
        }

        /// <inheritdoc/>
        public void MoveRelative(int dx, int dy)
        {
            // For large movements, split into multiple chunks
            while (dx != 0 || dy != 0)
            {
                int chunkDx = Math.Max(-127, Math.Min(127, dx));
                int chunkDy = Math.Max(-127, Math.Min(127, dy));

                _relativeCommunicator.SendMouseRelative((sbyte)chunkDx, (sbyte)chunkDy, _currentButtonState);

                dx -= chunkDx;
                dy -= chunkDy;

                if (dx != 0 || dy != 0)
                    Thread.Sleep(10); // Small delay between chunks
            }
        }

        /// <summary>
        /// Scrolls in human-like manner using arrow keys with random chunk sizes and pauses.
        /// </summary>
        /// <param name="totalDelta">Total scroll amount (positive = down, negative = up).</param>
        /// <param name="minChunk">Minimum chunk size per scroll.</param>
        /// <param name="maxChunk">Maximum chunk size per scroll.</param>
        public void ScrollHuman(int totalDelta, int minChunk = 1, int maxChunk = 3)
        {
            int remaining = Math.Abs(totalDelta);
            int direction = totalDelta > 0 ? 1 : -1;

            while (remaining > 0)
            {
                int chunk = Math.Min(remaining, _random.Next(minChunk, maxChunk + 1));
                Scroll(direction * chunk);

                remaining -= chunk;

                if (remaining > 0)
                {
                    // Random pause between scroll chunks
                    Thread.Sleep(RandomInRange(Configuration.KeystrokeDelay));
                }
            }
        }

        /// <summary>
        /// Returns random int from [Min, Max] range.
        /// </summary>
        private static int RandomInRange((int Min, int Max) range)
        {
            return _random.Next(range.Min, range.Max + 1);
        }

        /// <summary>
        /// Returns random double from [min, max] range.
        /// </summary>
        private static double RandomInRange(double min, double max)
        {
            return min + _random.NextDouble() * (max - min);
        }
    }
}
