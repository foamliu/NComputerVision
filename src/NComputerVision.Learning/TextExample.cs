using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NComputerVision.Learning
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;    
    using System.Text.RegularExpressions;

    public class TextExample : Example
    {
        #region Fields

        private string m_Text;
        private SortedDictionary<String, int> m_Tokens;

        #endregion

        #region Properties

        public string Text
        {
            get
            {
                return m_Text;
            }
            set
            {
                m_Text = value;
            }
        }
        public SortedDictionary<String, int> Tokens
        {
            get
            {
                return m_Tokens;
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Break down text into tokens
        /// </summary>
        public void BreakDownEnUs()
        {
            string pattern = @"([a-zA-Z]\w+)\W*";
            Regex re = new Regex(pattern, RegexOptions.Compiled);
            Match m = re.Match(Text);
            while (m.Success)
            {
                string token = m.Groups[1].Value;
                Utility.AddToken(m_Tokens, token);
                m = m.NextMatch();
            }
        }

        public void BreakDownZhCn()
        {
            for (int i = 0; i < this.Text.Length; i++)
            {
                string token = this.Text[i].ToString().Trim();
                if (token.Length > 0)
                    Utility.AddToken(m_Tokens, token);
            }
        }

        #endregion

        #region Constructors

        public TextExample(String t, Category c)
            : base(c)
        {
            this.m_Text = t;
            this.m_Tokens = new SortedDictionary<string, int>();
        }

        public TextExample()
            : base()
        {
            this.m_Tokens = new SortedDictionary<string, int>();
        }

        #endregion
    }

    class Utility
    {
        public static void AddToken(SortedDictionary<String, int> tokens, string token)
        {
            if (!tokens.ContainsKey(token))
            {
                tokens.Add(token, 1);
            }
            else
            {
                tokens[token]++;
            }
        }

        public static void AddToken(SortedDictionary<String, int> tokens, string token, int count)
        {
            if (!tokens.ContainsKey(token))
            {
                tokens.Add(token, count);
            }
            else
            {
                tokens[token] += count;
            }
        }
    }
}
