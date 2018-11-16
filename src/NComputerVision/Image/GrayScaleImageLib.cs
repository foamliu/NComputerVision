

namespace NComputerVision.Image
{
    using System;
    using System.Collections.Generic;
    using NComputerVision.Common;
    using NComputerVision.DataStructures;
    using NComputerVision.GraphicsLib;

    /// <summary>
    /// foamliu, 2009/02/06, 关于灰度图像的一些函数.
    /// 
    /// </summary>
    public static class GrayScaleImageLib
    {
        /// <summary>
        /// foamiu, 2009/02/06, 闵可夫斯基减法.
        /// 结构元必须全部落在区域内.
        /// 
        /// </summary>
        /// <param name="matR"></param>
        /// <param name="matS"></param>
        /// <param name="ox">原点在S中的位置</param>
        /// <param name="oy">原点在S中的位置</param>
        public static void MinkowskiSubtraction(int[][] matR, int[][] matS, int ox, int oy, out int[][] matResult)
        {
            int wR = matR.Length;
            int hR = matR[0].Length;
            int wS = matS.Length;
            int hS = matS[0].Length;

            matResult = Util.BuildMatInt(wR, hR);

            for (int y = 0; y < hR; y++)
            {
                for (int x = 0; x < wR; x++)
                {
                    int min = int.MaxValue;

                    for (int j = 0; j < hS; j++)
                    {
                        for (int i = 0; i < wS; i++)
                        {
                            if (matS[i][j] == 0) continue;

                            // 矢量和在 R 中的位置
                            //
                            int sumx = x + i - ox;
                            int sumy = y + j - oy;

                            int matxy = Util.GetPixel(matR, sumx, sumy);
                            if (matxy /*+ matS[i][j]*/ < min)
                            {
                                min = matxy /*+ matS[i][j]*/;
                            }
                        }
                    }

                    matResult[x][y] = min;
                }
            }

        }

        public static void MinkowskiSubtraction(int[][] mat, StructuringElement strel, out int[][] matResult)
        {
            MinkowskiSubtraction(mat, strel.Mat, strel.Ox, strel.Oy, out matResult);
        }

        public static int[][] MinkowskiSubtraction(int[][] mat, StructuringElement strel)
        {
            int[][] matResult;
            MinkowskiSubtraction(mat, strel.Mat, strel.Ox, strel.Oy, out matResult);
            return matResult;
        }

        /// <summary>
        /// foamiu, 2009/02/06, 闵可夫斯基加法.
        /// 结构元必须至少与区域存在一个公共点.
        /// 
        /// </summary>
        /// <param name="matR"></param>
        /// <param name="matS"></param>
        /// <param name="ox">原点在S中的位置</param>
        /// <param name="oy">原点在S中的位置</param>
        public static void MinkowskiAddtion(int[][] matR, int[][] matS, int ox, int oy, out int[][] matResult)
        {
            int wR = matR.Length;
            int hR = matR[0].Length;
            int wS = matS.Length;
            int hS = matS[0].Length;

            matResult = Util.BuildMatInt(wR, hR);

            for (int y = 0; y < hR; y++)
            {
                for (int x = 0; x < wR; x++)
                {
                    int max = int.MinValue;

                    for (int j = 0; j < hS; j++)
                    {
                        for (int i = 0; i < wS; i++)
                        {
                            if (matS[i][j] == 0) continue;

                            // 矢量和在 R 中的位置
                            //
                            int sumx = x + i - ox;
                            int sumy = y + j - oy;

                            int matxy = Util.GetPixel(matR, sumx, sumy);
                            if (matxy /*+ matS[i][j]*/ > max)
                            {
                                max = matxy /*+ matS[i][j]*/;
                            }
                        }
                    }

                    matResult[x][y] = max;
                }
            }
        }

        public static void MinkowskiAddtion(int[][] mat, StructuringElement strel, out int[][] matResult)
        {
            MinkowskiAddtion(mat, strel.Mat, strel.Ox, strel.Oy, out matResult);
        }

        public static int[][] MinkowskiAddtion(int[][] mat, StructuringElement strel)
        {
            int[][] matResult;
            MinkowskiAddtion(mat, strel.Mat, strel.Ox, strel.Oy, out matResult);
            return matResult;
        }

