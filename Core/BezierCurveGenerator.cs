using System;
using System.Collections.Generic;
using System.Drawing;

namespace SendSequenceCL.Core
{
    /// <summary>
    /// Generates cubic Bézier curves for human-like mouse movement.
    /// Formula: B(t) = (1-t)³P₀ + 3(1-t)²tP₁ + 3(1-t)t²P₂ + t³P₃
    /// </summary>
    internal static class BezierCurveGenerator
    {
        private static readonly Random _random = new Random();

        /// <summary>
        /// Generates a sequence of points along a cubic Bézier curve from start to end.
        /// </summary>
        /// <param name="startX">Starting X coordinate.</param>
        /// <param name="startY">Starting Y coordinate.</param>
        /// <param name="endX">Ending X coordinate.</param>
        /// <param name="endY">Ending Y coordinate.</param>
        /// <param name="steps">Number of intermediate points to generate.</param>
        /// <param name="randomization">Amount of randomization for control points (0.0-1.0).</param>
        /// <returns>List of points along the curve.</returns>
        public static List<Point> Generate(int startX, int startY, int endX, int endY, int steps, double randomization)
        {
            if (steps <= 0)
                steps = 1;

            // P₀ = start point
            Point p0 = new Point(startX, startY);

            // P₃ = end point
            Point p3 = new Point(endX, endY);

            // Calculate control points P₁ and P₂
            // Place them at 1/3 and 2/3 distance with perpendicular offset
            int deltaX = endX - startX;
            int deltaY = endY - startY;
            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            // Control point 1 (1/3 of the way)
            int p1x = startX + deltaX / 3;
            int p1y = startY + deltaY / 3;

            // Control point 2 (2/3 of the way)
            int p2x = startX + (2 * deltaX) / 3;
            int p2y = startY + (2 * deltaY) / 3;

            // Add randomization perpendicular to the movement direction
            if (randomization > 0 && distance > 0)
            {
                // Perpendicular vector (rotate 90 degrees)
                double perpX = -deltaY / distance;
                double perpY = deltaX / distance;

                // Random offset amount (as percentage of distance)
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

            // Generate points along the curve
            List<Point> points = new List<Point>(steps + 1);

            for (int i = 0; i <= steps; i++)
            {
                double t = (double)i / steps;
                Point point = CalculateBezierPoint(p0, p1, p2, p3, t);
                points.Add(point);
            }

            return points;
        }

        /// <summary>
        /// Calculates a point on a cubic Bézier curve at parameter t.
        /// Formula: B(t) = (1-t)³P₀ + 3(1-t)²tP₁ + 3(1-t)t²P₂ + t³P₃
        /// </summary>
        private static Point CalculateBezierPoint(Point p0, Point p1, Point p2, Point p3, double t)
        {
            double oneMinusT = 1 - t;
            double oneMinusT2 = oneMinusT * oneMinusT;
            double oneMinusT3 = oneMinusT2 * oneMinusT;
            double t2 = t * t;
            double t3 = t2 * t;

            // B(t) = (1-t)³P₀ + 3(1-t)²tP₁ + 3(1-t)t²P₂ + t³P₃
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
