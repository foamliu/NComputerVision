
namespace NComputerVision.GraphicsLib
{
    using System;
    using NComputerVision.Common;

    public enum ConvNorm
    {
        Norm_1,
        Norm_2
    }

    /// <summary>
    /// foamliu, 2009/01/29, 卷积.
    /// 
    /// </summary>
    public class Convolution
    {
        public void Calculate(int[][] input, ConvKernel kernel, out int[][] output)
        {
            int width = input.Length;
            int height = input[0].Length;

            output = new int[width][];

            for (int i = 0; i < width; i++)
            {
                output[i] = new int[height];
            }

            if (kernel == null) return;

            int m = kernel.M;
            int n = kernel.N;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double sum = 0;
                    for (int j = -n; j <= n; j++)
                    {
                        for (int i = -m; i <= m; i++)
                        {
                            sum += Util.GetPixel(input, x - i, y - j) * kernel.Filter(i, j);
                        }
                    }
                    //output[x][y] = (int)Math.Abs(sum);
                    //  foamliu, 2009/02/03, 这里不应取绝对值, 这么做使得在做图像锐化时犯了
                    //  <<数字图像处理 "Digital Image Processing Using MATLAB">> 图3.16 (b) 一样的错误.
                    //  正确的做法是保持负值, 用正规化处理.
                    output[x][y] = System.Convert.ToInt32(sum);
                }
            }
        }

        /// <summary>
        /// foamliu, 2009/02/01, 用两个核进行卷积.
        /// 主要用于边缘算子.
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="kernel"></param>
        /// <param name="output"></param>
        public void CalculateEdge(int[][] input, ConvKernel kx, ConvKernel ky, out int[][] output, ConvNorm norm)
        {
            int width = input.Length;
            int height = input[0].Length;

            output = Util.BuildMatInt(width, height);

            int m = kx.M;
            int n = kx.N;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double Gx = 0;
                    double Gy = 0;
                    for (int j = -n; j <= n; j++)
                    {
                        for (int i = -m; i <= m; i++)
                        {
                            Gx += Util.GetPixel(input, x - i, y - j) * kx.Filter(i, j);
                            Gy += Util.GetPixel(input, x - i, y - j) * ky.Filter(i, j);
                        }
                    }

                    if (norm == ConvNorm.Norm_1)
                    {
                        // 1-范数 (1-norm)
                        output[x][y] = System.Convert.ToInt32(Math.Abs(Gx) + Math.Abs(Gy));
                        //  foamliu, 2009/02/03, 用 2-norm 更准确, 但是我看不出差别, 
                        //  而且1-范数较快.
                    }
                    else if (norm == ConvNorm.Norm_2)
                    {
                        // 2-范数 (2-norm)
                        output[x][y] = (int)(Math.Sqrt(Gx * Gx + Gy * Gy));
                    }

                    // foamliu, 2009/02/03, 这两个梯度可以用于求方向 theta:
                    // theta = arctan(Gy/Gx).
                    // 当 theta = 0 时是一条垂直边界, 左侧更暗.

                    // 梯度大的地方更亮.
                }
            }
        }

        public void Calculate2(int[][] I, int[][] G)
        {
            int m = I.Length;
            int n = I[0].Length;
            int m1 = G.Length;
            int n1 = G[0].Length;

            //[n,m] = size(I);
            //[n1,m1] = size(G);


//FI = fft2(I,n+n1-1,m+m1-1);  % avoid aliasing
//FG = fft2(G,n+n1-1,m+m1-1);
//FY = FI.*FG;
//YT = real(ifft2(FY));
//nl = floor(n1/2);
//ml = floor(m1/2);
//Y = YT(1+nl:n+nl,1+ml:m+ml);
        }
    }
}
