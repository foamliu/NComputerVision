
namespace NComputerVision.Learning.AdaBoost
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading;
    using System.Collections.ObjectModel;
    using NComputerVision.Learning;
    using log4net;

    /// <summary>
    /// foamliu, 2009/04/17, 通用的Boost二类分类器.
    /// 
    /// </summary>
    [Serializable()]
    public class AdaBoost : Classifier
    {
        #region Fields
        [NonSerialized]
        private Collection<Example> m_t_set;
        // 训练集样本个数
        [NonSerialized]
        private int m_m;

        // 权重分部 (样例, 表情)
        [NonSerialized]
        private double[] m_dist;

        // foamliu, 2009/02/24, 循环次数.
        private int m_t;
        private double[] m_a;
        private WeakLearn[] m_h;
        private int m_n;    // 每个实例有多少维度

        private static readonly ILog log = LogManager.GetLogger(typeof(AdaBoost));
        #endregion

        #region Properties
        public Collection<Example> TrainSet
        {
            get
            {
                return m_t_set;
            }
            set
            {
                this.m_t_set = value;
            }
        }

        public int T
        {
            get { return m_t; }
        }

        /// <summary>
        /// foamliu, 2009/04/23, 各弱分类器的信用.
        /// </summary>
        public double[] Credits
        {
            get { return m_a; }
        }

        /// <summary>
        /// foamliu, 2009/04/23, 弱分类器列表.
        /// </summary>
        public WeakLearn[] WeakLearners
        {
            get { return m_h; }
        }
        #endregion

        #region Methods
        public AdaBoost(Collection<Example> t_set, int t, int n)
        {
            m_t_set = t_set;
            m_m = m_t_set.Count;
            m_dist = new double[this.m_m];
            m_t = t;
            m_n = n;

            m_a = new double[m_t];
            m_h = new WeakLearn[m_t];
        }

        public AdaBoost()
        {

        }

        /// <summary>
        /// foamliu, 2009/04/17, 训练.
        /// 
        /// </summary>
        public override void Train()
        {
            log.Info("Start Training AdaBoost...");

            log.Info("T: {0}", m_t);
            log.Info("Trainset number: {0}", m_t_set.Count);

            double rt;
            WeakLearn ht;
            double at;

            // foamliu, 2009/03/09, 移到线程内部.
            List<WeakLearn> pool = new List<WeakLearn>();

            for (int i = 0; i < m_n; i++)
            {
                pool.Add(new WeakLearn(i, m_t_set));
            }

            // foamliu, 2009/03/09, 移到线程内部.
            double[] dist = new double[this.m_m];

            InitializeWeights(this, ref dist);

            for (int t = 0; t < this.m_t; t++)
            {
                //Logging.Info(t.ToString());

                rt = Double.MaxValue;
                ht = null;

                // foamliu, 2009/03/03, 选择一个弱分类器 ht, 使得 rt 最小化.

                foreach (WeakLearn learn in pool)
                {
                    learn.Distribution = dist;
                    learn.Train();
                    double r = learn.CalcR();

                    if (rt > r)
                    {
                        rt = r;
                        ht = learn;
                    }
                }

                //ManualResetEvent[] doneEvents = new ManualResetEvent[2];
                //doneEvents[0] = new ManualResetEvent(false);
                //doneEvents[1] = new ManualResetEvent(false);

                //ThreadPool.QueueUserWorkItem(new WaitCallback(WeakLearnThread), new WeakLearnPara(pool, 0, pool.Count / 2, this, doneEvents[0]));
                //ThreadPool.QueueUserWorkItem(new WaitCallback(WeakLearnThread), new WeakLearnPara(pool, pool.Count / 2, pool.Count - pool.Count / 2, this, doneEvents[1]));
                //// foamliu, 2009/03/09, 等待线程池中的线程们凯旋.
                //WaitHandle.WaitAll(doneEvents);

                //// foamliu, 2009/03/03, 选择一个弱分类器 ht, 使得 rt 最小化.
                ////
                //foreach (WeakLearn learn in pool)
                //{
                //    double r = learn.R;

                //    if (rt > r)
                //    {
                //        rt = r;
                //        ht = learn;
                //    }
                //}

                if (null == ht)
                {
                    throw new ApplicationException("ht is null!!!");
                }

                // foamliu, 2009/03/04, 错误率不应该大于随机猜测 (7类).
                if (rt >= 0.5)
                {
                    //throw new ApplicationException("rt > 0.5!!!");
                    log.Info("rt > 0.5!!! t:{0}, rt:{1}", t, rt);
                    break;
                }

                at = 0.5 * Math.Log((1 + rt) / (1 - rt));
                //at = 0.5 * Math.Log((1 - rt) / rt);

                //Logging.Info("at: {0}, rt: {1}", at, rt);

                UpdateDistribution(this, ref dist, at, ht);

                // 注意: m_a/m_h 需要线程间同步.
                this.m_a[t] = at;
                this.m_h[t] = ht;

                NormalizeDistribution(this, ref dist);

                pool.Remove(ht);
                ht = null;
            }

            log.Info("Completed Training.");

            log.Info("AdaBoost Training Completed....");
        }

        class WeakLearnPara
        {
            private List<WeakLearn> m_pool;
            private int m_start;
            private int m_count;
            private AdaBoost m_ada;
            private ManualResetEvent m_doneEvent;

            public List<WeakLearn> Pool
            {
                get { return m_pool; }
            }

            public int Start
            {
                get { return m_start; }
            }

            public int Count
            {
                get { return m_count; }
            }

            public AdaBoost AdaBoost
            {
                get { return m_ada; }
            }

            public ManualResetEvent DoneEvent
            {
                get { return m_doneEvent; }
            }

            public WeakLearnPara(List<WeakLearn> pool, int start, int count, AdaBoost ada, ManualResetEvent doneEvent)
            {
                m_pool = pool;
                m_start = start;
                m_count = count;
                m_ada = ada;
                m_doneEvent = doneEvent;
            }
        }

        private static void WeakLearnThread(object o)
        {
            WeakLearnPara para = (WeakLearnPara)o;

            for (int i = para.Start; i < para.Start + para.Count; i++)
            {
                WeakLearn learn = para.Pool[i];
                learn.Distribution = para.AdaBoost.m_dist;
                learn.Train();
                learn.CalcR();
            }

            // foamliu, 2009/03/09, 完成, 通知主训练线程.
            para.DoneEvent.Set();

        }

        /// <summary>
        /// foamliu, 2009/04/17, 预测.
        /// 
        /// </summary>
        public override int Predict(SparseVector x, out double confidence)
        {
            double sum = 0;
            double aSum = 0;

            for (int t = 0; t < m_t; t++)
            {
                int iResult = m_h[t].Predict(x);

                sum += m_a[t] * iResult;
                aSum += m_a[t];
            }

            confidence = sum / aSum;
            return Math.Sign(sum);
        }

        public override int Predict(SparseVector x)
        {
            double confidence;
            return this.Predict(x, out confidence);
        }


        /// <summary>
        /// foamliu, 2009/02/24, 初始化权重.
        /// 
        /// </summary>
        private static void InitializeWeights(AdaBoost ada, ref double[] dist)
        {
            double vote = 1.0 / ada.m_m;

            for (int i = 0; i < ada.m_m; i++)
            {
                dist[i] = vote;
            }
        }

        /// <summary>
        /// foamliu, 2009/02/24, 归一化权重分布.
        /// 
        /// </summary>
        /// <param name="t"></param>
        private static void NormalizeDistribution(AdaBoost ada, ref double[] dist)
        {
            double sum = 0.0;

            for (int i = 0; i < ada.m_m; i++)
            {
                sum += dist[i];
            }

            double reciprocal = 1.0 / sum;

            for (int i = 0; i < ada.m_m; i++)
            {
                // foamliu, 2009/02/04, 优化.
                dist[i] *= reciprocal;
            }

        }

        /// <summary>
        /// foamliu, 2009/02/24, 更新权重分布.
        /// 
        /// </summary>
        /// <param name="at"></param>
        /// <param name="ht"></param>
        private static void UpdateDistribution(AdaBoost ada, ref double[] dist, double at, WeakLearn ht)
        {
            for (int i = 0; i < ada.m_m; i++)
            {
                Example example = ada.m_t_set[i];

                int iResult = ht.Predict(example.X);

                // foamliu, 2009/03/04, 更新权重, example.Label.Id 和 iResult 可能的取值均为 -1和+1, 
                // 当它们相同, 乘积为1, 整个项 > 0, 乘子 > 1, 权重提升;
                // 当它们不同, 乘积为-1, 整个项 < 0, 乘子 < 1, 权重下降.
                dist[i] = dist[i] * Math.Exp(-at * example.Label.Id * iResult);
            }
        }

        public override void SaveModel(string fileName)
        {
            FileStream filestream = new FileStream(fileName, FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(filestream, this);
            filestream.Close();
        }

        public static AdaBoost LoadModel(string fileName)
        {
            AdaBoost adaboost;
            IFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
            adaboost = (AdaBoost)formatter.Deserialize(stream);
            stream.Close();
            return adaboost;
        }
        #endregion
    }
}
