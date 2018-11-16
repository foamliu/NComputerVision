

namespace NComputerVision.DataStructures
{
    using System.Collections.Generic;

    /// <summary>
    /// 区域的行程表示
    /// </summary>
    class BinaryImage : BaseImage
    {
        private List<Run> runList;

        public List<Run> RunList
        {
            get
            {
                return runList;
            }
        }

        public BinaryImage(bool[][] mat, int width, int height)
        {
            this.m_width = width;
            this.m_height = height;

            runList = new List<Run>();
        }
    }
}
