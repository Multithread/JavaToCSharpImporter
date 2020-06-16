using CodeConverterCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeConverterCore.Analyzer
{
    public class AnalyzerCore
    {
        /// <summary>
        /// Links the classes and Types inside the Project
        /// </summary>
        /// <param name="inLoadedProject"></param>
        /// <param name="inSettings"></param>
        public void LinkProjectInformation(ProjectInformation inLoadedProject, AnalyzerSettings inSettings = null)
        {
            inSettings = inSettings ?? new AnalyzerSettings();

            var tmpTypeDictionary = inLoadedProject.KnownTypeDictionary;

            //Load all Types of Classes
            foreach (var tmpClass in inLoadedProject.ClassList)
            {
                tmpTypeDictionary.Add(tmpClass.Name, new BaseType(tmpClass.Name, tmpClass.Namespace));
                if (tmpTypeDictionary.TryGetValue(tmpClass.Name, inItem => inItem.Namespace == tmpClass.Namespace, out var tmpResult))
                {
                    tmpClass.Type.Type = tmpResult;
                }
                else
                {
                    throw new Exception("Added Type cannot be found when searching for it.");
                }
            }

            //Link Generic Types and Base-Types
            //Load all Types of Classes
            var tmpUnknownTypes = new List<ClassContainer>();
            foreach (var tmpClass in inLoadedProject.ClassList)
            {
                for (var tmpI = 0; tmpI < tmpClass.InterfaceList.Count; tmpI++)
                {
                    var tmpInterface = tmpClass.InterfaceList[tmpI];
                    if (tmpTypeDictionary.TryGetValue(tmpInterface.Name, inItem => tmpClass.FullUsingList.Contains(inItem.Namespace), out var tmpResult))
                    {
                        tmpClass.InterfaceList[tmpI] = tmpResult;
                    }
                    else
                    {
                        //Create new Unknown Type
                        var tmpUnknownType = new ClassContainer() { Type = new TypeContainer { Name = tmpInterface.Name } };
                        tmpUnknownType.UsingList.AddRange(tmpClass.FullUsingList);
                        tmpUnknownTypes.Add(tmpUnknownType);
                        //TODO? Run Warning Event to Settings?
                    }
                }
            }

            //TODO Run for Generic Types
        }
    }
}
