using System;

namespace SendSequenceCL
{
    /// <summary>
    /// Mouse motion algorithm selection for humanized movement.
    /// </summary>
    public enum MouseMotionAlgorithm
    {
        /// <summary>Bezier curve interpolation (smooth, predictable).</summary>
        Bezier,
        /// <summary>Perlin noise-based movement (organic, slightly chaotic).</summary>
        PerlinNoise
    }

    /// <summary>
    /// Provides runtime configuration of automation behavior parameters.
    /// All properties are thread-safe and changes apply immediately to subsequent operations.
    /// </summary>
    public static class Configuration
    {
        private static (int Min, int Max) _mouseMovementDuration = (300, 500);
        private static double _curveRandomization = 0.10;
        private static (int Min, int Max) _clickDuration = (30, 70);
        private static (int Min, int Max) _keystrokeDelay = (50, 120);
        private static (int Min, int Max) _modifierHoldDuration = (80, 150);
        private static (int Min, int Max) _dragPauseMs = (40, 80);
        private static (int Min, int Max) _interWordDelay = (100, 250);
        private static double _typoChance = 0.01;
        private static MouseMotionAlgorithm _motionAlgorithm = MouseMotionAlgorithm.Bezier;
        private static bool _enableMouseOvershoot = true;
        private static (double Min, double Max) _overshootAmount = (0.02, 0.07);
        private static (int Min, int Max) _perlinNoiseIntensity = (5, 15);

