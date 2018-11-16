using System;
using System.Collections.Generic;
using System.Text;

namespace NComputerVision.Common
{
    /// <summary>
    /// foamliu, 2009/01/31, 添加注释, m × n 矩阵.
    /// foamliu, 2009/02/02, 越发认识到矩阵的重要性, 于是重构并添加了一些重要函数.
    /// </summary>
    public class NcvMatrix
    {
        private double[,] data;
        //private int m_cols;
        //private int m_rows;

        public int Cols
        {
            get { return data.GetLength(0); }
        }

        public int Rows
        {
            get { return data.GetLength(1); }
        }

        public double[,] Data
        {
            get { return data; }
        }

        public int rows()
        {
            return this.Rows;
        }

        public int cols()
        {
            return this.Cols;
        }

        public NcvMatrix(double[,] numbers)
        {
            data = numbers;
        }

        public NcvMatrix(double[] f, int cols, int rows)
        {
            data = new double[cols, rows];

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    int i = c * rows + r;
                    data[c, r] = f[i];
                }
            }
        }


        // create a square matrix initialized to the identity
        public NcvMatrix(int dimension)
        {
            data = new double[dimension, dimension];
            for (int i = 0; i < dimension; i++)
                for (int j = 0; j < dimension; j++)
                    data[i, j] = ((i == j) ? 1 : 0);
        }

        /// <summary>
        /// foamliu, 2009/02/02, 构建矩阵新实例.
        /// 
        /// </summary>
        // create a matrix of any dimensions initialized
        // to all zeroes.
        public NcvMatrix(int cols, int rows)
        {
            data = new double[cols, rows];
            for (int j = 0; j < rows; j++)
                for (int i = 0; i < cols; i++)
                    data[i, j] = 0;
        }

        public NcvMatrix(NcvMatrix M)
        {
            data = new double[M.rows(), M.cols()];
            copy(M);
        }

        // copy each cell from M to this, throwing an error
        // if M and this don't have the same dimensions.
        public void copy(NcvMatrix M)
        {
            if (M.rows() != rows() || M.cols() != cols())
                throw new ArgumentException("M");

            subcopy(M);
        }

        // copy each cell from M to this (or a sub-matrix of
        // this with the dimensions of M), assuming M has
        // less then or equal number of rows and columns as this.
        public void subcopy(NcvMatrix M)
        {
            for (int i = 0; i < M.rows(); i++)
                for (int j = 0; j < M.cols(); j++)
                    this[i, j] = M[i, j];
        }

        public double this[int i, int j]
        {
            get { return data[i, j]; }
            set { data[i, j] = value; }
        }

        /// <summary>
        /// foamliu, 2009/02/02, 矩阵的转置.
        /// 
        /// </summary>
        /// <returns></returns>
        public NcvMatrix Transpose()
        {
            NcvMatrix trans = new NcvMatrix(rows(), cols());

            for (int y = 0; y < rows(); y++)
            {
                for (int x = 0; x < cols(); x++)
                {
                    trans[y, x] = data[x, y];
                }
            }

            return trans;
        }

        /// <summary>
        /// foamliu, 2009/02/02, 取得 n × n 单位矩阵.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static NcvMatrix Identity(int n)
        {
            NcvMatrix id = new NcvMatrix(n, n);
            for (int i = 0; i < n; i++)
            {
                id[i, i] = 1.0;
            }
            return id;
        }

        /// <summary>
        /// foamliu, 2009/02/02, Addition (m3 = m1 + m2)
        /// </summary>
        /// <returns></returns>
        public static NcvMatrix operator +(NcvMatrix m1, NcvMatrix m2)
        {
            NcvMatrix m3 = new NcvMatrix(m1.Cols, m1.Rows);
            // m3 <- m1 + m2
            for (int y = 0; y < m1.Rows; y++)
            {
                for (int x = 0; x < m1.Cols; x++)
                {
                    m3[x, y] = m1[x, y] + m2[x, y];
                }
            }
            return m3;
        }

        public void Add(NcvMatrix m1)
        {
            for (int y = 0; y < rows(); y++)
            {
                for (int x = 0; x < cols(); x++)
                {
                    this[x, y] = this[x, y] + m1[x, y];
                }
            }
        }

        public static NcvMatrix operator -(NcvMatrix m1, NcvMatrix m2)
        {
            NcvMatrix m3 = new NcvMatrix(m1.Cols, m1.Rows);
            // m3 <- m1 + m2
            for (int y = 0; y < m1.Rows; y++)
            {
                for (int x = 0; x < m1.Cols; x++)
                {
                    m3[x, y] = m1[x, y] - m2[x, y];
                }
            }
            return m3;
        }

        public static NcvMatrix operator -(NcvMatrix m1, double s2)
        {
            NcvMatrix m3 = new NcvMatrix(m1.Cols, m1.Rows);
            // m3 <- m1 + m2
            for (int y = 0; y < m1.Rows; y++)
            {
                for (int x = 0; x < m1.Cols; x++)
                {
                    m3[x, y] = m1[x, y] - s2;
                }
            }
            return m3;
        }

        public static NcvMatrix operator -(NcvMatrix m1)
        {
            NcvMatrix m2 = new NcvMatrix(m1.Cols, m1.Rows);

            for (int y = 0; y < m1.Rows; y++)
            {
                for (int x = 0; x < m1.Cols; x++)
                {
                    m2[x, y] = -m1[x, y];
                }
            }
            return m2;
        }

        /// <summary>
        /// Multiplication by a scalar (m3 = m1 * s2) 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static NcvMatrix operator *(NcvMatrix m1, double s2)
        {
            NcvMatrix m3 = new NcvMatrix(m1.Cols, m1.Rows);

            for (int y = 0; y < m1.Rows; y++)
            {
                for (int x = 0; x < m1.Cols; x++)
                {
                    m3[x, y] = m1[x, y] * s2;
                }
            }
            return m3;
        }

        public static NcvMatrix operator *(double s1, NcvMatrix m2)
        {
            return m2 * s1;
        }

        public static NcvMatrix operator /(NcvMatrix m1, double s2)
        {
            NcvMatrix m3 = new NcvMatrix(m1.Cols, m1.Rows);

            for (int y = 0; y < m1.Rows; y++)
            {
                for (int x = 0; x < m1.Cols; x++)
                {
                    m3[x, y] = m1[x, y] / s2;
                }
            }
            return m3;
        }

        /// <summary>
        /// foamliu, 2009/02/02, 简单算法时间复杂度为 O(n^3), 而 Strassen 可以到 O(n^2.81).
        /// 但鉴于 Strassen 实现开销使其只有在 n 较大的时候才比简单算法快, 而这里无此需求, 所以只实现简单算法.
        /// 
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        public static NcvMatrix Product(NcvMatrix m1, NcvMatrix m2)
        {
            NcvMatrix m3 = new NcvMatrix(m1.Cols, m2.Rows);

            for (int y = 0; y < m1.Rows; y++)
            {
                for (int x = 0; x < m1.Cols; x++)
                {
                    m3[x, y] = m1[x, y] * m2[x, y];
                }
            }

            return m3;
        }


        /// <summary>
        /// foamliu, 2009/01/30, n × n 矩阵乘以 1 × n 的向量.
        /// 
        ///  r~      a00 a01 a02     r
        /// (c~) = ( a10 a11 a12 ) ( c )
        ///  1        0   0   1      1  
        /// 
        /// </summary>
        /// <param name="vec"></param>
        public double[] Multiply(double[] vec)
        {
            double[] result = new double[rows()];
            //res[0] = m_a[0,0] * vec[0] + m_a[0,1] * vec[1] + m_a[0,2] * 1;
            //res[1] = m_a[1,0] * vec[0] + m_a[1,1] * vec[1] + m_a[1,2] * 1;
            //res[2] = 1;

            for (int i = 0; i < cols(); i++)
            {
                double sum = 0.0;
                for (int j = 0; j < rows(); j++)
                {
                    sum += data[i, j] * vec[j];
                }
                result[i] = sum;
            }

            return result;
        }

        public static double[] Translate(double[] vec, double tr, double tc)
        {
            double[,] m = new double[,]{
                                {1.0, 0.0, tr},
                                {0.0, 1.0, tc},
                                {0.0, 0.0, 1.0}};

            NcvMatrix mat = new NcvMatrix(m);

            return mat.Multiply(vec);
        }

        public static double[] Scale(double[] vec, double sr, double sc)
        {
            double[,] m = new double[,]{
                                {sr, 0.0, 0.0},
                                {0.0, sc, 0.0},
                                {0.0, 0.0, 1.0}};

            NcvMatrix mat = new NcvMatrix(m);

            return mat.Multiply(vec);
        }

        public static double[] Rotate(double[] vec, double alpha)
        {
            double cos = System.Math.Cos(alpha);
            double sin = System.Math.Sin(alpha);

            double[,] m = new double[,]{
                                {cos, -sin, 0.0},
                                {sin, cos, 0.0},
                                {0.0, 0.0, 1.0}};

            NcvMatrix mat = new NcvMatrix(m);

            return mat.Multiply(vec);
        }

        public static double[] Skew(double[] vec, double theta)
        {
            double cos = System.Math.Cos(theta);
            double sin = System.Math.Sin(theta);

            double[,] m = new double[,]{
                                {cos, 0.0, 0.0},
                                {sin, 1.0, 0.0},
                                {0.0, 0.0, 1.0}};

            NcvMatrix mat = new NcvMatrix(m);

            return mat.Multiply(vec);
        }

        public static double[] TranslateInvert(double[] vec, double tr, double tc)
        {
            double[,] m = new double[,]{
                                {1.0, 0.0, -tr},
                                {0.0, 1.0, -tc},
                                {0.0, 0.0, 1.0}};

            NcvMatrix mat = new NcvMatrix(m);

            return mat.Multiply(vec);
        }

        public static double[] ScaleInvert(double[] vec, double sr, double sc)
        {
            double[,] m = new double[,]{
                                {1/sr, 0.0, 0.0},
                                {0.0, 1/sc, 0.0},
                                {0.0, 0.0, 1.0}};

            NcvMatrix mat = new NcvMatrix(m);

            return mat.Multiply(vec);
        }

        public static double[] RotateInvert(double[] vec, double alpha)
        {
            double cos = System.Math.Cos(alpha);
            double sin = System.Math.Sin(alpha);

            double[,] m = new double[,]{
                                {cos, sin, 0.0},
                                {-sin, cos, 0.0},
                                {0.0, 0.0, 1.0}};

            NcvMatrix mat = new NcvMatrix(m);

            return mat.Multiply(vec);
        }

        public static double[] SkewInvert(double[] vec, double theta)
        {
            double cos = System.Math.Cos(theta);
            double sin = System.Math.Sin(theta);

            double[,] m = new double[,]{
                                {1/cos, 0.0, 0.0},
                                {-sin/cos, 1.0, 0.0},
                                {0.0, 0.0, 1.0}};

            NcvMatrix mat = new NcvMatrix(m);

            return mat.Multiply(vec);
        }

        /// <summary>
        /// foamliu, 2009/02/03, 图像相加.
        /// 
        /// </summary>
        /// <param name="img1"></param>
        /// <param name="img2"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void MatAdd(int[][] mat1, int[][] mat2)
        {
            int width = mat1.Length;
            int height = mat1[0].Length;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    mat1[x][y] += mat2[x][y];
                }
            }
        }

        /// <summary>
        /// foamliu, 2009/02/03, 图像相减.
        /// 
        /// </summary>
        /// <param name="img1"></param>
        /// <param name="img2"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void MatSubtract(int[][] mat1, int[][] mat2)
        {
            int width = mat1.Length;
            int height = mat1[0].Length;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    mat1[x][y] -= mat2[x][y];
                }
            }
        }

        /// <summary>
        /// foamliu, 2009/02/03, 最小的像素值.
        /// 
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static int MatMin(int[][] mat)
        {
            int width = mat.Length;
            int height = mat[0].Length;
            int min = Int32.MaxValue;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (mat[x][y] < min)
                        min = mat[x][y];
                }
            }
            return min;
        }

        /// <summary>
        /// foamliu, 2009/02/03, 最大的像素值.
        /// 
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static int MatMax(int[][] mat)
        {
            int width = mat.Length;
            int height = mat[0].Length;
            int max = Int32.MinValue;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (mat[x][y] > max)
                        max = mat[x][y];
                }
            }
            return max;
        }

        public static void MatProduct(int[][] mat, double scale)
        {
            int width = mat.Length;
            int height = mat[0].Length;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    mat[x][y] = System.Convert.ToInt32(System.Math.Round(mat[x][y] * scale));
                }
            }
        }

        public static void MatAdd(int[][] mat, double value)
        {
            int width = mat.Length;
            int height = mat[0].Length;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    mat[x][y] += System.Convert.ToInt32(System.Math.Round(value));

                }
            }
        }

        /// <summary>
        /// foamliu, 2009/02/06, 从 A 到 B.
        /// 
        /// </summary>
        /// <param name="matA"></param>
        /// <param name="matB"></param>
        public static void MatCopy(int[][] matA, int[][] matB)
        {
            int width = matA.Length;
            int height = matA[0].Length;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    matB[x][y] = matA[x][y];

                }
            }
        }

        /// <summary>
        /// Normalize the matrix so that the sum is equal to 1
        /// </summary>
        public void Normalize()
        {
            double sum = 0.0;
            for (int y = 0; y < rows(); y++)
            {
                for (int x = 0; x < cols(); x++)
                {
                    sum += this[x, y];
                }
            }
            if (sum != 0)
            {
                for (int y = 0; y < rows(); y++)
                {
                    for (int x = 0; x < cols(); x++)
                    {
                        this[x, y] /= sum;
                    }
                }
            }
        }

        public static NcvMatrix diag(NcvVector v, int k)
        {
            int n = v.n;
            int order = n + Math.Abs(k);

            NcvMatrix m = new NcvMatrix(order, order);

            for (int i = 0; i < n; i++)
            {
                if (k <= 0)
                {
                    m[i, i - k] = v[i];
                }
                else
                {
                    m[i + k, i] = v[i];
                }
            }
            return m;
        }

        public static NcvMatrix diag(NcvVector v)
        {
            int n = v.n;
            NcvMatrix m = new NcvMatrix(n, n);
            for (int i = 0; i < n; i++)
            {
                m[i, i] = v[i];
            }
            return m;
        }

        // sets this Matrix to the inverse of the original Matrix
        // and returns this.
        public NcvMatrix inverse(NcvMatrix original)
        {
            if (original.rows() < 1 || original.rows() != original.cols()
               || rows() != original.rows() || rows() != cols()) return this;
            int n = rows();
            copy(new NcvMatrix(n)); // make identity matrix

            if (rows() == 1)
            {
                this[0, 0] = 1 / original[0, 0];
                return this;
            }

            NcvMatrix b = new NcvMatrix(original);

            for (int i = 0; i < n; i++)
            {
                // find pivot
                double mag = 0;
                int pivot = -1;

                for (int j = i; j < n; j++)
                {
                    double mag2 = Math.Abs(b[j, i]);
                    if (mag2 > mag)
                    {
                        mag = mag2;
                        pivot = j;
                    }
                }

                // no pivot (error)
                if (pivot == -1 || mag == 0)
                {
                    return this;
                }

                // move pivot row into position
                if (pivot != i)
                {
                    double temp;
                    for (int j = i; j < n; j++)
                    {
                        temp = b[i, j];
                        this[i, j] = b[pivot, j];
                        b[pivot, j] = temp;
                    }

                    for (int j = 0; j < n; j++)
                    {
                        temp = this[i, j];
                        this[i, j] = this[pivot, j];
                        this[pivot, j] = temp;
                    }
                }

                // normalize pivot row
                mag = b[i, i];
                for (int j = i; j < n; j++) b[i, j] = b[i, j] / mag;
                for (int j = 0; j < n; j++) this[i, j] = this[i, j] / mag;

                // eliminate pivot row component from other rows
                for (int k = 0; k < n; k++)
                {
                    if (k == i) continue;
                    double mag2 = b[k, i];

                    for (int j = i; j < n; j++) b[k, j] = b[k, j] - mag2 * b[i, j];
                    for (int j = 0; j < n; j++) this[k, j] = this[k, j] - mag2 * this[i, j];
                }
            }
            return this;
        }

        public static NcvMatrix inv(NcvMatrix original)
        {
            NcvMatrix inv = new NcvMatrix(original.cols(), original.rows());
            return inv.inverse(original);
        }

        public static NcvVector operator *(NcvMatrix m1, NcvVector v2)
        {
            NcvVector v3 = new NcvVector(v2.n);

            for (int y = 0; y < m1.rows(); y++)
            {
                for (int x = 0; x < m1.cols(); x++)
                {
                    v3[y] += m1[x, y] * v2[y];
                }
            }

            return v3;
        }

        public static NcvMatrix operator *(NcvMatrix m1, NcvMatrix m2)
        {
            NcvMatrix m3 = new NcvMatrix(m1.Cols, m2.Rows);
            for (int i = 0; i < m1.Cols; i++)
            {
                for (int j = 0; j < m2.Rows; j++)
                {
                    m3[i, j] += m1[i, j] * m2[i, j];
                }
            }
            return m3;
        }

        public override bool Equals(object obj)
        {
            NcvMatrix other = (NcvMatrix)obj;
            if (this.Rows != other.Rows
                || this.Cols != other.Cols)
            {
                return false;
            }
            for (int y = 0; y < this.rows(); y++)
            {
                for (int x = 0; x < this.cols(); x++)
                {
                    if (this[x, y] != other[x, y])
                        return false;
                }
            }

            return true;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Matrix\n");

            for (int y = 0; y < this.rows(); y++)
            {
                for (int x = 0; x < this.cols(); x++)
                {
                    sb.Append("\t" + this[x, y]);
                }
                sb.Append("\n");
            }

            return sb.ToString();
        }

        internal double min()
        {
            double min = double.MaxValue;
            for (int y = 0; y < this.rows(); y++)
            {
                for (int x = 0; x < this.cols(); x++)
                {
                    if (this[x, y] < min)
                        min = this[x, y];
                }
            }
            return min;
        }

        internal double max()
        {
            double max = double.MinValue;
            for (int y = 0; y < this.rows(); y++)
            {
                for (int x = 0; x < this.cols(); x++)
                {
                    if (this[x, y] > max)
                        max = this[x, y];
                }
            }
            return max;
        }

        internal void gradient(out NcvMatrix fx, out NcvMatrix fy)
        {
            fx = new NcvMatrix(cols(), rows());
            fy = new NcvMatrix(cols(), rows());

            for (int y = 1; y <= rows() - 2; y++)
                for (int x = 1; x <= cols() - 2; x++)
                {
                    /* NOTE: f is stored in column major */
                    fx[x, y] = 0.5 * (this[(x + 1), y] - this[(x - 1), y]);
                    fy[x, y] = 0.5 * (this[x, y + 1] - this[x, y - 1]);
                }
        }

        internal NcvMatrix Clone()
        {
            NcvMatrix clone = new NcvMatrix(cols(), rows());

            for (int y = 0; y < this.rows(); y++)
            {
                for (int x = 0; x < this.cols(); x++)
                {
                    clone[x, y] = this[x, y];
                }
            }

            return clone;
        }

        public static NcvMatrix DotProduct(NcvMatrix m1, NcvMatrix m2)
        {
            NcvMatrix m3 = new NcvMatrix(m1.cols(), m1.rows());
            for (int y = 0; y < m1.rows(); y++)
            {
                for (int x = 0; x < m1.cols(); x++)
                {
                    m3[x, y] = m1[x, y] * m2[x, y];
                }
            }
            return m3;
        }

        public static NcvMatrix del2(NcvMatrix m)
        {
            int rows = m.rows();
            int cols = m.cols();

            /* IV: Compute Laplace operator using Neuman condition */
            /* IV.1: Deal with corners */
            NcvMatrix Lu = new NcvMatrix(cols, rows);
            int rows_1 = rows - 1; int rows_2 = rows - 2;
            int cols_1 = cols - 1; int cols_2 = cols - 2;

            Lu[0, 0] = (m[0, 1] + m[1, 0]) * 0.5 - m[0, 0];
            Lu[cols_1, 0] = (m[cols_2, 0] + m[cols_1, 1]) * 0.5 - m[cols_1, 0];
            Lu[0, rows_1] = (m[1, rows_1] + m[0, rows_2]) * 0.5 - m[0, rows_1];
            Lu[cols_1, rows_1] = (m[cols_2, rows_1] + m[cols_1, rows_2]) * 0.5 - m[cols_1, rows_1];

            /* IV.2: Deal with left and right columns */
            for (int y = 1; y <= rows_2; y++)
            {
                Lu[0, y] = (2 * m[1, y] + m[0, y - 1] + m[0, y + 1]) * 0.25 - m[0, y];
                Lu[cols_1, y] = (2 * m[cols_2, y] + m[cols_1, y - 1]
                       + m[cols_1, y + 1]) * 0.25 - m[cols_1, y];
            }

            /* IV.3: Deal with top and bottom rows */
            for (int x = 1; x <= cols_2; x++)
            {
                Lu[x, 0] = (2 * m[x, 1] + m[x - 1, 0] + m[x + 1, 0]) * 0.25 - m[x, 0];
                Lu[x, rows_1] = (2 * m[x, rows_2] + m[x - 1, rows_1]
                       + m[x + 1, rows_1]) * 0.25 - m[x, rows_1];
            }

            /* IV.4: Compute interior */
            for (int y = 1; y <= rows_2; y++)
                for (int x = 1; x <= cols_2; x++)
                {
                    Lu[x, y] = (m[x - 1, y] + m[x + 1, y]
                        + m[x, y - 1] + m[x, y + 1]) * 0.25 - m[x, y];
                }



            return Lu;
        }

        public override int GetHashCode()
        {
            return data.GetHashCode();
        }
    }
}
