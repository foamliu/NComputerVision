

namespace NComputerVision.DataStructures
{
    using System;
    using NComputerVision.Common;

    /// <summary>
    /// foamliu, 2009/02/06, 形态学结构单元.
    /// 
    /// </summary>
    public class StructuringElement
    {
        // 结构单元的尺寸.
        private int m_width;
        private int m_height;
        // 原点的位置, 默认(0,0).
        //
        private int m_ox;
        private int m_oy;

        // 0, 1 矩阵
        private int[][] m_mat;

        public int Width
        {
            get { return m_width; }
        }

        public int Height
        {
            get { return m_height; }
        }

        public int Ox
        {
            get { return m_ox; }
            set { m_ox = value; }
        }

        public int Oy
        {
            get { return m_oy; }
            set { m_oy = value; }
        }

        public StructuringElement(int m, int n, int[][] b)
        {
            m_width = m;
            m_height = n;

            m_mat = b;
        }

        public StructuringElement(int m, int n, int[][] b, int ox, int oy)
        {
            m_width = m;
            m_height = n;
            m_mat = b;
            m_ox = ox;
            m_oy = oy;
        }

        public StructuringElement Clone()
        {
            return new StructuringElement(this.Width, this.Height, (int[][])this.Mat.Clone(), this.Ox, this.Oy);
        }

        //public int B(int x, int y)
        //{
        //    if (Math.Abs(y) <= m_n && Math.Abs(x) <= m_m)
        //        return m_b[x + m_m][y + m_n];
        //    else
        //        return 0;
        //}

        // 矩阵
        //
        public int[][] Mat
        {
            get { return m_mat; }
        }

        /// <summary>
        /// foamliu, 2009/02/06, 结构元素转置
        /// 
        /// </summary>
        /// <param name="strel"></param>
        public static StructuringElement Transposition(StructuringElement strel)
        {
            StructuringElement output = strel.Clone();

            for (int y = 0; y < strel.Height; y++)
            {
                for (int x = 0; x < (strel.Width - 1) / 2; x++)
                {
                    int temp = output.Mat[x][y];
                    output.Mat[x][y] = output.Mat[output.Width - 1 - x][output.Height - 1 - y];
                    output.Mat[output.Width - 1 - x][output.Height - 1 - y] = temp;
                }
            }

            output.Ox = output.Width - 1 - output.Ox;
            output.Oy = output.Height - 1 - output.Oy;

            return output;
        }

        /// <summary>
        /// foamliu, 2009/02/04, 形态学结构单位也来凑凑热闹， ^_^.
        /// </summary>
        public static StructuringElement N4
        {
            get
            {
                int[][] strel = new int[3][]{
                                new int[3]{0, 1, 0},
                                new int[3]{1, 1, 1},
                                new int[3]{0, 1, 0}};

                return new StructuringElement(3, 3, strel, 1, 1);
            }
        }

        public static StructuringElement N8
        {
            get
            {
                int[][] strel = new int[3][]{
                                new int[3]{1, 1, 1},
                                new int[3]{1, 1, 1},
                                new int[3]{1, 1, 1}};

                return new StructuringElement(3, 3, strel, 1, 1);
            }
        }

        public static StructuringElement Nd
        {
            get
            {
                int[][] strel = new int[3][]{
                                new int[3]{1, 0, 1},
                                new int[3]{0, 1, 0},
                                new int[3]{1, 0, 1}};

                return new StructuringElement(3, 3, strel, 1, 1);
            }
        }

        /// <summary>
        /// foamliu, 2009/02/06, 实心圆形结构单元.
        /// 
        /// </summary>
        /// <param name="diameter">直径, 应为奇数.</param>
        /// <returns></returns>
        public static StructuringElement Disk(int diameter)
        {
            int[][] strel = Util.BuildMatInt(diameter, diameter);

            for (int y = 0; y < diameter; y++)
            {
                for (int x = 0; x < diameter; x++)
                {
                    if (Math.Sqrt((x - diameter / 2) * (x - diameter / 2) + (y - diameter / 2) * (y - diameter / 2)) <= diameter / 2)
                        strel[x][y] = 1;
                    else
                        strel[x][y] = 0;
                }
            }

            return new StructuringElement(diameter, diameter, strel, diameter / 2, diameter / 2);
        }

        /// <summary>
        /// foamliu, 2009/02/06, 实心方形结构单元.
        /// 
        /// </summary>
        /// <param name="edge">边长, 应为奇数.</param>
        /// <returns></returns>
        public static StructuringElement Square(int edge)
        {
            int[][] strel = Util.BuildMatInt(edge, edge);

            for (int y = 0; y < edge; y++)
            {
                for (int x = 0; x < edge; x++)
                {
                    strel[x][y] = 1;
                }
            }

            return new StructuringElement(edge, edge, strel, edge / 2, edge / 2);
        }
    }
}
