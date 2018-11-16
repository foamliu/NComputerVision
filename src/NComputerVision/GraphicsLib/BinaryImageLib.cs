

namespace NComputerVision.GraphicsLib
{
    using NComputerVision.Common;
    using NComputerVision.DataStructures;

    /// <summary>
    /// foamliu, 2009/02/06, 关于二值图像的一些函数, 主要参与与区域有关的算法.
    /// 
    /// </summary>
    public static class BinaryImageLib
    {
        /// <summary>
        /// foamiu, 2009/02/06, ROI是否为空.
        /// 
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        public static bool Empty(int[][] mat)
        {
            int width = mat.Length;
            int height = mat[0].Length;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (mat[x][y] == 1)
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// foamiu, 2009/02/06, 并集, 结果存在 A 中.
        /// 假定二者尺寸一致.
        /// 
        /// </summary>
        /// <param name="matA"></param>
        /// <param name="matB"></param>
        public static void Union(int[][] matA, int[][] matB)
        {
            int width = matA.Length;
            int height = matA[0].Length;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (matA[x][y] == 1 || matB[x][y] == 1)
                        matA[x][y] = 1;
                }
            }
        }

        /// <summary>
        /// foamiu, 2009/02/06, 交集, 结果存在 A 中.
        /// 假定二者尺寸一致.
        /// 
        /// </summary>
        /// <param name="matA"></param>
        /// <param name="matB"></param>
        public static void Intersection(int[][] matA, int[][] matB)
        {
            int width = matA.Length;
            int height = matA[0].Length;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (matA[x][y] == 1 && matB[x][y] == 1)
                        matA[x][y] = 1;
                }
            }
        }

        /// <summary>
        /// foamiu, 2009/02/06, 差集, 结果存在 A 中.
        /// 假定二者尺寸一致.
        /// 
        /// </summary>
        /// <param name="matA"></param>
        /// <param name="matB"></param>
        //public static void Difference(int[][] matA, int[][] matB)
        //{
        //    int width = matA.Length;
        //    int height = matA[0].Length;

        //    for (int y = 0; y < height; y++)
        //    {
        //        for (int x = 0; x < width; x++)
        //        {
        //            if (matA[x][y] == 1 && matB[x][y] == 0)
        //                matA[x][y] = 1;
        //        }
        //    }
        //}

        /// <summary>
        /// foamiu, 2009/02/06, 差集, 结果存在 A 中.
        /// 假定二者尺寸一致.
        /// 
        /// </summary>
        /// <param name="matA"></param>
        /// <param name="matB"></param>
        public static int[][] Difference(int[][] matA, int[][] matB)
        {
            int width = matA.Length;
            int height = matA[0].Length;

            int[][] output = Util.BuildMatInt(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (matA[x][y] == 1 && matB[x][y] == 0)
                        output[x][y] = 1;
                }
            }

            return output;
        }

        /// <summary>
        /// foamiu, 2009/02/06, 补集, 结果存在 R 中.
        /// 
        /// </summary>
        /// <param name="matR"></param>
        public static void Complement(int[][] matR)
        {
            int width = matR.Length;
            int height = matR[0].Length;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (matR[x][y] == 1)
                        matR[x][y] = 0;
                    else
                        matR[x][y] = 1;
                }
            }
        }

        /// <summary>
        /// foamiu, 2009/02/06, 平移, 结果返回.
        /// 
        /// </summary>       
        public static void Translation(int[][] matR, int tx, int ty, out int[][] matRt)
        {
            int width = matR.Length;
            int height = matR[0].Length;

            matRt = Util.BuildMatInt(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (matR[x][y] == 1 && Util.InRect(x + tx, y + ty, width, height))
                        matRt[x + tx][y + ty] = 1;
                }
            }
        }

        /// <summary>
        /// foamiu, 2009/02/06, 转置, 结果返回.
        /// 
        /// </summary>       
        public static void Transposition(int[][] matR, int ox, int oy, out int[][] matRtrans)
        {
            int width = matR.Length;
            int height = matR[0].Length;

            matRtrans = Util.BuildMatInt(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (matR[x][y] == 1 && Util.InRect(2 * ox - x, 2 * oy - y, width, height))
                        matRtrans[2 * ox - x][2 * oy - y] = 1;
                }
            }
        }

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
                    if (matR[x][y] == 0) continue;

                    for (int j = 0; j < hS; j++)
                    {
                        for (int i = 0; i < wS; i++)
                        {
                            if (matS[i][j] == 0) continue;

                            // 矢量和在 R 中的位置
                            //
                            int sumx = x + i - ox;
                            int sumy = y + j - oy;

                            // 必须所有平移后的点都在 R 中
                            if (Util.InRect(sumx, sumy, wR, hR) && matR[sumx][sumy] == 0)
                            {
                                goto next;
                            }
                        }
                    }

                    matResult[x][y] = 1;

                next: ;
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
                    if (matR[x][y] == 0) continue;

                    for (int j = 0; j < hS; j++)
                    {
                        for (int i = 0; i < wS; i++)
                        {
                            if (matS[i][j] == 0) continue;

                            // 矢量和在 R 中的位置
                            //
                            int sumx = x + i - ox;
                            int sumy = y + j - oy;

                            if (Util.InRect(sumx, sumy, wR, hR))
                            {
                                matResult[sumx][sumy] = 1;
                            }
                        }
                    }
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
        /// foamliu, 2009/02/04, 膨胀.
        /// 
        /// </summary>
        /// <param name="mat">二值图像</param>
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

            //int m = b.Width;
            //int n = b.height;

            //for (int y = 0; y < height; y++)
            //{
            //    for (int x = 0; x < width; x++)
            //    {
            //        if (mat[x][y] != 1)
            //            continue;

            //        for (int j = -n; j <= n; j++)
            //        {
            //            for (int i = -m; i <= m; i++)
            //            {
            //                // foamliu, 2009/02/04, 可能出界.
            //                if (b.B(i, j) == 1)
            //                    //output[x + i][y + j] = 1;
            //                    Util.SetPixel(output, x + i, y + j, 1);
            //            }
            //        }

            //    }
            //}

            // foamliu, 2009/02/06, 改用二值图像库中的算法.
            //
            // 先把 strel 求转置
            //
            StructuringElement trans = StructuringElement.Transposition(strel);

            BinaryImageLib.MinkowskiAddtion(mat, trans, out output);

        }

        /// <summary>
        /// foamliu, 2009/02/04, 腐蚀.
        /// 
        /// </summary>
        /// <param name="mat">二值图像</param>
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

            //int m = b.Width;
            //int n = b.height;

            //for (int y = 0; y < height; y++)
            //{
            //    for (int x = 0; x < width; x++)
            //    {
            //        if (mat[x][y] != 1)
            //            continue;

            //        for (int j = -n; j <= n; j++)
            //        {
            //            for (int i = -m; i <= m; i++)
            //            {
            //                if (b.B(i, j) == 0)
            //                    continue;

            //                if (Util.GetPixel(mat,x + i,y + j) == 0)
            //                    goto next;
            //            }
            //        }

            //        output[x][y] = 1;

            //    next: ;


            //    }
            //}


            // foamliu, 2009/02/06, 改用二值图像库中的算法.
            //
            // 先把 strel 求转置
            //            
            StructuringElement trans = StructuringElement.Transposition(strel);

            BinaryImageLib.MinkowskiSubtraction(mat, trans, out output);
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
            BinaryImageLib.MinkowskiAddtion(mat, strel, out output);
        }

        // foamliu, 2009/02/04, 开操作.
        //
        public static int[][] Open(int[][] mat, StructuringElement strel)
        {
            int[][] temp, output;
            Erosion(mat, strel, out temp);
            BinaryImageLib.MinkowskiAddtion(temp, strel, out output);
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
            BinaryImageLib.MinkowskiSubtraction(temp, strel, out output);
        }

        /// <summary>
        /// foamliu, 2009/02/04, 翻转.
        /// </summary>
        /// <param name="mat">二值图像</param>
        public static void Inverse(int[][] mat)
        {
            int width = mat.Length;
            int height = mat[0].Length;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    mat[x][y] = mat[x][y] == 1 ? 0 : 1;
                }
            }

        }

        /// <summary>
        /// foamliu, 2009/02/05, 抽取骨骼.
        /// 
        /// 主要参照:
        /// 
        ///     佳奇发给我 Morphological Image Process 幻灯中倒数第三页: Skeleton Equations.
        ///     
        /// </summary>
        public static void ExtractSkeleton(int[][] mat, StructuringElement strel, out int[][] output)
        {
            int width = mat.Length;
            int height = mat[0].Length;
            int[][] temp = (int[][])mat.Clone();
            int K = CalcK(mat, strel);

            int[][] Sk;
            output = Util.BuildMatInt(width, height);

            for (int k = 1; k <= K; k++)
            {
                temp = BinaryImageLib.MinkowskiSubtraction(temp, strel);
                Sk = BinaryImageLib.Difference(temp, BinaryImageLib.Open(temp, strel));
                BinaryImageLib.Union(output, Sk);
            }

        }

        // 计算 Skeleton Equations 中的 K.
        //
        private static int CalcK(int[][] mat, StructuringElement strel)
        {
            int[][] temp = (int[][])mat.Clone();
            int k = 0;

            while (true)
            {
                temp = BinaryImageLib.MinkowskiSubtraction(temp, strel);
                if (BinaryImageLib.Empty(temp))
                    break;
                else
                    k++;
            }

            return k;
        }

        public static void bwperim(int[,] src, out int[,] dst, int width, int height, int n)
        {
            int i;
            int j;

            dst = new int[width, height];

            for (j = 1; j < height - 1; j++)
            {
                for (i = 1; i < width - 1; i++)
                {
                    if (n == 4)
                    {
                        if ((src[i, j] == 255)
                            && (src[i, -1 + j] == 255)
                            && (src[i, +1 + j] == 255)
                            && (src[(i - 1), +j] == 255)
                            && (src[(i + 1), +j] == 255)
                            )
                        {
                            dst[i, +j] = 128;
                        }
                    }
                    else

                        if ((src[i, +j] == 255)
                            && (src[i, -1 + j] == 255)
                            && (src[i, +1 + j] == 255)
                            && (src[(i - 1), +j] == 255)
                            && (src[(i + 1), +j] == 255)
                            && (src[(i - 1), +1 + j] == 255)
                            && (src[(i + 1), -1 + j] == 255)
                            && (src[(i + 1), +1 + j] == 255)
                            && (src[(i - 1), -1 + j] == 255)
                         )
                        {
                            dst[i, +j] = 128;
                        }
                }
            }
        }

    }
}
