using NComputerVision.FeatureExtraction;

namespace NComputerVision.Common
{
    public class NcvGlobals
    {
        // ========== 常量 ==========

        // 角度/弧度转换   
        public const double DegreePerRadian = 180 / System.Math.PI;
        public const double RadianPerDegree = System.Math.PI / 180;

        // Hough变换 每步角幅度
        public const int ThetaStep = 10;                // 10度
        public const double ThetaStepRadian = ThetaStep / DegreePerRadian;   // 以弧度计

        public const double DStep = 5; // 约一个像素            
        public const double Gradient_Threshold = 10;   // 梯度阈值
        public const double Value_Threshold = 10;   // 累加器阈值
        public const double GrayValue_Threshold = 10;   // 累加器阈值

        public const int NumberOfLines_Threshold = 5;

        // 颜色
        public const int White = 255;
        public const int Black = 0;

        // 刺绣算法 迭代次数
        public const int StitchingMaxIter = 1024;

        // 特征提取 背景尺寸
        public const int Background_Size = 400;
        public const int Background_Margin = 3;
        
        public const int NumDimension = 64;     // 特征中浮点个数


        // ========== 设置 ==========

        public static FeatureExtractionType FeatureExtractionType = FeatureExtractionType.Snake;

        public static int ThresholdMin = 96;    // 灰度阈值
        public static int ThresholdMax = 256;   // 灰度阈值

        public const string ArchiveFolder = @"d:\archive\";     // 存档目录

    }

}
