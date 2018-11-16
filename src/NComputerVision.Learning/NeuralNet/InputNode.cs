
namespace NComputerVision.Learning.NeuralNet
{
    using System;

    [Serializable()]
    public class InputNode
    {
        private double m_value;
        private double[] m_weights;
        // foamliu, 2009/01/14, 为了引入冲量项 (Momentum) 而设.
        private double[] m_oldDeltaWeights;

        public double Value
        {
            get { return m_value; }
            set { m_value = value; }
        }

        public double[] Weights
        {
            get { return m_weights; }
            set { m_weights = value; }
        }

        public double[] OldDeltaWeights
        {
            get { return m_oldDeltaWeights; }
            set { m_oldDeltaWeights = value; }
        }

        public InputNode(int nextLayerNodeNum)
        {
            this.m_weights = new double[nextLayerNodeNum];
            this.m_oldDeltaWeights = new double[nextLayerNodeNum];
        }
    }
}
