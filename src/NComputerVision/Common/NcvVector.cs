using System;
using System.Collections.Generic;
using System.Text;
using NComputerVision.DataStructures;

namespace NComputerVision.Common
{
    public class NcvVector
    {
        public int n;
        protected double[] values;

        public NcvVector(int n)
        {
            this.n = n;
            values = new double[n];
        }

        public NcvVector(double[] data)
        {
            this.n = data.Length;
            values = data;
        }

        public double this[int i]
        {
            get { return values[i]; }
            set { values[i] = value; }
        }

        public double[] Data
        {
            get
            {
                return values;
            }
        }

        //public double Get(int i)
        //{
        //    return values[i];
        //}

        //public void Set(int i, double value)
        //{
        //    values[i] = value;
        //}

        public static NcvVector operator +(NcvVector v1, NcvVector v2)
        {
            NcvVector v3 = new NcvVector(v1.n);
            // v3 <- v1 + v2
            for (int i = 0; i < v1.n; i++)
            {
                v3[i] = v1[i] + v2[i];
            }
            return v3;
        }

        public static NcvVector operator -(NcvVector v1, NcvVector v2)
        {
            NcvVector v3 = new NcvVector(v1.n);
            // v3 <- v1 + v2
            for (int i = 0; i < v1.n; i++)
            {
                v3[i] = v1[i] - v2[i];

            }
            return v3;
        }

        public static NcvVector operator -(NcvVector v1)
        {
            NcvVector v2 = new NcvVector(v1.n);

            for (int i = 0; i < v1.n; i++)
            {
                v2[i] = -v1[i];
            }
            return v2;
        }

        public static NcvVector operator *(NcvVector v1, double s2)
        {
            NcvVector v3 = new NcvVector(v1.n);
            for (int i = 0; i < v1.n; i++)
            {
                v3[i] = v1[i] * s2;
            }
            return v3;
        }

        public static NcvVector operator *(double s1, NcvVector v2)
        {
            return v2 * s1;
        }

        public static NcvVector DotProduct(NcvVector v1, NcvVector v2)
        {
            NcvVector v3 = new NcvVector(v1.n);
            for (int i = 0; i < v1.n; i++)
            {
                v3[i] = v1[i] * v2[i];
            }
            return v3;
        }

        public static NcvVector DotDivide(NcvVector v1, NcvVector v2)
        {
            NcvVector v3 = new NcvVector(v1.n);
            for (int i = 0; i < v1.n; i++)
            {
                v3[i] = v1[i] / v2[i];
            }
            return v3;
        }

        public static NcvVector Sqrt(NcvVector v1)
        {
            NcvVector v2 = new NcvVector(v1.n);
            for (int i = 0; i < v1.n; i++)
            {
                v2[i] = Math.Sqrt(v1[i]);
            }
            return v2;
        }


        public NcvVector sub(int startIndex, int length)
        {
            if (startIndex + length > this.n)
                throw new ArgumentOutOfRangeException();

            NcvVector v = new NcvVector(length);
            for (int i = 0; i < length; i++)
            {
                v[i] = this[startIndex + i];
            }
            return v;
        }

        public static NcvVector cat(NcvVector v1, NcvVector v2)
        {
            NcvVector v3 = new NcvVector(v1.n + v2.n);
            for (int i = 0; i < v1.n; i++)
                v3[i] = v1[i];
            for (int j = 0; j < v2.n; j++)
                v3[v1.n + j] = v2[j];
            return v3;
        }

        public static NcvVector Ones(int n)
        {
            NcvVector ones = new NcvVector(n);
            for (int i = 0; i < ones.n; i++)
            {
                ones[i] = 1.0;
            }
            return ones;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Vector\n");
            sb.Append("[");
            for (int i = 0; i < n; i++)
            {
                sb.Append(this[i] + ", ");
            }
            sb.Append("]");
            return sb.ToString();
        }

        public NcvVector abs()
        {
            for (int i = 0; i < n; i++)
            {
                this[i] = Math.Abs(this[i]);
            }
            return this;
        }

