using System;
using System.Collections.Generic;
using System.Text;
using NComputerVision.Common;

namespace NComputerVision.Contour
{
    public static class GvfSnakeUtility
    {
        /* NOTE:
         * 
         * You must have "mex" command working in your matlab.  In matlab, 
         * type "mex gvfc.c" to compile the code. See usage for details of 
         * calling this function.
         *
         * Replace GVF(...) with GVFC in the snakedeform.m. The speed is 
         * significantly faster than the Matlab version.
         */

        /***************************************************************************
        Copyright (c) 1996-1999 Chenyang Xu and Jerry Prince.

        This software is copyrighted by Chenyang Xu and Jerry Prince.  The
        following terms apply to all files associated with the software unless
        explicitly disclaimed in individual files.

        The authors hereby grant permission to use this software and its
        documentation for any purpose, provided that existing copyright
        notices are retained in all copies and that this notice is included
        verbatim in any distributions. Additionally, the authors grant
        permission to modify this software and its documentation for any
        purpose, provided that such modifications are not distributed without
        the explicit consent of the authors and that existing copyright
        notices are retained in all copies.

        IN NO EVENT SHALL THE AUTHORS OR DISTRIBUTORS BE LIABLE TO ANY PARTY FOR
        DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES ARISING OUT
        OF THE USE OF THIS SOFTWARE, ITS DOCUMENTATION, OR ANY DERIVATIVES THEREOF,
        EVEN IF THE AUTHORS HAVE BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

        THE AUTHORS AND DISTRIBUTORS SPECIFICALLY DISCLAIM ANY WARRANTIES, INCLUDING,
        BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
        PARTICULAR PURPOSE, AND NON-INFRINGEMENT.  THIS SOFTWARE IS PROVIDED ON AN
        "AS IS" BASIS, AND THE AUTHORS AND DISTRIBUTORS HAVE NO OBLIGATION TO PROVIDE
        MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
        ****************************************************************************/


        /* 
         * [u,v] = GVFC(f, mu, ITER)
         * 
         * assuming dx = dy = 1, and dt = 1, 
         * need to figure out CFL
         *
         * Gradient Vector Flow
         *
         * Chenyang Xu and Jerry L. Prince 
         * Copyright (c) 1996-99 by Chenyang Xu and Jerry L. Prince
         * Image Analysis and Communications Lab, Johns Hopkins University
         *
         */


        /* recommended value for mu to get stable solution: mu in [0, 0.2] */

        /* PG Macros */


        //typedef unsigned char  PGbyte;
        //typedef          char  PGchar;
        //typedef unsigned short PGushort;
        //typedef          short PGshort; 
        //typedef unsigned int   PGuint;
        //typedef          int   PGint;
        //typedef float          PGfloat;
        //typedef double         double;
        //typedef void           PGvoid;


        private static double PG_MAX(double a, double b)
        {
            return (a > b ? a : b);
        }

        private static double PG_MIN(double a, double b)
        {
            return (a < b ? a : b);
        }


        /* Prints error message: modified to work in mex environment */
        private static void pgError(string error_text)
        {
            //Console.WriteLine("GVFC.MEX: {0}", error_text);
            return;
        }


        /* Allocates a matrix of doubles with range [nrl..nrh,ncl..nch] */
        private static double[,] pgDmatrix(int nrl, int nrh, int ncl, int nch)
        {
            return new double[nrh + 1, nch + 1];
        }


        /* Frees a matrix allocated by dmatrix */
        private static void pgFreeDmatrix(double[,] m, int nrl, int nrh, int ncl, int nch)
        {
            //free((char*) (m+nrl));
            return;
        }