        /// <summary>
        /// foamliu, 2009/02/04, 灰度值膨胀.
        /// 
        /// </summary>
        /// <param name="mat">灰度图像</param>
        /// <param name="b">结构元素</param>
        public static void Dilation(int[][] mat, StructuringElement strel, out int[][] output)
        {
            //int width = mat.Length;
            //int height = mat[0].Length;

            //output = new int[width][];

            //for (int i = 0; i < width; i++)
            //{
            //    output[i] = new int[height];
            //}            

            //int m = b.Length;
            //int n = b[0].Length;

            //for (int y = 0; y < height; y++)
            //{
            //    for (int x = 0; x < width; x++)
            //    {
            //        int max = int.MinValue;
            //        for (int j = 0; j < n; j++)
            //        {
            //            for (int i = 0; i < m; i++)
            //            {
            //                int matxy = Util.GetPixel(mat, x - i, y - j);
            //                if (matxy + b[i][j] > max)
            //                {
            //                    max = matxy + b[i][j];
            //                }
            //            }
            //        }

            //        output[x][y] = max;
            //    }
            //}


            // foamliu, 2009/02/06, 改用二值图像库中的算法.
            //
            // 先把 strel 求转置
            //
            StructuringElement trans = StructuringElement.Transposition(strel);

            GrayScaleImageLib.MinkowskiAddtion(mat, trans, out output);
        }

        /// <summary>
        /// foamliu, 2009/02/04, 灰度值腐蚀.
        /// 
        /// </summary>
        /// <param name="mat">灰度图像</param>
        /// <param name="b">结构元素</param>
        public static void Erosion(int[][] mat, StructuringElement strel, out int[][] output)
        {
            //int width = mat.Length;
            //int height = mat[0].Length;

            //output = new int[width][];

            //for (int i = 0; i < width; i++)
            //{
            //    output[i] = new int[height];
            //}

            //int m = b.Length;
            //int n = b[0].Length;

            //for (int y = 0; y < height; y++)
            //{
            //    for (int x = 0; x < width; x++)
            //    {
            //        int min = int.MaxValue;
            //        for (int j = 0; j < n; j++)
            //        {
            //            for (int i = 0; i < m; i++)
            //            {
            //                int matxy = Util.GetPixel(mat, x - i, y - j);
            //                if (matxy + b[i][j] < min)
            //                {
            //                    min = matxy + b[i][j];
            //                }
            //            }
            //        }

            //        output[x][y] = min;
            //    }
            //}


            // foamliu, 2009/02/06, 改用二值图像库中的算法.
            //
            // 先把 strel 求转置
            //
            StructuringElement trans = StructuringElement.Transposition(strel);

            GrayScaleImageLib.MinkowskiSubtraction(mat, trans, out output);
        }

