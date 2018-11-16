
namespace NComputerVision.Learning.K_NN
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// foamliu, 2008/12/29.
    /// </summary>

    /// <summary>
    /// k-nearest neighbor algorithm
    /// 
    /// http://en.wikipedia.org/wiki/K-nearest_neighbor_algorithm
    /// In pattern recognition, the k-nearest neighbors algorithm (k-NN) is a method for 
    /// classifying objects based on closest training examples in the feature space. 
    /// k-NN is a type of instance-based learning, or lazy learning where the function is 
    /// only approximated locally and all computation is deferred until classification.
    /// 
    /// Use:
    ///     Euclidean distance
    /// </summary>
    ///  
    [Serializable()]
    public class KNN : Classifier
    {
        #region Fields
        private Collection<Example> m_t_set;
        private int m_k;
        private int m_catnum;
        #endregion

        #region Properties

        public Collection<Example> TrainSet
        {
            set
            {
                this.m_t_set = value;
            }
        }

        public int KNN_K
        {
            get
            {
                return this.m_k;
            }
            set
            {
                if ((value & 1) == 0)
                    throw new ApplicationException("K should be odd number.");

                this.m_k = value;
            }
        }


        #endregion

        #region Methods

        public override void Train()
        {
        }

        public override int Predict(SparseVector x)
        {
            int iResult;
            if (m_t_set.Count < this.m_k)
                throw new ApplicationException("K great than training set count.");

            List<ExampleDistancePair> list = new List<ExampleDistancePair>();

            foreach (Example e in m_t_set)
            {
                list.Add(new ExampleDistancePair(e, SparseVector.Distance(x, e.X)));
            }

            list.Sort();

            int[] votes = new int[m_catnum];
            for (int i = 0; i < votes.Length; i++)
            {
                votes[i] = 0;
            }

            for (int i = 0; i < this.m_k; i++)
            {
                ExampleDistancePair pair = list[i];
                votes[(pair.Example.Label.Id + 1) / 2]++;
            }

            int max = 0;
            int index = -1;
            for (int i = 0; i < votes.Length; i++)
            {
                if (votes[i] > max)
                {
                    index = i;
                    max = votes[i];
                }
            }

            iResult = index * 2 - 1;
            return iResult;
        }

        /// <summary>
        /// foamliu, 2009/05/14, 补上这个方法.
        /// </summary>
        /// <param name="x"></param>       
        /// <param name="confidence"></param>
        public override int Predict(SparseVector x, out double confidence)
        {
            confidence = 1.0;
            return this.Predict(x);
        }

        public Collection<Example> GetProof(SparseVector x)
        {
            Collection<Example> res = new Collection<Example>();

            List<ExampleDistancePair> list = new List<ExampleDistancePair>();

            foreach (Example e in m_t_set)
            {
                list.Add(new ExampleDistancePair(e, SparseVector.Distance(x, e.X)));
            }

            list.Sort();

            int[] votes = new int[m_catnum];
            for (int i = 0; i < votes.Length; i++)
            {
                votes[i] = 0;
            }

            for (int i = 0; i < this.m_k; i++)
            {
                ExampleDistancePair pair = list[i];
                res.Add(pair.Example);
            }

            return res;
        }

        public override void SaveModel(string fileName)
        {
        }

        public static KNN LoadModel(string fileName)
        {
            return null;
        }

        #endregion

        #region Constructors

        public KNN()
        {
            // foamliu, 2008/12/29, default values
            this.m_k = 1;
            // how many categories
            this.m_catnum = 2;
        }

        #endregion
    }

    class ExampleDistancePair : IComparable
    {
        private Example m_example;
        private double m_distance;

        public Example Example
        {
            get { return m_example; }
        }

        public double Distance
        {
            get { return m_distance; }
        }

        public ExampleDistancePair(Example example, double distance)
        {
            this.m_example = example;
            this.m_distance = distance;
        }

        public int CompareTo(object obj)
        {
            ExampleDistancePair other = (ExampleDistancePair)obj;

            return Math.Sign(this.Distance - other.Distance);
        }
    }
}