        // Compute the gradient vector flow field
        /// <summary>
        /// 
        /// </summary>
        /// <param name="numRows"></param>
        /// <param name="numCols"></param>
        /// <param name="f">edge map f</param>
        /// <param name="ou"></param>
        /// <param name="ov"></param>
        /// <param name="mu">GVF regularization coefficient</param>
        /// <param name="ITER">the number of iterations that will be computed</param>
        internal static void GVFC(int numRows, int numCols, double[] f, ref double[] ou, ref double[] ov,
              double mu, int ITER)
        {
            double mag2, temp, tempx, tempy, fmax, fmin;
            int count, x, y, numCols_1, numCols_2, numRows_1, YN_2;

            double[,] fx, fy, u, v, Lu, Lv, g, c1, c2, b;

            /* define constants and create row-major double arrays */
            numCols_1 = numCols - 1;
            numCols_2 = numCols - 2;
            numRows_1 = numRows - 1;
            YN_2 = numRows - 2;
            fx = new double[numRows, numCols];
            fy = new double[numRows, numCols];
            u = new double[numRows, numCols];
            v = new double[numRows, numCols];
            Lu = new double[numRows, numCols];
            Lv = new double[numRows, numCols];
            g = new double[numRows, numCols];
            c1 = new double[numRows, numCols];
            c2 = new double[numRows, numCols];
            b = new double[numRows, numCols];


            /************** I: Normalize the edge map to [0,1] **************/
            fmax = double.MinValue;
            fmin = double.MaxValue;
            for (x = 0; x <= numRows * numCols - 1; x++)
            {
                fmax = PG_MAX(fmax, f[x]);
                fmin = PG_MIN(fmin, f[x]);
            }

            if (fmax == fmin)
                Console.Error.WriteLine("Edge map is a constant image.");
            else
                for (x = 0; x <= numRows * numCols - 1; x++)
                    f[x] = (f[x] - fmin) / (fmax - fmin);

            /**************** II: Compute edge map gradient *****************/
            /* I.1: Neumann boundary condition: 
             *      zero normal derivative at boundary 
             */
            /* Deal with corners */
            fx[0, 0] = fy[0, 0] = fx[0, numCols_1] = fy[0, numCols_1] = 0;
            fx[numRows_1, numCols_1] = fy[numRows_1, numCols_1] = fx[numRows_1, 0] = fy[numRows_1, 0] = 0;

            /* Deal with left and right column */
            for (y = 1; y <= YN_2; y++)
            {
                fx[y, 0] = fx[y, numCols_1] = 0;
                fy[y, 0] = 0.5 * (f[y + 1] - f[y - 1]);
                fy[y, numCols_1] = 0.5 * (f[y + 1 + numCols_1 * numRows] - f[y - 1 + numCols_1 * numRows]);
            }

            /* Deal with top and bottom row */
            for (x = 1; x <= numCols_2; x++)
            {
                fy[0, x] = fy[numRows_1, x] = 0;
                fx[0, x] = 0.5 * (f[(x + 1) * numRows] - f[(x - 1) * numRows]);
                fx[numRows_1, x] = 0.5 * (f[numRows_1 + (x + 1) * numRows] - f[numRows_1 + (x - 1) * numRows]);
            }

            /* I.2: Compute interior derivative using central difference */
            for (y = 1; y <= YN_2; y++)
                for (x = 1; x <= numCols_2; x++)
                {
                    /* NOTE: f is stored in column major */
                    fx[y, x] = 0.5 * (f[y + (x + 1) * numRows] - f[y + (x - 1) * numRows]);
                    fy[y, x] = 0.5 * (f[y + 1 + x * numRows] - f[y - 1 + x * numRows]);
                }

            /******* III: Compute parameters and initializing arrays **********/
            temp = -1.0 / (mu * mu);
            for (y = 0; y <= numRows_1; y++)
                for (x = 0; x <= numCols_1; x++)
                {
                    tempx = fx[y, x];
                    tempy = fy[y, x];
                    /* initial GVF vector */
                    u[y, x] = tempx;
                    v[y, x] = tempy;
                    /* gradient magnitude square */
                    mag2 = tempx * tempx + tempy * tempy;

                    g[y, x] = mu;
                    b[y, x] = mag2;

                    c1[y, x] = b[y, x] * tempx;
                    c2[y, x] = b[y, x] * tempy;
                }

            /* free memory of fx and fy */
            //pgFreeDmatrix(fx, 0, YN_1, 0, XN_1);
            //pgFreeDmatrix(fy, 0, YN_1, 0, XN_1);

            /************* Solve GVF = (u,v) iteratively ***************/
            for (count = 1; count <= ITER; count++)
            {
                /* IV: Compute Laplace operator using Neuman condition */
                /* IV.1: Deal with corners */
                Lu[0, 0] = (u[0, 1] + u[1, 0]) * 0.5 - u[0, 0];
                Lv[0, 0] = (v[0, 1] + v[1, 0]) * 0.5 - v[0, 0];
                Lu[0, numCols_1] = (u[0, numCols_2] + u[1, numCols_1]) * 0.5 - u[0, numCols_1];
                Lv[0, numCols_1] = (v[0, numCols_2] + v[1, numCols_1]) * 0.5 - v[0, numCols_1];
                Lu[numRows_1, 0] = (u[numRows_1, 1] + u[YN_2, 0]) * 0.5 - u[numRows_1, 0];
                Lv[numRows_1, 0] = (v[numRows_1, 1] + v[YN_2, 0]) * 0.5 - v[numRows_1, 0];
                Lu[numRows_1, numCols_1] = (u[numRows_1, numCols_2] + u[YN_2, numCols_1]) * 0.5 - u[numRows_1, numCols_1];
                Lv[numRows_1, numCols_1] = (v[numRows_1, numCols_2] + v[YN_2, numCols_1]) * 0.5 - v[numRows_1, numCols_1];

                /* IV.2: Deal with left and right columns */
                for (y = 1; y <= YN_2; y++)
                {
                    Lu[y, 0] = (2 * u[y, 1] + u[y - 1, 0] + u[y + 1, 0]) * 0.25 - u[y, 0];
                    Lv[y, 0] = (2 * v[y, 1] + v[y - 1, 0] + v[y + 1, 0]) * 0.25 - v[y, 0];
                    Lu[y, numCols_1] = (2 * u[y, numCols_2] + u[y - 1, numCols_1]
                           + u[y + 1, numCols_1]) * 0.25 - u[y, numCols_1];
                    Lv[y, numCols_1] = (2 * v[y, numCols_2] + v[y - 1, numCols_1]
                           + v[y + 1, numCols_1]) * 0.25 - v[y, numCols_1];
                }

                /* IV.3: Deal with top and bottom rows */
                for (x = 1; x <= numCols_2; x++)
                {
                    Lu[0, x] = (2 * u[1, x] + u[0, x - 1] + u[0, x + 1]) * 0.25 - u[0, x];
                    Lv[0, x] = (2 * v[1, x] + v[0, x - 1] + v[0, x + 1]) * 0.25 - v[0, x];
                    Lu[numRows_1, x] = (2 * u[YN_2, x] + u[numRows_1, x - 1]
                           + u[numRows_1, x + 1]) * 0.25 - u[numRows_1, x];
                    Lv[numRows_1, x] = (2 * v[YN_2, x] + v[numRows_1, x - 1]
                           + v[numRows_1, x + 1]) * 0.25 - v[numRows_1, x];
                }

                /* IV.4: Compute interior */
                for (y = 1; y <= YN_2; y++)
                    for (x = 1; x <= numCols_2; x++)
                    {
                        Lu[y, x] = (u[y, x - 1] + u[y, x + 1]
                            + u[y - 1, x] + u[y + 1, x]) * 0.25 - u[y, x];
                        Lv[y, x] = (v[y, x - 1] + v[y, x + 1]
                            + v[y - 1, x] + v[y + 1, x]) * 0.25 - v[y, x];
                    }

                /******** V: Update GVF ************/
                for (y = 0; y <= numRows_1; y++)
                    for (x = 0; x <= numCols_1; x++)
                    {
                        u[y, x] += -b[y, x] * u[y, x] + g[y, x] * Lu[y, x] * 4 + c1[y, x];
                        v[y, x] += -b[y, x] * v[y, x] + g[y, x] * Lv[y, x] * 4 + c2[y, x];
                    }

                /* print iteration number */
                //Console.WriteLine(count);
                //if (count % 15 == 0)
                //    Console.WriteLine("\n");
            }
            //Console.WriteLine("\n");

            /* copy u,v to the output in column major order */
            for (y = 0; y < numRows; y++)
                for (x = 0; x < numCols; x++)
                {
                    ou[x * numRows + y] = u[y, x];
                    ov[x * numRows + y] = v[y, x];
                }

            /* free all the array memory */
            //pgFreeDmatrix(u, 0, YN_1, 0, XN_1);
            //pgFreeDmatrix(v, 0, YN_1, 0, XN_1);
            //pgFreeDmatrix(Lu, 0, YN_1, 0, XN_1);
            //pgFreeDmatrix(Lv, 0, YN_1, 0, XN_1);
            //pgFreeDmatrix(g, 0, YN_1, 0, XN_1);
            //pgFreeDmatrix(c1, 0, YN_1, 0, XN_1);
            //pgFreeDmatrix(c2, 0, YN_1, 0, XN_1);
            //pgFreeDmatrix(b, 0, YN_1, 0, XN_1);

            return;
        }

