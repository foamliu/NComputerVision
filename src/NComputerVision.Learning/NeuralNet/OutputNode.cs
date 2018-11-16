
namespace NComputerVision.Learning.NeuralNet
{
    using System;

    [Serializable()]
    public class OutputNode//<T> where T : IComparable<T>
    {
        private double m_inputSum;
        private double m_output;
        private double m_delta;
        private double m_target;
        //private T m_value;

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

        public double Target
        {
            get { return m_target; }
            set { m_target = value; }
        }
        //public T Value
        //{
        //    get { return m_value; }
        //    set { m_value = value; }
        //}

        public OutputNode()
        {

        }
    }
}
