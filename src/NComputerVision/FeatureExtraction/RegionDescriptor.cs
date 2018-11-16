using System;
using System.Collections.Generic;
using System.Drawing;
using NComputerVision.Common;
using NComputerVision.Contour;
using NComputerVision.DataStructures;
using NComputerVision.GraphicsLib;
using NComputerVision.Image;
using System.Drawing.Imaging;

namespace NComputerVision.FeatureExtraction
{
    public class RegionDescriptor : BaseFeatureExtraction
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
                GrayValueFeatures features = new GrayValueFeatures(mat);
                features.CalcBasicFeatures();
                features.CalcMomentFeatures();

                int[][] rotated = GrayScaleAffineTransformation.Rotate(mat, -features.AngleOfEllipse, new DoublePoint(features.CenterX, features.CenterY), 0);
                                    
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

                // ======== Region/Geometric/Morphological Descriptor ========            
                double[] region = features.GetRegionFeature();

                for (int i = 0; i < region.Length; i++)
                {
                    feature.Add(region[i]);
                }

                int numContour = contour.Count;
                double perimeter = 0;
                for (int i = 0; i < numContour; i++)
                {
                    DoublePoint p = contour[i];
                    DoublePoint pNext = contour[(i + 1) % numContour];
                    perimeter += (p - pNext).Magnitude;
                }
                feature.Add(perimeter);

                double aspectRatio = features.SemiMajorAxe / features.SemiMinorAxe;
                feature.Add(aspectRatio);

                double formFactor = 4 * Math.PI * features.Area / perimeter;
                feature.Add(formFactor);

                double rectangularity = features.SemiMajorAxe * features.SemiMinorAxe / features.Area;
                feature.Add(rectangularity);

                double perimeterRatio = perimeter / (features.SemiMajorAxe + features.SemiMinorAxe);
                feature.Add(perimeterRatio);

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