        internal static void GVFC(int numRows,
            int numCols,
            NcvMatrix f,
            ref NcvMatrix ou,
            ref NcvMatrix ov,
            double mu, int ITER)
        {
            double fmin = f.min();
            double fmax = f.max();
            f = (f - fmin) / (fmax - fmin);

            f = BoundMirrorExpand(f);

            NcvMatrix fx, fy, u, v, SqrMagf;
            f.gradient(out fx, out fy);
            u = fx.Clone(); v = fy.Clone();
            SqrMagf = NcvMatrix.DotProduct(fx, fx) + NcvMatrix.DotProduct(fy, fy);

            for (int i = 0; i < ITER; i++)
            {
                u = BoundMirrorEnsure(u);
                v = BoundMirrorEnsure(v);
                u = u + mu * 4 * NcvMatrix.del2(u) - NcvMatrix.DotProduct(SqrMagf, (u - fx));
                v = v + mu * 4 * NcvMatrix.del2(v) - NcvMatrix.DotProduct(SqrMagf, (v - fy));
            }

            ou = u; ov = v;
        }

        //void mexFunction( int nlhs, mxArray *plhs[], 
        //         int nrhs, const mxArray *prhs[] )
        //{
        //   double  *f, *u, *v;
        //   int mrows, ncols;
        //   int m, n;
        //   double mu;
        //   int ITER;
        //   char buf[80];

