using System;
using System.Collections.Generic;
using System.Text;
using NComputerVision.Common;
using NComputerVision.DataStructures;

namespace NComputerVision.Contour
{
    /// <summary>
    /// 用参数化方法
    /// </summary>
    public class GvfSnake2
    {
        private int[][] mat;

        private int m_Cols;     /* Number of Columns */
        private int m_Rows;     /* Number of Rows */
        //private int no_of_snake_points;

        //public List<DoublePoint> Snake_points;
        public NcvVector x;
        public NcvVector y;
        NcvMatrix u, v;
        NcvMatrix mag;
        double[] px, py;

        public GvfSnake2(int[][] mat)
        {
            this.mat = mat;
            this.m_Cols = mat.Length;
            this.m_Rows = mat[0].Length;
        }

        public void InitSnake()
        {
            int xc = m_Cols / 2;
            int yc = m_Rows / 2;
            int radius = System.Math.Min(xc, yc) - 5;

            List<double> arrX = new List<double>();
            List<double> arrY = new List<double>();

            for (double t= 0; t <= 6.28; t+=0.05)
            {
                arrX.Add(xc + radius * System.Math.Cos(t));
                arrY.Add(yc - radius * System.Math.Sin(t));
            }

            x = new NcvVector(arrX.ToArray());
            y = new NcvVector(arrY.ToArray());
        }        

        public void InitGVF()
        {
            NcvMatrix f = new NcvMatrix(m_Cols, m_Rows);
            u = new NcvMatrix(m_Cols, m_Rows);
            v = new NcvMatrix(m_Cols, m_Rows);

            Console.WriteLine(" Compute edge map ...");
            for (int y = 0; y < m_Rows; y++)
            {
                for (int x = 0; x < m_Cols; x++)
                {
                    f[x, y] = 1 - mat[x][y] / 255.0;
                }
            }

            //Logger.Info("f=" + f.ToString());

            Console.WriteLine(" Compute GVF ...");
            GvfSnakeUtility.GVFC(m_Rows, m_Cols, f, ref u, ref v, 0.2, 80);

            //Logger.Info("u=" + u.ToString());
            //Logger.Info("v=" + v.ToString());

            Console.WriteLine(" Nomalizing the GVF external force ...");
            px = new double[m_Cols * m_Rows];
            py = new double[m_Cols * m_Rows];
            mag = new NcvMatrix(m_Cols, m_Rows);

            for (int y = 0; y < m_Rows; y++)
            {
                for (int x = 0; x < m_Cols; x++)
                {
                    int i = x * m_Rows + y;
                    mag[x, y] = System.Math.Sqrt(u[x, y] * u[x, y] + v[x, y] * v[x, y]);
                    px[i] = u[x, y] / (mag[x, y] + 1e-10); py[i] = v[x, y] / (mag[x, y] + 1e-10);
                }
            }

            //Logger.Info("px=" + px.ToString());
            //Logger.Info("px=" + px.ToString());
        }

        public void DoSnakeDeform(double alpha, double beta, double gamma, double kappa)
        {
            for (int i = 0; i < 10; i++)
            {
                SnakeDeform(ref x, ref y, alpha, beta, gamma, kappa, px, py, 10);
                SnakeInterp(ref x, ref y, 5, 2); // this is for student version   
                //
                //  for professional version, use     
                //  [x,y] = snakeinterp(x,y,2,0.5);  
            }

        }

        //  SNAKEINTERP -- Interpolate the snake adaptively    
        //  [xi,yi] = snakeinterp(x,y,dmax,dmin)
        //  dmax: the maximum distance between two snake points    
        //  dmin: the minimum distance between two snake points    
        //  d(i,i+1)>dmax, then a new point is added between i and i+1
        //  d(i,i+1)<dmin, then either i or i+1 is removed 
        //
        private void SnakeInterp(ref NcvVector x, ref NcvVector y, double dmax, double dmin)
        {
            NcvVector d = calc_d(x, y);

            List<int> IDX = new List<int>();
            for (int i = 0; i < x.n; i++)
                if (d[i] >= dmin) IDX.Add(i);

            NcvVector xx = new NcvVector(IDX.Count);
            NcvVector yy = new NcvVector(IDX.Count);
            for (int i = 0; i < IDX.Count; i++)
            {
                xx[i] = x[IDX[i]];
                yy[i] = y[IDX[i]];
            }
            x = xx; y = yy;


            IDX.Clear();
            for (int i = 0; i < x.n; i++)
                if (d[i] > dmax) IDX.Add(i);

            NcvVector z = Snakeindex(IDX, x.n);

            NcvVector p = new NcvVector(x.n + 1);
            for (int i = 0; i < x.n + 1; i++)
                p[i] = i;

            x = NcvVector.interp1(p, NcvVector.cat(x, x.sub(0, 1)), z);
            y = NcvVector.interp1(p, NcvVector.cat(y, y.sub(0, 1)), z);

            d = calc_d(x,y);
            Console.WriteLine(d.max());

            while (d.max() > dmax)
            {
                IDX.Clear();
                for (int i = 0; i < x.n; i++)
                    if (d[i] >= dmax) IDX.Add(i);
                z = Snakeindex(IDX, x.n);

                p = new NcvVector(x.n + 1);
                for (int i = 0; i < x.n + 1; i++)
                    p[i] = i;

                x = NcvVector.interp1(p, NcvVector.cat(x, x.sub(0, 1)), z);
                y = NcvVector.interp1(p, NcvVector.cat(y, y.sub(0, 1)), z);
              
                d = calc_d(x,y);
                Console.WriteLine(d.max());
            }   

        }

