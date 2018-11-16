
namespace NComputerVision.Learning.NeuralNet
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using log4net;

    /// <summary>
    /// foamliu, 2008/12/29. 
    ///
    /// Architecture: multilayer perceptron with two hidden layers.
    /// 多层感知器: 或者任意前馈 (无环路) 网络.
    /// 
    /// See also this article:
    /// http://galaxy.agh.edu.pl/~vlsi/AI/backp_t_en/backprop.html
    /// </summary>
    [Serializable()]
    public class ANN_BP : Classifier
    {
        #region Fields
        [NonSerialized]
        private Collection<Example> m_t_set;
        // Learning rate
        private double m_eta;
        // An acceptable error value 
        private double m_epsilon;
        // Reaching a maximum count of iterations means the NN training was not successful.
        private int m_maximumIteration;

        private double m_sigmoid_alpha;
        private double m_momentum;

        //private double m_error;

        private int m_logInterval;

        private Random m_rand = new Random();

        //private int m_d;

        private List<InputNode> m_inputLayer;
        private List<HiddenNode> m_firstHiddenLayer;
        private List<HiddenNode> m_secondHiddenLayer;
        private List<OutputNode/*<int>*/> m_outputLayer;

        private static readonly ILog log = LogManager.GetLogger(typeof(ANN_BP));

        #endregion

        #region Properties

        public Collection<Example> TrainSet
        {
            get
            {
                return this.m_t_set;
            }
            set
            {
                this.m_t_set = value;
            }
        }

        public int LogInterval
        {
            get
            {
                return this.m_logInterval;
            }
            set
            {
                this.m_logInterval = value;
            }
        }

        public double ANN_Epsilon
        {
            get
            {
                return this.m_epsilon;
            }
            set
            {
                this.m_epsilon = value;
            }
        }

        public double ANN_Eta
        {
            get
            {
                return this.m_eta;
            }
            set
            {
                this.m_eta = value;
            }
        }

        public double ANN_Sigmoid_Alpha
        {
            get
            {
                return this.m_sigmoid_alpha;
            }
            set
            {
                this.m_sigmoid_alpha = value;
            }
        }

        public double ANN_Momentum_Alpha
        {
            get
            {
                return this.m_momentum;
            }
            set
            {
                this.m_momentum = value;
            }
        }

        public int MaximumIteration
        {
            get
            {
                return this.m_maximumIteration;
            }
            set
            {
                this.m_maximumIteration = value;
            }
        }

        //public double Error
        //{
        //    get { return this.m_error; }
        //}

        public List<InputNode> InputLayer
        {
            get { return this.m_inputLayer; }
        }

        public List<HiddenNode> FirstHiddenLayer
        {
            get { return this.m_firstHiddenLayer; }
        }

        public List<HiddenNode> SecondHiddenLayer
        {
            get { return this.m_secondHiddenLayer; }
        }

        public List<OutputNode/*<int>*/> OutputLayer
        {
            get { return this.m_outputLayer; }
        }

        public int InputNum
        {
            get { return this.InputLayer.Count; }
        }

        public int FirstHiddenNum
        {
            get { return this.FirstHiddenLayer.Count; }
        }

        public int SecondHiddenNum
        {
            get { return this.SecondHiddenLayer.Count; }
        }

        public int OutputNum
        {
            get { return this.OutputLayer.Count; }
        }

        #endregion

        #region Methods

        //public int Predict(SparseVector x)
        //{
        //    int matched = 0;
        //    double max = -1.0;

        //    this.ForwardPropagate(x, 0);

        //    for (int i = 0; i < OutputNum; i++)
        //    {
        //        if (OutputLayer[i].Output > max)
        //        {
        //            matched = i;
        //            max = OutputLayer[i].Output;
        //        }
        //    }

        //    return matched;
        //}

        public override int Predict(SparseVector x, out double confidence)
        {
            int result;
            int matched = 0;
            double max = Double.MinValue;

            this.ForwardPropagate(x, 0);

            for (int i = 0; i < OutputNum; i++)
            {
                if (OutputLayer[i].Output > max)
                {
                    matched = i;
                    max = OutputLayer[i].Output;
                }
            }

            confidence = max;
            result = TargetMapTo(matched);
            return result;

            //this.ForwardPropagate(x, 0);

            //if (OutputLayer[0].Output > 0.5)
            //{
            //    result = +1;
            //    confidence = OutputLayer[0].Output;
            //}
            //else
            //{
            //    result = -1;
            //    confidence = 1 - OutputLayer[0].Output;
            //}

        }

        /// <summary>
        /// foamliu, 2009/05/12, 实现了一个简化版本.
        /// </summary>
        /// <param name="x"></param>       
        /// <param name="confidence"></param>
        public override int Predict(SparseVector x)
        {
            double confidence = 0;
            return this.Predict(x, out confidence);
        }

        public override void Train()
        {
            log.Info("Start Training ANN.");

            double errsum = 0;
            double lastError = 1.0;
            double rateOfChange = 0.0;
            int currentIteration = 0;

            do
            {

                // foamliu, 2008/12/30.
                // Notes: the order of presentation of training examples should be randomized from epoch
                //  to epoch. 
                //

                //ShuffleTrainSet();

                errsum = 0;

                foreach (Example e in this.TrainSet)
                {
                    // foamliu, 2008/12/30, two basic signal flows in a multilayer perceptron:
                    //  forward propagation of function signals and back propagation of error signals.
                    this.ForwardPropagate(e.X, e.Label.Id);
                    this.BackPropagate();
                    errsum += this.GetError();
                }

                ++currentIteration;

                //Logging.Info(currentError.ToString());
                if (currentIteration % m_logInterval == 0)
                    log.Info(errsum.ToString());
                //this.m_error = errsum;


                // foamliu, 2008/12/30, stopping criteria.
                // The back-propagation algorithm is considered to have converged when the absolute rate of
                //  change in the average squared error per epoch is sufficiently small.

                rateOfChange = Math.Abs((errsum - lastError) / lastError);
                lastError = errsum;
            }
            while (/*rateOfChange > this.ANN_Epsilon Math.Abs(currentError) > 1.0 &&*/ currentIteration < this.MaximumIteration && errsum > this.ANN_Epsilon);

        }

        /// <summary>
        /// foamliu, 2008/12/30.
        /// Notes: the order of presentation of training examples should be randomized from epoch
        ///  to epoch. 
        ///
        /// </summary>
        private void ShuffleTrainSet()
        {
            Collection<Example> t_set = this.TrainSet;
            int num = t_set.Count;
            Random rand = new Random();

            int[] cards = new int[num];
            for (int i = 0; i < num; i++)
                cards[i] = i;

            for (int i = 0; i < num; i++)
            {
                int temp;
                int j = (int)(rand.NextDouble() * num); // 0 - (num-1)
                temp = cards[i];
                cards[i] = cards[j];
                cards[j] = temp;
            }

            Example[] examples = new Example[num];

            for (int i = 0; i < num; i++)
            {
                examples[i] = t_set[cards[i]];
            }

            t_set.Clear();

            for (int i = 0; i < num; i++)
            {
                t_set.Add(examples[i]);
            }

        }

        /// <summary>
        /// foamliu, 2008/12/30, function signals: -->
        /// 
        /// A function signal is an input signal (stimulus) that comes in at
        /// the input end of the network, propagates forward (neuron by neuron) through the
        /// network, and emerges at the output end of the network as an output signal.
        /// 
        /// </summary>
        /// <param name="sparseVector"></param>
        /// <param name="output"></param>
        private void ForwardPropagate(SparseVector vector, int target)
        {
            double sum;
            double o;

            // Apply input to the network
            for (int i = 0; i < InputNum; i++)
            {
                InputLayer[i].Value = vector[i];
            }
            // Calculate The First Hidden Layer's Inputs and Outputs
            for (int i = 0; i < FirstHiddenNum; i++)
            {
                sum = 0.0;
                for (int j = 0; j < InputNum; j++)
                {
                    sum += InputLayer[j].Value * InputLayer[j].Weights[i];
                }
                FirstHiddenLayer[i].InputSum = sum;
                FirstHiddenLayer[i].Output = Squash(sum);
            }
            // Calculate The Second Hidden Layer's Inputs and Outputs
            for (int i = 0; i < SecondHiddenNum; i++)
            {
                sum = 0.0;
                for (int j = 0; j < FirstHiddenNum; j++)
                {
                    sum += FirstHiddenLayer[j].Output * FirstHiddenLayer[j].Weights[i];
                }

                SecondHiddenLayer[i].InputSum = sum;
                SecondHiddenLayer[i].Output = Squash(sum);
            }
            // Calculate The Output Layer's Inputs, Outputs and Errors
            for (int i = 0; i < OutputNum; i++)
            {
                sum = 0.0;
                for (int j = 0; j < SecondHiddenNum; j++)
                {
                    sum += SecondHiddenLayer[j].Output * SecondHiddenLayer[j].Weights[i];
                }

                OutputLayer[i].InputSum = sum;
                OutputLayer[i].Output = Squash(sum);
                o = OutputLayer[i].Output;

                // foamliu, 2009/01/14.
                // 两个输出单元, 当期望分类是 +1时, 则输出单元0的期望值为0, 输出单元1的期望值为1;
                //              当期望分类是 -1时, 则输出单元0的期望值为1, 输出单元1的期望值为0;
                //OutputLayer[i].Target = output == i ? 1 : 0;

                // target
                int t = (i == TargetMapFrom(target)) ? 1 : 0;

                OutputLayer[i].Target = t;

                // foamliu, 2009/01/14, 添加注释.
                // 对于网络中每个输出单位 j, 计算它的误差项:
                // deltaj = oj(1-oj)(tj-oj)
                OutputLayer[i].Delta = o * (1.0 - o) * (t - o);

            }

        }

        /// <summary>
        /// foamliu, 2008/12/30, error signals: <==
        /// 
        /// A error signal originates at an output neuron of the network and 
        /// propagates backward (layer by layer) through the network. 
        /// 
        /// 反向传播 (BP) 中比较重要的思想就是责任 (credit or blame) 在不同的节点间按照权值线性分配.
        /// 
        /// </summary>
        private void BackPropagate()
        {
            double sum;
            double o;

            // 这次的权值变化
            double new_dw;

            // foamliu, 2009/01/14.
            // 对于网络的每个隐藏单元 i, 计算它的误差项:
            //  deltai = oi(1-oi)Sigma wji deltaj
            // 为效率加一个变量: lhi = oi(1-oi)
            // lhi = left-hand-item

            // Fix Second Hidden Layer's Error
            for (int i = 0; i < SecondHiddenNum; i++)
            {
                sum = 0.0;
                o = SecondHiddenLayer[i].Output;

                for (int j = 0; j < OutputNum; j++)
                {
                    sum += SecondHiddenLayer[i].Weights[j] * OutputLayer[j].Delta;
                }
                SecondHiddenLayer[i].Delta = o * (1.0 - o) * sum;
            }
            // Fix First Hidden Layer's Error
            for (int i = 0; i < FirstHiddenNum; i++)
            {
                sum = 0.0;
                o = FirstHiddenLayer[i].Output;

                for (int j = 0; j < SecondHiddenNum; j++)
                {
                    sum += FirstHiddenLayer[i].Weights[j] * SecondHiddenLayer[j].Delta;
                }
                FirstHiddenLayer[i].Delta = o * (1.0 - o) * sum;
            }

            // foamliu, 2009/01/14, 更新每个网络权值 w(ji)
            // w(ji) <- w(ji) = delta w(ji)
            // delta w(ji) = eta deltaj xji

            // Update The Input Layer's Weights
            for (int i = 0; i < FirstHiddenNum; i++)
            {
                for (int j = 0; j < InputNum; j++)
                {
                    //InputLayer[j].Weights[i] +=
                    //    this.m_eta * FirstHiddenLayer[i].Delta * InputLayer[j].Value;

                    // foamliu, 2009/01/14, 引入冲量项 (Momentum).

                    // 初始化为第一项 (无冲量).
                    new_dw = this.m_eta * FirstHiddenLayer[i].Delta * InputLayer[j].Value;
                    // 加入冲量.
                    new_dw += this.m_momentum * InputLayer[j].OldDeltaWeights[i];
                    // 更新权值.
                    InputLayer[j].Weights[i] += new_dw;
                    // 存入这次的权值变化.
                    InputLayer[j].OldDeltaWeights[i] = new_dw;

                }
            }
            // Update The First Hidden Layer's Weights
            for (int i = 0; i < SecondHiddenNum; i++)
            {
                for (int j = 0; j < FirstHiddenNum; j++)
                {
                    //FirstHiddenLayer[j].Weights[i] +=
                    //    this.m_eta * SecondHiddenLayer[i].Delta * FirstHiddenLayer[j].Output;

                    // foamliu, 2009/01/14, 引入冲量项 (Momentum).

                    // 初始化为第一项 (无冲量).
                    new_dw = this.m_eta * SecondHiddenLayer[i].Delta * FirstHiddenLayer[j].Output;
                    // 加入冲量.
                    new_dw += this.m_momentum * FirstHiddenLayer[j].OldDeltaWeights[i];
                    // 更新权值.
                    FirstHiddenLayer[j].Weights[i] += new_dw;
                    // 存入这次的权值变化.
                    FirstHiddenLayer[j].OldDeltaWeights[i] = new_dw;

                }
            }
            // Update The Second Hidden Layer's Weights
            for (int i = 0; i < OutputNum; i++)
            {
                for (int j = 0; j < SecondHiddenNum; j++)
                {
                    //SecondHiddenLayer[j].Weights[i] +=
                    //    this.m_eta * OutputLayer[i].Delta * SecondHiddenLayer[j].Output;

                    // foamliu, 2009/01/14, 引入冲量项 (Momentum).

                    // 初始化为第一项 (无冲量).
                    new_dw = this.m_eta * OutputLayer[i].Delta * SecondHiddenLayer[j].Output;
                    // 加入冲量.
                    new_dw += this.m_momentum * SecondHiddenLayer[j].OldDeltaWeights[i];
                    // 更新权值.
                    SecondHiddenLayer[j].Weights[i] += new_dw;
                    // 存入这次的权值变化.
                    SecondHiddenLayer[j].OldDeltaWeights[i] = new_dw;

                }
            }

        }

        /// <summary>
        /// foamliu, 2008/12/29, Sigmoid Activation Function.
        ///                1
        /// s(x)=  --------------------               
        ///        1 + exp(-alpha * x )
        ///        
        /// 注意, 它的输出范围为 0 到 1, 随输入单调递增. 
        /// 因为这个函数把非常大的输入值域映射到一个小范围的输出, 它经常被称为 sigmoid 单元的挤压 (squash) 函数.
        /// 它的导数很容易用它的输出表示:
        /// ds(x)/dx = s(x)*(1-s(x)).
        /// </summary>
        /// <param name="total"></param>
        /// <returns>Output range of the function: [0, 1]</returns>
        private double Squash(double x)
        {
            // a is chosen between 0.5 and 2
            //double alpha = Constants.ANN_Sigmoid_Alpha;
            double s = 1.0 / (1.0 + Math.Exp(-this.m_sigmoid_alpha * x));

            return s;
        }

        /// <summary>
        /// foamliu, 2009/01/14.
        /// </summary>
        /// <returns>返回 -1.0 和 1.0之间的随机数</returns>
        private double Dpn1()
        {
            return (m_rand.NextDouble() * 2.0) - 1.0;
        }

        /// <summary>
        /// foamliu, 2009/01/14.
        /// 返回 x 的绝对值.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private double ABS(double x)
        {
            return Math.Abs(x);
        }

        /// <summary>
        /// foamliu, 2009/01/14, 将 {0,1} map 成 {-1,1}
        /// </summary>
        /// <returns></returns>
        private int TargetMapTo(int x)
        {
            switch (x)
            {
                case 0:
                    return -1;
                case 1:
                    return 1;
                default:
                    return 1;
            }
        }

        /// <summary>
        /// foamliu, 2009/01/14, 将 {-1,1} map 成 {0,1}
        /// </summary>
        /// <returns></returns>
        private int TargetMapFrom(int x)
        {
            switch (x)
            {
                case -1:
                    return 0;
                case 1:
                    return 1;
                default:
                    return 1;
            }
        }

        /// <summary>
        /// foamliu, 2009/01/14, 因为现在Error属性存的实际上是误差项delta, 所以需要另外开个函数.
        /// </summary>
        /// <returns></returns>
        private double GetError()
        {
            //return 0.5 * (this.OutputLayer[0].Error) * (this.OutputLayer[0].Error);

            double total = 0.0;
            for (int i = 0; i < OutputNum; i++)
            {
                total += Math.Pow((OutputLayer[i].Target - OutputLayer[i].Output), 2) / 2;
            }
            return total;
        }

        /// <summary>
        /// foamliu, 2009/01/14, 初始化所有的网络权值为小的随机值(例如 -0.05 和 0.05 之间的数).
        /// 上一次权值变化量 (last deltaweight) 不初始化, 默认为0.
        /// </summary>
        public void InitNet()
        {
            // 随机值:   [0.0, 1.0]
            // 所以权值:  [-0.05, 0.05]
            //Random rand = new Random();
            for (int i = 0; i < InputNum; i++)
            {
                for (int j = 0; j < FirstHiddenNum; j++)
                {
                    //InputLayer[i].Weights[j] = -0.05 + m_rand.NextDouble() * 0.1;
                    //InputLayer[i].Weights[j] = Dpn1();
                    InputLayer[i].Weights[j] = 1.0;
                }
            }

            for (int i = 0; i < FirstHiddenNum; i++)
            {
                for (int j = 0; j < SecondHiddenNum; j++)
                {
                    FirstHiddenLayer[i].Weights[j] = -0.05 + m_rand.NextDouble() * 0.1;
                    //FirstHiddenLayer[i].Weights[j] = Dpn1();
                }
            }

            for (int i = 0; i < SecondHiddenNum; i++)
            {
                for (int j = 0; j < OutputNum; j++)
                {
                    SecondHiddenLayer[i].Weights[j] = -0.05 + m_rand.NextDouble() * 0.1;
                    //SecondHiddenLayer[i].Weights[j] = Dpn1();
                }
            }
        }

        public override void SaveModel(string fileName)
        {
            FileStream filestream = new FileStream(fileName, FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(filestream, this);
            filestream.Close();
        }

        public static ANN_BP LoadModel(string fileName)
        {
            ANN_BP ann;
            IFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
            ann = (ANN_BP)formatter.Deserialize(stream);
            stream.Close();
            return ann;
        }

        #endregion

        #region Constructors

        public ANN_BP(int inputNum)
        {
            this.m_inputLayer = new List<InputNode>();
            this.m_firstHiddenLayer = new List<HiddenNode>();
            this.m_secondHiddenLayer = new List<HiddenNode>();
            this.m_outputLayer = new List<OutputNode/*<int>*/>();

            //this.m_d = inputNum;

            for (int i = 0; i < inputNum; i++)
            {
                this.m_inputLayer.Add(new InputNode(Constants.ANN_FirstHiddenLayerNodeNumber));
            }

            for (int i = 0; i < Constants.ANN_FirstHiddenLayerNodeNumber; i++)
            {
                this.m_firstHiddenLayer.Add(new HiddenNode(Constants.ANN_SecondHiddenLayerNodeNumber));
            }

            for (int i = 0; i < Constants.ANN_SecondHiddenLayerNodeNumber; i++)
            {
                this.m_secondHiddenLayer.Add(new HiddenNode(Constants.ANN_OutputLayerNodeNumbe));
            }

            for (int i = 0; i < Constants.ANN_OutputLayerNodeNumbe; i++)
            {
                this.m_outputLayer.Add(new OutputNode/*<int>*/());
            }

            this.InitNet();

            // foamliu, 2008/12/29, default values
            //
            this.m_sigmoid_alpha = Constants.ANN_Alpha;
            this.m_momentum = Constants.ANN_Momentum;
            this.m_epsilon = Constants.ANN_Epsilon;
            this.m_eta = Constants.ANN_Eta;
            this.m_maximumIteration = Constants.ANN_MaximumIteration;
            // foamliu, 2009/05/14, by default log every iteration.
            this.m_logInterval = 1;
        }

        #endregion

    }
}
