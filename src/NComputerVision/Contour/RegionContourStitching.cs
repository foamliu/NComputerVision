using System;
using System.Collections.Generic;
using System.Text;
using NComputerVision.DataStructures;
using NComputerVision.Common;

namespace NComputerVision.Contour
{
    public class RegionContourStitching
    {
        private int[][] m_mat;
        private int m_Cols;     /* Number of Columns */
        private int m_Rows;     /* Number of Rows */

        public RegionContourStitching(int[][] mat)
        {
            m_mat = mat;
            this.m_Cols = mat.Length;
            this.m_Rows = mat[0].Length;
        }

        public List<DoublePoint> DoIt(int startx, int starty)
        {
            int[] s = new int[9];
            int px = startx, py = starty;
            int px0, py0;
            List<DoublePoint> points = new List<DoublePoint>();
            int[] x = new int[5];

            // find object
            while (py < m_mat[0].Length && m_mat[px][py] == 0) py++;
            if (py == m_mat[0].Length)
                return points;
            py--;

            // record initial position
            px0 = px;
            py0 = py;

            // stitching
            while (true)
            {
                populate_s_x(px, py, s, x);

                if (points.Count > NcvGlobals.StitchingMaxIter) break;
                points.Add(new DoublePoint(px, py));

                if (x[1] == 1 && x[2] == 0) px++;
                if (x[2] == 1 && x[3] == 0) py++;
                if (x[3] == 1 && x[4] == 0) px--;
                if (x[4] == 1 && x[1] == 0) py--;

                if (px == px0 && py == py0)
                    break;
            }

            return points;
        }

        private void populate_s_x(int px, int py, int[] s, int[] x)
        {
            s[1] = perceive(px - 1, py - 1);
            s[2] = perceive(px, py - 1);
            s[3] = perceive(px + 1, py - 1);
            s[4] = perceive(px + 1, py);
            s[5] = perceive(px + 1, py + 1);
            s[6] = perceive(px, py + 1);
            s[7] = perceive(px - 1, py + 1);
            s[8] = perceive(px - 1, py);

            for (int i = 1; i <= 8; i++)
            {
                if (s[i] != 0)
                    s[i] = 1;
            }

            if (s[2] == 1 || s[3] == 1)
                x[1] = 1;
            else
                x[1] = 0;
            if (s[4] == 1 || s[5] == 1)
                x[2] = 1;
            else
                x[2] = 0;
            if (s[6] == 1 || s[7] == 1)
                x[3] = 1;
            else
                x[3] = 0;
            if (s[8] == 1 || s[1] == 1)
                x[4] = 1;
            else
                x[4] = 0;
        }

        private int perceive(int px, int py)
        {
            if (px < 0
                || px >= m_Cols
                || py < 0
                || py >= m_Rows)
                return 1;
            return m_mat[px][py];
        }
    }
}
