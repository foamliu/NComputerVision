using System;
using System.Collections.Generic;
using NComputerVision.Common;
using NComputerVision.DataStructures;

namespace NComputerVision.Contour
{
    public class LevelSetContour
    {
        private int[][] m_mat;
        private int m_width;
        private int m_height;
        private int m_gsub;
        private int[][] m_alive;
        //private List<ContourPoint> m_narrowBand;
        // foamliu, 2009/02/09, 换成最大值堆以提高效率.
        //
        private NcvBinaryHeap<LevelSetContourPoint> m_narrowBand;
        private int[] neighborX = new int[] { -1, 0, +1, +1, +1, 0, -1, -1 };
        private int[] neighborY = new int[] { -1, -1, -1, 0, +1, +1, +1, 0 };

        // foamliu, 2009/02/09, 扩展出的最后一个边界点.
        private int lastX, lastY;

        //// for each of the 26 neighbors
        ////
        //private int[][] exNeighbor =
        //                {new int[]{-1,-1},
        //                new int[]{0,-1},
        //                new int[]{1,-1},
        //                new int[]{1,0},
        //                new int[]{1,1},
        //                new int[]{0,1},
        //                new int[]{-1,1},
        //                new int[]{-1,0},
        //                new int[]{-2,0},
        //                new int[]{-2,-1},
        //                new int[]{-2,-2},
        //                new int[]{-1,-2},
        //                new int[]{0,-2},
        //                new int[]{1,-2},
        //                new int[]{2,-2},
        //                new int[]{2,-1},
        //                new int[]{2,0},
        //                new int[]{2,1},
        //                new int[]{2,2},
        //                new int[]{1,2},
        //                new int[]{0,2},
        //                new int[]{-1,2},
        //                new int[]{-2,2},
        //                new int[]{-2,1},
        //                new int[]{-2,0}};



        const int FAR = 0;
        const int NARROWBAND = 1;
        const int ALIVE = 2;

        /// <summary>
        /// foamliu, 2009/02/09, 构造器.
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="gsub"></param>
        public LevelSetContour(int[][] mat, int gsub)
        {
            m_mat = mat;
            m_gsub = gsub;
            m_width = mat.Length;
            m_height = mat[0].Length;

            //m_narrowBand = new List<ContourPoint>();
            // foamliu, 2009/02/09, 换成最大值堆以提高效率.
            m_narrowBand = new NcvBinaryHeap<LevelSetContourPoint>(new GrayScaleComparer());

            // 2 - Narrow Band 
            // 1 － Alive, 
            // 0 － Far
            //
            m_alive = Util.BuildMatInt(m_width, m_height);
        }

        /// <summary>
        /// foamliu, 2009/02/07, 提取下一个轮廓.
        /// 
        /// </summary>
        /// <param name="seedx"></param>
        /// <param name="seedy"></param>
        /// <param name="contour"></param>
        public void FastMarch(int seedx, int seedy)
        {
            //m_narrowBand.Add(new ContourPoint(seedx, seedy, m_mat[seedx][seedy]));\
            // foamliu, 2009/02/09, 换成最大值堆以提高效率.
            //
            m_narrowBand.Insert(new LevelSetContourPoint(seedx, seedy, m_mat[seedx][seedy]));
            m_alive[seedx][seedy] = NARROWBAND;

            while (true)
            {
                LevelSetContourPoint trial = m_narrowBand.Remove();
                if (trial == null)
                    break;

                // 把 trial 从 Narrow band 移动到 Alive.
                //
                //m_narrowBand.Remove(trial);
                m_alive[trial.X][trial.Y] = ALIVE;

                if (trial.GrayScale <= m_gsub)
                {
                    lastX = trial.X;
                    lastY = trial.Y;
                    break;
                }

                for (int i = 0; i < neighborX.Length; i++)
                {
                    int nx = trial.X + neighborX[i];
                    int ny = trial.Y + neighborY[i];
                    if (Util.InRect(nx, ny, m_width, m_height) && m_alive[nx][ny] == FAR)
                    {
                        //m_narrowBand.Add(new ContourPoint(nx, ny, m_mat[nx][ny]));
                        // foamliu, 2009/02/09, 换成最大值堆以提高效率.
                        //
                        m_narrowBand.Insert(new LevelSetContourPoint(nx, ny, m_mat[nx][ny]));
                        m_alive[nx][ny] = NARROWBAND;
                    }
                }
            }

            m_narrowBand.Clear();
        }

