
namespace NComputerVision.Learning.AdaBoost
{
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    /// foamliu, 2009/04/17, 弱分类器.
    /// 
    /// </summary>
    [Serializable()]
    public class WeakLearn : Classifier
    {
        // 把 [0, 1] 分成多少个子区间.
        public static int BinNum = 11;
        // 有和没有两种可能
        public static int ClassNum = 2;

        // 这个弱分类器关注第几个属性.
        private int m_index;

        // 分界点
        private double m_b;
        // 方向: 
        //  -1: 小于等于 m_b 为 -1.
        //  +1: 小于等于 m_b 为 +1.
        private int m_direction;

        // 训练集
        [NonSerialized]
        private Collection<Example> m_t_set;
        [NonSerialized]
        private int m_m;
        // 训练样例的权重
        [NonSerialized]
        private double[] m_dist;

        private double m_r;

        public int Index
        {
            get { return m_index; }
            set { m_index = value; }
        }

        public double[] Distribution
        {
            get { return m_dist; }
            set { m_dist = value; }
        }

        public double R
        {
            get { return m_r; }
            set { m_r = value; }
        }


        public WeakLearn(int index, Collection<Example> t_set)
        {
            m_index = index;
            m_t_set = t_set;
            m_m = t_set.Count;


        }

        /// <summary>
        /// foamliu, 2009/02/27, 计算 R.
        /// 
        /// </summary>
        /// <returns></returns>
        public double CalcR()
        {
            double r = 0.0;

            for (int i = 0; i < m_m; i++)
            {
                Example example = m_t_set[i];

                //foamliu, 2009/03/06, 优化.
                //
                int iResult = this.Predict(example.X);

                // foamliu, 2009/03/04, r 这个值在[0, 1] 之间, 0 表示全部正确, 1 表示全部错误.
                r += m_dist[i] * this.One(example.Label.Id != iResult);

            }

            m_r = r;
            return r;
        }

        /// <summary>
        /// foamliu, 2009/03/06, "1" 函数, 正确返回 1; 错误返回 0.
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private int One(bool expression)
        {
            return expression ? 1 : 0;
        }


        // foamliu, 2009/03/04, 每次都是针对变化了的权值分布进行训练        
        public override void Train()
        {
            // 类别为 -1 的样例中心.
            double negGravitCenter;
            // 类别为 +1 的样例中心.
            double posGravitCenter;

            double negSum = 0.0;
            double posSum = 0.0;

            double negArea = 0;
            double posArea = 0;

            for (int i = 0; i < m_m; i++)
            {
                Example example = m_t_set[i];

                if (example.Label.Id == +1)
                {
                    posSum += example.X[m_index] * m_dist[i];
                    posArea += m_dist[i];
                }
                else
                {
                    negSum += example.X[m_index] * m_dist[i];
                    negArea += m_dist[i];
                }
            }

            posGravitCenter = posSum / posArea;
            negGravitCenter = negSum / negArea;

            // 分界点在二者中间.
            m_b = (posGravitCenter + negGravitCenter) / 2;

            m_direction = (posGravitCenter <= negGravitCenter) ? +1 : -1;
        }

        /// <summary>
        /// foamliu, 2009/04/17, 对新的样例进行分类.
        /// 
        /// </summary>
        /// <param name="iImage"></param>
        /// <param name="emotion"></param> 
        public override int Predict(SparseVector x)
        {
            int iResult;
            // 方向: 
            //  -1: 小于等于 m_b 为 -1.
            //  +1: 小于等于 m_b 为 +1.
            if (m_direction == -1)
            {
                if (x[m_index] <= m_b)
                    iResult = -1;
                else
                    iResult = +1;
            }
            else
            {
                if (x[m_index] <= m_b)
                    iResult = +1;
                else
                    iResult = -1;
            }

            return iResult;
        }

        public override int Predict(SparseVector x, out double confidence)
        {
            confidence = 1.0;
            return this.Predict(x);
        }

        public override void SaveModel(string fileName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// foamliu, 2009/03/06, 初始化权重.
        /// 
        /// </summary>
        public void InitializeWeights()
        {
            m_dist = new double[m_m];
            double vote = 1.0 / m_m;

            for (int i = 0; i < m_m; i++)
            {
                m_dist[i] = vote;
            }
        }

    }
}
