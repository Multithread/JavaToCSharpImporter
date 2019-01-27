using System;
using System.Collections.Generic;
using System.Linq;

namespace JavaToCSharpConverter.Model
{
    public class ObjectInformation
    {
        public ObjectInformation FillClasses(List<ClassContainer> inClasses)
        {
            foreach (var tmpClass in inClasses)
            {
                AddClass(tmpClass);
            }
            return this;
        }

        /// <summary>
        /// ObjectName, Namespaces
        /// </summary>
        private Dictionary<string, List<ClassContainer>> ClassDict = new Dictionary<string, List<ClassContainer>>();

        /// <summary>
        /// Add Class to InformationList
        /// </summary>
        /// <param name="inClass"></param>
        public void AddClass(ClassContainer inClass)
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

        internal object ClassesForType(string inType)
        {
            throw new NotImplementedException();
        }
    }
}
