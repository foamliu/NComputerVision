using System;
using System.Collections.Generic;
using System.Text;
using NComputerVision.DataStructures;
using NComputerVision.GraphicsLib;
using System.Drawing;
using NComputerVision.Image;

namespace NComputerVision.Contour
{
    public class Snake2
    {
        private int[][] grad_mag;    /* Magnitude of Gradient */
        private int m_Cols;     /* Number of Columns */
        private int m_Rows;     /* Number of Rows */

        private bool termination_point;
        private bool neighbor3by3 = false;      /* true means 3x3 neighborhood, false means 5x5 neighborhood */
        private int no_of_snake_points; /* snaxel的数目.*/

        public List<DoublePoint> Snake_points;
        private double[] alpha;
        private double[] beta;
        private double[] gamma;

        public double init_alpha = 0.001;
        public double init_beta = 0.4;
        public double init_gamma = 0.4;

        private double avg_distance = 0.0;

        private int threshold_movement = 3;
        private double threshold_curvature = 0.3;
        private double threshold_image_energy = 120.0;


        private int Max_Iterations = 8000;

        public Snake2(int[][] mat, int number)
        {
            this.m_Cols = mat.Length;
            this.m_Rows = mat[0].Length;
            this.no_of_snake_points = number;

            Snake_points = new SubpixelContour();
            avg_distance = 0.0;

            //mat = GrayScaleProcessor.Gaussian(mat, 1.0);

            Convolution conv = new Convolution();
            conv.CalculateEdge(mat, ConvKernel.Sobel_Gx, ConvKernel.Sobel_Gy, out grad_mag, ConvNorm.Norm_2);
            //grad_mag = mat;
        }

        public List<DoublePoint> DoIt()
        {
            //bool flag_change = false;
            int movement, i, j, iteration = 0;
            double max_curvature;
            DoublePoint temp;
            double[] curvature;
            alpha = new double[no_of_snake_points];
            beta = new double[no_of_snake_points];
            gamma = new double[no_of_snake_points];
            curvature = new double[no_of_snake_points];
            termination_point = false;

            for (i = 0; i < no_of_snake_points; i++)
            {
                alpha[i] = init_alpha; // 0.001;
                beta[i] = init_beta;  // 0.4;
                gamma[i] = init_gamma; // 100;
                avg_distance += find_distance(i, Snake_points[i]);
            }
            j = no_of_snake_points;
           
            while (!termination_point)
            {
                movement = 0;
                avg_distance = avg_distance / (double)no_of_snake_points;
                max_curvature = double.MinValue;
                //for (i = 0; i < no_of_snake_points; i++)
                int[] v = randperm(no_of_snake_points);                
                for (int k = 0; k < no_of_snake_points; k++)
                {
                    i = v[k];
                    temp = find_min_energy(i, Snake_points[i], avg_distance);
                    //flag_change = false;
                    if (temp != Snake_points[i] 
                        && temp != Snake_points[(i - 1 + j) % j] 
                        && temp != Snake_points[(i + 1) % j])
                    {
                        Snake_points[i] = temp;
                        movement++;
                    }
                    curvature[i] = find_curvature(i, Snake_points[i]);
                    if (max_curvature < curvature[i])
                        max_curvature = curvature[i];
                }
                avg_distance = 0.0;
                for (i = 0; i < no_of_snake_points; i++)
                    curvature[i] = curvature[i] / max_curvature;
                for (i = 0; i < no_of_snake_points; i++)
                {
                    DoublePoint p = Snake_points[i];

                    avg_distance += find_distance(i, p);

                    if (curvature[i] > threshold_curvature
                        && curvature[i] > curvature[(i + 1) % no_of_snake_points]
                        && curvature[i] > curvature[(i - 1 + no_of_snake_points) % no_of_snake_points]
                        && (double)grad_mag[round(p.X)][round(p.Y)] > threshold_image_energy)
                        beta[i] = 0;
                }
                if (movement < threshold_movement)
                    termination_point = true;
                iteration++;
                if (iteration > Max_Iterations)
                    termination_point = true;

                if (null != MovementEvent)
                    MovementEvent(Snake_points);

            }

            return Snake_points;
        }

        double find_distance(int no, DoublePoint point)
        {
            int x = no_of_snake_points;
            point -= Snake_points[(no - 1 + x) % x];
            return (System.Math.Sqrt(point.X * point.X + point.Y * point.Y));
        }

        double find_curvature(int no, DoublePoint point)
        {
            int x = no_of_snake_points;
            point = Snake_points[(no - 1 + x) % x] - point - point + Snake_points[(no + 1) % x];
            return (point.X * point.X + point.Y * point.Y);
        }

        double find_continuity(int no, DoublePoint point, double avg_distance)
        {
            return (System.Math.Pow(avg_distance - find_distance(no, point), 2));
        }

