using CodeConverterCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeConverterCore.Model
{
    public class ProjectInformation
    {
        public ProjectInformation FillClasses(List<ClassContainer> inClasses)
        {
            ClassList.AddRange(inClasses);
            foreach (var tmpClass in inClasses)
            {
                AddClass(tmpClass);
            }
            return this;
        }

        public List<ClassContainer> ClassList = new List<ClassContainer>();

        /// <summary>
        /// All Known Types(Interface, Class) with the spezific Namespace they are in
        /// </summary>
        public Dictionary<string, List<BaseType>> KnownTypeDictionary = new Dictionary<string, List<BaseType>>();

        /// <summary>
        /// ObjectName, Namespaces
        /// </summary>
        private Dictionary<string, List<ClassContainer>> ClassDict = new Dictionary<string, List<ClassContainer>>();

        public string MigrationHelperNamespace { get; set; } = "Migration";

        public string MigrationHelperStaticClass { get; set; } = "MigrationHelper";

        /// <summary>
        /// List of Methodes, that are Missing
        /// </summary>
        public List<MissingFunctionInformation> MissingMethodes { get; set; } = new List<MissingFunctionInformation>();

        /// <summary>
        /// Add Class to InformationList
        /// </summary>
        /// <param name="inClass"></param>
        private void AddClass(ClassContainer inClass)
        {
            AddToDictList(ClassDict, inClass, inItem => inItem.Name, inItem => inItem);
        }
        /// <summary>
        /// Add Class to InformationList
        /// </summary>
        /// <param name="inClass"></param>
        public ClassContainer GetClassForType(string inType, List<string> inNamespaces)
        {
            if (ClassDict.TryGetValue(inType, out var tmpClasses))
            {
                var tmpResult = tmpClasses.FirstOrDefault(inItem => inNamespaces.Contains(inItem.Namespace))
                    ?? tmpClasses.FirstOrDefault(inItem => inNamespaces.Contains(inItem.Namespace + "." + inItem.Name))
                    ?? tmpClasses.FirstOrDefault(inItem => inItem.Namespace == "system");
                return tmpResult;
            }
            return null;
        }

        /// <summary>
        /// Information einem Dictionary mit Liste anfügen
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TIn"></typeparam>
        /// <param name="inDict"></param>
        /// <param name="inObject"></param>
        /// <param name="inKeySelector"></param>
        /// <param name="inSelector"></param>
        private void AddToDictList<T, TIn>(Dictionary<string, List<T>> inDict, TIn inObject, Func<TIn, string> inKeySelector, Func<TIn, T> inSelector)
        {
            if (!inDict.TryGetValue(inKeySelector(inObject), out var tmpDataList))
            {
                tmpDataList = new List<T>();
                inDict.Add(inKeySelector(inObject), tmpDataList);
            }
            tmpDataList.Add(inSelector(inObject));
        }

        /// <summary>
        /// List of Missing Classes
        /// </summary>
        public List<string> MissingClassList = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inType"></param>
        /// <returns>null if no Matching class is found</returns>
        public ClassContainer ClassForNameAndNamespaces(string inKey, List<string> inNamespaceList)
        {
            if (!ClassDict.TryGetValue(inKey, out var tmpClassList)){
                return null;
            }
            return tmpClassList.FirstOrDefault(inItem => inNamespaceList.Contains(inItem.Namespace));
        }

        /// <summary>
        /// Return Class for the known BaseType
        /// </summary>
        /// <param name="inType"></param>
        /// <returns>null if no Matching class is found</returns>
        public ClassContainer ClassFromBaseType(TypeContainer inType)
        {
            if (inType.Type == null)
            {
                inType.Type = new BaseType(inType.Name);
            }
            if (!ClassDict.TryGetValue(inType.Name, out var tmpClassList)){
                return null;
            }
            return tmpClassList.FirstOrDefault(inItem => inItem.Type.Type == inType.Type);
        }

        private Dictionary<string, ClassContainer> _systemAlias = new Dictionary<string, ClassContainer>();

        /// <summary>
        /// Add Unknonw Class to Project
        /// </summary>
        /// <param name="inClass"></param>
        public void AddAlias(string inName, ClassContainer inContainer)
        {
            _systemAlias.Add(inName, inContainer);
        }


        /// <summary>
        /// List of not known types
        /// </summary>
        private List<UnknownTypeClass> _UnknownTypeClassList = new List<UnknownTypeClass>();

        /// <summary>
        /// Get Unknown Type for Key and Namespace
        /// </summary>
        /// <param name="inType"></param>
        /// <returns>null if no Matching class is found</returns>
        public UnknownTypeClass UnknownClassForNameAndNamespaces(string inKey, List<string> inNamespaceList)
        {
            return _UnknownTypeClassList.FirstOrDefault(inItem => inItem.Type.Name == inKey
            && inItem.PossibleNamespace.Any(inNamespace => inNamespaceList.Contains(inNamespace)));
        }

        /// <summary>
        /// Add Unknonw Class to Project
        /// </summary>
        /// <param name="inClass"></param>
        public void AddUnknownClass(UnknownTypeClass inClass)
        {
            _UnknownTypeClassList.Add(inClass);
        }

        /// <summary>
        /// Get all Unknown Types from the Analyzer to be used later
        /// </summary>
        /// <returns></returns>
        public List<UnknownTypeClass> GetAllUnknownTypes()
        {
            var tmpList=new List<UnknownTypeClass>();
            tmpList.AddRange(_UnknownTypeClassList);
            return tmpList;
        }
    }
}
