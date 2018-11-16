
namespace NComputerVision.GraphicsLib
{
    using System;
    using NComputerVision.Common;

    /// <summary>
    /// foamliu, 2009/01/29, 卷积核.
    /// </summary>
    public class ConvKernel
    {
        private int m_m;
        private int m_n;

        private double[][] m_filter;

        public int M
        {
            get { return m_m; }
        }

        public int N
        {
            get { return m_n; }
        }

        public ConvKernel(int m, int n, double[][] filter)
        {
            m_m = m; 
            m_n = n;             
            
            m_filter = filter;
        }

        public double Filter(int x, int y)
        {
            if (Math.Abs(y) <= m_n && Math.Abs(x) <= m_m)
                return m_filter[x + m_m][y + m_n];
            else 
                return 0.0;
        }

        /// <summary>
        /// foamliu, 2009/02/03, 不是今天做的, 添点注释而已.
        /// 
        ///   +1 0 -1       1
        /// [ +2 0 -2 ] = [ 2 ] [+1 0 -1]
        ///   +1 0 -1       1 
        ///   
        /// 这个不错:
        /// http://en.wikipedia.org/wiki/Sobel_operator
        /// 
        /// </summary>
        public static ConvKernel Sobel_Gx
        {
            get
            {
                double[][] filter = new double[3][]{
                                new double[3]{+1, 0, -1},
                                new double[3]{+2, 0, -2},
                                new double[3]{+1, 0, -1}};

                return new ConvKernel(1, 1, filter);
            }
        }

        /// <summary>
        /// foamliu, 2009/02/03, 不是今天做的, 添点注释而已.
        /// 
        ///   +1 +2 +1       +1          
        /// [  0  0  0 ] = [  0 ] [+1 +2 +1]
        ///   -1 -2 -1       -1          
        /// 
        /// </summary>
        public static ConvKernel Sobel_Gy
        {
            get
            {
                double[][] filter = new double[3][]{
                                new double[3]{+1, +2, +1},
                                new double[3]{0, 0, 0},
                                new double[3]{-1, -2, -1}};

                return new ConvKernel(1, 1, filter);
            }
        }

        public static ConvKernel Prewitt_Gx
        {
            get
            {
                double[][] filter = new double[3][]{
                                new double[3]{+1, 0, -1},
                                new double[3]{+1, 0, -1},
                                new double[3]{+1, 0, -1}};

                return new ConvKernel(1, 1, filter);
            }
        }

        public static ConvKernel Prewitt_Gy
        {
            get
            {
                double[][] filter = new double[3][]{
                                new double[3]{+1, +1, +1},
                                new double[3]{0, 0, 0},
                                new double[3]{-1, -1, -1}};

                return new ConvKernel(1, 1, filter);
            }
        }

        public static ConvKernel Laplacian_4
        {
            get
            {
                double[][] filter = new double[3][]{
                                new double[3]{0, +1, 0},
                                new double[3]{+1, -4, +1},
                                new double[3]{0, +1, 0}};

                return new ConvKernel(1, 1, filter);
            }
        }

        public static ConvKernel Laplacian_8
        {
            get
            {
                double[][] filter = new double[3][]{
                                new double[3]{+1, +1, +1},
                                new double[3]{+1, -8, +1},
                                new double[3]{+1, +1, +1}};

                return new ConvKernel(1, 1, filter);
            }
        }

        /// <summary>
        /// foamliu, 2009/01/29, k × k (k = 2n + 1) 均值核.
        /// 
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public static ConvKernel GetKMeanKernal(int k)
        {
            int n = (k-1)/2;
            double[][] filter = Util.BuildMat(k, k);

            for (int i = 0; i <= 2 * n; i++)
            {
                for (int j = 0; j <= 2 * n; j++)
                {
                    filter[i][j] = 1.0 / (k * k);
                }
            }

            ConvKernel conv = new ConvKernel(n, n, filter);
            return conv;
        }
    }


    
    
}
