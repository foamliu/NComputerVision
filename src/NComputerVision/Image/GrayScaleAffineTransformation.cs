
namespace NComputerVision.Image
{
    using System;
    using System.Drawing;
    using NComputerVision.Common;
    using NComputerVision.GraphicsLib;    
    using NComputerVision.DataStructures; 

    public static class GrayScaleAffineTransformation// : AffineTransformationBase
    {
        /// <summary>
        /// foamliu, 2009/01/31, 平移.
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="tr"></param>
        /// <param name="tc"></param>
        /// <returns></returns>
        public static Bitmap Translate(Bitmap bmp, double tr, double tc)
        {
            int width, height;
            int[][] img, newImg;
            Bitmap newBmp;
            ImageConvert.Bitmap2Mat(bmp, out img, out width, out height);
            newImg = Util.BuildMatInt(width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double[] input = new double[] { y, x, 1 };
                    double[] output = NcvMatrix.TranslateInvert(input, tr, tc);
                    int oldY = (int)Math.Round(output[0]);
                    int oldX = (int)Math.Round(output[1]);
                    if (Util.InRect(oldX, oldY, width, height))
                    {
                        newImg[x][y] = img[oldX][oldY];
                    }
                }
            }

            ImageConvert.Mat2Bitmap(newImg, width, height, out newBmp);

            return newBmp;
        }

        /// <summary>
        /// foamliu, 2009/01/31, 缩放.
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="sr"></param>
        /// <param name="sc"></param>
        /// <returns></returns>
        public static Bitmap Scale(Bitmap bmp, double sr, double sc)
        {
            int width, height;
            int[][] img, newImg;
            Bitmap newBmp;
            ImageConvert.Bitmap2Mat(bmp, out img, out width, out height);

            int newW = (int)(width * sc);
            int newH = (int)(height * sr);

            newImg = Util.BuildMatInt(newW, newH);
            for (int y = 0; y < newH; y++)
            {
                for (int x = 0; x < newW; x++)
                {
                    double[] input = new double[] { y, x, 1 };
                    double[] output = NcvMatrix.ScaleInvert(input, sr, sc);
                    int oldY = (int)Math.Round(output[0]);
                    int oldX = (int)Math.Round(output[1]);
                    if (Util.InRect(oldX, oldY, width, height))
                    {
                        newImg[x][y] = img[oldX][oldY];
                    }
                }
            }

            ImageConvert.Mat2Bitmap(newImg, newW, newH, out newBmp);

            return newBmp;
        }

        /// <summary>
        /// foamliu, 2009/01/31, 旋转.
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="alpha">旋转角度(弧度)</param>
        /// <param name="bi">bilinear interpolation (双线性插值)</param>
        /// <returns></returns>
        public static Bitmap Rotate(Bitmap bmp, double alpha, bool bi)
        {
            int width, height;
            int[][] img, newImg;
            Bitmap newBmp;
            ImageConvert.Bitmap2Mat(bmp, out img, out width, out height);
            newImg = Util.BuildMatInt(width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (!bi)
                    {
                        double[] input = new double[] { y, x, 1 };
                        double[] output = NcvMatrix.RotateInvert(input, alpha);
                        int oldY = (int)Math.Round(output[0]);
                        int oldX = (int)Math.Round(output[1]);
                        if (Util.InRect(oldX, oldY, width, height))
                        {
                            newImg[x][y] = img[oldX][oldY];
                        }
                    }
                    else
                    {
                        // p00 ------------- p01
                        //  |                 |
                        // p10 ------------- p11
                        //
                        // 双线性插值需要更长的计算时间 (原来的5倍以上)
                        //
                        double[] input = new double[] { y, x, 1 };
                        double[] output = NcvMatrix.RotateInvert(input, alpha);
                        double newY = output[0];
                        double newX = output[1];
                        int p00Y = (int)Math.Floor(newY);
                        int p00X = (int)Math.Floor(newX);
                        int p01Y = (int)Math.Floor(newY);
                        int p01X = (int)Math.Ceiling(newX);
                        int p10Y = (int)Math.Ceiling(newY);
                        int p10X = (int)Math.Floor(newX);
                        int p11Y = (int)Math.Ceiling(newY);
                        int p11X = (int)Math.Ceiling(newX);

                        if (Util.InRect(p00X, p00Y, width, height)
                            && Util.InRect(p01X, p01Y, width, height)
                            && Util.InRect(p10X, p10Y, width, height)
                            && Util.InRect(p11X, p11Y, width, height))
                        {
                            double a = newY - p00Y;
                            double b = newX - p00X;
                            int g00 = img[p00X][p00Y];
                            int g01 = img[p01X][p01Y];
                            int g10 = img[p10X][p10Y];
                            int g11 = img[p11X][p11Y];
                            double g = b * (a * g11 + (1 - a) * g01) + (1 - b) * (a * g01 + (1 - a) * g00);
                            newImg[x][y] = (byte)Math.Round(g);
                        }
                    }
                }
            }