        //   mexUnlock();

        //   /* Check for proper number of arguments */
        //   if ( nrhs < 1 || nrhs > 3) 
        //      mexErrMsgTxt("GVFC.MEX: wrong number of input arguments.");
        //   if ( nlhs > 2 )
        //      mexErrMsgTxt("GVFC.MEX: wrong number of output arguments.");

        //   /* Obtain the dimension information of inputs */
        //   mrows = mxGetM(prhs[0]);   ncols = mxGetN(prhs[0]);

        //   /* Create matrix for the return argument */
        //   plhs[0] = mxCreateDoubleMatrix(mrows, ncols, mxREAL);
        //   plhs[1] = mxCreateDoubleMatrix(mrows, ncols, mxREAL);

        //   /* Assign pointers to each input and output */
        //   f = (double *)mxGetPr(prhs[0]); 
        //   u = (double *)mxGetPr(plhs[0]);
        //   v = (double *)mxGetPr(plhs[1]);

        //   if (nrhs >= 2 && nrhs <= 3) {
        //      /* The input must be a noncomplex scalar double.*/
        //      m = mxGetM(prhs[1]);
        //      n = mxGetN(prhs[1]);
        //      if( !mxIsDouble(prhs[1]) || mxIsComplex(prhs[1]) ||
        //     !(m==1 && n==1) ) {
        //     mexErrMsgTxt("Input mu must be a noncomplex scalar double.");
        //      }
        //      mu = mxGetScalar(prhs[1]);
        //   }