        private NcvVector calc_d(NcvVector x, NcvVector y)
        {
            int n = x.n;
            return (NcvVector.cat(x.sub(1, n - 1), x.sub(0, 1)) - x).abs()
                 + (NcvVector.cat(y.sub(1, n - 1), y.sub(0, 1)) - y).abs();
        }

        //  SNAKEINDEX  Create index for adpative interpolating the snake     
        //      y = snakeindex(IDX)    
        public NcvVector Snakeindex(List<int> IDX, int n)
        {
            List<double> y = new List<double>();
            for (int i = 0; i <= n - 1; i++)
            {
                y.Add(i);
                if (IDX.Contains(i))
                    y.Add(i + 0.5);
                
            }
            return new NcvVector(y.ToArray());
        }

        /// <summary>
        /// Deform snake in the given external force field
        /// 
        /// [x,y] = snakedeform(x,y,alpha,beta,gamma,kappa,fx,fy,ITER)    
        ///
        /// </summary>
        /// <param name="x">轮廓-X</param>
        /// <param name="y">轮廓-Y</param>
        /// <param name="alpha">elasticity parameter</param>
        /// <param name="beta">rigidity parameter</param>
        /// <param name="gamma">viscosity parameter</param>
        /// <param name="kappa">external force weight</param>
        /// <param name="fx">external force field (x)</param>
        /// <param name="fy">external force field (y)</param>
        /// <param name="ITER">iteration</param>
        public void SnakeDeform(ref NcvVector x, ref NcvVector y, double alpha, double beta, double gamma, double kappa, double[] fx, double[] fy, int ITER)
        {
            int n = x.n;
            NcvVector vecAlpha = alpha * NcvVector.Ones(n);
            NcvVector vecBeta = beta * NcvVector.Ones(n);

            // produce the five diagnal vectors
            NcvVector alpham1 = NcvVector.cat(vecAlpha.sub(1, n - 1), vecAlpha.sub(0, 1));
            NcvVector alphap1 = NcvVector.cat(vecAlpha.sub(n - 1, 1), vecAlpha.sub(0, n - 1));
            NcvVector betam1 = NcvVector.cat(vecBeta.sub(1, n - 1), vecBeta.sub(0, 1));
            NcvVector betap1 = NcvVector.cat(vecBeta.sub(n - 1, 1), vecBeta.sub(0, n - 1));

            NcvVector a = betam1;
            NcvVector b = -vecAlpha - 2 * vecBeta - 2 * betam1;
            NcvVector c = vecAlpha + alphap1 + betam1 + 4 * vecBeta + betap1;
            NcvVector d = -alphap1 - 2 * vecBeta - 2 * betap1;
            NcvVector e = betap1;

            // generate the parameters matrix
            NcvMatrix A = NcvMatrix.diag(a.sub(0, n - 2), -2) + NcvMatrix.diag(a.sub(n - 2, 2), n - 2);
            A = A + NcvMatrix.diag(b.sub(0, n - 1), -1) + NcvMatrix.diag(b.sub(n - 1, 1), n - 1);
            A = A + NcvMatrix.diag(c);
            A = A + NcvMatrix.diag(d.sub(0, n - 1), 1) + NcvMatrix.diag(d.sub(n - 1, 1), -(n - 1));
            A = A + NcvMatrix.diag(e.sub(0, n - 2), 2) + NcvMatrix.diag(e.sub(n - 2, 2), -(n - 2));

            //Logger.Info("A=" + A.ToString());

            NcvMatrix invAI = NcvMatrix.inv(A + gamma * NcvMatrix.diag(NcvVector.Ones(n)));

            //Logger.Info("invAI=" + invAI.ToString());

            //Logger.Info("fx=" + new Matrix(fx, m_Cols, m_Rows).ToString());
            //Logger.Info("fy=" + new Matrix(fy, m_Cols, m_Rows).ToString());

            for (int count = 0; count < ITER; count++)
            {
                NcvVector vfx = NcvVector.interp2(fx, m_Cols, m_Rows, x, y, "*linear");
                NcvVector vfy = NcvVector.interp2(fy, m_Cols, m_Rows, x, y, "*linear");

                //Logger.Info("vfx=" + vfx.ToString());
                //Logger.Info("vfy=" + vfy.ToString());

                // deform snake
                x = invAI * (gamma * x + kappa * vfx);
                y = invAI * (gamma * y + kappa * vfy);

                //Logger.Info("x=" + x.ToString());
                //Logger.Info("y=" + y.ToString());
            }
        }

