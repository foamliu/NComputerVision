

namespace NComputerVision.Common
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// 堆
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NcvBinaryHeap<T>
    {
        private ArrayList m_list;
        private IComparer<T> m_comparer;

        public IComparer<T> Comparer
        {
            set { this.m_comparer = value; }
        }

        public NcvBinaryHeap()
        {
            this.m_list = new ArrayList();
        }

        public NcvBinaryHeap(int capacity)
        {
            this.m_list = new ArrayList(capacity);
        }

        public NcvBinaryHeap(IComparer<T> comparer)
        {
            this.m_comparer = comparer;
            this.m_list = new ArrayList();
        }

        public virtual void Clear()
        {
            m_list.Clear();
        }

        public virtual void Insert(T value)
        {
            int pos = m_list.Add(value);
            if (pos == 0) return;

            while (pos > 0)
            {
                int nextPos = pos / 2;

                T toCheck = (T)m_list[nextPos];

                if (m_comparer.Compare(value, toCheck) > 0)
                {
                    m_list[pos] = toCheck;
                    pos = nextPos;
                }
                else
                    break;
            }

            m_list[pos] = value;

        }

        public virtual T Remove()
        {
            if (m_list.Count == 0)
                return default(T);
            T toReturn = (T)m_list[0];

            m_list.RemoveAt(0);

            if (m_list.Count > 1)
            {
                m_list.Insert(0, m_list[m_list.Count - 1]);
                m_list.RemoveAt(m_list.Count - 1);

                int current = 0, possibleSwap = 0;

                while (true)
                {
                    int leftChildPos = 2 * current + 1;
                    int rightChildPos = leftChildPos + 1;

                    if (leftChildPos < m_list.Count)
                    {
                        T entry1 = (T)m_list[current];
                        T entry2 = (T)m_list[leftChildPos];

                        if (m_comparer.Compare(entry2, entry1) > 0)
                            possibleSwap = leftChildPos;
                    }
                    else
                        break;

                    if (rightChildPos < m_list.Count)
                    {
                        T entry1 = (T)m_list[possibleSwap];
                        T entry2 = (T)m_list[rightChildPos];

                        if (m_comparer.Compare(entry2, entry1) > 0)
                            possibleSwap = rightChildPos;
                    }

                    if (current != possibleSwap)
                    {
                        object temp = m_list[current];
                        m_list[current] = m_list[possibleSwap];
                        m_list[possibleSwap] = temp;
                    }
                    else
                        break;

                    current = possibleSwap;
                }
            }

            return toReturn;

        }

        public virtual int Count
        {
            get
            {
                return m_list.Count;
            }
        }


    }
}
