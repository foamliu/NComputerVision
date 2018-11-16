

using System;
namespace NComputerVision.DataStructures
{

    /// <summary>
    /// foamiu, 2009/02/17, 亚像素的点, 也就是可为浮点数.
    /// 
    /// </summary>
    public struct DoublePoint
    {
        public double X;
        public double Y;

        public static double epsilon = 0.00001;

        public DoublePoint(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public static DoublePoint operator +(DoublePoint p1, DoublePoint p2)
        {
            return new DoublePoint(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static DoublePoint operator -(DoublePoint p1, DoublePoint p2)
        {
            return new DoublePoint(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static DoublePoint operator +(DoublePoint p, double valueToAdd)
        {
            return new DoublePoint(p.X + valueToAdd, p.Y + valueToAdd);
        }

        public static DoublePoint operator -(DoublePoint p, double valueToSubtract)
        {
            return new DoublePoint(p.X - valueToSubtract, p.Y - valueToSubtract);
        }

        public static DoublePoint operator *(DoublePoint p, double factor)
        {
            return new DoublePoint(p.X * factor, p.Y * factor);
        }

        public static DoublePoint operator /(DoublePoint p, double factor)
        {
            return new DoublePoint(p.X / factor, p.Y / factor);
        }

        public static bool operator ==(DoublePoint p1, DoublePoint p2)
        {
            return DoubleEquals(p1.X, p2.X) && DoubleEquals(p1.Y, p2.Y);
        }

        public static bool operator !=(DoublePoint p1, DoublePoint p2)
        {
            return !(p1 == p2);
        }

        public static explicit operator IntPoint(DoublePoint p)
        {
            return new IntPoint((int)p.X, (int)p.Y);
        }

        private static bool DoubleEquals(double d1, double d2)
        {
            return System.Math.Abs(d1 - d2) < epsilon;
        }

        public double Magnitude
        {
            get
            {
                return Math.Sqrt(X * X + Y * Y);
            }
        }

        public override bool Equals(object obj)
        {
            DoublePoint that = (DoublePoint)obj;
            return this == that;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("x={0} y={1}", this.X, this.Y);
        }
    }
}