        /// <summary>
        /// foamliu, 2009/02/07, 从临界区提取一个轮廓.
        /// 
        /// </summary>
        /// <param name="contour"></param>
        private List<SubpixelContour> ExtractContourList()
        {
            List<SubpixelContour> contourList = new List<SubpixelContour>();
            int frontX = lastX, frontY = lastY;
            //int nextX, nextY;                     

            SubpixelContour contour = new SubpixelContour();

            //while ((FindNeighbor(frontX, frontY, NARROWBAND, out nextX, out nextY)))
            //{
            //    // foamliu, 2009/02/07, 如果两点间距过大应分为不同的轮廓.
            //    if (Math.Abs(frontX - nextX) + Math.Abs(frontY - nextY) > 50)
            //    {
            //        if (contour.ControlPoints.Count > 0)
            //            contourList.Add(contour);
            //        contour = new SubpixelContour();
            //    }

            //    m_alive[nextX][nextY] = ALIVE;
            //    contour.ControlPoints.Add(new SubpixelPoint(nextX, nextY));
            //    frontX = nextX;
            //    frontY = nextY;
            //}

            contourList.Add(contour);
            for (int y = 0; y < m_height; y++)
            {
                for (int x = 0; x < m_width; x++)
                {
                    if (m_alive[x][y] == NARROWBAND)
                        contour.Add(new DoublePoint(x, y));
                }
            }

            return contourList;
        }


        /// <summary>
        /// foamliu, 2009/02/07, 找到一个邻居.
        /// 
        /// </summary>
        /// <param name="frontx"></param>
        /// <param name="fronty"></param>
        /// <param name="state"></param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <returns></returns>
        private bool FindNeighbor(int frontX, int frontY, int state, out int nextX, out int nextY)
        {
            for (int i = 0; i < Util.SearchWindow.Length; i++)
            {
                nextX = frontX + Util.SearchWindow[i][0];
                nextY = frontY + Util.SearchWindow[i][1];
                if (Util.InRect(nextX, nextY, m_width, m_height) && m_alive[nextX][nextY] == state)
                {
                    return true;
                }
            }

            nextX = 0;
            nextY = 0;
            return false;
        }

        /// <summary>
        /// foamliu, 2009/02/07, 提取轮廓.
        /// 
        /// </summary>
        /// <param name="contourList"></param>
        public void ExtractContourList(out List<SubpixelContour> contourList)
        {
            int seedx, seedy;
            contourList = new List<SubpixelContour>();

            while (GetNextSeed(out seedx, out seedy))
            {
                FastMarch(seedx, seedy);
            }

            // From Narrow Band
            ExtractContourList(out contourList);

        }

        /// <summary>
        /// foamliu, 2009/02/07, 下一个种子.
        /// 
        /// </summary>
        /// <param name="seedx"></param>
        /// <param name="seedy"></param>
        /// <returns></returns>
        public bool GetNextSeed(out int seedX, out int seedY)
        {
            for (int y = 0; y < m_height; y++)
            {
                for (int x = 0; x < m_width; x++)
                {
                    if (m_alive[x][y] == FAR && m_mat[x][y] > m_gsub)
                    {
                        seedX = x;
                        seedY = y;
                        return true;
                    }
                }
            }

            seedX = 0;
            seedY = 0;
            return false;
        }

        /// <summary>
        /// foamliu, 2009/02/07, 找到灰度值最大的.
        /// </summary>
        /// <returns></returns>
        //private ContourPoint PickSummit()
        //{
        //int max = int.MinValue;
        //int pos = 0;

        //for (int i = 0; i < m_narrowBand.Count; i++)
        //{
        //    if (max < m_narrowBand[i].GrayScale)
        //    {
        //        max = m_narrowBand[i].GrayScale;
        //        pos = i;
        //    }
        //}
        //return m_narrowBand[pos];


        // foamliu, 2009/02/09, 换成最大值堆以提高效率.
        //
        //return m_narrowBand.Remove();

        //}

    }


}
