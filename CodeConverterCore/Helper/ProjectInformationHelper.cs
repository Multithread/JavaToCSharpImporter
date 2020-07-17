using CodeConverterCore.ImportExport;
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
            if (string.IsNullOrEmpty(inClassName))
            {
                return null;
            }
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

        /// <summary>
        /// Create Project from system known data
        /// </summary>
        /// <param name="inProject"></param>
        /// <param name="inAliasList"></param>
        /// <param name="inSystemNamespace"></param>
        public static ProjectInformation CreateSystemProjectInformation(List<ClassContainer> inSystemClassList, List<AliasObject> inAliasList, string inSystemNamespace)
        {
            var tmpProject = new ProjectInformation();
            tmpProject.FillClasses(inSystemClassList);

            foreach (var tmpAlias in inAliasList)
            {
                tmpProject.AddAlias(tmpAlias.To, tmpProject.GetClassOrUnknownForType(tmpAlias.From, new List<string> { inSystemNamespace }));
            }

            return tmpProject;
        }
    }
}