        DoublePoint find_min_energy(int no, DoublePoint point, double avg_distance)
        {
            DoublePoint p, min_point;
            p = new DoublePoint();
            min_point = new DoublePoint();
            double max_curvature, max_continuity, max_internal, min_internal, min_energy, energy;
            double[,] curvatures = new double[5, 5];
            double[,] continuities = new double[5, 5];
            double[,] internal_energies = new double[5, 5];

            int i, j, limit = 1;
            max_curvature = max_continuity = max_internal = double.MinValue;
            min_internal = double.MaxValue;
            if (!neighbor3by3)
                limit++;
            for (i = -limit; i <= limit; i++)
            {
                p.Y = point.Y + i;
                if (p.Y < 0) p.Y = 0;
                if (p.Y >= m_Rows)
                    p.Y = m_Rows - 1;
                for (j = -limit; j <= limit; j++)
                {
                    p.X = point.X + j;
                    if (p.X < 0) p.X = 0;
                    if (p.X >= m_Cols) p.X = m_Cols - 1;
                    curvatures[i + limit, j + limit] = find_curvature(no, p);
                    //This code can cause problem near 
                    continuities[i + limit, j + limit] = find_continuity(no, p, avg_distance);
                    //border of image 
                    internal_energies[i + limit, j + limit] = (double)grad_mag[round(p.X)][round(p.Y)];

                    if (curvatures[i + limit, j + limit] > max_curvature)
                        max_curvature = curvatures[i + limit, j + limit];

                    if (continuities[i + limit, j + limit] > max_continuity)
                        max_continuity = continuities[i + limit, j + limit];
                    if (internal_energies[i + limit, j + limit] > max_internal)
                        max_internal = internal_energies[i + limit, j + limit];
                    if (internal_energies[i + limit, j + limit] < min_internal)
                        min_internal = internal_energies[i + limit, j + limit];
                }
            }
            for (i = 0; i <= 2 * limit; i++)
            {
                for (j = 0; j <= 2 * limit; j++)
                {
                    curvatures[i, j] = curvatures[i, j] / max_curvature;
                    continuities[i, j] = continuities[i, j] / max_continuity;
                    if ((max_internal - min_internal) < 5)
                        min_internal = max_internal - 5;
                    internal_energies[i, j] = (internal_energies[i, j] - min_internal) / (max_internal - min_internal);

                }
            }
            min_point.X = -limit;
            min_point.Y = -limit;
            min_energy = double.MaxValue;
            for (i = -limit; i <= limit; i++)
            {
                for (j = -limit; j <= limit; j++)
                {
                    energy = alpha[no] * continuities[i + limit, j + limit]
                        + beta[no] * curvatures[i + limit, j + limit]
                        - gamma[no] * internal_energies[i + limit, j + limit];
                    if (energy < min_energy || (energy == min_energy && i == 0 && j == 0))
                    {
                        min_energy = energy;
                        min_point.X = j;
                        min_point.Y = i;
                    }
                }
            }
            min_point.X = min_point.X + point.X;
            min_point.Y = min_point.Y + point.Y;
            return (min_point);
        }

        int round(double d)
        {
            return Convert.ToInt32(System.Math.Round(d));
        }

        int[] randperm(int n)
        {
            int[] v = new int[n];
            for (int i = 0; i < n; i++)
                v[i] = i;

            Random r = new Random();

            for (int i = 0; i < n; i++)
            {
                int j = (int)(r.NextDouble() * n);
                if (j == n) j = n - 1;
                int t = v[i];
                v[i] = v[j];
                v[j] = t;
            }

            return v;
        }

        /// <summary>
        /// foamliu, 2009/02/18, 初始化轮廓为一个矩形.
        /// 顺时针，左上角开始。
        /// 
        /// </summary>
        private void InitRect(int x, int y, int width, int height)
        {
            int margin = 5;
            int circumference = 2 * (width + height - 4 * margin);
            int step = Convert.ToInt32(1.0 * circumference / this.no_of_snake_points);

            for (int i = 0; i < width; i += step)
            {
                Snake_points.Add(new DoublePoint(x + i, y + margin));
            }
            for (int j = 0; j < height; j += step)
            {
                Snake_points.Add(new DoublePoint(x + width - margin, y + j));
            }
            for (int i = width; i > 0; i -= step)
            {
                Snake_points.Add(new DoublePoint(x + i, y + height - margin));
            }
            for (int j = height; j > 0; j -= step)
            {
                Snake_points.Add(new DoublePoint(x + margin, y + j));
            }
        }

        private void InitRect(Rectangle rc)
        {
            InitRect(rc.X, rc.Y, rc.Width, rc.Height);
        }

        public void InitRect()
        {
            InitRect(0, 0, m_Cols, m_Rows);
        }

        /// <summary>
        /// 从最右边开始，顺时针
        /// </summary>
        public void InitCircle()
        {
            int xc = m_Cols / 2;
            int yc = m_Rows / 2;
            int radius = System.Math.Min(xc, yc) - 5;

            for (int i = 0; i < no_of_snake_points; i++)
            {
                double theta = 2 * System.Math.PI * i / no_of_snake_points;
                int xi = Convert.ToInt32(xc + radius * System.Math.Cos(theta));
                int yi = Convert.ToInt32(yc - radius * System.Math.Sin(theta));
                Snake_points.Add(new DoublePoint(xi, yi));
            }
        }

        public void InitSmallCircle()
        {
            int xc = m_Cols / 2;
            int yc = m_Rows / 2;
            int radius = 25;

            for (int i = 0; i < no_of_snake_points; i++)
            {
                double theta = 2 * System.Math.PI * i / no_of_snake_points;
                int xi = Convert.ToInt32(xc + radius * System.Math.Cos(theta));
                int yi = Convert.ToInt32(yc - radius * System.Math.Sin(theta));
                Snake_points.Add(new DoublePoint(xi, yi));
            }
        }

        public delegate void MovementEventHandler(List<DoublePoint> countor);
        public event MovementEventHandler MovementEvent;

    }
}
