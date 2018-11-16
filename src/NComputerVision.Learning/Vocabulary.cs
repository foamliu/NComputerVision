
namespace NComputerVision.Learning
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;    

    /// <summary>
    /// foamliu, 2009/01/13, add comments.    
    /// </summary>
    [Serializable()]
    public class Vocabulary : ISerializable
    {
        #region Fields

        // 词包, 包是一个允许出现重复元素的集合, 所以它不仅考虑词的出现, 而且还考虑词的频率, 词的顺序和标点符号则被忽略.

        // 词 -> 出现次数
        private SortedDictionary<String, int> m_WordBag;
        // 词 -> 在词典中的位置
        private SortedDictionary<String, int> m_WordPositionMap = new SortedDictionary<string, int>();
        // example count which contains the word
        // 词 -> 包含这个词的样例数
        private SortedDictionary<String, int> m_WordExampleOccurMap = new SortedDictionary<string, int>();
        #endregion

        #region Properties
        public SortedDictionary<String, int> WordBag
        {
            get
            {
                return m_WordBag;
            }
            set
            {
                m_WordBag = value;
            }
        }
        public int Count
        {
            get
            {
                return m_WordBag.Count;
            }
        }
        public SortedDictionary<String, int> WordPositionMap
        {
            get
            {
                return m_WordPositionMap;
            }
        }
        public SortedDictionary<String, int> WordExampleOccurMap
        {
            get
            {
                return m_WordExampleOccurMap;
            }
        }

        #endregion

        #region Methods

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            int i = 0;

            info.AddValue("Size", m_WordBag.Count);
            foreach (string word in m_WordBag.Keys)
            {
                info.AddValue("Word" + i, word);
                info.AddValue("Count" + i, m_WordBag[word]);
                info.AddValue("ExampleOccur" + i, m_WordExampleOccurMap[word]);
                i++;
            }
        }

        public int GetWordCount(string word)
        {
            if (m_WordBag.ContainsKey(word))
            {
                return m_WordBag[word];
            }
            else
                return 0;
        }

        public int GetWordPosition(string word)
        {
            if (m_WordBag.ContainsKey(word))
            {
                return m_WordPositionMap[word];
            }
            else
                return Constants.KEY_NOT_FOUND;
        }

        public void CalculateWordPositionMap()
        {
            int pos = 0;

            foreach (string word in m_WordBag.Keys)
            {
                m_WordPositionMap.Add(word, pos++);
            }
        }


        #endregion

        #region Constructors

        public Vocabulary()
        {
        }

        public Vocabulary(SortedDictionary<String, int> c)
        {
            m_WordBag = c;

            this.CalculateWordPositionMap();
        }

        public Vocabulary(SerializationInfo info, StreamingContext ctxt)
        {
            m_WordBag = new SortedDictionary<string, int>();
            string word;
            int count;
            int exampleOccur;

            int size = info.GetInt32("Size");
            for (int i = 0; i < size; i++)
            {
                word = info.GetString("Word" + i);
                count = info.GetInt32("Count" + i);
                exampleOccur = info.GetInt32("ExampleOccur" + i);

                m_WordBag.Add(word, count);
                m_WordExampleOccurMap.Add(word, exampleOccur);
            }

            this.CalculateWordPositionMap();
        }

        #endregion
    }
}
