using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace MC.Graphical
{
    public class ColorDistribution
    {
        public List<double> Values;

        public ColorDistribution()
        {
            this.Values = new List<double>();
        }

        public ColorDistribution(List<double> values)
        {
            this.Values = values;
        }

        public void Add(double value)
        {
            this.Values.Add(value);
        }

        public void Add(List<double> values)
        {
            foreach (var value in values)
            {
                this.Values.Add(value);
            }
        }

        public double Mean()
        {
            if (Values.Count > 0)
            {
                double sum = 0;
                for (int i = 0; i < Values.Count; i++)
                {
                    sum += Values[i];
                }
                return sum / Values.Count;
            }
            else
            {
                return 0;
            }
        }

        public double Variance()
        {
            if (Values.Count > 0)
            {
                double sum = 0;
                for (int i = 0; i < Values.Count; i++)
                {
                    sum += Values[i] * Values[i];
                }
                var m = Mean();
                return (sum / Values.Count) - (m * m);
            }
            else
            {
                return 0;
            }
        }

        public double StandardDeviation()
        {
            return Math.Sqrt(Variance());
        }
        
    }
}
