using System;

namespace NCV.ML
{
    [Serializable]
    public class Node : IComparable<Node>
    {
        internal int _index;
        internal double _value;

        public Node()
        {
        }

        public Node(int index, double value)
        {
            _index = index;
            _value = value;
        }

        public int Index
        {
            get
            {
                return _index;
            }
            set
            {
                _index = value;
            }
        }

        public double Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", _index, _value);
        }

        #region IComparable<Node> Members

        public int CompareTo(Node other)
        {
            return _index.CompareTo(other._index);
        }

        #endregion
    }
}