        /// <summary>
        /// foamliu, 2009/02/09, 灰度图像取反
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static void Inverse(int[][] mat)
        {
            int width = mat.Length;
            int height = mat[0].Length;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    mat[x][y] = 255 - mat[x][y];
                }
            }
        }

        /// <summary>
        /// foamliu, 2009/02/04, 开操作.
        /// 先进行腐蚀操作再紧接着进行一个使用同样结构元的闵可夫斯基加法.
        /// 
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="b"></param>
        /// <param name="output"></param>
        public static void Open(int[][] mat, StructuringElement strel, out int[][] output)
        {
            int[][] temp;
            Erosion(mat, strel, out temp);
            GrayScaleImageLib.MinkowskiAddtion(temp, strel, out output);
        }

        // foamliu, 2009/02/04, 开操作.
        //
        public static int[][] Open(int[][] mat, StructuringElement strel)
        {
            int[][] temp, output;
            Erosion(mat, strel, out temp);
            GrayScaleImageLib.MinkowskiAddtion(temp, strel, out output);
            return output;
        }

        /// <summary>
        /// foamliu, 2009/02/04, 闭操作.
        /// 先执行一个膨胀操作后紧接着再用同一个结构元进行闵可夫斯基减法.
        /// 
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="b"></param>
        /// <param name="output"></param>
        public static void Close(int[][] mat, StructuringElement strel, out int[][] output)
        {
            int[][] temp;
            Dilation(mat, strel, out temp);
            GrayScaleImageLib.MinkowskiSubtraction(temp, strel, out output);
        }

        public static int[][] Close(int[][] mat, StructuringElement strel)
        {
            int[][] output;
            Close(mat, strel, out output);
            return output;
        }

        public static void SubpixelContour(int[][] mat, int gsub, out List<SubpixelLineSegment> output)
        {
            int width = mat.Length;
            int height = mat[0].Length;

            output = new List<SubpixelLineSegment>();

            for (int y = 0; y < height - 1; y++)
            {
                for (int x = 0; x < width - 1; x++)
                {
                    List<DoublePoint> line = new List<DoublePoint>();
                    int g00 = mat[x][y];
                    int g01 = mat[x][y + 1];
                    int g10 = mat[x + 1][y];
                    int g11 = mat[x + 1][y + 1];
                    if ((g00 - gsub) * (gsub - g10) > 0 && g00 != g10)
                    {
                        line.Add(new DoublePoint(x + 1.0f * (g00 - gsub) / (g00 - g10), y));
                    }
                    if ((g00 - gsub) * (gsub - g01) > 0 && g00 != g01)
                    {
                        line.Add(new DoublePoint(x, y + 1.0f * (g00 - gsub) / (g00 - g01)));
                    }
                    if ((g10 - gsub) * (gsub - g11) > 0 && g10 != g11)
                    {
                        line.Add(new DoublePoint(x + 1.0f * (g01 - gsub) / (g10 - g11), y + 1));
                    }
                    if ((g01 - gsub) * (gsub - g11) > 0 && g01 != g11)
                    {
                        line.Add(new DoublePoint(x + 1, y + 1.0f * (g01 - gsub) / (g01 - g11)));
                    }
                    if (line.Count >= 2)
                    {
                        output.Add(new SubpixelLineSegment(line[0], line[1]));
                    }
                }
            }
        }

        /// <summary>
        /// foamliu, 2009/02/03, 提出来比较好.
        /// 
        /// </summary>
        /// <param name="mat"></param>
        public static void Normalize(int[][] mat)
        {
            int width = mat.Length;
            int height = mat[0].Length;

            // 第一遍， 记录最大最小值.

            int min = Int32.MaxValue;
            int max = Int32.MinValue;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (mat[x][y] > max)
                        max = mat[x][y];
                    if (mat[x][y] < min)
                        min = mat[x][y];
                }
            }

            // 第二遍，归一化.

            if (max != min)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        mat[x][y] = 255 * (mat[x][y] - min) / (max - min);
                    }

                }
            }
        }

        /// <summary>
        /// foamliu, 2009/02/03, 鲁棒的(这个词好怪 -_-!!)正则化.
        /// 比如 pl=0.1, pu=0.99.
        /// 
        /// </summary>
        /// <param name="mat"></param>
        public static void RobustNormalize(int[][] mat, double pl, double pu)
        {
            int width = mat.Length;
            int height = mat[0].Length;

            // 第一遍， 取得最大最小值.           

            int min = 0, max = 255;

            double[] cumuHist = GrayScaleImageLib.CumulativeHistogram(mat);

            for (int i = 0; i < 256; i++)
            {
                if (cumuHist[i] > pl)
                {
                    min = i - 1;
                    break;
                }
            }

            for (int i = 255; i >= 0; i--)
            {
                if (cumuHist[i] < pu)
                {
                    max = i + 1;
                    break;
                }
            }


            // 第二遍，归一化.

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int grayScale = mat[x][y];
                    if (grayScale > max) grayScale = max;
                    if (grayScale < min) grayScale = min;
                    mat[x][y] = 255 * (grayScale - min) / (max - min);
                }

            }
        }

        /// <summary>
        /// foamliu, 2009/02/03, 灰度直方图.
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static double[] Histogram(int[][] mat)
        {
            double[] histogram = new double[256];
            for (int i = 0; i < histogram.Length; i++)
                histogram[i] = 0;

            int width = mat.Length;
            int height = mat[0].Length;

            double dot = 1.0 / (width * height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    histogram[mat[x][y]] += dot;
                }
            }

            return histogram;
        }

        /// <summary>
        /// foamliu, 2009/02/03, 累积灰度直方图.
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static double[] CumulativeHistogram(int[][] mat)
        {
            double[] hist = GrayScaleImageLib.Histogram(mat);
            double[] cumu = new double[256];
            double sum = 0.0;

            for (int i = 0; i < 256; i++)
            {
                sum += hist[i];
                cumu[i] = sum;
            }

            return cumu;
        }

        /// <summary>
        /// foamliu, 2009/02/11, 垂直扫描线平均灰度在X轴上变化图.
        /// 
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        public static double[] IntensityDistributionX(int[][] mat)
        {
            int width = mat.Length;
            int height = mat[0].Length;

            double[] idx = new double[width];

            for (int x = 0; x < width; x++)
            {
                idx[x] = 0;
                for (int y = 0; y < height; y++)
                {
                    idx[x] += mat[x][y];
                }
                // 取平均灰度并归一到 [0,255]区间
                idx[x] /= height * 256;
            }

            return idx;
        }

        /// <summary>
        /// foamliu, 2009/02/11, 水平扫描线平均灰度在Y轴上变化图.
        /// 
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        public static double[] IntensityDistributionY(int[][] mat)
        {
            int width = mat.Length;
            int height = mat[0].Length;

            double[] idy = new double[height];

            for (int y = 0; y < height; y++)
            {
                idy[y] = 0;
                for (int x = 0; x < width; x++)
                {
                    idy[y] += mat[x][y];
                }
                // 取平均灰度并归一到 [0,255]区间
                idy[y] /= width * 256;
            }

            return idy;
        }

        /// <summary>
        /// 灰度均值
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        public static double mean(int[,] mat)
        {
            int cols = mat.GetLength(0);
            int rows = mat.GetLength(1);
            double sum = 0.0;

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    sum += mat[x, y];
                }
            }

            return sum / (cols * rows);
        }


    }
}
