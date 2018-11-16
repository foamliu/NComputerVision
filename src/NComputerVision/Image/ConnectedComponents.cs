
namespace NComputerVision.Image
{
    using System.Collections.Generic;
    using System.Drawing;
    using NComputerVision.Common;
    using NComputerVision.GraphicsLib;

    /// <summary>
    /// 提取连通分支
    /// </summary>
    public class ConnectedComponents
    {
        private Bitmap m_bmp;
        private int m_width, m_height;
        private int[][] m_img, m_newImg;

        private int[] colors = new int[] { 32, 64, 96, 128, 160, 192, 224 };

        public ConnectedComponents(Bitmap bmp)
        {
            m_bmp = bmp;

            ImageConvert.Bitmap2Mat(bmp, out m_img, out m_width, out m_height);
            m_newImg = Util.BuildMatInt(m_width, m_height);

        }

        public int Width
        {
            get { return m_width; }
        }

        public int Height
        {
            get { return m_height; }
        } 

        public int[][] Image
        {
            get { return m_img; }
        }

        public int[][] NewImage
        {
            get { return m_newImg; }
        }

        public Bitmap Caculate()
        {
            Bitmap newBmp;

            int colorIndex = 0;
            int nextColor = colors[0];

            Point node;

            while (NextUnhandledPoint(out node))
            {
                FloodFill(node, m_img[node.X][node.Y], nextColor);
                nextColor = colors[++colorIndex % colors.Length];
            }

            ImageConvert.Mat2Bitmap(m_newImg, m_width, m_height, out newBmp);

            return newBmp;
        }

        public void FloodFill(Point node, int target_color, int replacement_color)
        {
            Queue<Point> queue = new Queue<Point>();
            if (m_img[node.X][node.Y] != target_color)
            {
                return;
            }
            queue.Enqueue(node);

            // while queue is not empty
            while (queue.Count != 0)
            {
                Point n = queue.Dequeue();
                if (m_img[n.X][n.Y] == target_color)
                {
                    int w = n.X;
                    int e = n.X;

                    while (w > 0 && m_img[w][n.Y] == target_color)
                    {
                        w--;
                    }
                    while (e < m_width - 1 && m_img[e][n.Y] == target_color)
                    {
                        e++;
                    }
                    for (int x = w; x <= e; x++)
                    {
                        m_newImg[x][n.Y] = replacement_color;
                    }
                    if (n.Y > 0)
                    {
                        for (int x = w; x <= e; x++)
                        {
                            if (m_img[x][n.Y - 1] == target_color && m_newImg[x][n.Y - 1] == 0)
                            {
                                queue.Enqueue(new Point(x, n.Y - 1));

                                while (x <= e && m_img[x][n.Y - 1] == target_color)
                                    x++;
                            }

                        }
                    }
                    if (n.Y < m_height - 1)
                    {
                        for (int x = w; x <= e; x++)
                        {
                            if (m_img[x][n.Y + 1] == target_color && m_newImg[x][n.Y + 1] == 0)
                            {
                                queue.Enqueue(new Point(x, n.Y + 1));

                                while (x <= e && m_img[x][n.Y + 1] == target_color)
                                    x++;

                            }
                        }
                    }
                }
            }
        }

        public bool NextUnhandledPoint(out Point node)
        {
            for (int y = 0; y < m_height; y++)
            {
                for (int x = 0; x < m_width; x++)
                {
                    if (m_newImg[x][y] == 0)
                    {
                        node = new Point(x, y);
                        return true;
                    }
                }
            }
            node = new Point(0, 0);
            return false;
        }


    }
}
