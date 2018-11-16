
namespace NComputerVision.Learning
{
    public class Constants
    {
        public const int KEY_NOT_FOUND = -1;

        public const double TrainingSetRatio = 0.7;

        //public const double SVM_C = double.MaxValue;  // hard margin
        // an optimal choice for C in sthe objective function of the resulting
        //  optimisation problem should be R^(-2)
        public const double SVM_C = 1.0;    // soft margin

        // learning rate
        public const double SVM_Eta = 0.1;
        public const double SVM_Tolerance = 1e-2;
        // typically eps can be set in the range 10^-2 to 10^-3
        public const double SVM_Epsilon = 1e-2;

        public const double ANN_Alpha = 1.0;
        public const double ANN_Eta = 0.5;
        //public const double ANN_Epsilon = 1e-10;
        // foamliu, 2009/05/14, 过于严格了.
        public const double ANN_Epsilon = 1e-3;
        public const double ANN_Momentum = 0.5;
        //public const int ANN_MaximumIteration = 1000000;
        // foamliu, 2009/05/14, 因为一次迭代要40sec, 百万次需要一年多, 这是不现实的.
        // 设置为1000则最多11个小时.
        public const int ANN_MaximumIteration = 10000;

        public const int ANN_FirstHiddenLayerNodeNumber = 10;
        public const int ANN_SecondHiddenLayerNodeNumber = 10;
        public const int ANN_OutputLayerNodeNumbe = 2;

    }

    public enum AlgorithmType
    {
        Unknown,
        LLM,
        SVM,
        NaiveBayes,
        KNN,
        ANN,
        DT
    }
}
