
using System.Collections.ObjectModel;

namespace NComputerVision.Learning
{
    public class Problem
    {
        public int Dimension { get; set; }
        public Collection<Category> CategoryCollection { get; set; }
        public Collection<Example> TrainingSet { get; set; }
        public Collection<Example> ValidationSet { get; set; } 
    }
}
