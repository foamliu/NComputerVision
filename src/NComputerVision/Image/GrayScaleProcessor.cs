

namespace NComputerVision.Image
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using NComputerVision.Common;
    using NComputerVision.DataStructures;
    using NComputerVision.GraphicsLib;

    /// <summary>
    /// foamliu, 2009/01/28.
    /// 封装一批图像处理函数
    /// </summary>
    public static class GrayScaleProcessor
    {
        /// <summary>
        /// 彩色位图转化为灰度图.
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap GrayScale(Bitmap bmp)
        {
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                byte* ptr = (byte*)bmpData.Scan0;
                int remain = bmpData.Stride - bmpData.Width * 3;
                int bytesPerPixel = 3;

                int mean;

                for (int y = 0; y < bmpData.Height; y++)
                {
                    for (int x = 0; x < bmpData.Width; x++)
                    {
                        mean = (int)(ptr[0] * 0.29 + ptr[1] * 0.587 + ptr[2] * 0.114);
                        //mean = (int)((ptr[0] + ptr[1] + ptr[2]) / 3);
                        ptr[2] = ptr[1] = ptr[0] = (byte)mean;
                        ptr += bytesPerPixel;
                    }
                }
            }

            bmp.UnlockBits(bmpData);
            return bmp;
        }

        /// <summary>
        /// 灰度值归一化处理
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap Normalize(Bitmap bmp)
        {            
            int width, height;
            int[][] mat;
            Bitmap newBmp;
            ImageConvert.Bitmap2Mat(bmp, out mat, out width, out height);
            GrayScaleImageLib.Normalize(mat);
            ImageConvert.Mat2Bitmap(mat, width, height, out newBmp);

            return newBmp;
        }

        /// <summary>
        /// 鲁棒的灰度值归一化处理
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="pl"></param>
        /// <param name="pu"></param>
        /// <returns></returns>
        public static Bitmap RobustNormalize(Bitmap bmp, double pl, double pu)
        {
            int width, height;
            int[][] mat;
            Bitmap newBmp;
            ImageConvert.Bitmap2Mat(bmp, out mat, out width, out height);
            GrayScaleImageLib.RobustNormalize(mat, pl, pu);
            ImageConvert.Mat2Bitmap(mat, width, height, out newBmp);

            return newBmp;
        }

        /// <summary>
        /// 灰度直方图
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static double[] Histogram(Bitmap bmp)
        {            
            int width, height;
            int[][] mat;
            ImageConvert.Bitmap2Mat(bmp, out mat, out width, out height);
            double[] hist = GrayScaleImageLib.Histogram(mat);

            return hist;
        }

        /// <summary>
        /// foamliu, 2009/02/03, 累积灰度直方图.
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static double[] CumulativeHistogram(Bitmap bmp)
        {
            int width, height;
            int[][] mat;
            ImageConvert.Bitmap2Mat(bmp, out mat, out width, out height);
            double[] cumuHist = GrayScaleImageLib.CumulativeHistogram(mat);

            return cumuHist;
        }

        public static Bitmap K_Mean(Bitmap bmp, int k)
        {
            int width, height;
            int[][] img, filtered;
            Bitmap newBmp;
            ImageConvert.Bitmap2Mat(bmp, out img, out width, out height);

            ConvKernel kernel = ConvKernel.GetKMeanKernal(k);

            Convolution conv = new Convolution();
            conv.Calculate(img, kernel, out filtered);

            ImageConvert.Mat2Bitmap(filtered, width, height, out newBmp);

            return newBmp;
        }

        public static Bitmap Gaussian(Bitmap bmp, double sigma)
        {
            int width, height;
            int[][] img, filtered;
            Bitmap newBmp;
            ImageConvert.Bitmap2Mat(bmp, out img, out width, out height);

            //double sigma = 1.0;
            int n = 4;
            ConvKernel kernel = Util.GetGaussianKernal(sigma, n);

            Convolution conv = new Convolution();
            conv.Calculate(img, kernel, out filtered);

            GrayScaleImageLib.Normalize(filtered);
            ImageConvert.Mat2Bitmap(filtered, width, height, out newBmp);

            return newBmp;
        }

        public static int[][] Gaussian(int[][] mat, double sigma)
        {            
            int[][] filtered;
            
            int n = 4;
            ConvKernel kernel = Util.GetGaussianKernal(sigma, n);

            Convolution conv = new Convolution();
            conv.Calculate(mat, kernel, out filtered);

            GrayScaleImageLib.Normalize(filtered);

            return filtered;
        }

        public static Bitmap Median(Bitmap bmp, int k)
        {
            int width, height;
            int[][] mat, filtered;
            Bitmap newBmp;
            ImageConvert.Bitmap2Mat(bmp, out mat, out width, out height);

            filtered = Util.Median(mat, k);

            GrayScaleImageLib.Normalize(filtered);
            ImageConvert.Mat2Bitmap(filtered, width, height, out newBmp);

            return newBmp;
        }

        public static Bitmap Sobel(Bitmap bmp)
        {
            int width, height;
            int[][] img, filtered;
            Bitmap newBmp;
            ImageConvert.Bitmap2Mat(bmp, out img, out width, out height);

            Convolution conv = new Convolution();
            conv.CalculateEdge(img, ConvKernel.Sobel_Gx, ConvKernel.Sobel_Gy, out filtered, ConvNorm.Norm_2);

            GrayScaleImageLib.Normalize(filtered);
            ImageConvert.Mat2Bitmap(filtered, width, height, out newBmp);

            return newBmp;
        }

        //public Bitmap Scharr(Bitmap bmp)
        //{
        //    int width, height;
        //    int[][] img, newImg;
        //    Bitmap newBmp;
        //    Util.Bitmap2Mat(bmp, out img, out width, out height);            

        //    Convolution conv = new Convolution();
        //    conv.Calculate2Knl(img, ConvKernel.Scharr_Gx, ConvKernel.Scharr_Gy, out newImg);

        //    Util.Mat2Bitmap(newImg, width, height, out newBmp);

        //    return newBmp;
        //}



        public static Bitmap Prewitt(Bitmap bmp)
        {
            int width, height;
            int[][] img, filtered;
            Bitmap newBmp;
            ImageConvert.Bitmap2Mat(bmp, out img, out width, out height);

            Convolution conv = new Convolution();
            conv.CalculateEdge(img, ConvKernel.Prewitt_Gx, ConvKernel.Prewitt_Gy, out filtered, ConvNorm.Norm_1);

            ImageConvert.Mat2Bitmap(filtered, width, height, out newBmp);

            return newBmp;
        }

        /// <summary>
        /// foamliu, 2009/03/03, 边缘锐化.
        /// 从原图像中减去拉普拉斯算子处理后的结果.
        /// 我做的效果是图像锐化的同时产生了噪音. 
        /// 用(0.01, 0.99)做鲁棒正规化可以大大改善效果.
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Bitmap SharpenEdges(Bitmap bmp)
        {
            int width, height;
            int[][] mat, filtered;
            Bitmap newBmp;
            ImageConvert.Bitmap2Mat(bmp, out mat, out width, out height);

            Convolution conv = new Convolution();

            conv.Calculate(mat, ConvKernel.Laplacian_4, out filtered);

            NcvMatrix.MatSubtract(mat, filtered);
            GrayScaleImageLib.Normalize(mat);

            ImageConvert.Mat2Bitmap(mat, width, height, out newBmp);

            return newBmp;
        }

        public static Bitmap SharpenMore(Bitmap bmp)
        {
            int width, height;
            int[][] mat, filtered;
            Bitmap newBmp;
            ImageConvert.Bitmap2Mat(bmp, out mat, out width, out height);

            Convolution conv = new Convolution();

            conv.Calculate(mat, ConvKernel.Laplacian_8, out filtered);

            NcvMatrix.MatSubtract(mat, filtered);
            GrayScaleImageLib.Normalize(mat);

            ImageConvert.Mat2Bitmap(mat, width, height, out newBmp);

            return newBmp;
        }

        /// <summary>
        /// foamliu, 2009/03/03, 试图整合这些滤波器函数.
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Bitmap Filter(Bitmap bmp, FilterType type)
        {
            int width, height;
            int[][] mat, filtered;
            Bitmap newBmp;
            ImageConvert.Bitmap2Mat(bmp, out mat, out width, out height);

            Convolution conv = new Convolution();
            ConvKernel kernel = null;

            switch (type)
            {
                case FilterType.Sobel_Gx:
                    kernel = ConvKernel.Sobel_Gx;
                    break;
                case FilterType.Sobel_Gy:
                    kernel = ConvKernel.Sobel_Gy;
                    break;
                case FilterType.Prewitt_Gx:
                    kernel = ConvKernel.Prewitt_Gx;
                    break;
                case FilterType.Prewitt_Gy:
                    kernel = ConvKernel.Prewitt_Gy;
                    break;
                case FilterType.Laplacian_4:
                    kernel = ConvKernel.Laplacian_4;
                    break;
                case FilterType.Laplacian_8:
                    kernel = ConvKernel.Laplacian_8;
                    break;
                default:
                    break;
            }

            conv.Calculate(mat, kernel, out filtered);

            GrayScaleImageLib.Normalize(filtered);
            ImageConvert.Mat2Bitmap(filtered, width, height, out newBmp);

            return newBmp;
        }


        // foamliu, 2009/02/01, 处理后所有点的灰度值有：
        //     gmin <= g <= gmax
        // 
        //  Notes: 或者为0.
        public static Bitmap ThresholdSimple(Bitmap bmp, int gmin, int gmax)
        {
            int width, height;
            int[][] mat;
            Bitmap newBmp;

            ImageConvert.Bitmap2Mat(bmp, out mat, out width, out height);
            ThresholdLib.ThresholdSimple(mat, gmin, gmax);
            ImageConvert.Mat2Bitmap(mat, width, height, out newBmp);

            return newBmp;

        }

        public static Bitmap ThresholdDynamic(Bitmap bmp, int gdiff, int k)
        {
            int width, height;
            int[][] mat;
            Bitmap newBmp;

            ImageConvert.Bitmap2Mat(bmp, out mat, out width, out height);

            ThresholdLib.ThresholdDynamic(mat, gdiff, k);

            // foamliu, 2009/02/04, 阈值化的结果是二值图像, 需要转化为可显示的灰度图.           
            ImageConvert.Binary2GrayValue(mat);

            ImageConvert.Mat2Bitmap(mat, width, height, out newBmp);

            return newBmp;
        }

        /// <summary>
        /// foamliu, 2009/02/09, 傅立叶变换
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap Fourier(Bitmap bmp, NComputerVision.GraphicsLib.FourierTransform.Direction dire)
        {
            int width, height;
            int[][] mat, filtered1, filtered2;
            Bitmap newBmp;
            ImageConvert.Bitmap2Mat(bmp, out mat, out width, out height);

            filtered1 = FourierTransform.Transform(mat, dire);
            filtered2 = Util.LogarithmicTransform(filtered1);

            ImageConvert.Mat2Bitmap(filtered2, width, height, out newBmp);

            return newBmp;
            
        }

        /// <summary>
        /// foamliu, 2009/02/09, 灰度图像膨胀
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap GrayValueDilation(Bitmap bmp)
        {
            int width, height;
            int[][] mat, filtered;
            Bitmap newBmp;
            ImageConvert.Bitmap2Mat(bmp, out mat, out width, out height);

            GrayScaleImageLib.Dilation(mat, StructuringElement.N4, out filtered);

            ImageConvert.Mat2Bitmap(filtered, width, height, out newBmp);

            return newBmp;
        }

        /// <summary>
        /// foamliu, 2009/02/09, 灰度图像腐蚀
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap GrayValueErosion(Bitmap bmp)
        {
            int width, height;
            int[][] mat, filtered;
            Bitmap newBmp;
            ImageConvert.Bitmap2Mat(bmp, out mat, out width, out height);

            GrayScaleImageLib.Erosion(mat, StructuringElement.N4, out filtered);

            ImageConvert.Mat2Bitmap(filtered, width, height, out newBmp);

            return newBmp;
        }

        /// <summary>
        /// foamliu, 2009/02/09, 灰度图像取反
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap GrayValueInverse(Bitmap bmp)
        {
            int width, height;
            int[][] mat;
            Bitmap newBmp;
            ImageConvert.Bitmap2Mat(bmp, out mat, out width, out height);

            GrayScaleImageLib.Inverse(mat);

            ImageConvert.Mat2Bitmap(mat, width, height, out newBmp);

            return newBmp;
        }

        /// <summary>
        /// foamliu, 2009/02/09, 灰度图像开操作
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap GrayValueOpen(Bitmap bmp)
        {
            int width, height;
            int[][] mat, filtered;
            Bitmap newBmp;
            ImageConvert.Bitmap2Mat(bmp, out mat, out width, out height);

            GrayScaleImageLib.Open(mat, StructuringElement.N4, out filtered);

            ImageConvert.Mat2Bitmap(filtered, width, height, out newBmp);

            return newBmp;
        }

        /// <summary>
        /// foamliu, 2009/02/09, 灰度图像闭操作
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap GrayValueClose(Bitmap bmp)
        {
            int width, height;
            int[][] mat, filtered;
            Bitmap newBmp;
            ImageConvert.Bitmap2Mat(bmp, out mat, out width, out height);

            GrayScaleImageLib.Close(mat, StructuringElement.N4, out filtered);

            ImageConvert.Mat2Bitmap(filtered, width, height, out newBmp);

            return newBmp;
        }

        /// <summary>
        /// foamliu, 2009/02/09, 二值图像膨胀
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap Dilation(Bitmap bmp)
        {
            int width, height;
            int[][] mat, filtered;
            Bitmap newBmp;
            ImageConvert.Bitmap2Mat(bmp, out mat, out width, out height);
            ImageConvert.GrayValue2Binary(mat);

            BinaryImageLib.Dilation(mat, StructuringElement.N4, out filtered);

            ImageConvert.Binary2GrayValue(filtered);
            ImageConvert.Mat2Bitmap(filtered, width, height, out newBmp);

            return newBmp;
        }

        /// <summary>
        /// foamliu, 2009/02/09, 二值图像腐蚀
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap Erosion(Bitmap bmp)
        {
            int width, height;
            int[][] mat, filtered;
            Bitmap newBmp;
            ImageConvert.Bitmap2Mat(bmp, out mat, out width, out height);
            ImageConvert.GrayValue2Binary(mat);

            BinaryImageLib.Erosion(mat, StructuringElement.N4, out filtered);

            ImageConvert.Binary2GrayValue(filtered);
            ImageConvert.Mat2Bitmap(filtered, width, height, out newBmp);

            return newBmp;
        }

        /// <summary>
        /// foamliu, 2009/02/09, 二值图像求逆
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap Inverse(Bitmap bmp)
        {
            int width, height;
            int[][] mat;
            Bitmap newBmp;
            ImageConvert.Bitmap2Mat(bmp, out mat, out width, out height);
            ImageConvert.GrayValue2Binary(mat);

            BinaryImageLib.Inverse(mat);

            ImageConvert.Binary2GrayValue(mat);
            ImageConvert.Mat2Bitmap(mat, width, height, out newBmp);

            return newBmp;
        }


        /// <summary>
        /// foamliu, 2009/02/09, 提取骨骼
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap ExtractSkeleton(Bitmap bmp)
        {
            int width, height;
            int[][] mat, filtered;
            Bitmap newBmp;
            ImageConvert.Bitmap2Mat(bmp, out mat, out width, out height);
            ImageConvert.GrayValue2Binary(mat);

            BinaryImageLib.ExtractSkeleton(mat, StructuringElement.N4, out filtered);

            ImageConvert.Binary2GrayValue(filtered);
            ImageConvert.Mat2Bitmap(filtered, width, height, out newBmp);

            return newBmp;
        }

        /// <summary>
        /// foamliu, 2009/02/09, 二值图像开操作
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap BinaryOpen(Bitmap bmp)
        {
            int width, height;
            int[][] mat, filtered;
            Bitmap newBmp;
            ImageConvert.Bitmap2Mat(bmp, out mat, out width, out height);
            ImageConvert.GrayValue2Binary(mat);

            BinaryImageLib.Open(mat, StructuringElement.N4, out filtered);

            ImageConvert.Binary2GrayValue(filtered);
            ImageConvert.Mat2Bitmap(filtered, width, height, out newBmp);

            return newBmp;
        }

        /// <summary>
        /// foamliu, 2009/02/09, 二值图像闭操作
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap BinaryClose(Bitmap bmp)
        {
            int width, height;
            int[][] mat, filtered;
            Bitmap newBmp;
            ImageConvert.Bitmap2Mat(bmp, out mat, out width, out height);
            ImageConvert.GrayValue2Binary(mat);

            BinaryImageLib.Close(mat, StructuringElement.N4, out filtered);

            ImageConvert.Binary2GrayValue(filtered);
            ImageConvert.Mat2Bitmap(filtered, width, height, out newBmp);

            return newBmp;
        }       

        public static void RetrieveFeatureSet(Bitmap bmp, out GrayValueFeatures features)
        {
            int width, height;
            int[][] mat;
            ImageConvert.Bitmap2Mat(bmp, out mat, out width, out height);
            features = new GrayValueFeatures(mat);
            features.CalcBasicFeatures();

        }

        /// <summary>
        /// 绘制灰度图的距
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        //public static Bitmap GrayValueMoment(Bitmap bmp)
        //{
        //    int width, height;
        //    int[][] mat;
        //    Bitmap newBmp = (Bitmap)bmp.Clone();

        //    ImageConvert.Bitmap2Mat(bmp, out mat, out width, out height);
        //    GrayValueFeatures features = new GrayValueFeatures(mat);
        //    features.CalcMomentFeatures();

        //    Drawing.DrawMoments(newBmp, features);

        //    return newBmp;
        //}


        public static void IntensityDist(Bitmap bmp, out double[] intensityDistX, out double[] intensityDistY)
        {
            int width, height;
            int[][] mat;
            Bitmap newBmp = (Bitmap)bmp.Clone();

            ImageConvert.Bitmap2Mat(bmp, out mat, out width, out height);

            intensityDistX = GrayScaleImageLib.IntensityDistributionX(mat);
            intensityDistY = GrayScaleImageLib.IntensityDistributionY(mat);            
        }       

    }
}
