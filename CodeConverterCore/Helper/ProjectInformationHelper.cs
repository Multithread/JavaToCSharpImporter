using CodeConverterCore.Model;
using System.Collections.Generic;

namespace CodeConverterCore.Helper
{
    public static class ProjectInformationHelper
    {
        /// <summary>
        /// Find or Create Class for Name and Using-List
        /// </summary>
        /// <param name="inProject"></param>
        /// <param name="inClassName"></param>
        /// <param name="inPossibleNamespaces"></param>
        /// <returns></returns>
        public static ClassContainer GetClassOrUnknownForType(this ProjectInformation inProject, string inClassName, List<string> inPossibleNamespaces)
        {
            var tmpClass = inProject.ClassForNameAndNamespaces(inClassName, inPossibleNamespaces);
            if (tmpClass != null)
            {
                return tmpClass;
            }
            var tmpUnknown = inProject.UnknownClassForNameAndNamespaces(inClassName, inPossibleNamespaces);
            if (tmpUnknown == null)
            {
                tmpUnknown = new UnknownTypeClass(inClassName)
                {
                    PossibleNamespace = inPossibleNamespaces
                };
                inProject.AddUnknownClass(tmpUnknown);
            }
            return tmpUnknown;
        }
    }
}
