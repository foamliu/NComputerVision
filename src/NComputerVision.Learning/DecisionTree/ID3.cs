using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NComputerVision.Learning.DecisionTree
{
    /// <summary>
    /// foamliu, 2008/01/12.
    /// 关于某个目标概念的正反样例的集合S,其相对于这个布尔型分类的熵为:
    /// 
    ///     Entropy(S) = -p(+)log2p(+) - p(-)log2p(-)
    /// 
    /// p(+)为S中正例的比例, p(-)是在S中反例的比例.
    /// 
    /// 比如在20_newsgroups这个问题中, 选两类alt.atheism 和c omp.graphics做分类,
    /// 每类用70%作为训练, 则总共训练样例1400个, 分属不同类别, 于是 p(+) = p(-) = 0.5.
    /// 则Entropy(S) = -0.5log2(0.5) - 0.5log2(0.5) = 1.
    /// 
    /// "信息增益" (information gain) 是定义属性分类训练数据能力的标准, 一个属性的信息增益
    /// 就是由于使用这个属性分割样例而导致的期望熵降低. 一个属性A相对于样例集合S的信息增益
    ///     Gain(S, A) = Entropy(S) - Sigma |Sv|Entropy(Sv)/|S|
    /// </summary>
    public class ID3
    {
        #region Fields
        private Collection<Example> m_t_set;
        private Vocabulary m_voc;
        // word -> information gain
        private List<WordGainPair> m_wordGainList;
        #endregion

        #region Properties
        public Collection<Example> TrainSet
        {
            get { return this.m_t_set; }
            set { this.m_t_set = value; }
        }
        #endregion

        #region Constructors
        private ID3()
        {
        }

        public ID3(Collection<Example> t_set, Vocabulary voc)
        {
            this.m_t_set = t_set;
            this.m_voc = voc;
            this.m_wordGainList = new List<WordGainPair>();
        }
        #endregion

        #region Methods
        /// <summary>
        /// word 对两个类别的区分能力.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private double GetGain(string word)
        {
            // Gain(S, A) = Entropy(S) - Sigma |Sv|Entropy(Sv)/|S|
            // v 为 0,1 分别表示含有此word为0 和大于等于1.
            double gain;

            // S = [700+, 700-]
            double numS = this.m_t_set.Count;

            // S0 = [numS0plus, numS0minus]
            // S1 = [numS1plus, numS1minus]

            // numS0plus 表示包含这个单词数为0, 而且类别为 +1 的样例数目;
            // numS1minus 表示包含这个单词数大于等于, 而且类别为 -1 的样例数目;

            double numSplus = 0, numSminus = 0;
            double numS0 = 0, numS0plus = 0, numS0minus = 0;
            double numS1 = 0, numS1plus = 0, numS1minus = 0;

            foreach (TextExample example in m_t_set)
            {
                if (example.Label.Id == +1)
                    numSplus++;
                else
                    numSminus++;

                if (!example.Tokens.ContainsKey(word))
                {
                    numS0++;
                    // 由此我们有 numS0 = numS0plus + numS0minus
                    if (example.Label.Id == +1)
                        numS0plus++;
                    else
                        numS0minus++;
                }
                else
                {
                    numS1++;
                    // 由此我们有 numS1 = numS1plus + numS1minus
                    if (example.Label.Id == +1)
                        numS1plus++;
                    else
                        numS1minus++;
                }
            }

            //double entropyS = -1.0 * (numSplus / numS) * SafeLog(numSplus / numS) / Math.Log(2.0)
            //                    - 1.0 * (numSminus / numS) * SafeLog(numSminus / numS) / Math.Log(2.0);

            // foamliu, 2009/01/13, optimization.
            double entropyS = 1.0;

            double entropyS0 = -1.0 * (numS0plus / numS0) * SafeLog(numS0plus / numS0) / Math.Log(2.0)
                                - 1.0 * (numS0minus / numS0) * SafeLog(numS0minus / numS0) / Math.Log(2.0);
            double entropyS1 = -1.0 * (numS1plus / numS1) * SafeLog(numS1plus / numS1) / Math.Log(2.0)
                                - 1.0 * (numS1minus / numS1) * SafeLog(numS1minus / numS1) / Math.Log(2.0);

            gain = entropyS - numS0 * entropyS0 / numS - numS1 * entropyS1 / numS;

            return gain;
        }

        private double SafeLog(double value)
        {
            if (value == 0.0)
                return 0.0;

            return Math.Log(value);
        }

        public void ComputeGain()
        {
            foreach (string word in m_voc.WordBag.Keys)
            {
                m_wordGainList.Add(new WordGainPair(word, GetGain(word)));
            }
            m_wordGainList.Sort();

        }

        public void GetFirstN(int n, out List<string> result)
        {
            if (n > this.m_wordGainList.Count)
                n = this.m_wordGainList.Count;

            result = new List<string>();

            for (int i = 0; i < n; i++)
            {
                result.Add(this.m_wordGainList[i].Word);
            }

        }

        #endregion

        //public static void Main()
        //{
        //    //ExampleSet t_set;
        //    //Vocabulary voc;
        //    //ClassificationProblem problem = new ClassificationProblem(AlgorithmType.SVM, 0, 1);            
        //    //problem.RetrieveTrainingSet(out t_set);
        //    //problem.RetrieveTrainingSetVocabulary(out voc);

        //    //foreach (Example example in t_set.Examples)
        //    //{
        //    //    problem.BuildExample(example, voc, t_set.Examples.Count);
        //    //}

        //    //ID3 id3 = new ID3(t_set, voc);
        //    //id3.ComputeGain();

        //    //List<string> res;
        //    //id3.GetFirstN(10, out res);

        //    //foreach (string word in res)
        //    //{
        //    //    System.Console.WriteLine(word);
        //    //}           
        //}

    }

    class WordGainPair : IComparable
    {
        private string m_word;
        private double m_gain;

        public string Word
        {
            get { return m_word; }
        }

        public double Gain
        {
            get { return m_gain; }
        }

        public WordGainPair(string word, double gain)
        {
            this.m_word = word;
            this.m_gain = gain;
        }

        public int CompareTo(object obj)
        {
            WordGainPair other = (WordGainPair)obj;
            return Math.Sign(other.Gain - this.Gain);
        }
    }
}