        //   if (nrhs == 3) {
        //      /* The input must be an integer.*/
        //      m = mxGetM(prhs[2]);
        //      n = mxGetN(prhs[2]);
        //      if( !mxIsDouble(prhs[2]) || mxIsComplex(prhs[2]) ||
        //     !(m==1 && n==1) ) {
        //     mexErrMsgTxt("Input mu must be an integer.");
        //      }
        //      ITER = mxGetScalar(prhs[2]);
        //   }


        //   /* set default mu and ITER */
        //   if (nrhs <= 1) 
        //     mu = 0.2;
        //   if (nrhs <= 2)
        //      ITER = PG_MAX(mrows, ncols);

        //   /* Call the distedt subroutine. */
        //   GVFC(mrows, ncols, f, u, v, mu, ITER);

        //   /* done */
        //   return;
        //}


        //  k: the scaling factor    
        //  s: standard variance   
        public static double[,] GaussianMask(double k, double s)
        {
            int R = Convert.ToInt32(Math.Ceiling(3 * s)); // cutoff radius of the gaussian kernal   
            double[,] M = new double[2 * R + 1, 2 * R + 1];
            for (int i = -R; i <= R; i++)
            {
                for (int j = -R; j <= R; j++)
                {
                    M[i + R, j + R] = k * Math.Exp(-(i * i + j * j) / 2 / s / s) / (2 * Math.PI * s * s);
                }
            }
            return M;
        }



        //  GAUSSIANBLUR blur the image with a gaussian kernel    
        //      GI = gaussianBlur(I,s)     
        //          I is the image,     
        //          s is the standard deviation of the gaussian kernel    
        //          GI is the gaussian blurred image.   
        public static double[,] GaussianBlur(NcvMatrix I, double s)
        {
            NcvMatrix M = new NcvMatrix(GaussianMask(1, s));
            M.Normalize();  // normalize the gaussian mask so that the sum is equal to 1                     
            double[,] GI = Xconv2(I, M);
            return GI;
        }

        private static double[,] Xconv2(NcvMatrix I, NcvMatrix M)
        {
            throw new NotImplementedException();
        }

        //////////////////////////////////////////////////////////    

        //  DT apply Eucledian distance transform    
        //      D = dt(B) compute the Eucledian distance transform of B    
        //          B must be a binary map.     
        //  NOTE: this is not an efficient way to implement distance transform.     
        //      If one is interested in using DT, one may want to implement its    
        //      own DT.    
        private static double[,] dt(NcvMatrix B)
        {
            //[i,j] = find(B);   

            int m = B.Cols;
            int n = B.Rows;

            for (int x = 0; x < n; x++)
            {
                for (int y = 0; y < m; y++)
                {
                    //dx = i-x;   
                    //dy = j-y;   
                    //dmag = sqrt(dx.*dx+dy.*dy);   
                    //D(x,y) = min(dmag);   
                }
            }
            return null;
        }


        // Ensure mirror boundary condition
        //    
        //  The number of rows and columns of A must be greater than 2    
        //    
        //  for example (X means value that is not of interest)    
        //     
        //  A = [    
        //      X  X  X  X  X   X    
        //      X  1  2  3  11  X    
        //      X  4  5  6  12  X     
        //      X  7  8  9  13  X     
        //      X  X  X  X  X   X    
        //      ]    
        //    
        //  B = BoundMirrorEnsure(A) will yield    
        //    
        //      5  4  5  6  12  6    
        //      2  1  2  3  11  3    
        //      5  4  5  6  12  6     
        //      8  7  8  9  13  9     
        //      5  4  5  6  12  6  
        public static double[,] BoundMirrorEnsure(double[,] A, int m, int n)
        {
            if (m < 3 || n < 3)
                throw new ArgumentException("either the number of rows or columns is smaller than 3");

            double[,] B = new double[m, n];

            for (int yi = 1; yi < n - 1; yi++)
            {
                for (int xi = 1; xi < m - 1; xi++)
                {
                    B[xi, yi] = A[xi, yi];
                }
            }

            // mirror corners
            B[m - 1, 0] = B[m - 3, 2]; B[0, n - 1] = B[2, n - 3];
            B[0, 0] = B[2, 2]; B[m - 1, n - 1] = B[m - 3, n - 3];

            // mirror top and bottom boundary
            for (int xi = 1; xi < m - 1; xi++)
            {
                B[xi, 0] = B[xi, 2]; B[xi, n - 1] = B[xi, n - 3];
            }

            // mirror left and right boundary
            for (int yi = 1; yi < n - 1; yi++)
            {
                B[0, yi] = B[2, yi]; B[m - 1, yi] = B[m - 3, yi];
            }

            return B;
        }

