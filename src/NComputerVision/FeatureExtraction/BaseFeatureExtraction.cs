
using System.Drawing;
using System.IO;
using NComputerVision.Common;

namespace NComputerVision.FeatureExtraction
{
    public abstract class BaseFeatureExtraction
    {
        public abstract double[] DoWork(System.Drawing.Image image, string archivePath);

        /// <summary>
        /// 模板方法 (Template Method)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public double[] DoWork(string path)
        {
            System.Drawing.Image image = Bitmap.FromFile(path);

            string archivePath = NcvGlobals.ArchiveFolder + Path.GetFileName(path);
            return DoWork(image, archivePath);
        }

        /// <summary>
        /// 模板方法 (Template Method)
        /// </summary>
        /// <param name="path"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public double[] DoWork(string path, Stream stream)
        {
            System.Drawing.Image image = Bitmap.FromStream(stream);

            string archivePath = NcvGlobals.ArchiveFolder + Path.GetFileName(path);
            return DoWork(image, archivePath);
        }
    }
}
