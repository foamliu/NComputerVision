
namespace NComputerVision.Learning
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// 
    /// Sparse Vector:
    /// ==============
    /// 
    /// A sparse vector is a vector having a relatively small number of nonzero elements.
    /// 
    /// Consider the following as an example of a sparse vector x with n elements, where n is 11, 
    /// and vector x is: 
    /// 
    ///          (0.0, 0.0, 1.0, 0.0, 2.0, 3.0, 0.0, 4.0, 0.0, 5.0, 0.0)
    /// 
    /// 
    /// </summary>


    [Serializable]
    public class SparseVector : IEquatable<SparseVector>, IComparable<SparseVector>
    {
        const double MinDouble = Double.Epsilon;

        #region Fields
        // Stores dimension of space
        private int m_nDimension;
        // index-value pairs
        private SortedDictionary<int, double> m_Components;
        #endregion

        #region Properties
        public int Dimension
        {
            get
            {
                return m_nDimension;
            }
            set
            {
                m_Components.Clear();
                m_nDimension = value;
            }
        }
        public SortedDictionary<int, double> Components
        {
            get
            {
                return m_Components;
            }
        }
        public int NumberOfElements
        {
            get
            {
                return m_Components.Count;
            }
        }
        public double Magnitude
        {
            get
            {
                return Math.Sqrt(SumComponentSqrs());
            }
        }
        public bool IsEmpty
        {
            get
            {
                return (0 == NumberOfElements);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructs zero vector in SpaceDimension space
        /// </summary>
        /// <param name="SpaceDimension"></param>
        public SparseVector(int n)
        {
            this.m_nDimension = n;
            m_Components = new SortedDictionary<int, double>();
        }
        #endregion

        #region Methods

        public double this[int index]
        {
            get
            {
                if (index < 0 || index >= m_nDimension)
                    throw new ArgumentOutOfRangeException("index");

                double result;

                if (m_Components.ContainsKey(index))
                {
                    result = m_Components[index];
                }
                else
                {
                    result = 0.0;
                }

                return result;
            }
            set
            {
                if (index < 0 || index >= m_nDimension)
                    throw new ArgumentOutOfRangeException("index");

                if (Math.Abs(value) > MinDouble)
                    m_Components[index] = value;
            }

        }

        /// <summary>
        /// Addition (v3 = v1 + v2)
        /// </summary>
        /// <returns></returns>
        public static SparseVector operator +(SparseVector v1, SparseVector v2)
        {
            if (v1.Dimension != v2.Dimension)
                throw new ArgumentException("v1 and v2 have different dimensions.");

            SparseVector v3 = new SparseVector(v1.Dimension);
            // v3 <- v1
            foreach (int i in v1.Components.Keys)
            {
                v3.Components.Add(i, v1.Components[i]);
            }
            // v3 <- v1 + v2
            foreach (int j in v2.Components.Keys)
            {
                if (v3.Components.ContainsKey(j))
                {
                    v3.Components[j] += v2.Components[j];
                }
                else
                {
                    v3.Components.Add(j, v2.Components[j]);
                }
            }
            return SparseVector.RemoveZeros(v3);
        }

        public void Add(SparseVector v1)
        {
            if (this.Dimension != v1.Dimension)
                throw new ArgumentException("v1 and v2 have different dimensions.");

            // v3 <- v1 + v2
            foreach (int j in v1.Components.Keys)
            {
                if (this.Components.ContainsKey(j))
                {
                    this.Components[j] += v1.Components[j];
                }
                else
                {
                    this.Components.Add(j, v1.Components[j]);
                }
            }
            SparseVector.RemoveZeros(this);
        }

        /// <summary>
        /// Addition (v3 = v1 + v2)
        /// </summary>
        /// <returns></returns>
        public static SparseVector operator -(SparseVector v1, SparseVector v2)
        {
            if (v1.Dimension != v2.Dimension)
                throw new ArgumentException("v1 and v2 have different dimensions.");

            SparseVector v3 = new SparseVector(v1.Dimension);
            // v3 <- v1
            foreach (int i in v1.Components.Keys)
            {
                v3.Components.Add(i, v1.Components[i]);
            }
            // v3 <- v1 - v2
            foreach (int j in v2.Components.Keys)
            {
                if (v3.Components.ContainsKey(j))
                {
                    v3.Components[j] -= v2.Components[j];
                }
                else
                {
                    v3.Components.Add(j, -v2.Components[j]);
                }
            }
            return SparseVector.RemoveZeros(v3);
        }

        /// <summary>
        /// Negation (v2 = -v1)
        /// </summary>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static SparseVector operator -(SparseVector v1)
        {
            SparseVector v2 = new SparseVector(v1.Dimension);

            foreach (int i in v1.Components.Keys)
            {
                v2.Components.Add(i, -v1.Components[i]);
            }
            return v2;
        }

        /// <summary>
        /// Less-than (result = v1 < v2)
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool operator <(SparseVector v1, SparseVector v2)
        {
            return v1.Magnitude < v2.Magnitude;
        }

        /// <summary>
        /// Less-than or Equal-to (result = v1 <= v2)
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool operator <=(SparseVector v1, SparseVector v2)
        {
            return v1.Magnitude <= v2.Magnitude;
        }

        /// <summary>
        /// Greater-than (result = v1 > v2)
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool operator >(SparseVector v1, SparseVector v2)
        {
            return v1.Magnitude > v2.Magnitude;
        }

        /// <summary>
        /// Greater-than or Equal-to (result = v1 >= v2)
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool operator >=(SparseVector v1, SparseVector v2)
        {
            return v1.Magnitude >= v2.Magnitude;
        }

        /// <summary>
        /// Equality (result = v1 == v2)
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool operator ==(SparseVector v1, SparseVector v2)
        {
            // another option is returing false.
            if (v1.Dimension != v2.Dimension)
                throw new ArgumentException("v1 and v2 have different dimensions.");

            if (v1.NumberOfElements != v2.NumberOfElements)
                return false;

            foreach (int i in v1.Components.Keys)
            {
                if (!v2.Components.ContainsKey(i)
                    || Math.Abs(v1.Components[i] - v2.Components[i]) > MinDouble)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Inequality (result = v1 != v2)
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool operator !=(SparseVector v1, SparseVector v2)
        {
            return !(v1 == v2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return
            (
               (int)(SumComponents() % Int32.MaxValue)
            );
        }

        /// <summary>
        /// Division (v3 = v1 / s2)
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static SparseVector operator /(SparseVector v1, double s2)
        {
            SparseVector v3 = new SparseVector(v1.Dimension);

            foreach (int i in v1.Components.Keys)
            {
                v3.Components.Add(i, v1.Components[i] / s2);
            }
            return v3;
        }

        /// <summary>
        /// Multiplication by a scalar (v3 = v1 * s2) 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static SparseVector operator *(SparseVector v1, double s2)
        {
            SparseVector v3 = new SparseVector(v1.Dimension);

            foreach (int i in v1.Components.Keys)
            {
                v3.Components.Add(i, v1.Components[i] * s2);
            }
            return v3;
        }

        /// <summary>
        /// The order of operands in multiplication can be reversed; this is known as being commutable.
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static SparseVector operator *(double s1, SparseVector v2)
        {
            return v2 * s1;
        }

        /// <summary>
        /// Dot product (s3 = v1 . v2) 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static double DotProduct(SparseVector v1, SparseVector v2)
        {
            if (v1.Dimension != v2.Dimension)
                throw new ArgumentException("v1 and v2 have different dimensions.");

            double s3 = 0.0;

            foreach (int i in v1.Components.Keys)
            {
                if (v2.Components.ContainsKey(i))
                {
                    s3 += v1.Components[i] * v2.Components[i];
                }
            }

            return s3;
        }

        //And its counterpart:

        public double DotProduct(SparseVector other)
        {
            return DotProduct(this, other);
        }

        /// <summary>
        /// Sum
        /// </summary>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double SumComponents(SparseVector v1)
        {
            double sum = 0.0;
            foreach (int i in v1.Components.Keys)
            {
                sum += v1.Components[i];
            }
            return sum;
        }

        public double SumComponents()
        {
            return SumComponents(this);
        }

        /// <summary>
        /// Sum of squares
        /// </summary>
        /// <param name="v1"></param>
        /// <returns></returns>
        public double SumComponentSqrs(SparseVector v1)
        {
            double sum = 0.0;
            foreach (int i in v1.Components.Keys)
            {
                sum += v1.Components[i] * v1.Components[i];
            }
            return sum;
        }

        public double SumComponentSqrs()
        {
            return SumComponentSqrs(this);
        }

        public override bool Equals(object other)
        {
            // Check object other is a Vector3 object
            if (other is SparseVector)
            {
                // Convert object to Vector3
                SparseVector otherVector = (SparseVector)other;

                // Check for equality
                return otherVector == this;
            }
            else
            {
                return false;
            }
        }

        public bool Equals(SparseVector other)
        {
            return other == this;
        }

        /// <summary>
        /// This method finds the distance between two positional vectors using Pythagoras theorem.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static double Distance(SparseVector v1, SparseVector v2)
        {
            if (v1.Dimension != v2.Dimension)
                throw new ArgumentException("v1 and v2 have different dimensions.");

            return Math.Sqrt(((SparseVector)(v1 - v2)).SumComponentSqrs());
        }

        public double Distance(SparseVector other)
        {
            return Distance(this, other);
        }

        /// <summary>
        /// Comparison method for two vectors which returns: 
        /// 
        /// -1 if the magnitude is less than the others magnitude 
        /// 0 if the magnitude equals the magnitude of the other 
        /// 1 if the magnitude is greater than the magnitude of the other 
        /// 
        /// This allows the Vector type to implement the IComparable and IComparable<Vector3> interfaces.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(object other)
        {
            if (other is SparseVector)
            {
                SparseVector otherVector = (SparseVector)other;

                if (this < otherVector) { return -1; }
                else if (this > otherVector) { return 1; }

                return 0;
            }
            else
            {
                // Error condition: other is not a Vector object
                throw new ArgumentException("other is not a Vector object");
            }
        }

        public int CompareTo(SparseVector other)
        {
            if (this < other)
            {
                return -1;
            }
            else if (this > other)
            {
                return 1;
            }

            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static SparseVector Normalize(SparseVector v1)
        {
            // Check for divide by zero errors
            if (v1.Magnitude == 0)
            {
                throw new DivideByZeroException("Can not normalize a vector when it's magnitude is zero");
            }
            else
            {
                // find the inverse of the vectors magnitude
                double inverse = 1 / v1.Magnitude;
                int d = v1.Dimension;
                SparseVector v3 = new SparseVector(d);

                for (int i = 0; i < d; i++)
                {
                    v3[i] = v1[i] * inverse;
                }
                return v3;
            }
        }

        public void Normalize()
        {
            // Check for divide by zero errors
            if (this.Magnitude == 0)
            {
                throw new DivideByZeroException("Can not normalize a vector when it's magnitude is zero");
            }
            else
            {
                Collection<int> changeList = new Collection<int>();
                foreach (int i in this.Components.Keys)
                {
                    changeList.Add(i);
                }

                // find the inverse of the vectors magnitude
                double inverse = 1 / this.Magnitude;

                foreach (int i in changeList)
                {
                    this[i] = this[i] * inverse;
                }
            }
        }

        // foamliu, 2008/01/13, fix an unit test bug.
        public static SparseVector RemoveZeros(SparseVector v)
        {
            List<int> dellist = new List<int>();
            foreach (int i in v.Components.Keys)
            {
                if (Math.Abs(v[i]) < MinDouble)
                    dellist.Add(i);
            }

            foreach (int i in dellist)
            {
                v.Components.Remove(i);
            }

            return v;
        }

        #endregion

    }
}
