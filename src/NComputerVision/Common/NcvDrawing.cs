

namespace NComputerVision.Common
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using NComputerVision.DataStructures;
    using NComputerVision.GraphicsLib;
    using NComputerVision.NcvMath;

    /// <summary>
    /// Drawing primitives.
    /// </summary>
    /// 
    /// <remarks><para>The class allows to do drawing of some primitives directly on
    /// locked image data or unmanaged image.</para>
    /// 
    /// <para>All methods of this class support drawing only on color 24/32 bpp images and
    /// on grayscale 8 bpp indexed images.</para>
    /// </remarks>
    /// 
    public class NcvDrawing
    {
        public static void DrawEllipse(Graphics g, Pen pen, int x, int y, double r1, double r2, double angle)
        {
            int steps = 36;
            List<Point> points = new List<Point>();

            double beta = -angle;
            double sinbeta = Math.Sin(beta);
            double cosbeta = Math.Cos(beta);

            for (int i = 0; i < 360; i += 360 / steps)
            {
                double alpha = i * (Math.PI / 180);
                double sinalpha = Math.Sin(alpha);
                double cosalpha = Math.Cos(alpha);

                double X = x + (r1 * cosalpha * cosbeta - r2 * sinalpha * sinbeta);
                double Y = y + (r1 * cosalpha * sinbeta + r2 * sinalpha * cosbeta);

                points.Add(new Point((int)X, (int)Y));
            }

            points.Add(new Point(points[0].X, points[0].Y));

            g.DrawLines(pen, points.ToArray());

        }

        /// <summary>
        /// foamliu, 2009/02/10, 在位图上画上轮廓.
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="contourList"></param>
        //public static void DrawContours(Bitmap bmp, List<SubpixelContour> contourList)
        //{
        //    if (contourList != null)
        //    {
        //        Graphics g = Graphics.FromImage(bmp);
        //        Pen pen = new Pen(Color.Red);

        //        List<PointF> points = new List<PointF>();

        //        foreach (SubpixelContour contour in contourList)
        //        {
        //            //points.Clear();
        //            foreach (DoublePoint point in contour)
        //            {
        //                //points.Add(new PointF(point.X, point.Y));
        //                g.DrawRectangle(pen, (float)point.X, (float)point.Y, 1, 1);
        //            }
        //            //if (points.Count >= 2)
        //            //    g.DrawLines(pen, points.ToArray());
        //        }

        //        g.Dispose();
        //    }
        //}

        /// <summary>
        /// foamliu, 2009/02/10, 画轮廓.
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="contourList"></param>
        public static void DrawContour(Bitmap bmp, List<DoublePoint> contour)
        {
            if (contour != null)
            {
                Graphics g = Graphics.FromImage(bmp);
                Pen pen = new Pen(Color.Navy);
                Pen penPoint = new Pen(Color.Yellow);

                for (int i = 1; i < contour.Count; i++)
                {
                    g.DrawLine(pen, (int)contour[i - 1].X, (int)contour[i - 1].Y, (int)contour[i].X, (int)contour[i].Y);
                }
                g.DrawLine(pen, (int)contour[contour.Count - 1].X, (int)contour[contour.Count - 1].Y, (int)contour[0].X, (int)contour[0].Y);

                foreach (DoublePoint pt in contour)
                {
                    g.DrawRectangle(penPoint, (float)pt.X, (float)pt.Y, 2, 2);
                }

                g.Dispose();
            }
        }

        public static void DrawContour(Bitmap bmp, NcvVector x, NcvVector y)
        {
            List<DoublePoint> contour = new List<DoublePoint>();
            for (int i = 0; i < x.n; i++)
            {
                contour.Add(new DoublePoint(x[i], y[i]));
            }
            DrawContour(bmp, contour);
        }

        /// <summary>
        /// foamliu, 2009/02/11, 在位图上绘制距.
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="features"></param>
        public static void DrawMoments(Bitmap bmp, GrayValueFeatures features)
        {
            if (features != null)
            {
                Graphics g = Graphics.FromImage(bmp);
                Pen pen = new Pen(Color.Red);

                // 先画重心.
                g.DrawRectangle(pen, features.CenterX, features.CenterY, 4, 4);

                int x = features.CenterX;
                int y = features.CenterY;
                double angle = features.AngleOfEllipse;
                double r1 = features.SemiMajorAxe;
                double r2 = features.SemiMinorAxe;

                // 半长轴
                g.DrawLine(pen,
                    features.CenterX,
                    features.CenterY,
                    (int)(x + features.SemiMajorAxe * Math.Cos(angle)),
                    (int)(y - features.SemiMajorAxe * Math.Sin(angle)));

                // 半短轴
                g.DrawLine(pen,
                    features.CenterX,
                    features.CenterY,
                    (int)(x - features.SemiMinorAxe * Math.Sin(angle)),
                    (int)(y - features.SemiMinorAxe * Math.Cos(angle)));

                // 画椭圆
                NcvDrawing.DrawEllipse(g, pen, x, y, r1, r2, angle);

                g.Dispose();
            }
        }

        /// <summary>
        /// foamliu, 2009/02/11, 在位图上绘制线段.
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="features"></param>
        public static void DrawLines(Bitmap bmp, List<LineSegment> segs)
        {
            if (segs != null)
            {
                Graphics g = Graphics.FromImage(bmp);
                Pen pen = new Pen(Color.Red);

                foreach (LineSegment seg in segs)
                {
                    g.DrawLine(pen, seg.Pair[0].X, seg.Pair[0].Y, seg.Pair[1].X, seg.Pair[1].Y);

                }

                g.Dispose();
            }
        }

        /// <summary>
        /// foamliu, 2009/02/12, 在位图上画圆.
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="features"></param>
        public static void DrawCircles(Bitmap bmp, List<Circle> circles)
        {
            if (circles != null && circles.Count > 0)
            {
                Graphics g = Graphics.FromImage(bmp);
                Pen pen = new Pen(Color.Red);

                foreach (Circle c in circles)
                {
                    g.DrawEllipse(pen, c.X - c.Radius, c.Y - c.Radius, c.Radius * 2, c.Radius * 2);

                }

                g.Dispose();
            }
        }

        /// <summary>
        /// 绘制矩形
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="rects"></param>
        public static void DrawRectangles(Bitmap bmp, List<Rectangle> rects)
        {
            if (rects != null && rects.Count > 0)
            {
                Graphics g = Graphics.FromImage(bmp);
                Pen pen = new Pen(Color.Red);

                g.DrawRectangles(pen, rects.ToArray());
                g.Dispose();

            }
        }

        /// <summary>
        /// 绘制表情标记
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="rects"></param>
        /// <param name="exprs"></param>
        /// <param name="strExprs"></param>
        public static void DrawExpressions(Bitmap bmp, List<Rectangle> rects, int[] exprs, string[] strExprs)
        {
            if (rects.Count > 0)
            {
                Graphics g = Graphics.FromImage(bmp);
                Pen pen = new Pen(Color.Red);
                Brush brushBg = new SolidBrush(Color.LightGray);
                Brush brush = new SolidBrush(Color.Black);
                Font font = new Font("Calibri", 11, FontStyle.Regular, GraphicsUnit.Point);

                g.DrawRectangles(pen, rects.ToArray());

                for (int i = 0; i < exprs.Length; i++)
                {
                    int expr = exprs[i];
                    g.FillRectangle(brushBg, rects[i].Left + 1, rects[i].Top + 1, 80, 20);
                    g.DrawString(strExprs[expr], font, brush, rects[i].Left, rects[i].Top);
                }

                g.Dispose();
            }
        }

        /// <summary>
        /// Fill rectangle on the specified image.
        /// </summary>
        /// 
        /// <param name="imageData">Source image data to draw on.</param>
        /// <param name="rectangle">Rectangle's coordinates to fill.</param>
        /// <param name="color">Rectangle's color.</param>
        /// 
        /// <exception cref="UnsupportedImageFormatException">The source image has incorrect pixel format.</exception>
        /// 
        public static unsafe void FillRectangle(BitmapData imageData, Rectangle rectangle, Color color)
        {
            FillRectangle(new UnmanagedImage(imageData), rectangle, color);
        }

        /// <summary>
        /// Fill rectangle on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image to draw on.</param>
        /// <param name="rectangle">Rectangle's coordinates to fill.</param>
        /// <param name="color">Rectangle's color.</param>
        /// 
        /// <exception cref="UnsupportedImageFormatException">The source image has incorrect pixel format.</exception>
        /// 
        public static unsafe void FillRectangle(UnmanagedImage image, Rectangle rectangle, Color color)
        {
            CheckPixelFormat(image.PixelFormat);

            int pixelSize = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;

            // image dimension
            int imageWidth = image.Width;
            int imageHeight = image.Height;
            int stride = image.Stride;

            // rectangle dimension and position
            int rectX1 = rectangle.X;
            int rectY1 = rectangle.Y;
            int rectX2 = rectangle.X + rectangle.Width - 1;
            int rectY2 = rectangle.Y + rectangle.Height - 1;

            // check if rectangle is in the image
            if ((rectX1 >= imageWidth) || (rectY1 >= imageHeight) || (rectX2 < 0) || (rectY2 < 0))
            {
                // nothing to draw
                return;
            }

            int startX = Math.Max(0, rectX1);
            int stopX = Math.Min(imageWidth - 1, rectX2);
            int startY = Math.Max(0, rectY1);
            int stopY = Math.Min(imageHeight - 1, rectY2);

            // do the job
            byte* ptr = (byte*)image.ImageData.ToPointer() + startY * stride + startX * pixelSize;

            if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                // grayscale image
                byte gray = (byte)(0.2125 * color.R + 0.7154 * color.G + 0.0721 * color.B);

                int fillWidth = stopX - startX + 1;

                for (int y = startY; y <= stopY; y++)
                {
                    SystemTools.SetUnmanagedMemory(ptr, gray, fillWidth);
                    ptr += stride;
                }
            }
            else
            {
                // color image
                byte red = color.R;
                byte green = color.G;
                byte blue = color.B;

                int offset = stride - (stopX - startX + 1) * pixelSize;

                for (int y = startY; y <= stopY; y++)
                {
                    for (int x = startX; x <= stopX; x++, ptr += pixelSize)
                    {
                        ptr[RGB.R] = red;
                        ptr[RGB.G] = green;
                        ptr[RGB.B] = blue;
                    }
                    ptr += offset;
                }
            }
        }

        /// <summary>
        /// Draw rectangle on the specified image.
        /// </summary>
        /// 
        /// <param name="imageData">Source image data to draw on.</param>
        /// <param name="rectangle">Rectangle's coordinates to draw.</param>
        /// <param name="color">Rectangle's color.</param>
        /// 
        /// <exception cref="UnsupportedImageFormatException">The source image has incorrect pixel format.</exception>
        /// 
        public static unsafe void Rectangle(BitmapData imageData, Rectangle rectangle, Color color)
        {
            Rectangle(new UnmanagedImage(imageData), rectangle, color);
        }

        /// <summary>
        /// Draw rectangle on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image to draw on.</param>
        /// <param name="rectangle">Rectangle's coordinates to draw.</param>
        /// <param name="color">Rectangle's color.</param>
        /// 
        /// <exception cref="UnsupportedImageFormatException">The source image has incorrect pixel format.</exception>
        /// 
        public static unsafe void Rectangle(UnmanagedImage image, Rectangle rectangle, Color color)
        {
            CheckPixelFormat(image.PixelFormat);

            int pixelSize = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;

            // image dimension
            int imageWidth = image.Width;
            int imageHeight = image.Height;
            int stride = image.Stride;

            // rectangle dimension and position
            int rectX1 = rectangle.X;
            int rectY1 = rectangle.Y;
            int rectX2 = rectangle.X + rectangle.Width - 1;
            int rectY2 = rectangle.Y + rectangle.Height - 1;

            // check if rectangle is in the image
            if ((rectX1 >= imageWidth) || (rectY1 >= imageHeight) || (rectX2 < 0) || (rectY2 < 0))
            {
                // nothing to draw
                return;
            }

            int startX = Math.Max(0, rectX1);
            int stopX = Math.Min(imageWidth - 1, rectX2);
            int startY = Math.Max(0, rectY1);
            int stopY = Math.Min(imageHeight - 1, rectY2);

            if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                // grayscale image
                byte gray = (byte)(0.2125 * color.R + 0.7154 * color.G + 0.0721 * color.B);

                // draw top horizontal line
                if (rectY1 >= 0)
                {
                    byte* ptr = (byte*)image.ImageData.ToPointer() + rectY1 * stride + startX;

                    SystemTools.SetUnmanagedMemory(ptr, gray, stopX - startX);
                }

                // draw bottom horizontal line
                if (rectY2 < imageHeight)
                {
                    byte* ptr = (byte*)image.ImageData.ToPointer() + rectY2 * stride + startX;

                    SystemTools.SetUnmanagedMemory(ptr, gray, stopX - startX);
                }

                // draw left vertical line
                if (rectX1 >= 0)
                {
                    byte* ptr = (byte*)image.ImageData.ToPointer() + startY * stride + rectX1;

                    for (int y = startY; y <= stopY; y++, ptr += stride)
                    {
                        *ptr = gray;
                    }
                }

                // draw right vertical line
                if (rectX2 < imageWidth)
                {
                    byte* ptr = (byte*)image.ImageData.ToPointer() + startY * stride + rectX2;

                    for (int y = startY; y <= stopY; y++, ptr += stride)
                    {
                        *ptr = gray;
                    }
                }
            }
            else
            {
                // color image
                byte red = color.R;
                byte green = color.G;
                byte blue = color.B;

                // draw top horizontal line
                if (rectY1 >= 0)
                {
                    byte* ptr = (byte*)image.ImageData.ToPointer() + rectY1 * stride + startX * pixelSize;

                    for (int x = startX; x <= stopX; x++, ptr += pixelSize)
                    {
                        ptr[RGB.R] = red;
                        ptr[RGB.G] = green;
                        ptr[RGB.B] = blue;
                    }
                }

                // draw bottom horizontal line
                if (rectY2 < imageHeight)
                {
                    byte* ptr = (byte*)image.ImageData.ToPointer() + rectY2 * stride + startX * pixelSize;

                    for (int x = startX; x <= stopX; x++, ptr += pixelSize)
                    {
                        ptr[RGB.R] = red;
                        ptr[RGB.G] = green;
                        ptr[RGB.B] = blue;
                    }
                }

                // draw left vertical line
                if (rectX1 >= 0)
                {
                    byte* ptr = (byte*)image.ImageData.ToPointer() + startY * stride + rectX1 * pixelSize;

                    for (int y = startY; y <= stopY; y++, ptr += stride)
                    {
                        ptr[RGB.R] = red;
                        ptr[RGB.G] = green;
                        ptr[RGB.B] = blue;
                    }
                }

                // draw right vertical line
                if (rectX2 < imageWidth)
                {
                    byte* ptr = (byte*)image.ImageData.ToPointer() + startY * stride + rectX2 * pixelSize;

                    for (int y = startY; y <= stopY; y++, ptr += stride)
                    {
                        ptr[RGB.R] = red;
                        ptr[RGB.G] = green;
                        ptr[RGB.B] = blue;
                    }
                }
            }
        }

        /// <summary>
        /// Draw a line on the specified image.
        /// </summary>
        /// 
        /// <param name="imageData">Source image data to draw on.</param>
        /// <param name="point1">The first point to connect.</param>
        /// <param name="point2">The second point to connect.</param>
        /// <param name="color">Line's color.</param>
        /// 
        /// <exception cref="UnsupportedImageFormatException">The source image has incorrect pixel format.</exception>
        /// 
        public static unsafe void Line(BitmapData imageData, IntPoint point1, IntPoint point2, Color color)
        {
            Line(new UnmanagedImage(imageData), point1, point2, color);
        }

        /// <summary>
        /// Draw a line on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image to draw on.</param>
        /// <param name="point1">The first point to connect.</param>
        /// <param name="point2">The second point to connect.</param>
        /// <param name="color">Line's color.</param>
        /// 
        /// <exception cref="UnsupportedImageFormatException">The source image has incorrect pixel format.</exception>
        /// 
        public static unsafe void Line(UnmanagedImage image, IntPoint point1, IntPoint point2, Color color)
        {
            // TODO: faster line drawing algorithm may be implemented with integer math

            CheckPixelFormat(image.PixelFormat);

            int pixelSize = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;

            // image dimension
            int imageWidth = image.Width;
            int imageHeight = image.Height;
            int stride = image.Stride;

            // check if there is something to draw
            if (
                ((point1.X < 0) && (point2.X < 0)) ||
                ((point1.Y < 0) && (point2.Y < 0)) ||
                ((point1.X >= imageWidth) && (point2.X >= imageWidth)) ||
                ((point1.Y >= imageHeight) && (point2.Y >= imageHeight)))
            {
                // nothing to draw
                return;
            }

            CheckEndPoint(imageWidth, imageHeight, point1, ref point2);
            CheckEndPoint(imageWidth, imageHeight, point2, ref point1);

            // check again if there is something to draw
            if (
                ((point1.X < 0) && (point2.X < 0)) ||
                ((point1.Y < 0) && (point2.Y < 0)) ||
                ((point1.X >= imageWidth) && (point2.X >= imageWidth)) ||
                ((point1.Y >= imageHeight) && (point2.Y >= imageHeight)))
            {
                // nothing to draw
                return;
            }

            int startX = point1.X;
            int startY = point1.Y;
            int stopX = point2.X;
            int stopY = point2.Y;

            // compute pixel for grayscale image
            byte gray = 0;
            if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                gray = (byte)(0.2125 * color.R + 0.7154 * color.G + 0.0721 * color.B);
            }

            // draw the line
            int dx = stopX - startX;
            int dy = stopY - startY;

            if (Math.Abs(dx) >= Math.Abs(dy))
            {
                // the line is more horizontal, we'll plot along the X axis
                float slope = (float)dy / (float)dx;
                int step = (dx > 0) ? 1 : -1;

                // correct dx so last point is included as well
                dx += step;

                if (image.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    // grayscale image
                    for (int x = 0; x != dx; x += step)
                    {
                        int px = startX + x;
                        int py = (int)((float)startY + (slope * (float)x));

                        byte* ptr = (byte*)image.ImageData.ToPointer() + py * stride + px;
                        *ptr = gray;
                    }
                }
                else
                {
                    // color image
                    for (int x = 0; x != dx; x += step)
                    {
                        int px = startX + x;
                        int py = (int)((float)startY + (slope * (float)x));

                        byte* ptr = (byte*)image.ImageData.ToPointer() + py * stride + px * pixelSize;

                        ptr[RGB.R] = color.R;
                        ptr[RGB.G] = color.G;
                        ptr[RGB.B] = color.B;
                    }
                }
            }
            else
            {
                // the line is more vertical, we'll plot along the y axis.
                float slope = (float)dx / (float)dy;
                int step = (dy > 0) ? 1 : -1;

                // correct dy so last point is included as well
                dy += step;

                if (image.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    // grayscale image
                    for (int y = 0; y != dy; y += step)
                    {
                        int px = (int)((float)startX + (slope * (float)y));
                        int py = startY + y;

                        byte* ptr = (byte*)image.ImageData.ToPointer() + py * stride + px;
                        *ptr = gray;
                    }
                }
                else
                {
                    // color image
                    for (int y = 0; y != dy; y += step)
                    {
                        int px = (int)((float)startX + (slope * (float)y));
                        int py = startY + y;

                        byte* ptr = (byte*)image.ImageData.ToPointer() + py * stride + px * pixelSize;

                        ptr[RGB.R] = color.R;
                        ptr[RGB.G] = color.G;
                        ptr[RGB.B] = color.B;
                    }
                }
            }
        }

        /// <summary>
        /// Draw a polygon on the specified image.
        /// </summary>
        /// 
        /// <param name="imageData">Source image data to draw on.</param>
        /// <param name="points">Points of the polygon to draw.</param>
        /// <param name="color">Polygon's color.</param>
        /// 
        /// <remarks><para>The method draws a polygon by connecting all points from the
        /// first one to the last one and then connecting the last point with the first one.
        /// </para></remarks>
        /// 
        public static void Polygon(BitmapData imageData, List<IntPoint> points, Color color)
        {
            Polygon(new UnmanagedImage(imageData), points, color);
        }

        /// <summary>
        /// Draw a polygon on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image to draw on.</param>
        /// <param name="points">Points of the polygon to draw.</param>
        /// <param name="color">Polygon's color.</param>
        /// 
        /// <remarks><para>The method draws a polygon by connecting all points from the
        /// first one to the last one and then connecting the last point with the first one.
        /// </para></remarks>
        /// 
        public static void Polygon(UnmanagedImage image, List<IntPoint> points, Color color)
        {
            for (int i = 1, n = points.Count; i < n; i++)
            {
                Line(image, points[i - 1], points[i], color);
            }
            Line(image, points[points.Count - 1], points[0], color);
        }

        /// <summary>
        /// Draw a polyline on the specified image.
        /// </summary>
        /// 
        /// <param name="imageData">Source image data to draw on.</param>
        /// <param name="points">Points of the polyline to draw.</param>
        /// <param name="color">polyline's color.</param>
        /// 
        /// <remarks><para>The method draws a polyline by connecting all points from the
        /// first one to the last one. Unlike <see cref="Polygon( BitmapData, List{IntPoint}, Color )"/>
        /// method, this method does not connect the last point with the first one.
        /// </para></remarks>
        /// 
        public static void Polyline(BitmapData imageData, List<IntPoint> points, Color color)
        {
            Polyline(new UnmanagedImage(imageData), points, color);
        }

        /// <summary>
        /// Draw a polyline on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image to draw on.</param>
        /// <param name="points">Points of the polyline to draw.</param>
        /// <param name="color">polyline's color.</param>
        /// 
        /// <remarks><para>The method draws a polyline by connecting all points from the
        /// first one to the last one. Unlike <see cref="Polygon( UnmanagedImage, List{IntPoint}, Color )"/>
        /// method, this method does not connect the last point with the first one.
        /// </para></remarks>
        /// 
        public static void Polyline(UnmanagedImage image, List<IntPoint> points, Color color)
        {
            for (int i = 1, n = points.Count; i < n; i++)
            {
                Line(image, points[i - 1], points[i], color);
            }
        }

        // Check for supported pixel format
        private static void CheckPixelFormat(PixelFormat format)
        {
            // check pixel format
            if (
                (format != PixelFormat.Format24bppRgb) &&
                (format != PixelFormat.Format8bppIndexed) &&
                (format != PixelFormat.Format32bppArgb) &&
                (format != PixelFormat.Format32bppRgb)
                )
            {
                throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
            }
        }

        // Check end point and make sure it is in the image
        private static void CheckEndPoint(int width, int height, IntPoint start, ref IntPoint end)
        {
            if (end.X >= width)
            {
                int newEndX = width - 1;

                double c = (double)(newEndX - start.X) / (end.X - start.X);

                end.Y = (int)(start.Y + c * (end.Y - start.Y));
                end.X = newEndX;
            }

            if (end.Y >= height)
            {
                int newEndY = height - 1;

                double c = (double)(newEndY - start.Y) / (end.Y - start.Y);

                end.X = (int)(start.X + c * (end.X - start.X));
                end.Y = newEndY;
            }

            if (end.X < 0)
            {
                double c = (double)(0 - start.X) / (end.X - start.X);

                end.Y = (int)(start.Y + c * (end.Y - start.Y));
                end.X = 0;
            }

            if (end.Y < 0)
            {
                double c = (double)(0 - start.Y) / (end.Y - start.Y);

                end.X = (int)(start.X + c * (end.X - start.X));
                end.Y = 0;
            }
        }

        public static Bitmap DrawVectorField(Complex[,] vf, int m, int n)
        {
            Bitmap bitmap = new Bitmap(m, n);
            Graphics g = Graphics.FromImage(bitmap);
            Pen p = new Pen(Color.Navy);

            int gridsize = 10;

            for (int y = 0; y < n; y += gridsize)
            {
                for (int x = 0; x < m; x += gridsize)
                {
                    int x1 = x, y1 = y;
                    int x2 = Convert.ToInt32(x + gridsize * vf[x, y].Re);
                    int y2 = Convert.ToInt32(y + gridsize * vf[x, y].Im);
                    g.DrawLine(p, x1, y1, x2, y2);
                    g.DrawEllipse(p, x2, y2, 2, 2);
                }
            }

            g.Dispose();
            return bitmap;
        }


    }
}
