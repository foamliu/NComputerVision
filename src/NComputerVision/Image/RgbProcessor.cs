
namespace NComputerVision.Image
{
    using System.Drawing;
    using NComputerVision.Common;
    using NComputerVision.DataStructures;
    using NComputerVision.GraphicsLib;

    public static class RgbProcessor
    {
        public static Bitmap Gaussian(Bitmap bmp, double sigma)
        {
            int width, height;
            int[][][] img, newImg;
            Bitmap newBmp;
            ImageConvert.Bitmap2MatColor(bmp, out img, out width, out height);

            newImg = new int[3][][];

            int n = 4;
            ConvKernel kernel = Util.GetGaussianKernal(sigma, n);

            Convolution conv = new Convolution();
            // RGB
            conv.Calculate(img[0], kernel, out newImg[0]);
            conv.Calculate(img[1], kernel, out newImg[1]);
            conv.Calculate(img[2], kernel, out newImg[2]);

            ImageConvert.Mat2BitmapColor(newImg, width, height, out newBmp);

            return newBmp;
        }


        public static Bitmap K_Mean(Bitmap bmp, int k)
        {
            int width, height;
            int[][][] img, newImg;
            Bitmap newBmp;
            ImageConvert.Bitmap2MatColor(bmp, out img, out width, out height);

            newImg = new int[3][][];
            
            ConvKernel kernel = ConvKernel.GetKMeanKernal(k);

            Convolution conv = new Convolution();
            // RGB
            conv.Calculate(img[0], kernel, out newImg[0]);
            conv.Calculate(img[1], kernel, out newImg[1]);
            conv.Calculate(img[2], kernel, out newImg[2]);

            ImageConvert.Mat2BitmapColor(newImg, width, height, out newBmp);

            return newBmp;
        }

        public static Bitmap Sobel(Bitmap bmp)
        {
            int width, height;
            int[][][] img, newImg;
            Bitmap newBmp;
            ImageConvert.Bitmap2MatColor(bmp, out img, out width, out height);

            newImg = new int[3][][];

            Convolution conv = new Convolution();
            // RGB
            conv.CalculateEdge(img[0], ConvKernel.Sobel_Gx, ConvKernel.Sobel_Gy, out newImg[0], ConvNorm.Norm_1);
            conv.CalculateEdge(img[1], ConvKernel.Sobel_Gx, ConvKernel.Sobel_Gy, out newImg[1], ConvNorm.Norm_1);
            conv.CalculateEdge(img[2], ConvKernel.Sobel_Gx, ConvKernel.Sobel_Gy, out newImg[2], ConvNorm.Norm_1);

            ImageConvert.Mat2BitmapColor(newImg, width, height, out newBmp);

            return newBmp;
        }


        /// <summary>
        /// foamliu, 2009/03/03, 边缘锐化.
        /// 从原图像中减去拉普拉斯算子处理后的结果.
        /// 我做的效果是图像锐化的同时产生了噪音. 与这个结果类似:
        /// 
        /// http://www.dfanning.com/ip_tips/sharpen.html
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Bitmap SharpenEdges(Bitmap bmp)
        {
            int width, height;
            int[][][] mat, filtered = new int[3][][];
            Bitmap newBmp;
            ImageConvert.Bitmap2MatColor(bmp, out mat, out width, out height);

            Convolution conv = new Convolution();

            conv.Calculate(mat[0], ConvKernel.Laplacian_4, out filtered[0]);
            conv.Calculate(mat[1], ConvKernel.Laplacian_4, out filtered[1]);
            conv.Calculate(mat[2], ConvKernel.Laplacian_4, out filtered[2]);

            NcvMatrix.MatSubtract(mat[0], filtered[0]);
            NcvMatrix.MatSubtract(mat[1], filtered[1]);
            NcvMatrix.MatSubtract(mat[2], filtered[2]);

            GrayScaleImageLib.Normalize(mat[0]);
            GrayScaleImageLib.Normalize(mat[1]);
            GrayScaleImageLib.Normalize(mat[2]);

            ImageConvert.Mat2BitmapColor(mat, width, height, out newBmp);

            return newBmp;
        }

        public static Bitmap Dilation(Bitmap bmp)
        {
            int width, height;
            int[][][] mat, filtered = new int[3][][];
            Bitmap newBmp;
            ImageConvert.Bitmap2MatColor(bmp, out mat, out width, out height);

            GrayScaleImageLib.Dilation(mat[0], StructuringElement.N4, out filtered[0]);
            GrayScaleImageLib.Dilation(mat[1], StructuringElement.N4, out filtered[1]);
            GrayScaleImageLib.Dilation(mat[2], StructuringElement.N4, out filtered[2]);

            ImageConvert.Mat2BitmapColor(filtered, width, height, out newBmp);

            return newBmp;
        }

        public static Bitmap Erosion(Bitmap bmp)
        {
            int width, height;
            int[][][] mat, filtered = new int[3][][];
            Bitmap newBmp;
            ImageConvert.Bitmap2MatColor(bmp, out mat, out width, out height);

            GrayScaleImageLib.Erosion(mat[0], StructuringElement.N4, out filtered[0]);
            GrayScaleImageLib.Erosion(mat[1], StructuringElement.N4, out filtered[1]);
            GrayScaleImageLib.Erosion(mat[2], StructuringElement.N4, out filtered[2]);

            ImageConvert.Mat2BitmapColor(filtered, width, height, out newBmp);

            return newBmp;
        }

        public static Bitmap Median(Bitmap bmp, int k)
        {
            int width, height;
            int[][][] mat, filtered = new int[3][][];
            Bitmap newBmp;
            ImageConvert.Bitmap2MatColor(bmp, out mat, out width, out height);

            filtered[0] = Util.Median(mat[0], k);
            filtered[1] = Util.Median(mat[1], k);
            filtered[2] = Util.Median(mat[2], k);

            //GrayValueImageLib.Normalize(filtered[0]);
            //GrayValueImageLib.Normalize(filtered[1]);
            //GrayValueImageLib.Normalize(filtered[2]);

            ImageConvert.Mat2BitmapColor(filtered, width, height, out newBmp);

            return newBmp;
        }



    }
}
