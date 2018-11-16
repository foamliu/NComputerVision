

// foamliu, 2009/02/02, 扫描速度的试验, 对于 n × n 数组下列哪种扫描更快:
//
// int[][] mat; 
//
//  1) for (int x=0;x<n;x++)
//     {
//          for (int y=0;y<n;y++)
//          {
//              temp = mat[x][y];
//          }
//      }
//
//  2) for (int y=0;y<n;y++)
//     {
//          for (int x=0;x<n;x++)
//          {
//              temp = mat[x][y];
//          }
//      }
//
// 令 n=10000, 做了10次试验, 结果如下:
//              1       2       3       4       5       6       7       8       9       10
//  (1) 时间     562     578     578     578     578     562     562     578     578     562
//  (2) 时间     203     203     203     187     218     203     187     250     203     203
//
//
// 平均来看, (2) 比 (1) 快:
// 5716/2060 = 2.77 (倍)
//

// 不使用二维数据形式 (比如: int[,] img = new int[width,height]), 是因为存取上述链表形式矩阵更快 (765ms, 快32.3%), 
//  而二者使用内存大小相差无几:
//  形式      内存         平均占用(字节)
//  [,]     396,544k    4.06
//  [][]    398,032k    4.08
//
namespace NComputerVision.Common
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using NComputerVision.GraphicsLib;

    public class Util
    {
        // foamliu, 2009/02/17, 5×5 搜索窗口.
        //
        // for each of the 26 neighbors
        //
        public static int[][] SearchWindow =
                        {new int[]{-1,-1},
                        new int[]{0,-1},
                        new int[]{1,-1},
                        new int[]{1,0},
                        new int[]{1,1},
                        new int[]{0,1},
                        new int[]{-1,1},
                        new int[]{-1,0},
                        new int[]{-2,0},
                        new int[]{-2,-1},
                        new int[]{-2,-2},
                        new int[]{-1,-2},
                        new int[]{0,-2},
                        new int[]{1,-2},
                        new int[]{2,-2},
                        new int[]{2,-1},
                        new int[]{2,0},
                        new int[]{2,1},
                        new int[]{2,2},
                        new int[]{1,2},
                        new int[]{0,2},
                        new int[]{-1,2},
                        new int[]{-2,2},
                        new int[]{-2,1},
                        new int[]{-2,0}};

        /// <summary>
        /// foamliu, 2009/01/29, 建构 m × n 的矩阵.
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static double[][] BuildMat(int width, int height)
        {
            double[][] output = new double[width][];
            for (int i = 0; i < width; i++)
            {
                output[i] = new double[height];
            }
            return output;
        }

        /// <summary>
        /// foamliu, 2009/02/01, 构建三个矩阵以存放灰度图片.
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static int[][] BuildMatInt(int width, int height)
        {
            int[][] output = new int[width][];
            for (int i = 0; i < width; i++)
            {
                output[i] = new int[height];
            }
            return output;
        }

        /// <summary>
        /// foamliu, 2009/02/01, 构建三个矩阵以存放彩色图片.
        /// 
        /// 用第一个位置表示颜色(RGB)是为了方便重用为灰度图编写的算法.
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static int[][][] BuildMatIntColor(int width, int height)
        {
            int[][][] output = new int[3][][];
            // rgb
            for (int i = 0; i < 3; i++)
            {
                output[i] = new int[width][];

                for (int j = 0; j < width; j++)
                {
                    output[i][j] = new int[height];
                }
            }

            return output;
        }

        /// <summary>
        /// foamliu, 2009/01/29, 构建制定 sigma, (2n+1) × (2n+1) 的高斯核.
        /// 
        /// </summary>
        /// <param name="sigma"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static ConvKernel GetGaussianKernal(double sigma, int n)
        {
            double[][] filter = BuildMat(2 * n + 1, 2 * n + 1);

            for (int j = 0; j <= 2 * n; j++)
            {
                for (int i = 0; i <= 2 * n; i++)
                {
                    filter[i][j] = GetGaussian(sigma, i - n) * GetGaussian(sigma, j - n);
                }
            }

            ConvKernel conv = new ConvKernel(n, n, filter);
            return conv;
        }

        /// <summary>
        /// foamliu, 2009/01/29.
        /// 
        /// </summary>
        /// <param name="sigma"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private static double GetGaussian(double sigma, double x)
        {
            double part1 = Math.Exp(-x * x / (2 * sigma * sigma));
            double part2 = Math.Sqrt(2 * Math.PI) * sigma;
            return part1 / part2;
        }

        /// <summary>
        /// foamliu, 2009/02/04, 图像增强 - 对数变换.
        /// 
        /// filtered(x,y) = c log (1 + mat(x,y))
        /// 
        /// 主要参照：
        /// 
        ///     1.《数字图像处理(MATLAB版)》, 3.22节.
        ///     2. "Logarithm Operator" (http://homepages.inf.ed.ac.uk/rbf/HIPR2/pixlog.htm)
        /// 
        /// </summary>
        /// <param name="mat"></param>
        public static int[][] LogarithmicTransform(int[][] mat)
        {
            int width = mat.Length;
            int height = mat[0].Length;

            int max = Int32.MinValue;
            // 用于处理可能的输入负像素, 以后实现吧.
            int min = Int32.MaxValue;

            int[][] filtered = Util.BuildMatInt(width, height);

            double c = 0.0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (mat[x][y] > max)
                        max = mat[x][y];
                    // 用于处理可能的输入负像素, 以后实现吧.
                    if (mat[x][y] < min)
                        min = mat[x][y];
                }
            }

            // 保证最大输出为255.
            c = 255 / (Math.Log(1 + max));

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    filtered[x][y] = System.Convert.ToInt32(c * Math.Log(1 + mat[x][y]));
                }
            }

            System.Console.WriteLine("c = {0}", c);

            return filtered;
        }

        /// <summary>
        /// foamliu, 2009/02/04, 图像增强 - 对数变换的逆变换.
        /// 
        /// </summary>
        /// <param name="mat">对自身操作</param>
        /// <returns></returns>
        public static void InverseLogarithmicTransform(int[][] mat)
        {
            int width = mat.Length;
            int height = mat[0].Length;

            // 估计值
            double c = 20.0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    mat[x][y] = System.Convert.ToInt32(Math.Exp(mat[x][y] / c - 1));
                }
            }
        }

        /// <summary>
        /// foamliu, 2009/02/04, 中值滤波.
        /// 
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="k"></param>
        public static int[][] Median(int[][] mat, int k)
        {
            int width = mat.Length;
            int height = mat[0].Length;

            int[][] filtered = Util.BuildMatInt(width, height);

            List<int> list = new List<int>();
            int n = (k - 1) / 2;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int j = -n; j <= n; j++)
                    {
                        for (int i = -n; i <= n; i++)
                        {
                            list.Add(Util.GetPixel(mat, (x + i), (y + j)));
                        }
                    }
                    list.Sort();
                    filtered[x][y] = list[(k * k - 1) / 2];
                    list.Clear();
                }
            }

            return filtered;
        }

        /// <summary>
        /// foamliu, 2009/01/29, 以图像边界为中心, 令图像外某位置上未定义的灰度值
        /// 等于图像内其镜像位置的灰度值.
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public static int GetPixel(int[][] input, int x, int y)
        {
            int width = input.Length;
            int height = input[0].Length;

            if (x > 2 * width || x < -width) return 0;
            if (y > 2 * height || y < -height) return 0;

            int i, j;

            // 如果越界则返回镜像
            if (x < 0)
                i = -x;
            else if (x >= width)
                i = 2 * width - x - 2;
            else
                i = x;

            if (y < 0)
                j = -y;
            else if (y >= height)
                j = 2 * height - y - 2;
            else
                j = y;

            return input[i][j];
        }

        /// <summary>
        /// foamliu, 2009/02/11, 出界算0.
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        //public static int GetPixelZero(int[][] input, int x, int y)
        //{
        //    int width = input.Length;
        //    int height = input[0].Length;

        //    if (x >= 0 && x < width && y >= 0 && y < height)
        //    {
        //        return input[x][y];
        //    }
        //    else
        //        return 0;
        //}

        public static void SetPixel(int[][] output, int x, int y, int value)
        {
            int width = output.Length;
            int height = output[0].Length;

            if (x >= 0 && x < width && y >= 0 && y < height)
                output[x][y] = value;
        }

        /// <summary>
        /// foamliu, 2009/01/31, 点是否在指定矩形中.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static bool InRect(int x, int y, int width, int height)
        {
            return (y >= 0 && y < height && x >= 0 && x < width);
        }


        /// <summary>
        /// foamliu, 2009/02/11, 计算梯度.
        /// 
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="kernel"></param>
        /// <returns></returns>
        public static double Gradient(int[][] mat, int x, int y, ConvKernel kernel)
        {
            int m = kernel.M;
            int n = kernel.N;

            double sum = 0;
            for (int j = -n; j <= n; j++)
            {
                for (int i = -m; i <= m; i++)
                {
                    sum += Util.GetPixel(mat, x - i, y - j) * kernel.Filter(i, j);
                }
            }
            return sum;
        }

        public static double Gradient_Prewitt_X(int[][] mat, int x, int y)
        {
            return Gradient(mat, x, y, ConvKernel.Prewitt_Gx);
        }

        public static double Gradient_Prewitt_Y(int[][] mat, int x, int y)
        {
            return Gradient(mat, x, y, ConvKernel.Prewitt_Gy);
        }

        public static double Gradient_Sobel_X(int[][] mat, int x, int y)
        {
            return Gradient(mat, x, y, ConvKernel.Sobel_Gx);
        }

        public static double Gradient_Sobel_Y(int[][] mat, int x, int y)
        {
            return Gradient(mat, x, y, ConvKernel.Sobel_Gy);
        }

        /// <summary>
        /// foamliu, 2009/02/17, 两个点之间的欧几里德距离.
        /// 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double Distance(Point p1, Point p2)
        {
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }

        /// <summary>
        /// foamliu, 2009/02/17, 曲率.
        /// 
        /// </summary>
        /// <returns></returns>
        public static double Curvature(int[][] mat, Point p1, Point p2)
        {
            return 0.0;
        }

        /// <summary>
        /// foamliu, 2009/02/17, 计算平方.
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double Square(double x)
        {
            return x * x;
        }        
    }
}
