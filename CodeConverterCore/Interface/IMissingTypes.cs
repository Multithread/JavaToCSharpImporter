using CodeConverterCore.Model;
using System.Collections.Generic;

namespace CodeConverterCore.Interface
{
    public interface IMissingTypes
    {
        /// <summary>
        /// Add a Missing Class
        /// </summary>
        void AddMissingClass(string inTypeName);

        /// <summary>
        /// Add missing Methode to Class Lsit
        /// </summary>
        void AddMissingMethode(string inMethodeName, TypeContainer inClassType, List<TypeContainer> inParamList, string inOutType);

    }
}