        public static NcvMatrix BoundMirrorEnsure(NcvMatrix A)
        {
            int m = A.Data.GetLength(0);
            int n = A.Data.GetLength(1);
            return new NcvMatrix(BoundMirrorEnsure(A.Data, m, n));
        }

        // Expand the matrix using mirror boundary condition
        //
        //  for example     
        //    
        //  A = [    
        //      1  2  3  11    
        //      4  5  6  12    
        //      7  8  9  13    
        //      ]    
        //    
        //  B = BoundMirrorExpand(A) will yield    
        //    
        //      5  4  5  6  12  6    
        //      2  1  2  3  11  3    
        //      5  4  5  6  12  6     
        //      8  7  8  9  13  9     
        //      5  4  5  6  12  6  
        public static double[,] BoundMirrorExpand(double[,] A, int m, int n)
        {
            double[,] B = new double[m + 2, n + 2];

            for (int yi = 0; yi < n; yi++)
            {
                for (int xi = 0; xi < m; xi++)
                {
                    B[xi + 1, yi + 1] = A[xi, yi];
                }
            }

            // mirror corners
            B[m + 1, 0] = B[m - 1, 2]; B[0, n + 1] = B[2, n - 1];
            B[0, 0] = B[2, 2]; B[m + 1, n + 1] = B[m - 1, n - 1];

            // mirror top and bottom boundary
            for (int xi = 1; xi < m + 1; xi++)
            {
                B[xi, 0] = B[xi, 2]; B[xi, n + 1] = B[xi, n - 1];
            }

            // mirror left and right boundary
            for (int yi = 1; yi < n + 1; yi++)
            {
                B[0, yi] = B[2, yi]; B[m + 1, yi] = B[m - 1, yi];
            }

            return B;
        }

        public static NcvMatrix BoundMirrorExpand(NcvMatrix A)
        {
            return new NcvMatrix(BoundMirrorExpand(A.Data, A.cols(), A.rows()));
        }

        // Shrink the matrix to remove the padded mirror boundaries
        //
        //  Shrink the matrix to remove the padded mirror boundaries    
        //    
        //  for example     
        //      
        //  A = [    
        //      5  4  5  6  12  6    
        //      2  1  2  3  11  3    
        //      5  4  5  6  12  6     
        //      8  7  8  9  13  9     
        //      5  4  5  6  12  6    
        //      ]    
        //          
        //  B = BoundMirrorShrink(A) will yield    
        //          
        //      1  2  3  11    
        //      4  5  6  12    
        //      7  8  9  13    
        //    
        // Chenyang Xu and Jerry L. Prince, 9/9/1999    
        // http://iacl.ece.jhu.edu/projects/gvf   
        public static double[,] BoundMirrorShrink(double[,] A, int m, int n)
        {
            double[,] B = new double[m - 2, n - 2];

            for (int yi = 0; yi < n - 2; yi++)
            {
                for (int xi = 0; xi < m - 2; xi++)
                {
                    B[xi, yi] = A[xi + 1, yi + 1];
                }
            }

            return B;
        }

        public static NcvMatrix BoundMirrorShrink(NcvMatrix A)
        {
            int m = A.Data.GetLength(0);
            int n = A.Data.GetLength(1);
            return new NcvMatrix(BoundMirrorShrink(A.Data, m, n));
        }
    }
}
