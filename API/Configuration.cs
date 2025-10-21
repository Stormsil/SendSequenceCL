using System;

namespace SendSequenceCL
{
    /// <summary>
    /// Provides runtime configuration of automation behavior parameters.
    /// All properties are thread-safe and changes apply immediately to subsequent operations.
    /// </summary>
    public static class Configuration
    {
        private static int _mouseMovementDuration = 400;
        private static double _curveRandomization = 0.10;
        private static int _clickDuration = 50;
        private static int _keystrokeDelay = 75;
        private static int _modifierHoldDuration = 100;
        private static int _dragPauseMs = 50;

        /// <summary>
        /// Controls duration of human-like mouse movements (MoveHuman method).
        /// Default: 400 milliseconds (natural human speed).
        /// Valid range: &gt;= 0 (0 = instant like MoveTo, 100-300 = fast, 300-500 = natural, 500+ = slow).
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if value &lt; 0.</exception>
        public static int MouseMovementDuration
        {
            get => _mouseMovementDuration;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "MouseMovementDuration must be >= 0.");
                _mouseMovementDuration = value;
            }
        }

        /// <summary>
        /// Controls amount of random variation in Bézier curve control points.
        /// Default: 0.10 (10% randomization - subtle natural variation).
        /// Valid range: 0.0 to 1.0 (0 = deterministic paths, 0.05-0.15 = natural, 0.5+ = erratic).
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if value &lt; 0.0 or &gt; 1.0.</exception>
        public static double CurveRandomization
        {
            get => _curveRandomization;
            set
            {
                if (value < 0.0 || value > 1.0)
                    throw new ArgumentOutOfRangeException(nameof(value), "CurveRandomization must be between 0.0 and 1.0.");
                _curveRandomization = value;
            }
        }

        /// <summary>
        /// Controls delay between mouse button down and up events during clicks.
        /// Default: 50 milliseconds.
        /// Valid range: &gt;= 0 (0 = instant, 30-100 = normal, 100-200 = slow, 200+ = very slow).
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if value &lt; 0.</exception>
        public static int ClickDuration
        {
            get => _clickDuration;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "ClickDuration must be >= 0.");
                _clickDuration = value;
            }
        }

        /// <summary>
        /// Controls delay between key press and release, and between consecutive keystrokes.
        /// Default: 75 milliseconds.
        /// Valid range: &gt;= 0 (0 = instant/robotic, 30-50 = fast, 50-100 = normal, 100+ = slow).
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if value &lt; 0.</exception>
        public static int KeystrokeDelay
        {
            get => _keystrokeDelay;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "KeystrokeDelay must be >= 0.");
                _keystrokeDelay = value;
            }
        }

        /// <summary>
        /// Controls delay after pressing modifier key down, before pressing main key in shortcuts.
        /// Default: 100 milliseconds.
        /// Valid range: &gt;= 0 (0 = instant/risky, 50-150 = natural, 150+ = slow).
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if value &lt; 0.</exception>
        public static int ModifierHoldDuration
        {
            get => _modifierHoldDuration;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "ModifierHoldDuration must be >= 0.");
                _modifierHoldDuration = value;
            }
        }

        /// <summary>
        /// Controls brief pause duration before and after dragging movement.
        /// Default: 50 milliseconds.
        /// Valid range: &gt;= 0 (0 = instant, 30-100 = normal, 100+ = slow).
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if value &lt; 0.</exception>
        public static int DragPauseMs
        {
            get => _dragPauseMs;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "DragPauseMs must be >= 0.");
                _dragPauseMs = value;
            }
        }

        private static int _minCurveSteps = 10;
        private static int _millisecondsPerCurveStep = 10;
        private static bool _throwOnUnsupportedChar = false;

        /// <summary>
        /// Minimum number of steps used when generating a mouse movement curve.
        /// Default: 10 steps.
        /// Valid range: &gt; 0 (1-5 = choppy, 10-20 = smooth, 20+ = very smooth).
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if value &lt;= 0.</exception>
        public static int MinCurveSteps
        {
            get => _minCurveSteps;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "MinCurveSteps must be > 0.");
                _minCurveSteps = value;
            }
        }

        /// <summary>
        /// Desired duration of a single step in the mouse movement curve in milliseconds.
        /// Used to calculate total number of steps: duration / MillisecondsPerCurveStep.
        /// Default: 10 milliseconds per step.
        /// Valid range: &gt; 0 (1-5 = fast updates, 10-20 = normal, 20+ = slow updates).
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if value &lt;= 0.</exception>
        public static int MillisecondsPerCurveStep
        {
            get => _millisecondsPerCurveStep;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "MillisecondsPerCurveStep must be > 0.");
                _millisecondsPerCurveStep = value;
            }
        }

        /// <summary>
        /// If true, TypeText method will throw an exception when encountering an unsupported character.
        /// If false (default), unsupported characters are silently skipped.
        /// Default: false (skip unsupported characters).
        /// </summary>
        public static bool ThrowOnUnsupportedChar
        {
            get => _throwOnUnsupportedChar;
            set => _throwOnUnsupportedChar = value;
        }
    }
}
