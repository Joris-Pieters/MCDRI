using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MC.Other;

namespace MC.Python
{
    [Serializable]
    public class PythonCode
    {
        public string Script;
        public List<PythonVariable> OutputVariables;

        public PythonCode()
        {
        }

        public PythonCode(string script, PythonVariable outputVariable)
        {
            Script = script;
            OutputVariables = new List<PythonVariable>();
            OutputVariables.Add(outputVariable);
        }

        public PythonCode(string script, List<PythonVariable> outputVariables)
        {
            Script = script;
            OutputVariables = outputVariables;
        }
    }
    
}
