using CodeConverterCore.Analyzer;
using CodeConverterCore.ImportExport;
using CodeConverterCore.Interface;
using CodeConverterCore.Model;
using System.Collections.Generic;
using System.Linq;
using CodeConverterCore.Converter;

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
        public static ClassContainer GetClassOrUnknownForType(this ProjectInformation inProject, string inClassName, ClassContainer inParentClass)
        {
            if (string.IsNullOrEmpty(inClassName))
            {
                return null;
            }
            var tmpClass = inProject.ClassForNameAndNamespaces(inClassName, inParentClass.FullUsingList);
            if (tmpClass != null)
            {
                return tmpClass;
            }
            if (inProject.GetAliasType(inClassName) != null)
            {
                return inProject.GetAliasType(inClassName);
            }

            var tmpGenericType = inParentClass.Type.GenericTypes.FirstOrDefault(inItem => inItem.Name == inClassName);
            if (tmpGenericType != null)
            {
                return new ClassContainer
                {
                    Type = tmpGenericType,
                    Parent = inProject
                };
            }

            var tmpParentClass = inParentClass;
            while (tmpParentClass != null)
            {
                var tmpField = tmpParentClass.FieldList.FirstOrDefault(inItem => inItem.Name == inClassName);
                if (tmpField != null)
                {
                    return new ClassContainer
                    {
                        Type = tmpField.Type,
                        Parent = inProject
                    };
                }
                tmpParentClass = tmpParentClass.GetParentClass();
            }

            var tmpUnknown = inProject.UnknownClassForNameAndNamespaces(inClassName, inParentClass.FullUsingList);
            if (tmpUnknown == null)
            {
                tmpUnknown = new UnknownTypeClass(inClassName)
                {
                    PossibleNamespace = inParentClass.FullUsingList,
                    Parent = inProject
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
                tmpProject.AddAlias(tmpAlias.To, tmpProject.GetClassOrUnknownForType(tmpAlias.From, new ClassContainer { Namespace = inSystemNamespace }));
            }

            return tmpProject;
        }

        /// <summary>
        /// Create Project from system known data
        /// </summary>
        /// <param name="inProject"></param>
        /// <param name="inAliasList"></param>
        /// <param name="inSystemNamespace"></param>
        public static void MapLanguageNames(ProjectInformation inProjectInfo, List<LanguageMappingObject> inLanguageMapping)
        {
            var tmpMappingByFullName = new Dictionary<string, List<LanguageMappingObject>>();
            foreach (var tmpMap in inLanguageMapping)
            {
                tmpMappingByFullName.Add(tmpMap.Source.Namespace + "." + tmpMap.Source.ClassName, tmpMap);
            }

            foreach (var tmpClass in inProjectInfo.ClassList)
            {
                if (tmpMappingByFullName.TryGetValue(tmpClass.Namespace + "." + tmpClass.Name, out var tmpMapList))
                {
                    //If there is a mapping for a spezific Class, we need to apply it
                    foreach (var tmpMap in tmpMapList)
                    {
                        if (tmpMap.Source.IsMethodeMapping())
                        {
                            //Apply all Methode renamings that need to be applied
                            foreach (var tmpMethode in tmpClass.MethodeList)
                            {
                                if (tmpMethode.Name == tmpMap.Source.MethodeName)
                                {
                                    tmpMethode.Name = tmpMap.Target.MethodeName;
                                    tmpMethode.IsProperty = tmpMap.Target.MethodeAsProperty;
                                }
                            }
                        }
                        else if (tmpMap.Source.IsClassMapping())
                        {
                            tmpClass.Namespace = tmpMap.Target.Namespace;
                            tmpClass.Type.Type.Name = tmpMap.Target.ClassName;
                            tmpClass.Type.Name = tmpMap.Target.ClassName;
                            inProjectInfo.AddToDictionaryWithNewName(tmpClass);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Create Project from system known data
        /// </summary>
        /// <param name="inProject"></param>
        /// <param name="inAliasList"></param>
        /// <param name="inSystemNamespace"></param>
        public static ProjectInformation DoFullRun(List<LanguageMappingObject> inLanguageMapping, IConverter inConverter, ILoadOOPLanguage inLanguageLoader, params string[] inClassStringData)
        {
            var tmpProject = inLanguageLoader.CreateObjectInformation(inClassStringData.ToList(), null);
            foreach (var tmpClass in tmpProject.ClassList)
            {
                if (!tmpClass.UsingList.Contains("java.lang"))
                {
                    tmpClass.UsingList.Add("java.lang");
                }
            }

            new AnalyzerCore().LinkProjectInformation(tmpProject);

            new NamingConvertionHelper(inConverter).ConvertProject(tmpProject);

            MapLanguageNames(tmpProject, inLanguageMapping);

            foreach (var tmpCurrentClass in tmpProject.ClassList.Where(inItem => inItem.ClassType == Enum.ClassTypeEnum.Normal))
            {
                inConverter.AnalyzerClassModifier(tmpCurrentClass);
            }

            return tmpProject;
        }
    }
}
