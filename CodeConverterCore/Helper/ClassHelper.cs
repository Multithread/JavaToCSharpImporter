using CodeConverterCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeConverterCore.Helper
{
    public static class ClassHelper
    {
        /// <summary>
        /// Get Parent Class for Class
        /// </summary>
        /// <param name="inClass"></param>
        /// <param name="inProjectInfo"></param>
        /// <returns></returns>
        public static ClassContainer GetParentClass(this ClassContainer inClass)
        {
            var tmpPartentClass = inClass.InterfaceList
                .Where(inItem => inItem.Type != null)
                .Select(inItem => inClass.Parent.GetClassForType(inItem.Type.Name, inClass.FullUsingList))
                .FirstOrDefault(inItem => inItem != null && !inItem.ModifierList.Any(inModifier => inModifier == "interface"));

            if (tmpPartentClass != null)
            {
                return tmpPartentClass;
            }

            if (inClass.Name?.ToLower() != "object")
            {
                //object type laden
                return inClass.Parent.GetClassForType("Object", new List<string> { inClass.Parent.SystemNamespace })
                    ?? inClass.Parent.GetAliasType("object");
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Find a Methode in the CLass, Matching the Template
        /// </summary>
        /// <param name="inMethodeTemplate"></param>
        /// <param name="inClass"></param>
        /// <returns></returns>
        public static MethodeContainer FindMatchingMethode(this MethodeContainer inMethodeTemplate, ClassContainer inClass, bool inCheckParams = true)
        {
            var tmpMethodeList = inClass.MethodeList
                .Where(inItem => inItem.Name == inMethodeTemplate.Name);

            if (inCheckParams)
            {
                //Match Methode Parameter lenght
                tmpMethodeList = tmpMethodeList.Where(inItem => inItem.Parameter.Count == inMethodeTemplate.Parameter.Count);

                //TODO Find Methode by Matching Params
            }

            return tmpMethodeList.FirstOrDefault();
        }

        /// <summary>
        /// Handle Modifiers of a List to add/Remove spezific Modifiers
        /// </summary>
        /// <param name="inModifierList"></param>
        /// <param name="inModifierToHandle"></param>
        /// <param name="inAddModifier"></param>
        public static void HandleListContent(this List<string> inModifierList, string inModifierToHandle, bool inAddModifier = true)
        {
            if (inAddModifier)
            {
                if (!inModifierList.Contains(inModifierToHandle))
                {
                    inModifierList.Add(inModifierToHandle);
                }
            }
            else
            {
                if (inModifierList.Contains(inModifierToHandle))
                {
                    inModifierList.Remove(inModifierToHandle);
                }
            }
        }
    }
}
