
namespace NComputerVision.Image
{
    using System;
    using NComputerVision.GraphicsLib;

    public static class ThresholdLib
    {
        // foamliu, 2009/02/01, 在范围内的点进入ROI：
        //     gmin <= g <= gmax
        // 
        //  去掉的点设为0, 表示不感兴趣(不在ROI).
        //
        public static void ThresholdSimple(int[][] mat, int gmin, int gmax)
        {
            int width = mat.Length;
            int height = mat[0].Length;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (mat[x][y] < gmin || mat[x][y] > gmax)
                    {
                        // foamliu, 2009/01/31, 不感兴趣的部分 0.
                        mat[x][y] = 0;
                    }
                    else
                    {
                        // foamliu, 2009/02/04, 感兴趣的部分 1.
                        mat[x][y] = 255;
                    }
                }
            }

        }

        /// <summary>
        /// foamliu, 2009/02/01, 动态或自适应阈值化, 与局部背景差别大于 gdiff 的点进入ROI.
        /// 
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="gdiff"></param>
        /// <param name="k">用于模糊的K-均值的k</param>
        public static void ThresholdDynamic(int[][] mat, int gdiff, int k)
        {
            int width = mat.Length;
            int height = mat[0].Length;

            int[][] newImg;

            ConvKernel kernel = ConvKernel.GetKMeanKernal(k);

            Convolution conv = new Convolution();
            conv.Calculate(mat, kernel, out newImg);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int g = mat[x][y];
                    int newg = newImg[x][y];
                    if (Math.Abs(g - newg) >= gdiff)
                    {
                        // foamliu, 2009/01/31, 感兴趣的部分1.
                        mat[x][y] = 1;
                    }
                    else
                    {
                        // foamliu, 2009/01/31, 不感兴趣的部分0.
                        mat[x][y] = 0;
                    }
                }

            }
        }

        /// <summary>
        /// 自动阈值化
        /// </summary>
        /// <param name="mat"></param>
        public static void ThresholdAutomatic(int[,] mat, out int[,] newMat)
        {
            int cols = mat.GetLength(0);
            int rows = mat.GetLength(1);
            newMat = new int[cols, rows];

            double threshold = GrayScaleImageLib.mean(mat);
            int d = Math.Sign(mat[cols/2,rows/2]-threshold);

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    if ((mat[x, y] - threshold)*d>0)
                    {
                        newMat[x, y] = 255;
                    }
                    else
                    {
                        newMat[x, y] = 0;
                    }
                }
            }

        }
    }
}
