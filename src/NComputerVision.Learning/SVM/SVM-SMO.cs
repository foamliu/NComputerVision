
namespace NComputerVision.Learning.SVM
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using Learning;
    using System.Collections.ObjectModel;

    // KTT (Karush-Kuhn-Tucker) condition:
    //     0 <= alphai <= C
    //
    //      alphai == 0     ->  yi f(xi) >= 1
    //      0 <= alphai <= C->  yi f(xi) == 1
    //      alphai == C     ->  yi f(xi) <= 1
    //
    [Serializable()]
    public class Binary_SVM_SMO : Classifier
    {
        #region Fields
        private Kernel m_kernel;    // Kernel to use
        private double[] m_alpha;   // The Lagrange multipliers
        [NonSerialized]
        private double[] m_error;   // error cache
        private SparseVector m_weight;  // weight vector
        [NonSerialized]
        private int m_l;                // little-case L, number of training set        
        [NonSerialized]
        private List<int> m_NonBound;   // the non-bound subset, most likely to violate the KKT conditions

        private double m_b;             // The thresholds        

        //public const double SVM_C = double.MaxValue;  // hard margin
        // an optimal choice for C in the objective function of the resulting
        //  optimisation problem should be R^(-2)
        private double m_c;             // soft margin
        private double m_eta;           // learning rate
        private double m_tolerance;
        // typically eps can be set in the range 10^-2 to 10^-3
        private double m_epsilon;

        [NonSerialized]
        private Problem m_problem;
        [NonSerialized]
        private Collection<Example> m_t_set;
        // foamliu, 2009/05/14, 除了KNN其他算法都是不需要存训练集的.

        [NonSerialized]
        private Random m_rand;
        //[NonSerialized]
        //private Vocabulary m_voc;

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
                this.m_l = m_t_set.Count;
                this.m_alpha = new double[m_l];
                this.m_error = new double[m_l];
            }

        }

        public Kernel Kernel
        {
            get
            {
                return m_kernel;
            }
            set
            {
                this.m_kernel = value;
            }
        }

        public double SVM_C
        {
            get { return m_c; }
            set { m_c = value; }
        }

        public double SVM_Eta
        {
            get { return m_eta; }
            set { m_eta = value; }
        }

        public double SVM_Tolerance
        {
            get { return m_tolerance; }
            set { m_tolerance = value; }
        }

        public double SVM_Epsilon
        {
            get { return m_epsilon; }
            set { m_epsilon = value; }
        }

        #endregion

        #region Methods

        public override void Train()
        {
            // initialize alpha array to all zero            
            Array.Clear(m_alpha, 0, m_alpha.Length);

            // initialize threshold to zero
            m_b = 0;

            // 迭代次数
            //int countIter = 0;
            // 多少次迭代计算输出一次W
            //const int countCalcW = 10;


            // the outer loop keeps alternating between single passes
            //  over the entire training set and multiple passes over 
            // the non-bound subset until the entire training set obeys
            // the KKT conditions within eps.
            int numChanged = 0;
            bool examineAll = true;
            while (numChanged > 0 || examineAll)
            {
                numChanged = 0;
                // first iterates over the entire training set
                if (examineAll)
                {
                    for (int i = 0; i < m_l; i++)
                    {
                        numChanged += this.ExamineExample(i);
                    }
                }
                else
                {
                    // after one pass through the training set, the outer loop iterates
                    //  over only those examples whose Lagrange multipliers are neither 
                    //  0 nor C (the non-bound examples)
                    for (int i = 0; i < m_l; i++)
                    {
                        if (this.IsNotAtBounds(m_alpha[i]))
                        {
                            numChanged += this.ExamineExample(i);
                        }
                    }
                }
                if (examineAll)
                {
                    examineAll = false;
                }
                else if (numChanged == 0)
                {
                    // all of the non-bound examples obey the KKTconditions within eps
                    // the outer loop then iterates over the entire training set again
                    examineAll = true;
                }

                //foamliu, 2008/08/19, remove some logs.
                //Logger.Info("Number Changed: {0}", numChanged);

                //Logger.Info("W: {0}", Calculate_W());

                //foamliu, 2009/04/14, 计算W代价很高, 不宜每次迭代都计算.
                //if (++countIter % countCalcW == 0)
                //    Logger.Info("W: {0}", Calculate_W());

            }

            // the entire training set obeys the KKT conditions within eps, 
            //  the algorithm terminates.


            // foamliu, 2009/01/03, judge whether an example is a SV.
            for (int i = 0; i < m_t_set.Count; i++)
            {
                Example e = m_t_set[i];
                e.IsSV = !IsEqual(m_alpha[i], 0);
            }
        }

        public double SVMOutput(SparseVector x)
        {
            return Calculate_F(x);
        }

        public override int Predict(SparseVector x, out double confidence)
        {
            int result;
            double f;

            f = Calculate_F(x);

            confidence = S(Math.Abs(f));

            if (f >= 0)
            {
                result = +1;
            }
            else
            {
                result = -1;
            }

            return result;
        }

        /// <summary>
        /// foamliu, 2009/05/14, 补上这个方法.
        /// </summary>
        /// <param name="x">要分类的向量</param>
        /// <param name="iResult">分类结果: +1 或者 -1</param>
        public override int Predict(SparseVector x)
        {
            double confidence;
            return this.Predict(x, out confidence);
        }

        /// <summary>
        /// foamliu, 2008/12/29, Sigmoid Activation Function.
        ///                1
        /// s(x)=  --------------------               
        ///        1 + exp(-alpha * x )
        /// </summary>
        /// <param name="total"></param>
        /// <returns>Output range of the function: [0, 1]</returns>
        private double S(double x)
        {
            // a is chosen between 0.5 and 2
            double alpha = 1.0;
            double s = 1.0 / (1.0 + Math.Exp(-alpha * x));

            return s;
        }

        private bool TakeStep(int i1, int i2)
        {
            if (i1 == i2)
                return false;

            double alpha1 = 0, alpha2 = 0;
            int y1 = 0, y2 = 0;
            double E1 = 0, E2 = 0;
            double s = 0;
            double L = 0, H = 0;
            double Lobj = 0, Hobj = 0;
            double k11 = 0, k12 = 0, k22 = 0;
            double eta = 0;
            double a1 = 0, a2 = 0;
            double old_b, new_b;

            alpha1 = m_alpha[i1];
            alpha2 = m_alpha[i2];

            y1 = y(i1);
            y2 = y(i2);

            if (IsNotAtBounds(alpha1))
            {
                E1 = m_error[i1];
            }
            else
            {
                E1 = this.SVMOutput(x(i1)) - y1;
            }

            if (IsNotAtBounds(alpha2))
            {
                E2 = m_error[i2];
            }
            else
            {
                E2 = this.SVMOutput(x(i2)) - y2;
            }

            s = y1 * y2;

            // compute L, H
            if (y1 == y2)
            {
                L = Math.Max(0, (alpha2 + alpha1 - /*Constants.SVM_C*/ this.m_c));
                H = Math.Min(/*Constants.SVM_C*/  this.m_c, alpha2 + alpha1);

            }
            else // y1 != y2
            {
                L = Math.Max(0, (alpha2 - alpha1));
                H = Math.Min(/*Constants.SVM_C*/  this.m_c, /*Constants.SVM_C*/  this.m_c - alpha1 + alpha2);
            }

            if (IsEqual(L, H))
                return false;

            k11 = m_kernel.Compute(x(i1), x(i1));
            //k11 = 1;
            k12 = m_kernel.Compute(x(i1), x(i2));
            k22 = m_kernel.Compute(x(i2), x(i2));
            //k22 = 1;

            eta = 2 * k12 - k11 - k22;
            if (eta < 0)
            {
                a2 = alpha2 - y2 * (E1 - E2) / eta;
                if (a2 < L)
                    a2 = L;
                else if (a2 > H)
                    a2 = H;
            }
            else
            {
                Lobj = CalculateObjectiveFunction(i1, i2, alpha1 + s * (alpha2 - L), L);
                Hobj = CalculateObjectiveFunction(i1, i2, alpha1 + s * (alpha2 - H), H);
                if (Lobj > Hobj + /*Constants.SVM_Epsilon*/ this.m_epsilon)
                    a2 = L;
                else if (Lobj < Hobj - /*Constants.SVM_Epsilon*/ this.m_epsilon)
                    a2 = H;
                else
                    a2 = alpha2;
            }

            // This prevents round-off error from mistakenly forcing a point to be a support vector.

            if (a2 < 1e-8)
                a2 = 0;
            else if (a2 > /*Constants.SVM_C*/ this.m_c - 1e-8)
                a2 = /*Constants.SVM_C*/ this.m_c;

            if (Math.Abs(a2 - alpha2) < /*Constants.SVM_Epsilon*/ this.m_epsilon * (a2 + alpha2 + /*Constants.SVM_Epsilon*/ this.m_epsilon))
                return false;
            a1 = alpha1 + s * (alpha2 - a2);

            // Update threshold to reflect change in Lagrange multipliers
            old_b = m_b;
            UpdateThreshold(i1, i2, a1, a2);
            new_b = m_b;

            // Update weight vector to reflect change in a1 & a2, if linear SVM
            UpdateWeightVector(i1, i2, a1, a2);

            // Update error cache using new Lagrange multipliers
            UpdateErrorCache(i1, i2, a1, a2, old_b, new_b);

            m_error[i1] = E1;
            m_error[i2] = E2;

            if (IsNotAtBounds(alpha1))
            {
                m_error[i1] = 0;
            }
            if (IsNotAtBounds(alpha2))
            {
                m_error[i2] = 0;
            }

            m_alpha[i1] = a1;
            m_alpha[i2] = a2;

            UpdateNonBoundCache(i1, a1);
            UpdateNonBoundCache(i2, a2);

            return true;
        }

        /// <summary>
        /// determining whether each example violates the KKT conditions
        /// </summary>
        /// <param name="i2"></param>
        /// <returns></returns>
        private int ExamineExample(int i2)
        {
            int i1;
            int y2 = 0;
            double alpha2 = 0;
            double E2 = 0;
            double r2 = 0;

            y2 = y(i2);
            alpha2 = m_alpha[i2];
            if (IsNotAtBounds(alpha2))
            {
                E2 = m_error[i2];
            }
            else
            {
                E2 = this.SVMOutput(x(i2)) - y2;
            }

            r2 = E2 * y2;

            // test whether i2 vialate the KKT conditions
            if ((r2 < -/*Constants.SVM_Tolerance*/ this.m_tolerance && alpha2 < /*Constants.SVM_C*/ this.m_c) || (r2 > /*Constants.SVM_Tolerance*/ this.m_tolerance && alpha2 > 0))
            {
                if (m_NonBound.Count > 1)
                {
                    i1 = GetSecondHeuristicChoice(E2);
                    if (TakeStep(i1, i2))
                        return 1;
                }

                // loop over non-zero and non-C alpha, starting at random point
                //
                int startpoint;
                int pos;
                if (m_NonBound.Count > 0)
                {
                    startpoint = (int)Math.Floor(m_rand.NextDouble() * m_NonBound.Count);
                    for (int i = 0; i < m_NonBound.Count; i++)
                    {
                        pos = (startpoint + i) % m_NonBound.Count;
                        i1 = m_NonBound[pos];
                        if (TakeStep(i1, i2))
                            return 1;
                    }
                }

                // loop over all possible i1, starting at a random point
                //
                if (m_l > 0)
                {
                    startpoint = (int)Math.Floor(m_rand.NextDouble() * m_l);
                    for (int i = 0; i < m_l; i++)
                    {
                        i1 = (startpoint + i) % m_l;
                        if (TakeStep(i1, i2))
                            return 1;
                    }
                }

            }

            return 0;
        }

        /// <summary>
        /// With bias
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private double Calculate_F(SparseVector test)
        {
            double f = 0.0;

            // foamliu, 2009/01/12, optimization.

            if (this.m_kernel is LinearKernel)
            {
                f += SparseVector.DotProduct(m_weight, test);
            }
            else
            {
                for (int i = 0; i < m_alpha.Length; i++)
                {
                    //f += m_alpha[i] * y(i) * m_kernel.Compute(x(i), test);
                    //foamliu, 2009/04/17, 用于调试.
                    int yi = y(i);
                    double ker = m_kernel.Compute(x(i), test);
                    f += m_alpha[i] * yi * ker;
                }
            }

            // b is subtracted from the weighted sum of the kernels, not added.
            f -= m_b;

            return f;
        }

        private double Calculate_W()
        {
            double W = 0.0;
            double temp = 0.0;

            for (int i = 0; i < m_l; i++)
            {
                temp += m_alpha[i];
            }
            W = temp;

            temp = 0.0;
            for (int i = 0; i < m_l; i++)
            {
                for (int j = 0; j < m_l; j++)
                {
                    temp += y(i) * y(j) * m_alpha[i] * m_alpha[j] * m_kernel.Compute(x(i), x(j));
                }
            }
            W = W - temp / 2;

            return W;
        }

        private double CalculateObjectiveFunction(int i1, int i2, double a1, double a2)
        {
            double W = 0.0;
            double temp = 0.0;
            double alpha_i, alpha_j;

            for (int i = 0; i < m_alpha.Length; i++)
            {
                temp += m_alpha[i];
            }
            W = temp;

            temp = 0.0;
            for (int i = 0; i < m_alpha.Length; i++)
            {
                for (int j = 0; j < m_alpha.Length; j++)
                {
                    alpha_i = m_alpha[i];
                    alpha_j = m_alpha[j];
                    if (i == i1)
                        alpha_i = a1;
                    if (i == i2)
                        alpha_i = a2;
                    if (j == i1)
                        alpha_j = a1;
                    if (j == i2)
                        alpha_j = a2;
                    temp += y(i) * y(j) * alpha_i * alpha_j * m_kernel.Compute(x(i), x(j));
                }
            }
            W = W - temp / 2;

            return W;
        }

        private void UpdateThreshold(int i1, int i2, double a1, double a2)
        {
            double alpha1, alpha2;
            double b1, b2;
            double E1, E2;
            double y1, y2;
            SparseVector x1, x2;
            double old_b;
            double k11, k22, k12;

            alpha1 = m_alpha[i1];
            alpha2 = m_alpha[i2];

            E1 = m_error[i1];
            E2 = m_error[i2];

            y1 = y(i1);
            y2 = y(i2);

            x1 = x(i1);
            x2 = x(i2);

            //k11 = m_kernel.Compute(x(i1), x(i1));
            k11 = 1;
            k12 = m_kernel.Compute(x(i1), x(i2));
            //k22 = m_kernel.Compute(x(i2), x(i2));
            k22 = 1;

            old_b = m_b;

            b1 = E1 + y1 * (a1 - alpha1) * k11 + y2 * (a2 - alpha2) * k12 + old_b;
            b2 = E2 + y1 * (a1 - alpha1) * k12 + y2 * (a2 - alpha2) * k22 + old_b;

            // when both b1 and b2 are valid, they are equal
            if (IsNotAtBounds(a1))  // b1 is valid
            {
                // b1 is valid
                m_b = b1;
            }
            else if (IsNotAtBounds(a2)) // b2 is valid
            {
                // b2 is valid
                m_b = b2;
            }
            else if (IsAtBounds(a1) && IsAtBounds(a2))
            {
                m_b = (b1 + b2) / 2;
            }

        }

        private void UpdateWeightVector(int i1, int i2, double a1, double a2)
        {
            double alpha1, alpha2;

            alpha1 = m_alpha[i1];
            alpha2 = m_alpha[i2];

            m_weight.Add((y(i1) * (a1 - alpha1)) * x(i1));
            m_weight.Add((y(i2) * (a2 - alpha2)) * x(i2));
        }

        private void UpdateErrorCache(int i1, int i2, double a1, double a2, double old_b, double new_b)
        {
            double old_E, new_E;
            double alpha1, alpha2;
            int y1, y2;
            SparseVector x1, x2;

            alpha1 = m_alpha[i1];
            alpha2 = m_alpha[i2];

            y1 = y(i1);
            y2 = y(i2);

            x1 = x(i1);
            x2 = x(i2);

            foreach (int i in m_NonBound)
            {
                if (i == i1 || i == i2)
                    continue;

                old_E = m_error[i];
                new_E = old_E + y1 * (a1 - alpha1) * m_kernel.Compute(x1, x(i))
                        + y2 * (a2 - alpha2) * m_kernel.Compute(x2, x(i))
                        + old_b - new_b;

                m_error[i] = new_E;
            }
        }

        private void UpdateNonBoundCache(int i, double ai)
        {
            if (IsNotAtBounds(ai) && !m_NonBound.Contains(i))
            {
                m_NonBound.Add(i);
            }
            else if (!IsNotAtBounds(ai) && m_NonBound.Contains(i))
            {
                m_NonBound.Remove(i);
            }
        }

        private int GetSecondHeuristicChoice(double E1)
        {
            int i2 = 0;

            if (E1 >= 0)
            {
                double min = double.MaxValue;
                // choose an example x2 with minimum error E2                
                for (int j = 0; j < m_error.Length; j++)
                {
                    if (m_error[j] < min)
                    {
                        min = m_error[j];
                        i2 = j;
                    }
                }
            }
            else // E1 < 0
            {
                double max = double.MinValue;
                // maximises the error E2
                for (int j = 0; j < m_error.Length; j++)
                {
                    if (m_error[j] > max)
                    {
                        max = m_error[j];
                        i2 = j;
                    }
                }
            }

            return i2;
        }

        private int y(int i)
        {
            return m_t_set[i].Label.Id;
        }

        private SparseVector x(int i)
        {
            return m_t_set[i].X;
        }

        private bool IsNotAtBounds(double alpha)
        {
            return (!IsAtBounds(alpha));
        }

        private bool IsAtBounds(double alpha)
        {
            return (IsEqual(alpha, 0) || IsEqual(alpha, /*Constants.SVM_C*/ this.m_c));
        }

        private bool IsEqual(double d1, double d2)
        {
            return (Math.Abs(d1 - d2) <= /*Constants.SVM_Epsilon*/ this.m_epsilon);
        }

        /// <summary>
        /// foamliu, 2009/04/15, 在特征空间中球心在原点的球半径.
        /// </summary>
        /// <returns></returns>
        public double CalculateR()
        {
            double r = double.MinValue;

            foreach (Example e in m_t_set)
            {
                double k = m_kernel.Compute(e.X, e.X);
                if (k > r)
                    r = k;
            }

            return r;
        }

        public override void SaveModel(string fileName)
        {
            FileStream filestream = new FileStream(fileName, FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(filestream, this);
            filestream.Close();
        }

        public static Binary_SVM_SMO LoadModel(string fileName)
        {
            Binary_SVM_SMO svm;
            IFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
            svm = (Binary_SVM_SMO)formatter.Deserialize(stream);
            stream.Close();
            return svm;
        }

        #endregion

        #region Constructors

        public Binary_SVM_SMO(Problem problem)
        {
            this.m_problem = problem;
            this.m_t_set = this.m_problem.TrainingSet;
            // this.m_problem.RetrieveVocabulary(out this.m_voc);
            this.m_l = m_t_set.Count;
            this.m_alpha = new double[m_l];
            this.m_error = new double[m_l];

            this.m_kernel = new LinearKernel();
            this.m_NonBound = new List<int>();
            this.m_rand = new Random();
            this.m_weight = new SparseVector(problem.Dimension);

            // foamliu, 2009/01/12, default values
            this.m_c = Constants.SVM_C;
            this.m_eta = Constants.SVM_Eta;
            this.m_tolerance = Constants.SVM_Tolerance;
            this.m_epsilon = Constants.SVM_Epsilon;
        }

        public Binary_SVM_SMO(int n)
        {
            this.m_NonBound = new List<int>();
            this.m_rand = new Random();
            this.m_weight = new SparseVector(n);

            // foamliu, 2008/12/29, default values
            this.m_c = Constants.SVM_C;
            this.m_eta = Constants.SVM_Eta;
            this.m_tolerance = Constants.SVM_Tolerance;
            this.m_epsilon = Constants.SVM_Epsilon;

        }
        #endregion



    }
}
