

namespace NComputerVision.GraphicsLib
{
    using System;
    using NComputerVision.GraphicsLib;
    using NComputerVision.Common;
    using NComputerVision.NcvMath;

    public static class FourierTransform
    {
        /// <summary>
        /// Fourier transformation direction.
        /// </summary>
        public enum Direction
        {
            /// <summary>
            /// Forward direction of Fourier transformation.
            /// </summary>
            Forward = 1,

            /// <summary>
            /// Backward direction of Fourier transformation.
            /// </summary>
            Backward = -1
        };

        /// <summary>
        /// One dimensional Discrete Fourier Transform.
        /// </summary>
        /// 
        /// <param name="data">Data to transform.</param>
        /// <param name="direction">Transformation direction.</param>
        /// 
        public static void DFT(Complex[] data, Direction direction)
        {
            int n = data.Length;
            double arg, cos, sin;
            Complex[] dst = new Complex[n];

            // for each destination element
            for (int i = 0; i < n; i++)
            {
                dst[i] = Complex.Zero;

                arg = -(int)direction * 2.0 * System.Math.PI * (double)i / (double)n;

                // sum source elements
                for (int j = 0; j < n; j++)
                {
                    cos = System.Math.Cos(j * arg);
                    sin = System.Math.Sin(j * arg);

                    dst[i].Re += (data[j].Re * cos - data[j].Im * sin);
                    dst[i].Im += (data[j].Re * sin + data[j].Im * cos);
                }
            }

            // copy elements
            if (direction == Direction.Forward)
            {
                // devide also for forward transform
                for (int i = 0; i < n; i++)
                {
                    data[i].Re = dst[i].Re / n;
                    data[i].Im = dst[i].Im / n;
                }
            }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    data[i].Re = dst[i].Re;
                    data[i].Im = dst[i].Im;
                }
            }
        }

        /// <summary>
        /// Two dimensional Discrete Fourier Transform.
        /// </summary>
        /// 
        /// <param name="data">Data to transform.</param>
        /// <param name="direction">Transformation direction.</param>
        /// 
        public static void DFT2(Complex[,] data, Direction direction)
        {
            int n = data.GetLength(0);	// rows
            int m = data.GetLength(1);	// columns
            double arg, cos, sin;
            Complex[] dst = new Complex[System.Math.Max(n, m)];

            // process rows
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    dst[j] = Complex.Zero;

                    arg = -(int)direction * 2.0 * System.Math.PI * (double)j / (double)m;

                    // sum source elements
                    for (int k = 0; k < m; k++)
                    {
                        cos = System.Math.Cos(k * arg);
                        sin = System.Math.Sin(k * arg);

                        dst[j].Re += (data[i, k].Re * cos - data[i, k].Im * sin);
                        dst[j].Im += (data[i, k].Re * sin + data[i, k].Im * cos);
                    }
                }

                // copy elements
                if (direction == Direction.Forward)
                {
                    // devide also for forward transform
                    for (int j = 0; j < m; j++)
                    {
                        data[i, j].Re = dst[j].Re / m;
                        data[i, j].Im = dst[j].Im / m;
                    }
                }
                else
                {
                    for (int j = 0; j < m; j++)
                    {
                        data[i, j].Re = dst[j].Re;
                        data[i, j].Im = dst[j].Im;
                    }
                }
            }

            // process columns
            for (int j = 0; j < m; j++)
            {
                for (int i = 0; i < n; i++)
                {
                    dst[i] = Complex.Zero;

                    arg = -(int)direction * 2.0 * System.Math.PI * (double)i / (double)n;

                    // sum source elements
                    for (int k = 0; k < n; k++)
                    {
                        cos = System.Math.Cos(k * arg);
                        sin = System.Math.Sin(k * arg);

                        dst[i].Re += (data[k, j].Re * cos - data[k, j].Im * sin);
                        dst[i].Im += (data[k, j].Re * sin + data[k, j].Im * cos);
                    }
                }

                // copy elements
                if (direction == Direction.Forward)
                {
                    // devide also for forward transform
                    for (int i = 0; i < n; i++)
                    {
                        data[i, j].Re = dst[i].Re / n;
                        data[i, j].Im = dst[i].Im / n;
                    }
                }
                else
                {
                    for (int i = 0; i < n; i++)
                    {
                        data[i, j].Re = dst[i].Re;
                        data[i, j].Im = dst[i].Im;
                    }
                }
            }
        }


        /// <summary>
        /// One dimensional Fast Fourier Transform.
        /// </summary>
        /// 
        /// <param name="data">Data to transform.</param>
        /// <param name="direction">Transformation direction.</param>
        /// 
        /// <remarks><para><note>The method accepts <paramref name="data"/> array of 2<sup>n</sup> size
        /// only, where <b>n</b> may vary in the [1, 14] range.</note></para></remarks>
        /// 
        /// <exception cref="ArgumentException">Incorrect data length.</exception>
        /// 
        public static void FFT(Complex[] data, Direction direction)
        {
            int n = data.Length;
            int m = Tools.Log2(n);

            // reorder data first
            ReorderData(data);

            // compute FFT
            int tn = 1, tm;

            for (int k = 1; k <= m; k++)
            {
                Complex[] rotation = FourierTransform.GetComplexRotation(k, direction);

                tm = tn;
                tn <<= 1;

                for (int i = 0; i < tm; i++)
                {
                    Complex t = rotation[i];

                    for (int even = i; even < n; even += tn)
                    {
                        int odd = even + tm;
                        Complex ce = data[even];
                        Complex co = data[odd];

                        double tr = co.Re * t.Re - co.Im * t.Im;
                        double ti = co.Re * t.Im + co.Im * t.Re;

                        data[even].Re += tr;
                        data[even].Im += ti;

                        data[odd].Re = ce.Re - tr;
                        data[odd].Im = ce.Im - ti;
                    }
                }
            }

            if (direction == Direction.Forward)
            {
                for (int i = 0; i < n; i++)
                {
                    data[i].Re /= (double)n;
                    data[i].Im /= (double)n;
                }
            }
        }

        /// <summary>
        /// Two dimensional Fast Fourier Transform.
        /// </summary>
        /// 
        /// <param name="data">Data to transform.</param>
        /// <param name="direction">Transformation direction.</param>
        /// 
        /// <remarks><para><note>The method accepts <paramref name="data"/> array of 2<sup>n</sup> size
        /// only in each dimension, where <b>n</b> may vary in the [1, 14] range. For example, 16x16 array
        /// is valid, but 15x15 is not.</note></para></remarks>
        /// 
        /// <exception cref="ArgumentException">Incorrect data length.</exception>
        /// 
        public static void FFT2(Complex[,] data, Direction direction)
        {
            int k = data.GetLength(0);
            int n = data.GetLength(1);

            // check data size
            if (
                (!Tools.IsPowerOf2(k)) ||
                (!Tools.IsPowerOf2(n)) ||
                (k < minLength) || (k > maxLength) ||
                (n < minLength) || (n > maxLength)
                )
            {
                throw new ArgumentException("Incorrect data length.");
            }

            // process rows
            Complex[] row = new Complex[n];

            for (int i = 0; i < k; i++)
            {
                // copy row
                for (int j = 0; j < n; j++)
                    row[j] = data[i, j];
                // transform it
                FourierTransform.FFT(row, direction);
                // copy back
                for (int j = 0; j < n; j++)
                    data[i, j] = row[j];
            }

            // process columns
            Complex[] col = new Complex[k];

            for (int j = 0; j < n; j++)
            {
                // copy column
                for (int i = 0; i < k; i++)
                    col[i] = data[i, j];
                // transform it
                FourierTransform.FFT(col, direction);
                // copy back
                for (int i = 0; i < k; i++)
                    data[i, j] = col[i];
            }
        }

        #region Private Region

        private const int minLength = 2;
        private const int maxLength = 16384;
        private const int minBits = 1;
        private const int maxBits = 14;
        private static int[][] reversedBits = new int[maxBits][];
        private static Complex[,][] complexRotation = new Complex[maxBits, 2][];

        // Get array, indicating which data members should be swapped before FFT
        private static int[] GetReversedBits(int numberOfBits)
        {
            if ((numberOfBits < minBits) || (numberOfBits > maxBits))
                throw new ArgumentOutOfRangeException();

            // check if the array is already calculated
            if (reversedBits[numberOfBits - 1] == null)
            {
                int n = Tools.Pow2(numberOfBits);
                int[] rBits = new int[n];

                // calculate the array
                for (int i = 0; i < n; i++)
                {
                    int oldBits = i;
                    int newBits = 0;

                    for (int j = 0; j < numberOfBits; j++)
                    {
                        newBits = (newBits << 1) | (oldBits & 1);
                        oldBits = (oldBits >> 1);
                    }
                    rBits[i] = newBits;
                }
                reversedBits[numberOfBits - 1] = rBits;
            }
            return reversedBits[numberOfBits - 1];
        }

        // Get rotation of complex number
        private static Complex[] GetComplexRotation(int numberOfBits, Direction direction)
        {
            int directionIndex = (direction == Direction.Forward) ? 0 : 1;

            // check if the array is already calculated
            if (complexRotation[numberOfBits - 1, directionIndex] == null)
            {
                int n = 1 << (numberOfBits - 1);
                double uR = 1.0;
                double uI = 0.0;
                double angle = System.Math.PI / n * (int)direction;
                double wR = System.Math.Cos(angle);
                double wI = System.Math.Sin(angle);
                double t;
                Complex[] rotation = new Complex[n];

                for (int i = 0; i < n; i++)
                {
                    rotation[i] = new Complex(uR, uI);
                    t = uR * wI + uI * wR;
                    uR = uR * wR - uI * wI;
                    uI = t;
                }

                complexRotation[numberOfBits - 1, directionIndex] = rotation;
            }
            return complexRotation[numberOfBits - 1, directionIndex];
        }

        // Reorder data for FFT using
        private static void ReorderData(Complex[] data)
        {
            int len = data.Length;

            // check data length
            if ((len < minLength) || (len > maxLength) || (!Tools.IsPowerOf2(len)))
                throw new ArgumentException("Incorrect data length.");

            int[] rBits = GetReversedBits(Tools.Log2(len));

            for (int i = 0; i < len; i++)
            {
                int s = rBits[i];

                if (s > i)
                {
                    Complex t = data[i];
                    data[i] = data[s];
                    data[s] = t;
                }
            }
        }

        #endregion

        /// <summary>
        /// foamliu, 2009/02/04, 二维离散傅立叶变换.
        /// 
        /// 主要参照:
        /// 
        ///     1. 《数字图像处理(MATLAB版)》, 4.1节.
        ///     2. 维基百科 － 复数： http://en.wikipedia.org/wiki/Complex_number.
        ///     3. 《机器视觉算法与应用》.
        /// 
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        public static int[][] Transform(int[][] mat, Direction dire)
        {
            int width = mat.Length;
            int height = mat[0].Length;

            int fft_width = width;
            int fft_height = height;

            if (!Tools.IsPowerOf2(fft_width))
            {
                fft_width = Tools.Pow2(Convert.ToInt32(Tools.Log2(fft_width) + 1));
            }

            if (!Tools.IsPowerOf2(fft_height))
            {
                fft_height = Tools.Pow2(Convert.ToInt32(Tools.Log2(fft_height) + 1));
            }

            int[][] filtered = Util.BuildMatInt(fft_width, fft_height);

            Complex[,] data = new Complex[fft_width, fft_height];

            for (int y = 0; y < fft_height; y++)
            {
                for (int x = 0; x < fft_width; x++)
                {
                    if (x < width && y < height)
                    {
                        data[x, y].Re = mat[x][y];
                        data[x, y].Im = 0;
                    }
                    else
                    {
                        data[x, y].Re = 0;
                        data[x, y].Im = 0;
                    }

                }
            }

            FFT2(data, dire);

            for (int y = 0; y < fft_height; y++)
            {
                for (int x = 0; x < fft_width; x++)
                {
                    double re = data[x, y].Re;
                    double im = data[x, y].Im;

                    // 1-范数, 复数坐标系下向量长度.
                    double z = Math.Sqrt(re * re + im * im);
                    filtered[x][y] = System.Convert.ToInt32(z);
                }
            }

            // 二维离散傅立叶变换可表示为 F(u,v).
            //
            //int[][] F = Util.BuildMatInt(width, height);

            //for (int v = -height / 2; v < height / 2; v++)
            //{
            //    for (int u = -width / 2; u < width / 2; u++)
            //    {
            //        // 实部 (real part)
            //        double real = 0;
            //        // 虚部 (imaginary part)
            //        double imaginary = 0;

            //        for (int y = 0; y < height; y++)
            //        {
            //            for (int x = 0; x < width; x++)
            //            {
            //                double phi = 2 * Math.PI * (1.0 * u * x / width + 1.0 * v * y / height);

            //                double fxy = mat[x][y];
            //                real += fxy * Math.Cos(phi);
            //                imaginary += fxy * Math.Sin(phi);
            //            }
            //        }

            //        realpart[u + width / 2][v + height / 2] = real;
            //        imaginarypart[u + width / 2][v + height / 2] = imaginary;

            //        // 1-范数, 复数坐标系下向量长度.
            //        double z = Math.Sqrt(real * real + imaginary * imaginary);
            //        //F[u][v] = (int)Math.Round(z);                    
            //        F[u + width / 2][v + height / 2] = System.Convert.ToInt32(z);
            //    }
            //}

            return filtered;
        }

        /// <summary>
        /// foamliu, 2009/02/04, 二维离散傅立叶逆变换.
        /// 
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        //public static int[][] InverseFourierTransform(int[][] mat, double[][] realpart, double[][] imaginarypart)
        //{
        //    int width = mat.Length;
        //    int height = mat[0].Length;

        //    // 原来的图像可表示为 f(x,y).
        //    //
        //    int[][] f = Util.BuildMatInt(width, height);

        //    for (int y = 0; y < height; y++)
        //    {
        //        for (int x = 0; x < width; x++)
        //        {
        //            // 实部 (real part)
        //            double real = 0;
        //            // 虚部 (imaginary part)
        //            double imaginary = 0;

        //            for (int v = -height / 2; v < height / 2; v++)
        //            {
        //                for (int u = -width / 2; u < width / 2; u++)
        //                {
        //                    double phi = -2 * Math.PI * (1.0 * u * x / width + 1.0 * v * y / height);

        //                    //double Fuv = mat[u + width / 2][v + height / 2];
        //                    //real += Fuv * Math.Cos(phi);
        //                    //imaginary += Fuv * Math.Sin(phi);
        //                    real += realpart[u + width / 2][v + height / 2] * Math.Cos(phi) - imaginarypart[u + width / 2][v + height / 2] * Math.Sin(phi);
        //                    imaginary += realpart[u + width / 2][v + height / 2] * Math.Sin(phi) + imaginarypart[u + width / 2][v + height / 2] * Math.Cos(phi);
        //                }
        //            }

        //            real = real / (width * height);
        //            imaginary = imaginary / (width * height);

        //            // 1-范数, 复数坐标系下向量长度.
        //            double z = Math.Sqrt(real * real + imaginary * imaginary);
        //            f[x][y] = System.Convert.ToInt32(z);
        //        }
        //    }

        //    return f;
        //}

        /// <summary>
        /// foamliu, 2009/02/09, 二维快速傅立叶变换.
        /// 
        /// 主要参照:
        ///     1. "The Fast Fourier Transform" 这个幻灯片：
        ///        http://www.cs.drexel.edu/~jjohnson/sp04/cs300/lectures/lec10.html.
        /// </summary>
        /// <param name="A"></param>
        /// <param name="root"></param>
        /// <param name="start">开始位置</param>
        /// <param name="length">长度, 2的幂</param>
        /// <param name="Y"></param>
        //public static void FFT(int[] A, int[] root, int start, int length, int[] Y)
        //{
        //    if (length == 1)
        //    {
        //        Y[start] = A[start];
        //        return;
        //    }

        //    // 计算 Y[start] 到 Y[start+n/2-1]
        //    FFT(A, root, start, length / 2, Y);
        //    // 计算 Y[start+n/2] 到 Y[start+n-1]
        //    FFT(A, root, start + length / 2, length / 2, Y);

        //    for (int i = 0; i < length / 2; i++)
        //    {

        //    }
        //}
    }
}
