using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NComputerVision.Learning
{
    using System;    

    [Serializable()]
    public class Example
    {
        #region Fields

        private string m_id;
        private SparseVector m_X;
        private Category m_Label;

        // foamliu, 2009/01/03, for SVM.
        private bool m_isSV;
        #endregion

        #region Properties
        public string ID
        {
            get
            {
                return m_id;
            }
            set
            {
                m_id = value;
            }
        }
        public SparseVector X
        {
            get
            {
                return m_X;
            }
            set
            {
                m_X = value;
            }
        }
        public Category Label
        {
            get
            {
                return m_Label;
            }
            set
            {
                m_Label = value;
            }
        }

        // foamliu, 2009/01/03, for SVM.
        public bool IsSV
        {
            get
            {
                return m_isSV;
            }
            set
            {
                m_isSV = value;
            }
        }
        #endregion

        #region Methods

        #endregion

        #region Constructors

        public Example(Category c)
        {
            this.m_Label = c;
        }

        public Example()
        {

        }

        #endregion

    }
}
