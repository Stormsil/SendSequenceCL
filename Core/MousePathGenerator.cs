using System;
using System.Collections.Generic;
using System.Drawing;

namespace SendSequenceCL.Core
{
    /// <summary>
    /// Generates mouse movement paths using various algorithms (Bezier, Perlin Noise).
    /// </summary>
    internal static class MousePathGenerator
    {
        private static readonly Random _random = new Random();

        /// <summary>
        /// Generates a path from start to end using the specified algorithm.
        /// </summary>
        public static List<Point> Generate(int startX, int startY, int endX, int endY, int steps,
                                          MouseMotionAlgorithm algorithm, double randomization, int noiseIntensity = 10)
        {
            return algorithm switch
            {
                MouseMotionAlgorithm.Bezier => GenerateBezier(startX, startY, endX, endY, steps, randomization),
                MouseMotionAlgorithm.PerlinNoise => GeneratePerlinNoise(startX, startY, endX, endY, steps, noiseIntensity),
                _ => GenerateBezier(startX, startY, endX, endY, steps, randomization)
            };
        }

        /// <summary>
        /// Generates cubic Bézier curve path (original implementation).
        /// Formula: B(t) = (1-t)³P₀ + 3(1-t)²tP₁ + 3(1-t)t²P₂ + t³P₃
        /// </summary>
        private static List<Point> GenerateBezier(int startX, int startY, int endX, int endY, int steps, double randomization)
        {
            if (steps <= 0) steps = 1;

            Point p0 = new Point(startX, startY);
            Point p3 = new Point(endX, endY);

            int deltaX = endX - startX;
            int deltaY = endY - startY;
            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            // Control points at 1/3 and 2/3 distance
            int p1x = startX + deltaX / 3;
            int p1y = startY + deltaY / 3;
            int p2x = startX + (2 * deltaX) / 3;
            int p2y = startY + (2 * deltaY) / 3;

            // Add perpendicular randomization
            if (randomization > 0 && distance > 0)
            {
                double perpX = -deltaY / distance;
                double perpY = deltaX / distance;
                double maxOffset = distance * randomization;
                double offset1 = (_random.NextDouble() * 2 - 1) * maxOffset;
                double offset2 = (_random.NextDouble() * 2 - 1) * maxOffset;

                p1x += (int)(perpX * offset1);
                p1y += (int)(perpY * offset1);
                p2x += (int)(perpX * offset2);
                p2y += (int)(perpY * offset2);
            }

            Point p1 = new Point(p1x, p1y);
            Point p2 = new Point(p2x, p2y);

            List<Point> points = new List<Point>(steps + 1);
            for (int i = 0; i <= steps; i++)
            {
                double t = (double)i / steps;
                points.Add(CalculateBezierPoint(p0, p1, p2, p3, t));
            }

            return points;
        }

        /// <summary>
        /// Generates path with Perlin-like noise for organic movement.
        /// Linear interpolation with perpendicular noise offset.
        /// </summary>
        private static List<Point> GeneratePerlinNoise(int startX, int startY, int endX, int endY, int steps, int intensity)
        {
            if (steps <= 0) steps = 1;

            List<Point> points = new List<Point>(steps + 1);
            int deltaX = endX - startX;
            int deltaY = endY - startY;
            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            // Perpendicular vector for noise offset
            double perpX = distance > 0 ? -deltaY / distance : 0;
            double perpY = distance > 0 ? deltaX / distance : 0;

            // Simple Perlin-like noise using sine waves with different frequencies
            double baseFreq = 2.0 * Math.PI / steps;

            for (int i = 0; i <= steps; i++)
            {
                double t = (double)i / steps;

                // Linear interpolation
                int baseX = (int)(startX + t * deltaX);
                int baseY = (int)(startY + t * deltaY);

                // Multi-frequency noise (simplified Perlin)
                double noise = Math.Sin(i * baseFreq) * 0.5 +
                              Math.Sin(i * baseFreq * 2.3) * 0.3 +
                              Math.Sin(i * baseFreq * 5.7) * 0.2;

                // Smooth out at start and end
                double envelope = Math.Sin(t * Math.PI);
                noise *= envelope;

                // Apply perpendicular offset
                int offsetX = (int)(perpX * noise * intensity);
                int offsetY = (int)(perpY * noise * intensity);

                points.Add(new Point(baseX + offsetX, baseY + offsetY));
            }

            return points;
        }

        /// <summary>
        /// Calculates point on cubic Bézier curve at parameter t.
        /// </summary>
        private static Point CalculateBezierPoint(Point p0, Point p1, Point p2, Point p3, double t)
        {
            double oneMinusT = 1 - t;
            double oneMinusT2 = oneMinusT * oneMinusT;
            double oneMinusT3 = oneMinusT2 * oneMinusT;
            double t2 = t * t;
            double t3 = t2 * t;

            int x = (int)(oneMinusT3 * p0.X +
                         3 * oneMinusT2 * t * p1.X +
                         3 * oneMinusT * t2 * p2.X +
                         t3 * p3.X);

            int y = (int)(oneMinusT3 * p0.Y +
                         3 * oneMinusT2 * t * p1.Y +
                         3 * oneMinusT * t2 * p2.Y +
                         t3 * p3.Y);

            return new Point(x, y);
        }
    }
}
