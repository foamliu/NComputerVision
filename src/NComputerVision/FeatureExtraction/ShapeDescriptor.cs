using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using NComputerVision.Common;
using NComputerVision.Contour;
using NComputerVision.DataStructures;
using NComputerVision.GraphicsLib;
using NComputerVision.Image;
using NComputerVision.NcvMath;

namespace NComputerVision.FeatureExtraction
{
    public class ShapeDescriptor : BaseFeatureExtraction
    {
        public override double[] DoWork(System.Drawing.Image image, string archivePath)
        {
            Bitmap background = null;
            List<double> feature = new List<double>();
            List<DoublePoint> contour = null;
            Graphics g = null;
            Bitmap archiveBmp = null;

            try
            {
                background = new Bitmap(NcvGlobals.Background_Size, NcvGlobals.Background_Size, PixelFormat.Format24bppRgb);
                g = Graphics.FromImage(background);
                g.FillRectangle(new SolidBrush(Color.White), 0, 0, background.Width, background.Height);

                int imageDisplayWidth = Convert.ToInt32(image.Width * (g.DpiX / image.HorizontalResolution));
                int imageDisplayHeight = Convert.ToInt32(image.Height * (g.DpiY / image.VerticalResolution));

                double ratio = 1.0d * imageDisplayWidth / imageDisplayHeight;
                double ww = Math.Sqrt(ratio * ratio / (1 + ratio * ratio));
                double hh = Math.Sqrt(1 / (1 + ratio * ratio));
                double xx = (1 - ww) / 2;
                double yy = (1 - hh) / 2;

                int x = Convert.ToInt32(xx * NcvGlobals.Background_Size);
                int y = Convert.ToInt32(yy * NcvGlobals.Background_Size);
                int w = Convert.ToInt32(ww * NcvGlobals.Background_Size);
                int h = Convert.ToInt32(hh * NcvGlobals.Background_Size);

                Rectangle rect = new Rectangle(x, y, w, h);

                g.DrawImage(image, rect);

                int[][] mat;

                mat = ImageConvert.Bitmap2GreenScale(background);
                ThresholdLib.ThresholdSimple(mat, NcvGlobals.ThresholdMin, NcvGlobals.ThresholdMax);

                mat = GrayScaleImageLib.Open(mat, StructuringElement.N4);
                mat = GrayScaleImageLib.Close(mat, StructuringElement.N8);

                Convolution conv = new Convolution();
                conv.CalculateEdge(mat, ConvKernel.Sobel_Gx, ConvKernel.Sobel_Gy, out mat, ConvNorm.Norm_2);

                GrayScaleImageLib.Normalize(mat);

                GrayValueFeatures features = new GrayValueFeatures(mat);
                features.CalcBasicFeatures();
                features.CalcMomentFeatures();

                int[][] rotated = GrayScaleAffineTransformation.Rotate(mat, -features.AngleOfEllipse, new DoublePoint(features.CenterX, features.CenterY), 0);

                // ======== Shape Descriptor ========            
                if (NcvGlobals.FeatureExtractionType == FeatureExtractionType.Snake)
                {
                    Snake2 s = new Snake2(rotated, NcvGlobals.NumDimension);
                    s.InitCircle();
                    contour = s.DoIt();
                }
                else
                {
                    RegionContourStitching s = new RegionContourStitching(rotated);
                    List<DoublePoint> temp = s.DoIt(rotated.Length / 2, 0);
                    contour = NcvVector.interp1(temp, NcvGlobals.NumDimension);
                }

                double xc = 0;
                double yc = 0;

                for (int i = 0; i < contour.Count; i++)
                {
                    xc += contour[i].X / contour.Count;
                    yc += contour[i].Y / contour.Count;
                }

                Complex[] data = new Complex[NcvGlobals.NumDimension];
                for (int i = 0; i < NcvGlobals.NumDimension; i++)
                {
                    double r = Math.Sqrt((contour[i].X - xc) * (contour[i].X - xc) + (contour[i].Y - yc) * (contour[i].Y - yc));
                    //data[i] = new Complex(contour[i].X - xc, contour[i].Y - yc);
                    data[i] = new Complex(r, 0);
                }
                FourierTransform.FFT(data, FourierTransform.Direction.Forward);

                for (int i = 1; i < NcvGlobals.NumDimension; i++)
                {
                    double v = data[i].Magnitude / data[0].Magnitude;
                    feature.Add(v);
                }

                // ======== Archiving ========
                archiveBmp = ImageConvert.Mat2Bitmap(rotated);
                NComputerVision.Common.NcvDrawing.DrawContour(archiveBmp, contour);
                if (!Directory.Exists(@"D:\achive\"))
                    Directory.CreateDirectory(@"D:\achive\");
                //archiveBmp.Save(@"D:\achive\" + Path.GetFileName(path));
                archiveBmp.Save(archivePath);

            }
            finally
            {
                if (background != null)
                    background.Dispose();
                if (archiveBmp != null)
                    archiveBmp.Dispose();
                if (g != null)
                    g.Dispose();
                if (image != null)
                    image.Dispose();
            }

            return feature.ToArray();
        }
    }
}
