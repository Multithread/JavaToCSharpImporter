using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeConverterCore.Model
{
    public class FieldNameFinder
    {
        public FieldNameFinder() { }

        public FieldNameFinder(ClassContainer inClass)
        {
            Class = inClass;
        }

        public ClassContainer Class { get; set; }

        public List<VariableDeclaration> VariableList { get; set; }

    }
}
