using System;

namespace NComputerVision.Learning
{
    [Serializable()]
    public abstract class Classifier
    {
        public abstract void Train();
        public abstract int Predict(SparseVector x);
        public abstract int Predict(SparseVector x, out double confidence);

        public abstract void SaveModel(string fileName);
    }
}
