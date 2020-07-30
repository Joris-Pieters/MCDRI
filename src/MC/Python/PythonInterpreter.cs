using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using IronPython;
using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using MC.Other;
using MC.Design;
using MC.Testing;

namespace MC.Python
{
    class PythonInterpreter
    {
        Dictionary<String, Object> options;
        public ScriptEngine engine;
        ScriptScope scope;
        PythonCode code;
        ScriptSource source;

        TextBoxWriter outputWriter = new TextBoxWriter();

        public PythonInterpreter()
        {
            options = new Dictionary<string, object>();
            options["DivisionOptions"] = PythonDivisionOptions.New;
            engine = IronPython.Hosting.Python.CreateEngine(options);
            engine.Runtime.IO.RedirectToConsole();
            Console.SetOut(outputWriter);
            scope = engine.CreateScope();
        }

        /// <summary>
        /// Do this only once per correction, not for each subject separatly (would be much slower)
        /// </summary>        
        public void Initialize(PythonCode code)
        {
            this.code = code;                        
            source = engine.CreateScriptSourceFromString(code.Script);
        }

        /// <summary>
        /// For each subject
        /// </summary>
        public List<PythonVariable> Score(CorrectedSubject subject)
        {          
            // Set all output variables to zero, otherwise you can add up to previous value
            for (int i = 0; i < code.OutputVariables.Count; i++)
            {
                scope.SetVariable(code.OutputVariables[i].Name, code.OutputVariables[i].Value.ToDouble());
            }

            for (int j = 0; j < subject.ItemCollections.Count; j++)
            {
                var setScope = new byte[subject.ItemCollections[j].Items.Count][];
                for (int k = 0; k < subject.ItemCollections[j].Items.Count; k++)
                {
                    var itemScope = new byte[subject.ItemCollections[j].Alternatives.Count];
                    for (int l = 0; l < subject.ItemCollections[j].Alternatives.Count; l++)
                    {
                        itemScope[l] = subject.ItemCollections[j].Items[k].Checked[l] == ItemCheckedState.Checked ? (byte)1 : (byte)0;
                    }
                    setScope[k] = itemScope;
                }
                scope.SetVariable(subject.ItemCollections[j].Name, setScope);
            }
           
            source.Execute(scope);

            var outList = new List<PythonVariable>();
            for (int i = 0; i < code.OutputVariables.Count; i++)
            {
                outList.Add(new PythonVariable(code.OutputVariables[i].Name, new Fraction(scope.GetVariable<double>(code.OutputVariables[i].Name))));
            }
            return outList;
        }
    }

    class TextBoxWriter : TextWriter
    {
        StringBuilder str = new StringBuilder();

        public override void Write(string str)
        {
            Logger.LogHigh(str);
        }

        public override System.Text.Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }
    }  
}

