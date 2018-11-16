

namespace NComputerVision.GraphicsLib
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using NComputerVision.Common;
    using NComputerVision.DataStructures;
    using NComputerVision.Image;

    /// <summary>
    /// 图像转换.
    /// 
    /// </summary>
    public class ImageConvert
    {

        /// <summary>
        /// foamliu, 2009/01/29, 位图转换为矩阵.
        /// 
        /// </summary>
        /// <param name="bmp">位图</param>
        /// <param name="newImg"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void Bitmap2Mat(Bitmap bmp, out int[][] mat, out int width, out int height)
        {
            width = bmp.Width;
            height = bmp.Height;

            mat = Util.BuildMatInt(width, height);

            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, /*bmp.PixelFormat*/ PixelFormat.Format24bppRgb);
            unsafe
            {
                byte* ptr = (byte*)bmpData.Scan0;
                int remain = bmpData.Stride - bmpData.Width * 3;
                int bytesPerPixel = 3;
                for (int y = 0; y < bmpData.Height; y++)
                {
                    for (int x = 0; x < bmpData.Width; x++)
                    {
                        //mat[x][y] = System.Convert.ToInt32((ptr[0] + ptr[1] + ptr[2]) / 3);
                        mat[x][y] = System.Convert.ToInt32(ptr[0] * 0.29 + ptr[1] * 0.587 + ptr[2] * 0.114);
                        ptr += bytesPerPixel;
                    }

                    ptr += remain;
                }
            }
            bmp.UnlockBits(bmpData);
        }

        public static int[][] Bitmap2Mat(Bitmap bmp)
        {
            int width, height;
            int[][] mat;
            Bitmap2Mat(bmp, out mat, out width, out height);      
            return mat;
        }

        // 2-dim array version
        public static void Bitmap2Mat2Dim(Bitmap bmp, out int[,] mat, out int width, out int height)
        {
            width = bmp.Width;
            height = bmp.Height;

            mat = new int[width, height];

            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, /*bmp.PixelFormat*/PixelFormat.Format24bppRgb);
            unsafe
            {
                byte* ptr = (byte*)bmpData.Scan0;
                int remain = bmpData.Stride - bmpData.Width * 3;
                int bytesPerPixel = 3;
                for (int y = 0; y < bmpData.Height; y++)
                {
                    for (int x = 0; x < bmpData.Width; x++)
                    {
                        //mat[x,y] = System.Convert.ToInt32((ptr[0] + ptr[1] + ptr[2]) / 3);
                        mat[x, y] = System.Convert.ToInt32(ptr[0] * 0.29 + ptr[1] * 0.587 + ptr[2] * 0.114);
                        ptr += bytesPerPixel;
                    }

                    ptr += remain;
                }
            }
            bmp.UnlockBits(bmpData);
        }

        public static int[][] Bitmap2GreenScale(Bitmap bmp)
        {
            int[][] mat;
            int width = bmp.Width;
            int height = bmp.Height;

            mat = Util.BuildMatInt(width, height);

            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, /*bmp.PixelFormat*/PixelFormat.Format24bppRgb);
            unsafe
            {
                byte* ptr = (byte*)bmpData.Scan0;
                int remain = bmpData.Stride - bmpData.Width * 3;
                int bytesPerPixel = 3;
                for (int y = 0; y < bmpData.Height; y++)
                {
                    for (int x = 0; x < bmpData.Width; x++)
                    {
                        byte r = ptr[0]; byte g = ptr[1]; byte b = ptr[2];
                        //mat[x,y] = System.Convert.ToInt32((r + g + b) / 3);
                        //mat[x][y] = System.Convert.ToInt32(-r * 0.29 + g * 0.587 - b * 0.114);
                        // RGB Filter
                        mat[x][y] = -r + 2 * g - b;
                        ptr += bytesPerPixel;
                    }

                    ptr += remain;
                }
            }
            bmp.UnlockBits(bmpData);

            GrayScaleImageLib.Normalize(mat);
            GrayScaleImageLib.RobustNormalize(mat, 0.1, 0.99);

            return mat;
        }

        public static int[,] Bitmap2Mat2Dim(Bitmap bmp)
        {
            int[,] mat;
            int cols, rows;
            Bitmap2Mat2Dim(bmp, out mat, out cols, out rows);
            return mat;
        }

        /// <summary>
        /// foamliu, 2009/02/01, 位图转换为矩阵, 彩色版本.
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="output"></param>
        /// <param name="m"></param>
        /// <param name="n"></param>
        public static void Bitmap2MatColor(Bitmap bmp, out int[][][] newImg, out int width, out int height)
        {
            width = bmp.Width;
            height = bmp.Height;

            newImg = Util.BuildMatIntColor(width, height);

            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, /*bmp.PixelFormat*/PixelFormat.Format24bppRgb);
            unsafe
            {
                byte* ptr = (byte*)bmpData.Scan0;
                int remain = bmpData.Stride - bmpData.Width * 3;
                int bytesPerPixel = 3;
                for (int y = 0; y < bmpData.Height; y++)
                {
                    for (int x = 0; x < bmpData.Width; x++)
                    {
                        newImg[0][x][y] = ptr[0];
                        newImg[1][x][y] = ptr[1];
                        newImg[2][x][y] = ptr[2];

                        ptr += bytesPerPixel;
                    }

                    ptr += remain;
                }
            }
            bmp.UnlockBits(bmpData);
        }

        // 2-dim array version
        public static void Bitmap2MatColor2Dim(Bitmap bmp, out NcvRgbColor[,] newImg, out int width, out int height)
        {
            width = bmp.Width;
            height = bmp.Height;

            newImg = new NcvRgbColor[width, height];

            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, /*bmp.PixelFormat*/PixelFormat.Format24bppRgb);
            unsafe
            {
                byte* ptr = (byte*)bmpData.Scan0;
                int remain = bmpData.Stride - bmpData.Width * 3;
                int bytesPerPixel = 3;
                for (int y = 0; y < bmpData.Height; y++)
                {
                    for (int x = 0; x < bmpData.Width; x++)
                    {
                        newImg[x, y].R = ptr[0];
                        newImg[x, y].G = ptr[1];
                        newImg[x, y].B = ptr[2];

                        ptr += bytesPerPixel;
                    }

                    ptr += remain;
                }
            }
            bmp.UnlockBits(bmpData);
        }

        /// <summary>
        /// foamliu, 2009/01/29, 矩阵转换为位图.
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <param name="bmp"></param>
        public static void Mat2Bitmap(int[][] img, int width, int height, out Bitmap bmp)
        {
            bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, /*bmp.PixelFormat*/PixelFormat.Format24bppRgb);
            unsafe
            {
                byte* ptr = (byte*)bmpData.Scan0;
                int remain = bmpData.Stride - bmpData.Width * 3;
                int bytesPerPixel = 3;
                for (int y = 0; y < bmpData.Height; y++)
                {
                    for (int x = 0; x < bmpData.Width; x++)
                    {
                        ptr[0] = ptr[1] = ptr[2] = (byte)(img[x][y]);
                        ptr += bytesPerPixel;
                    }

                    ptr += remain;
                }
            }
            bmp.UnlockBits(bmpData);
        }

        public static Bitmap Mat2Bitmap(int[][] mat)
        {
            Bitmap bmp;
            int width = mat.Length;
            int height = mat[0].Length;
            Mat2Bitmap(mat, width, height, out bmp);
            return bmp;
        }

        // 2-dim array version
        public static void Mat2Bitmap2Dim(int[,] img, int width, int height, out Bitmap bmp)
        {
            bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, /*bmp.PixelFormat*/PixelFormat.Format24bppRgb);
            unsafe
            {
                byte* ptr = (byte*)bmpData.Scan0;
                int remain = bmpData.Stride - bmpData.Width * 3;
                int bytesPerPixel = 3;
                for (int y = 0; y < bmpData.Height; y++)
                {
                    for (int x = 0; x < bmpData.Width; x++)
                    {
                        ptr[0] = ptr[1] = ptr[2] = (byte)(img[x, y]);
                        ptr += bytesPerPixel;
                    }

                    ptr += remain;
                }
            }
            bmp.UnlockBits(bmpData);
        }

        /// <summary>
        /// foamliu, 2009/02/01, 矩阵转换为位图, 彩色版本.
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <param name="bmp"></param>
        public static void Mat2BitmapColor(int[][][] img, int width, int height, out Bitmap bmp)
        {
            bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, /*bmp.PixelFormat*/PixelFormat.Format24bppRgb);
            unsafe
            {
                byte* ptr = (byte*)bmpData.Scan0;
                int remain = bmpData.Stride - bmpData.Width * 3;
                int bytesPerPixel = 3;
                for (int y = 0; y < bmpData.Height; y++)
                {
                    for (int x = 0; x < bmpData.Width; x++)
                    {
                        ptr[0] = (byte)(img[0][x][y]);
                        ptr[1] = (byte)(img[1][x][y]);
                        ptr[2] = (byte)(img[2][x][y]);
                        ptr += bytesPerPixel;
                    }

                    ptr += remain;
                }
            }
            bmp.UnlockBits(bmpData);
        }

        /// <summary>
        /// foamliu, 2009/02/01, 行程表示转换为矩阵.
        /// 
        /// </summary>
        /// <param name="runList"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        //public static int[][] RunList2Mat(List<Run> runList, int width, int height)
        //{
        //    int[][] newImg = Util.BuildMatInt(width, height);

        //    foreach (Run run in runList)
        //    {
        //        for (int x = run.StartX; x <= run.EndX; x++)
        //        {
        //            newImg[x][run.Y] = run.G;
        //        }
        //    }

        //    return newImg;
        //}

        /// <summary>
        /// foamliu, 2009/02/02, 矩阵转换为行程表示.
        /// 
        /// </summary>
        /// <param name="img"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="runList"></param>
        //public static void Mat2RunList(int[][] mat, int width, int height, out List<Run> runList)
        //{
        //    runList = new List<Run>();

        //    int startX;
        //    int lastG = mat[0][0];

        //    for (int y = 0; y < height; y++)
        //    {
        //        startX = 0;

        //        for (int x = 0; x < width; x++)
        //        {
        //            if (mat[x][y] != lastG)
        //            {
        //                runList.Add(new Run(y, startX, x - 1, lastG));

        //                startX = x;
        //                lastG = mat[x][y];
        //            }
        //            if (x == width - 1)
        //            {
        //                runList.Add(new Run(y, startX, x, lastG));

        //                lastG = mat[x][y];
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// foamliu, 2009/02/02, 二值图像转化成可显示的灰度图.
        /// 
        /// 规则：
        ///     0 -> 255
        ///     1 -> 0
        ///     
        /// 因为 ROI 通常用白色表示, 而黑色通常充当背景.
        /// 
        /// </summary>
        /// <param name="mat"></param>
        public static void Binary2GrayValue(int[][] mat)
        {
            int width = mat.Length;
            int height = mat[0].Length;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    mat[x][y] = (mat[x][y] == 1 ? 255 : 0);
                }
            }
        }

        /// <summary>
        /// foamliu, 2009/02/02, 可显示的灰度图转化成二值图像.
        /// 
        /// 规则：
        ///     0-127   -> 1
        ///     128-255 -> 0
        ///     
        /// 因为 ROI 通常用白色表示, 而黑色通常充当背景.
        /// 
        /// </summary>
        /// <param name="mat"></param>
        public static void GrayValue2Binary(int[][] mat)
        {
            int width = mat.Length;
            int height = mat[0].Length;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (mat[x][y] <= 127)
                        mat[x][y] = 1;
                    else
                        mat[x][y] = 0;
                }
            }
        }

        /// <summary>
        /// 2010/04/26, 位图转化为RGBImage.
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="rgbImg"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void Bitmap2RGBImage(Bitmap bmp, out RgbImage rgbImg, out int width, out int height)
        {
            int[][][] rgb_mat;
            Bitmap2MatColor(bmp, out rgb_mat, out width, out height);
            rgbImg = new RgbImage(rgb_mat, width, height);
        }

        /// <summary>
        /// 2010/04/26, RGBImage转化为位图.
        /// </summary>
        /// <param name="img"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="bmp"></param>
        public static void RGBImage2Bitmap(RgbImage rgbImg, int width, int height, out Bitmap bmp)
        {
            int[][][] rgb_mat = new int[3][][];
            rgb_mat[0] = rgbImg.R;
            rgb_mat[1] = rgbImg.G;
            rgb_mat[2] = rgbImg.B;

            Mat2BitmapColor(rgb_mat, width, height, out bmp);
        }

        /// <summary>
        /// 2010/04/26, 位图转化为GrayScaleImage.
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="rgbImg"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void Bitmap2GrayScaleImage(Bitmap bmp, out GrayScaleImage gsImage, out int width, out int height)
        {
            int[][] mat;
            Bitmap2Mat(bmp, out mat, out width, out height);
            gsImage = new GrayScaleImage(mat, width, height);
        }

        /// <summary>
        /// 2010/04/26, GrayScaleImage转化为位图.
        /// </summary>
        /// <param name="img"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="bmp"></param>
        public static void GrayScaleImage2Bitmap(GrayScaleImage gsImage, int width, int height, out Bitmap bmp)
        {
            Mat2Bitmap(gsImage.GrayValue, width, height, out bmp);
        }
    }
}
