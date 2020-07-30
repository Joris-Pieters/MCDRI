using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using MC;
using MC.Other;

namespace MC.Testing
{
    public class UndoManager
    {
        public DropOutStack<Test> UndoStack = new DropOutStack<Test>(Program.UserSettings.numberOfUndo);
        public DropOutStack<Test> RedoStack = new DropOutStack<Test>(Program.UserSettings.numberOfUndo);

        public UndoManager()
        {
        }

        public void Update()
        {            
            UndoStack.Push(Program.Test.Copy());
            RedoStack.Clear();
        }

        public void Reset()
        {
            UndoStack.Clear();
            RedoStack.Clear();
        }
    }
}
