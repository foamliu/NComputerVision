using System;
using System.Collections.Generic;
using System.Text;

namespace NComputerVision.DataStructures
{
    public struct IntPoint
    {
        public int X;

        public int Y;

        public IntPoint(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public double DistanceTo(IntPoint anotherPoint)
        {
            int dx = X - anotherPoint.X;
            int dy = Y - anotherPoint.Y;

            return System.Math.Sqrt(dx * dx + dy * dy);
        }

        public static IntPoint operator +(IntPoint p1, IntPoint p2)
        {
            return new IntPoint(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static IntPoint operator -(IntPoint p1, IntPoint p2)
        {
            return new IntPoint(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static IntPoint operator +(IntPoint p, int valueToAdd)
        {
            return new IntPoint(p.X + valueToAdd, p.Y + valueToAdd);
        }

        public static IntPoint operator -(IntPoint p, int valueToSubtract)
        {
            return new IntPoint(p.X - valueToSubtract, p.Y - valueToSubtract);
        }

        public static IntPoint operator *(IntPoint p, int factor)
        {
            return new IntPoint(p.X * factor, p.Y * factor);
        }

        public static IntPoint operator /(IntPoint p, int factor)
        {
            return new IntPoint(p.X / factor, p.Y / factor);
        }

        public static implicit operator DoublePoint(IntPoint p)
        {
            return new DoublePoint(p.X, p.Y);
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}", X, Y);
        }
    }
}