            ImageConvert.Mat2Bitmap(newImg, width, height, out newBmp);

            return newBmp;
        }

        public static int[][] Rotate(int[][] mat, double alpha, DoublePoint c, int bg)
        {
            int width = mat.Length;
            int height = mat[0].Length;
            int[][] newImg = Util.BuildMatInt(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {

                    double[] input = new double[] { y-c.Y, x-c.X, 1 };
                    double[] output = NcvMatrix.RotateInvert(input, alpha);
                    int oldY = (int)Math.Round(output[0] + c.Y);
                    int oldX = (int)Math.Round(output[1] + c.X);
                    if (Util.InRect(oldX, oldY, width, height))
                    {
                        newImg[x][y] = mat[oldX][oldY];
                    }
                    else
                    {
                        newImg[x][y] = bg;
                    }

                }
            }

            return newImg;
        }

        public static int[][] Rotate(int[][] mat, double alpha, DoublePoint c)
        {
            return Rotate(mat, alpha, c, 255);
        }

        /// <summary>
        /// foamliu, 2009/01/31, 倾斜.
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="theta">倾斜角度(弧度)</param>
        /// <param name="bi">bilinear interpolation (双线性插值)</param>
        /// <returns></returns>
        public static Bitmap Skew(Bitmap bmp, double theta, bool bi)
        {
            int width, height;
            int[][] img, newImg;
            Bitmap newBmp;
            ImageConvert.Bitmap2Mat(bmp, out img, out width, out height);
            newImg = Util.BuildMatInt(width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (!bi)
                    {
                        double[] input = new double[] { y, x, 1 };
                        double[] output = NcvMatrix.SkewInvert(input, theta);
                        int oldY = (int)Math.Round(output[0]);
                        int oldX = (int)Math.Round(output[1]);
                        if (Util.InRect(oldX, oldY, width, height))
                        {
                            newImg[x][y] = img[oldX][oldY];
                        }
                    }
                    else
                    {
                        // p00 ------------- p01
                        //  |                 |
                        // p10 ------------- p11
                        //
                        // 双线性插值需要更长的计算时间 (原来的5倍以上)
                        //
                        double[] input = new double[] { y, x, 1 };
                        double[] output = NcvMatrix.SkewInvert(input, theta);
                        double newY = output[0];
                        double newX = output[1];
                        int p00Y = (int)Math.Floor(newY);
                        int p00X = (int)Math.Floor(newX);
                        int p01Y = (int)Math.Floor(newY);
                        int p01X = (int)Math.Ceiling(newX);
                        int p10Y = (int)Math.Ceiling(newY);
                        int p10X = (int)Math.Floor(newX);
                        int p11Y = (int)Math.Ceiling(newY);
                        int p11X = (int)Math.Ceiling(newX);

                        if (Util.InRect(p00X, p00Y, width, height)
                            && Util.InRect(p01X, p01Y, width, height)
                            && Util.InRect(p10X, p10Y, width, height)
                            && Util.InRect(p11X, p11Y, width, height))
                        {
                            double a = newY - p00Y;
                            double b = newX - p00X;
                            int g00 = img[p00X][p00Y];
                            int g01 = img[p01X][p01Y];
                            int g10 = img[p10X][p10Y];
                            int g11 = img[p11X][p11Y];
                            double g = b * (a * g11 + (1 - a) * g01) + (1 - b) * (a * g01 + (1 - a) * g00);
                            newImg[x][y] = (byte)Math.Round(g);
                        }
                    }
                }
            }

            ImageConvert.Mat2Bitmap(newImg, width, height, out newBmp);

            return newBmp;
        }

        /// <summary>
        /// foamliu, 2009/03/05, 镜像.
        /// 
        /// </summary>
        /// <returns></returns>
        public static Bitmap Mirror(Bitmap bmp)
        {
            int width, height;
            int[][] img, newImg;
            Bitmap newBmp;
            ImageConvert.Bitmap2Mat(bmp, out img, out width, out height);
            newImg = Util.BuildMatInt(width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    newImg[x][y] = img[width-1-x][y];
                }
            }

            ImageConvert.Mat2Bitmap(newImg, width, height, out newBmp);

            return newBmp;
        }
        
    }
}