        /// <summary>
        /// Deform snake in the given external force field with pressure force added
        /// </summary>
        /// <param name="x">轮廓-X</param>
        /// <param name="y">轮廓-Y</param>
        /// <param name="alpha">elasticity parameter</param>
        /// <param name="beta">rigidity parameter</param>
        /// <param name="gamma">viscosity parameter</param>
        /// <param name="kappa">external force weight</param>
        /// <param name="kappap">pressure force weight</param>
        /// <param name="fx">external force field (x)</param>
        /// <param name="fy">external force field (y)</param>
        /// <param name="ITER"></param>
        public void SnakeDeform2(ref NcvVector x, ref NcvVector y, double alpha, double beta, double gamma, double kappa, double kappap, double[] fx, double[] fy, int ITER)
        {
            int n = x.n;

            NcvVector vecAlpha = alpha * NcvVector.Ones(n);
            NcvVector vecBeta = beta * NcvVector.Ones(n);

            // produce the five diagnal vectors
            NcvVector alpham1 = NcvVector.cat(vecAlpha.sub(1, n - 1), vecAlpha.sub(0, 1));
            NcvVector alphap1 = NcvVector.cat(vecAlpha.sub(n - 1, 1), vecAlpha.sub(0, n - 1));
            NcvVector betam1 = NcvVector.cat(vecBeta.sub(1, n - 1), vecBeta.sub(0, 1));
            NcvVector betap1 = NcvVector.cat(vecBeta.sub(n - 1, 1), vecBeta.sub(0, n - 1));

            NcvVector a = betam1;
            NcvVector b = -vecAlpha - 2 * vecBeta - 2 * betam1;
            NcvVector c = vecAlpha + alphap1 + betam1 + 4 * vecBeta + betap1;
            NcvVector d = -alphap1 - 2 * vecBeta - 2 * betap1;
            NcvVector e = betap1;

            // generate the parameters matrix
            NcvMatrix A = NcvMatrix.diag(a.sub(0, n - 2), -2) + NcvMatrix.diag(a.sub(n - 2, 2), n - 2);
            A = A + NcvMatrix.diag(b.sub(0, n - 1), -1) + NcvMatrix.diag(b.sub(n - 1, 1), n - 1);
            A = A + NcvMatrix.diag(c);
            A = A + NcvMatrix.diag(d.sub(0, n - 1), 1) + NcvMatrix.diag(d.sub(n - 1, 1), -(n - 1));
            A = A + NcvMatrix.diag(e.sub(0, n - 2), 2) + NcvMatrix.diag(e.sub(n - 2, 2), -(n - 2));

            NcvMatrix invAI = NcvMatrix.inv(A + gamma * NcvMatrix.diag(NcvVector.Ones(n)));

            for (int count = 0; count < ITER; count++)
            {
                NcvVector vfx = NcvVector.interp2(fx, m_Cols, m_Rows, x, y, "*linear");
                NcvVector vfy = NcvVector.interp2(fy, m_Cols, m_Rows, x, y, "*linear");

                // adding the pressure force 	
                NcvVector xp = NcvVector.cat(x.sub(1, n - 1), x.sub(0, 1)); NcvVector yp = NcvVector.cat(y.sub(1, n - 1), y.sub(0, 1));
                NcvVector xm = NcvVector.cat(x.sub(n - 1, 1), x.sub(0, n - 1)); NcvVector ym = NcvVector.cat(y.sub(n - 1, 1), y.sub(0, n - 1));

                NcvVector qx = xp - xm; NcvVector qy = yp - ym;
                NcvVector pmag = NcvVector.Sqrt(NcvVector.DotProduct(qx, qx) + NcvVector.DotProduct(qy, qy));
                NcvVector px = NcvVector.DotDivide(qy, pmag); NcvVector py = NcvVector.DotDivide(-qx, pmag);

                // deform snake
                x = invAI * (gamma * x + kappa * vfx + kappap * px);
                y = invAI * (gamma * y + kappa * vfy + kappap * py);
            }
        }



        private int round(double value)
        {
            return Convert.ToInt32(Math.Round(value));
        }
    }
}