        public double max()
        {
            double value = double.MinValue;
            for (int i = 0; i < n; i++)
            {
                if (value < this[i])
                    value = this[i];
            }
            return value;
        }

        public double min()
        {
            double value = double.MaxValue;
            for (int i = 0; i < n; i++)
            {
                if (value > this[i])
                    value = this[i];
            }
            return value;
        }

        /// <summary>
        /// 双线性插值
        /// p00 ------------- p01
        ///  |                 |
        /// p10 ------------- p11
        /// 
        /// TODO: handle boundary.
        /// </summary>
        /// <param name="f">二维图</param>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static NcvVector interp2(double[] f, int cols, int rows, NcvVector x, NcvVector y, string method)
        {
            int n = x.n;
            NcvVector vf = new NcvVector(n);
            if (method.Equals("*linear", StringComparison.OrdinalIgnoreCase))
            {
                for (int k = 0; k < n; k++)
                {
                    int p00Y = (int)Math.Floor(y[k]);
                    int p00X = (int)Math.Floor(x[k]);
                    int p01Y = (int)Math.Floor(y[k]);
                    int p01X = (int)Math.Ceiling(x[k]);
                    int p10Y = (int)Math.Ceiling(y[k]);
                    int p10X = (int)Math.Floor(x[k]);
                    int p11Y = (int)Math.Ceiling(y[k]);
                    int p11X = (int)Math.Ceiling(x[k]);

                    double a = y[k] - p00Y;
                    double b = x[k] - p00X;
                    double f00 = f[p00X * rows + p00Y];
                    double f01 = f[p01X * rows + p01Y];
                    double f10 = f[p10X * rows + p10Y];
                    double f11 = f[p11X * rows + p11Y];
                    double g = b * (a * f11 + (1 - a) * f01) + (1 - b) * (a * f01 + (1 - a) * f00);

                    //int i = round(x[k]) * m_Rows + round(y[k]);
                    vf[k] = g;
                }
            }
            return vf;
        }

        public static double[] interp1(double[] x, double[] Y, double[] xi)
        {
            int len = x.Length;
            int leni = xi.Length;
            double[] yi = new double[leni];

            for (int i = 0; i < leni; i++)
            {
                for (int j = 1; j < len; j++)
                {
                    int j_1 = j - 1;
                    double xx = xi[i];
                    double x1 = x[j_1];
                    double x2 = x[j];
                    double y1 = Y[j_1];
                    double y2 = Y[j];

                    if (between(xx, x1, x2))
                    {
                        yi[i] = y1 + (xx - x1) / (x2 - x1) * (y2 - y1);
                        if (double.IsNaN(yi[i]))
                            System.Diagnostics.Debugger.Launch();
                        break;
                    }
                }
            }
            return yi;
        }

        // v1 <= v2
        private static bool between(double v, double v1, double v2)
        {
            return (v >= v1 && v < v2);
        }

        public static NcvVector interp1(NcvVector x, NcvVector Y, NcvVector xi)
        {
            return new NcvVector(interp1(x.Data, Y.Data, xi.Data));
        }

        public static List<DoublePoint> interp1(List<DoublePoint> contour, int leni)
        {
            List<DoublePoint> points = new List<DoublePoint>();
            int len = contour.Count;

            if (len == 0)
            {
                for (int i = 0; i < leni; i++)
                {
                    points.Add(new DoublePoint(0, 0));
                }
                return points;
            }

            double[] X = new double[len];
            double[] xi = new double[leni];
            double[] Yx = new double[len];
            double[] Yy = new double[len];

            for (int i = 0; i < len; i++)
            {
                X[i] = i;
                Yx[i] = contour[i].X;
                Yy[i] = contour[i].Y;
            }
            for (int i = 0; i < leni; i++)
            {
                xi[i] = 1.0 * i * len / leni;
            }

            double[] yix = interp1(X, Yx, xi);
            double[] yiy = interp1(X, Yy, xi);

            for (int i = 0; i < leni; i++)
            {
                points.Add(new DoublePoint(yix[i], yiy[i]));
            }

            return points;
        }
    }

}
