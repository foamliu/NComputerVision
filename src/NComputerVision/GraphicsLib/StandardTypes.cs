
namespace NComputerVision.GraphicsLib
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    /// <summary>
    /// foamliu, 2009/02/11, 像素的类型.
    /// </summary>
    public enum PixelType
    {
        /// <summary>
        /// 孤立点
        /// </summary>
        Isolated,
        /// <summary>
        /// 开始点
        /// </summary>
        Starting,
        /// <summary>
        /// 内部点
        /// </summary>
        Interior,
        /// <summary>
        /// 结束点
        /// </summary>
        Ending,
        /// <summary>
        /// 角点
        /// </summary>
        Corner,
        /// <summary>
        /// 连接点
        /// </summary>
        Junction,
    }

    /// <summary>
    /// foamliu, 2009/02/11, 滤波器的类型.
    /// </summary>
    public enum FilterType
    {
        Sobel_Gx,
        Sobel_Gy,
        Prewitt_Gx,
        Prewitt_Gy,
        Sobel,
        Prewitt,
        Laplacian_4,
        Laplacian_8,
    }

    public class StandardTypes
    {
        public const int BackGroundPixel = 0;
    }
 



    // foamliu, 2009/02/11, 用于点的比较.
    //
    public class PointComparer : IComparer<Point>
    {
        public int Compare(Point pt1, Point pt2)
        {
            //return Math.Sign(pt1.X - pt2.X);

            // foamliu, 2009/02/12, 对于垂直的情况.
            //
            if (pt1.X != pt2.X)
                return Math.Sign(pt1.X - pt2.X);
            else
                return Math.Sign(pt1.Y - pt2.Y);
        }

    }


}
