using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MC.Other;

namespace MC.Python
{
    [Serializable]
    public class PythonVariable
    {
        public string Name;
        public Fraction Value = new Fraction();

        public PythonVariable()
        {
        }

        public PythonVariable(string name)
        {
            Name = name;
        }

        public PythonVariable(string name, Fraction value)
        {
            Name = name;
            Value = value;
        }
    }
}
