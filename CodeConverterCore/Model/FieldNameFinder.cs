using System.Collections.Generic;

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
        }

        public ClassContainer Class { get; set; }

        public List<VariableDeclaration> VariableList { get; set; }

    }
}
