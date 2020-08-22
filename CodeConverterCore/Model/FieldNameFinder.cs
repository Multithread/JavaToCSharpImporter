using System.Collections.Generic;
using System.Security.Permissions;

namespace CodeConverterCore.Model
{
    public class FieldNameFinder
    {
        public FieldNameFinder() { }

        public FieldNameFinder(ClassContainer inClass)
        {
            Class = inClass;
        }

        public FieldNameFinder(FieldNameFinder inParentFinder)
        {
            Class = inParentFinder.Class;
            VariableList = inParentFinder.VariableList;
            VariableStack = inParentFinder.VariableStack;
            MethodeVarList = inParentFinder.MethodeVarList;
        }

        public ClassContainer Class { get; set; }

        public List<VariableDeclaration> VariableList { get; set; } = new List<VariableDeclaration>();

        private Stack<List<VariableDeclaration>> VariableStack = new Stack<List<VariableDeclaration>>();

        private List<VariableDeclaration> MethodeVarList;

        public List<VariableDeclaration> GetMethodeVariableList()
        {
            if (MethodeVarList != null)
            {
                return MethodeVarList;
            }
            return VariableList;
        }

        public void StackVariables(bool inCopyCurrentList = false, bool inMethodeStack = false)
        {
            var tmpVarList = new List<VariableDeclaration>(VariableList);
            VariableStack.Push(tmpVarList);
            if (inMethodeStack && tmpVarList.Count > 0)
            {
                MethodeVarList = tmpVarList;
            }
            if (!inCopyCurrentList)
            {
                VariableList.Clear();
            }
        }

        public void UnstackVariableList()
        {
            VariableList = VariableStack.Pop();
            if (MethodeVarList == VariableList)
            {
                MethodeVarList = null;
            }
        }
    }
}
