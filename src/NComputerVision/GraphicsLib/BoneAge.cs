using System.Drawing;
using System;
using NComputerVision.Common;

namespace NComputerVision.GraphicsLib
{
    public class BoneAge
    {
        public static Bitmap Step1(Bitmap bmp)
        {
            int[][] mat = ImageConvert.Bitmap2Mat(bmp);
            //mat = Util.Median(mat, 5);

            int width = mat.Length; int height = mat[0].Length;
            int[][] target = Util.BuildMatInt(width, height);
            int state;
            int previous;

            for (int y = 0; y < height; y++)
            {
                state = 0;
                previous = mat[0][y];
                for (int x = 0; x < width; x++)
                {
                    int _this = mat[x][y];
                    // 状态机
                    switch (state)
                    {
                        case 0:
                            state = 1;
                            _this = 0;
                            break;
                        case 1:
                            if (_this - previous >= BoneAgeConstants.TransitionThreshold)
                            {
                                state = 2;                                
                            }
                            _this = 0;
                            break;
                        case 2:
                            if (_this - previous >= BoneAgeConstants.TransitionThreshold)
                            {
                                state = 3;
                            }
                            _this = 0;
                            break;
                        case 3:
                            if (previous - _this >= BoneAgeConstants.TransitionThreshold)
                            {
                                state = 4;
                            }
                            _this = 255;
                            break;
                        case 4:
                            if (previous - _this >= BoneAgeConstants.TransitionThreshold)
                            {
                                state = 5;
                            }
                            _this = 0;
                            break;
                        case 5:
                            _this = 0;
                            break;
                        default:
                            throw new ApplicationException();
                    }
                    previous = mat[x][y];
                    target[x][y] = _this;
                }
            }

            Bitmap result = ImageConvert.Mat2Bitmap(target);
            return result;
        }

        public static Bitmap Step2(Bitmap bmp)
        {
            int[][] mat = ImageConvert.Bitmap2Mat(bmp);
            ConvKernel kernel = ConvKernel.GetKMeanKernal(3);
            Convolution conv = new Convolution();
            conv.Calculate(mat, kernel, out mat);
            Bitmap result = ImageConvert.Mat2Bitmap(mat);
            return result;
        }

        public static Bitmap Step3(Bitmap bmp)
        {
            return bmp;
        }

        public static Bitmap Step4(Bitmap bmp)
        {
            return bmp;
        }

        #region Private Methods

        private static int abs(int value)
        {
            return Math.Abs(value);
        }

        #endregion
    }

    internal class BoneAgeConstants
    {
        internal const int TransitionThreshold = 25;
    }
}
