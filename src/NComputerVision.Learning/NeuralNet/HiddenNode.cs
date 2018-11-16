
namespace NComputerVision.Learning.NeuralNet
{
    using System;

    [Serializable()]
    public class HiddenNode
    {
        private double m_inputSum;
        private double m_output;
        private double m_delta;
        private double[] m_weights;
        // foamliu, 2009/01/14, 为了引入冲量项 (Momentum) 而设.
        private double[] m_oldDeltaWeights;

        public double InputSum
        {
            get { return m_inputSum; }
            set { m_inputSum = value; }
        }
        public double Output
        {
            get { return m_output; }
            set { m_output = value; }
        }
        public double Delta
        {
            get { return m_delta; }
            set { m_delta = value; }
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

        public HiddenNode(int nextLayerNodeNum)
        {
            this.m_weights = new double[nextLayerNodeNum];
            this.m_oldDeltaWeights = new double[nextLayerNodeNum];
        }
    }
}
