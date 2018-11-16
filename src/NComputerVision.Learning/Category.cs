
namespace NComputerVision.Learning
{
    using System;
    using System.Collections.Generic;

    [Serializable()]
    public class Category : IComparable<Category>
    {
        #region Fields
        [NonSerialized]
        private List<Example> examples;

        #endregion

        #region Properties
        public int Id
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public List<Example> Examples
        {
            get { return examples; }
        }

        #endregion

        #region Methods
        public int CompareTo(Category other)
        {
            if (this.Id < other.Id)
            {
                return -1;
            }
            else if (this.Id > other.Id)
            {
                return 1;
            }

            return 0;
        }
        #endregion

        #region Constructors

        public Category(int c, string n)
        {
            this.Id = c;
            this.Name = n;
            examples = new List<Example>();
        }

        #endregion
    }
}