        /// <summary>
        /// Controls duration range of human-like mouse movements (MoveHuman method).
        /// Random value picked from [Min, Max] for each movement.
        /// Default: (300, 500) milliseconds.
        /// Valid range: Min >= 0, Max >= Min (0 = instant, 100-300 = fast, 300-500 = natural, 500+ = slow).
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if Min &lt; 0 or Max &lt; Min.</exception>
        public static (int Min, int Max) MouseMovementDuration
        {
            get => _mouseMovementDuration;
            set
            {
                if (value.Min < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "MouseMovementDuration.Min must be >= 0.");
                if (value.Max < value.Min)
                    throw new ArgumentOutOfRangeException(nameof(value), "MouseMovementDuration.Max must be >= Min.");
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
        /// Controls delay range between mouse button down and up events during clicks.
        /// Random value picked from [Min, Max] for each click.
        /// Default: (30, 70) milliseconds.
        /// Valid range: Min >= 0, Max >= Min (0 = instant, 30-100 = normal, 100-200 = slow).
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if Min &lt; 0 or Max &lt; Min.</exception>
        public static (int Min, int Max) ClickDuration
        {
            get => _clickDuration;
            set
            {
                if (value.Min < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "ClickDuration.Min must be >= 0.");
                if (value.Max < value.Min)
                    throw new ArgumentOutOfRangeException(nameof(value), "ClickDuration.Max must be >= Min.");
                _clickDuration = value;
            }
        }

        /// <summary>
        /// Controls delay range between key press/release and between consecutive keystrokes.
        /// Random value picked from [Min, Max] for each keystroke.
        /// Default: (50, 120) milliseconds.
        /// Valid range: Min >= 0, Max >= Min (0 = robotic, 30-50 = fast, 50-100 = normal, 100+ = slow).
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if Min &lt; 0 or Max &lt; Min.</exception>
        public static (int Min, int Max) KeystrokeDelay
        {
            get => _keystrokeDelay;
            set
            {
                if (value.Min < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "KeystrokeDelay.Min must be >= 0.");
                if (value.Max < value.Min)
                    throw new ArgumentOutOfRangeException(nameof(value), "KeystrokeDelay.Max must be >= Min.");
                _keystrokeDelay = value;
            }
        }

        /// <summary>
        /// Controls delay range after pressing modifier key down, before pressing main key.
        /// Random value picked from [Min, Max] for each chord.
        /// Default: (80, 150) milliseconds.
        /// Valid range: Min >= 0, Max >= Min (0 = risky, 50-150 = natural, 150+ = slow).
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if Min &lt; 0 or Max &lt; Min.</exception>
        public static (int Min, int Max) ModifierHoldDuration
        {
            get => _modifierHoldDuration;
            set
            {
                if (value.Min < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "ModifierHoldDuration.Min must be >= 0.");
                if (value.Max < value.Min)
                    throw new ArgumentOutOfRangeException(nameof(value), "ModifierHoldDuration.Max must be >= Min.");
                _modifierHoldDuration = value;
            }
        }

        /// <summary>
        /// Controls brief pause duration range before and after dragging movement.
        /// Random value picked from [Min, Max] for each drag.
        /// Default: (40, 80) milliseconds.
        /// Valid range: Min >= 0, Max >= Min (0 = instant, 30-100 = normal, 100+ = slow).
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if Min &lt; 0 or Max &lt; Min.</exception>
        public static (int Min, int Max) DragPauseMs
        {
            get => _dragPauseMs;
            set
            {
                if (value.Min < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "DragPauseMs.Min must be >= 0.");
                if (value.Max < value.Min)
                    throw new ArgumentOutOfRangeException(nameof(value), "DragPauseMs.Max must be >= Min.");
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

        /// <summary>
        /// Controls additional delay range after typing a space (word boundary).
        /// Random value picked from [Min, Max] after each space character.
        /// Default: (100, 250) milliseconds.
        /// Valid range: Min >= 0, Max >= Min.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if Min &lt; 0 or Max &lt; Min.</exception>
        public static (int Min, int Max) InterWordDelay
        {
            get => _interWordDelay;
            set
            {
                if (value.Min < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "InterWordDelay.Min must be >= 0.");
                if (value.Max < value.Min)
                    throw new ArgumentOutOfRangeException(nameof(value), "InterWordDelay.Max must be >= Min.");
                _interWordDelay = value;
            }
        }

        /// <summary>
        /// Probability of typing mistake (typo) during TypeTextHuman.
        /// After typing correct key, typo simulation: wrong key → backspace → continue.
        /// Default: 0.01 (1% chance per keystroke).
        /// Valid range: 0.0 to 1.0 (0 = no typos, 0.01 = 1%, 0.1 = 10%).
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if value &lt; 0.0 or &gt; 1.0.</exception>
        public static double TypoChance
        {
            get => _typoChance;
            set
            {
                if (value < 0.0 || value > 1.0)
                    throw new ArgumentOutOfRangeException(nameof(value), "TypoChance must be between 0.0 and 1.0.");
                _typoChance = value;
            }
        }

        /// <summary>
        /// Mouse motion path generation algorithm for MoveHuman.
        /// Default: Bezier (smooth, predictable curves).
        /// PerlinNoise: organic, slightly chaotic paths (more human-like).
        /// </summary>
        public static MouseMotionAlgorithm MotionAlgorithm
        {
            get => _motionAlgorithm;
            set => _motionAlgorithm = value;
        }

        /// <summary>
        /// If true, MoveHuman will occasionally "overshoot" target and correct back.
        /// Simulates natural human mouse movement imprecision.
        /// Default: true.
        /// </summary>
        public static bool EnableMouseOvershoot
        {
            get => _enableMouseOvershoot;
            set => _enableMouseOvershoot = value;
        }

        /// <summary>
        /// Overshoot distance as percentage of total movement distance.
        /// Random value from [Min, Max] when overshoot occurs.
        /// Default: (0.02, 0.07) equals 2-7 percent of distance.
        /// Valid range: Min >= 0.0, Max &lt;= 1.0, Max >= Min.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if out of valid range.</exception>
        public static (double Min, double Max) OvershootAmount
        {
            get => _overshootAmount;
            set
            {
                if (value.Min < 0.0 || value.Max > 1.0)
                    throw new ArgumentOutOfRangeException(nameof(value), "OvershootAmount must be in range [0.0, 1.0].");
                if (value.Max < value.Min)
                    throw new ArgumentOutOfRangeException(nameof(value), "OvershootAmount.Max must be >= Min.");
                _overshootAmount = value;
            }
        }

        /// <summary>
        /// Intensity range of Perlin noise when MotionAlgorithm = PerlinNoise.
        /// Random value from [Min, Max] for each movement.
        /// Higher values = more chaotic/jittery movement.
        /// Default: (5, 15) pixels.
        /// Valid range: Min >= 0, Max >= Min.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if Min &lt; 0 or Max &lt; Min.</exception>
        public static (int Min, int Max) PerlinNoiseIntensity
        {
            get => _perlinNoiseIntensity;
            set
            {
                if (value.Min < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "PerlinNoiseIntensity.Min must be >= 0.");
                if (value.Max < value.Min)
                    throw new ArgumentOutOfRangeException(nameof(value), "PerlinNoiseIntensity.Max must be >= Min.");
                _perlinNoiseIntensity = value;
            }
        }
    }
}
