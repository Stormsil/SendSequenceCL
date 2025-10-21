using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace SendSequenceCL
{
    /// <summary>
    /// Fluent API for building and executing complex automation sequences.
    /// Allows chaining multiple operations together and executing them with Run().
    /// </summary>
    /// <example>
    /// <code>
    /// new SequenceBuilder()
    ///     .MoveHuman(500, 500)
    ///     .Wait(200)
    ///     .Click(MouseButton.Left)
    ///     .TypeText("Hello World")
    ///     .Run();
    /// </code>
    /// </example>
    public class SequenceBuilder
    {
        private readonly List<Action> _actions = new List<Action>();

        // ===== Mouse Operations =====

        /// <summary>
        /// Adds instant mouse movement to the sequence.
        /// </summary>
        /// <param name="x">Target X coordinate.</param>
        /// <param name="y">Target Y coordinate.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public SequenceBuilder MoveTo(int x, int y)
        {
            _actions.Add(() => Input.Mouse.MoveTo(x, y));
            return this;
        }

        /// <summary>
        /// Adds human-like mouse movement to the sequence.
        /// </summary>
        /// <param name="x">Target X coordinate.</param>
        /// <param name="y">Target Y coordinate.</param>
        /// <param name="durationMs">Optional movement duration override.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public SequenceBuilder MoveHuman(int x, int y, int? durationMs = null)
        {
            _actions.Add(() => Input.Mouse.MoveHuman(x, y, durationMs));
            return this;
        }

        /// <summary>
        /// Adds relative mouse movement to the sequence.
        /// </summary>
        /// <param name="dx">Relative X movement.</param>
        /// <param name="dy">Relative Y movement.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public SequenceBuilder MoveRelative(int dx, int dy)
        {
            _actions.Add(() => Input.Mouse.MoveRelative(dx, dy));
            return this;
        }

        /// <summary>
        /// Adds mouse click to the sequence.
        /// </summary>
        /// <param name="button">Button to click.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public SequenceBuilder Click(MouseButton button)
        {
            _actions.Add(() => Input.Mouse.Click(button));
            return this;
        }

        /// <summary>
        /// Adds mouse double-click to the sequence.
        /// </summary>
        /// <param name="button">Button to double-click.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public SequenceBuilder DoubleClick(MouseButton button)
        {
            _actions.Add(() => Input.Mouse.DoubleClick(button));
            return this;
        }

        /// <summary>
        /// Adds mouse button down to the sequence.
        /// </summary>
        /// <param name="button">Button to press down.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public SequenceBuilder Down(MouseButton button)
        {
            _actions.Add(() => Input.Mouse.Down(button));
            return this;
        }

        /// <summary>
        /// Adds mouse button up to the sequence.
        /// </summary>
        /// <param name="button">Button to release.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public SequenceBuilder Up(MouseButton button)
        {
            _actions.Add(() => Input.Mouse.Up(button));
            return this;
        }

        /// <summary>
        /// Adds drag operation to the sequence.
        /// </summary>
        /// <param name="startX">Starting X coordinate.</param>
        /// <param name="startY">Starting Y coordinate.</param>
        /// <param name="endX">Ending X coordinate.</param>
        /// <param name="endY">Ending Y coordinate.</param>
        /// <param name="button">Button to hold during drag (default: Left).</param>
        /// <param name="durationMs">Optional drag duration override.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public SequenceBuilder Drag(int startX, int startY, int endX, int endY, MouseButton button = MouseButton.Left, int? durationMs = null)
        {
            _actions.Add(() => Input.Mouse.Drag(startX, startY, endX, endY, button, durationMs));
            return this;
        }

        /// <summary>
        /// Adds mouse scroll to the sequence.
        /// </summary>
        /// <param name="delta">Scroll amount (positive = down, negative = up).</param>
        /// <returns>This builder instance for method chaining.</returns>
        public SequenceBuilder Scroll(int delta)
        {
            _actions.Add(() => Input.Mouse.Scroll(delta));
            return this;
        }

        /// <summary>
        /// Adds human-like scroll to the sequence (random chunks with pauses).
        /// </summary>
        /// <param name="totalDelta">Total scroll amount.</param>
        /// <param name="minChunk">Minimum chunk size (default: 1).</param>
        /// <param name="maxChunk">Maximum chunk size (default: 3).</param>
        /// <returns>This builder instance for method chaining.</returns>
        public SequenceBuilder ScrollHuman(int totalDelta, int minChunk = 1, int maxChunk = 3)
        {
            _actions.Add(() => Input.Mouse.ScrollHuman(totalDelta, minChunk, maxChunk));
            return this;
        }

        // ===== Keyboard Operations =====

        /// <summary>
        /// Adds text typing to the sequence.
        /// </summary>
        /// <param name="text">Text to type.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public SequenceBuilder TypeText(string text)
        {
            _actions.Add(() => Input.Keyboard.TypeText(text));
            return this;
        }

        /// <summary>
        /// Adds human-like text typing to the sequence (with typos and variable delays).
        /// </summary>
        /// <param name="text">Text to type.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public SequenceBuilder TypeTextHuman(string text)
        {
            _actions.Add(() => Input.Keyboard.TypeTextHuman(text));
            return this;
        }

        /// <summary>
        /// Adds single key press to the sequence.
        /// </summary>
        /// <param name="key">Key to press.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public SequenceBuilder KeyPress(VirtualKey key)
        {
            _actions.Add(() => Input.Keyboard.KeyPress(key));
            return this;
        }

        /// <summary>
        /// Adds keyboard chord (modifier + key) to the sequence.
        /// </summary>
        /// <param name="modifier">Modifier key (Ctrl, Shift, Alt, Win).</param>
        /// <param name="key">Key to press with modifier.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public SequenceBuilder Chord(VirtualKey modifier, VirtualKey key)
        {
            _actions.Add(() => Input.Keyboard.Chord(modifier, key));
            return this;
        }

        /// <summary>
        /// Adds keyboard chord with multiple modifiers to the sequence.
        /// </summary>
        /// <param name="modifiers">Modifier keys (Ctrl, Shift, Alt, Win).</param>
        /// <param name="key">Key to press with modifiers.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public SequenceBuilder Chord(IEnumerable<VirtualKey> modifiers, VirtualKey key)
        {
            _actions.Add(() => Input.Keyboard.Chord(modifiers, key));
            return this;
        }

        /// <summary>
        /// Adds key down to the sequence.
        /// </summary>
        /// <param name="key">Key to press down.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public SequenceBuilder KeyDown(VirtualKey key)
        {
            _actions.Add(() => Input.Keyboard.KeyDown(key));
            return this;
        }

        /// <summary>
        /// Adds key up to the sequence.
        /// </summary>
        /// <param name="key">Key to release.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public SequenceBuilder KeyUp(VirtualKey key)
        {
            _actions.Add(() => Input.Keyboard.KeyUp(key));
            return this;
        }

        // ===== Timing Operations =====

        /// <summary>
        /// Adds fixed delay to the sequence.
        /// </summary>
        /// <param name="milliseconds">Duration to wait in milliseconds.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public SequenceBuilder Wait(int milliseconds)
        {
            _actions.Add(() => Input.Wait(milliseconds));
            return this;
        }

        /// <summary>
        /// Adds random delay to the sequence.
        /// </summary>
        /// <param name="minMs">Minimum delay in milliseconds.</param>
        /// <param name="maxMs">Maximum delay in milliseconds.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public SequenceBuilder WaitRandom(int minMs, int maxMs)
        {
            _actions.Add(() =>
            {
                var random = new Random();
                int delay = random.Next(minMs, maxMs + 1);
                Input.Wait(delay);
            });
            return this;
        }

        // ===== Conditional Operations (Guards) =====

        /// <summary>
        /// Adds conditional action to the sequence.
        /// Action executes only if condition returns true at runtime.
        /// </summary>
        /// <param name="condition">Condition to check before executing action.</param>
        /// <param name="action">Action to execute if condition is true.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public SequenceBuilder DoIf(Func<bool> condition, Action action)
        {
            _actions.Add(() =>
            {
                if (condition())
                {
                    action();
                }
            });
            return this;
        }

        /// <summary>
        /// Adds wait-until condition to the sequence.
        /// Blocks execution until condition returns true or timeout expires.
        /// </summary>
        /// <param name="condition">Condition to wait for.</param>
        /// <param name="timeoutMs">Maximum time to wait in milliseconds (default: 5000).</param>
        /// <param name="checkIntervalMs">Interval between condition checks in milliseconds (default: 50).</param>
        /// <returns>This builder instance for method chaining.</returns>
        /// <exception cref="TimeoutException">Thrown if condition doesn't become true within timeout.</exception>
        public SequenceBuilder WaitUntil(Func<bool> condition, int timeoutMs = 5000, int checkIntervalMs = 50)
        {
            _actions.Add(() =>
            {
                int elapsed = 0;
                while (!condition())
                {
                    if (elapsed >= timeoutMs)
                    {
                        throw new TimeoutException($"WaitUntil condition not met within {timeoutMs}ms timeout.");
                    }

                    Thread.Sleep(checkIntervalMs);
                    elapsed += checkIntervalMs;
                }
            });
            return this;
        }

        /// <summary>
        /// Adds conditional click to the sequence.
        /// Clicks only if cursor is within tolerance of target position.
        /// </summary>
        /// <param name="targetPosition">Expected cursor position.</param>
        /// <param name="button">Button to click if position matches.</param>
        /// <param name="tolerance">Maximum distance from target in pixels (default: 5).</param>
        /// <returns>This builder instance for method chaining.</returns>
        public SequenceBuilder ClickIfAt(Point targetPosition, MouseButton button, int tolerance = 5)
        {
            _actions.Add(() =>
            {
                Point currentPos = Input.Mouse.GetPosition();
                double distance = Math.Sqrt(
                    Math.Pow(currentPos.X - targetPosition.X, 2) +
                    Math.Pow(currentPos.Y - targetPosition.Y, 2));

                if (distance <= tolerance)
                {
                    Input.Mouse.Click(button);
                }
            });
            return this;
        }

        // ===== Execution =====

        /// <summary>
        /// Executes all accumulated actions in sequence.
        /// Actions are executed in the order they were added.
        /// </summary>
        /// <exception cref="Exception">Any exception thrown by individual actions will propagate to caller.</exception>
        public void Run()
        {
            foreach (var action in _actions)
            {
                action();
            }
        }

        /// <summary>
        /// Clears all accumulated actions from the sequence.
        /// Allows reusing the same builder instance for multiple sequences.
        /// </summary>
        /// <returns>This builder instance for method chaining.</returns>
        public SequenceBuilder Clear()
        {
            _actions.Clear();
            return this;
        }

        /// <summary>
        /// Gets the number of actions currently in the sequence.
        /// </summary>
        public int Count => _actions.Count;
    }
}
