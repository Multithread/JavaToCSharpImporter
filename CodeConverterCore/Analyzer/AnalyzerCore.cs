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

        private List<UnknownTypeClass> UnknownTypes = new List<UnknownTypeClass>();

        private ProjectInformation ProjectInformation;

        /// <summary>
        /// Links the classes and Types inside the Project
        /// </summary>
        /// <param name="inLoadedProject"></param>
        /// <param name="inSettings"></param>
        public void LinkProjectInformation(ProjectInformation inLoadedProject, AnalyzerSettings inSettings = null)
        {
            ProjectInformation = inLoadedProject;
            inSettings = inSettings ?? new AnalyzerSettings();

            var tmpTypeDictionary = inLoadedProject.KnownTypeDictionary;

            //Load all Types of Classes
            foreach (var tmpClass in inLoadedProject.ClassList)
            {
                tmpTypeDictionary.Add(tmpClass.Name, new BaseType(tmpClass.Type.Name, tmpClass.Namespace));
                if (tmpTypeDictionary.TryGetValue(tmpClass.Name, inItem => inItem.Namespace == tmpClass.Namespace, out var tmpResult))
                {
                    tmpClass.Type.Type = tmpResult;
                }
                else
                {
                    throw new Exception("Added Type cannot be found when searching for it.");
                }
            }

            //Load all Types of Inner Classes
            foreach (var tmpClass in inLoadedProject.ClassList)
            {
                for (var tmpI = 0; tmpI < tmpClass.InnerClasses.Count; tmpI++)
                {
                    tmpTypeDictionary.Add(tmpClass.InnerClasses[tmpI].Name, new BaseType(tmpClass.InnerClasses[tmpI].Type.Name, tmpClass.Namespace));
                    if (tmpTypeDictionary.TryGetValue(tmpClass.InnerClasses[tmpI].Name, inItem => inItem.Namespace == tmpClass.Namespace, out var tmpResult))
                    {
                        tmpClass.InnerClasses[tmpI].Type.Type = tmpResult;
                    }
                    else
                    {
                        throw new Exception("Added Type cannot be found when searching for it.");
                    }
                    if (tmpClass.InnerClasses[tmpI].InnerClasses.Count > 0)
                    {
                        throw new NotImplementedException("Inner Class for Inner Class is not implemented");
                    }
                }
            }

            //Link Generic Types and Base-Types
            //Load all Types of Classes
            foreach (var tmpClass in inLoadedProject.ClassList)
            {
                for (var tmpI = 0; tmpI < tmpClass.InterfaceList.Count; tmpI++)
                {
                    var tmpInterface = tmpClass.InterfaceList[tmpI];
                    ManageTypeContainer(tmpTypeDictionary, tmpInterface, tmpClass);
                }
                //Inner Class Handling
                for (var tmpInnerClassId = 0; tmpInnerClassId < tmpClass.InnerClasses.Count; tmpInnerClassId++)
                {
                    for (var tmpI = 0; tmpI < tmpClass.InnerClasses[tmpInnerClassId].InterfaceList.Count; tmpI++)
                    {
                        var tmpInterface = tmpClass.InnerClasses[tmpInnerClassId].InterfaceList[tmpI];
                        ManageTypeContainer(tmpTypeDictionary, tmpInterface, tmpClass.InnerClasses[tmpInnerClassId]);
                    }
                }
            }

            //Load Types of Methode Definitions
            //Parallel.ForEach(inLoadedProject.ClassList, new ParallelOptions { MaxDegreeOfParallelism = inSettings.MaxAmountOfParallelism },
            //    tmpClass =>
            foreach (var tmpClass in inLoadedProject.ClassList)
            {
                for (var tmpI = 0; tmpI < tmpClass.MethodeList.Count; tmpI++)
                {
                    var tmpMethode = tmpClass.MethodeList[tmpI];
                    ManageTypeContainer(tmpTypeDictionary, tmpMethode.ReturnType, tmpClass);
                    foreach (var tmpParamType in tmpMethode.Parameter)
                    {
                        ManageTypeContainer(tmpTypeDictionary, tmpParamType.Type, tmpClass);
                    }
                    foreach (var tmpGenericType in tmpMethode.GenericTypes)
                    {
                        ManageTypeContainer(tmpTypeDictionary, tmpGenericType, tmpClass);
                    }
                }
            }//);

            //Run over all Methode Code to set Variable Types inside the Methode Code
            foreach (var tmpClass in inLoadedProject.ClassList)
            {
                for (var tmpI = 0; tmpI < tmpClass.MethodeList.Count; tmpI++)
                {
                    ManageCodeBlockOfMethode(tmpClass.MethodeList[tmpI]);
                }
            }


            //Last thing to do: set Classes as loaded Corectly
            foreach (var tmpClass in inLoadedProject.ClassList)
            {
                tmpClass.IsConverted = true;
            }
        }

        private void ManageCodeBlockOfMethode(MethodeContainer inMethodeContainer)
        {
            var tmpVariableList = new List<VariableDeclaration>();
            tmpVariableList.AddRange(inMethodeContainer.Parameter);
            if (inMethodeContainer.Code != null)
            {
                foreach (var tmpCodeBlock in inMethodeContainer.Code.CodeEntries)
                {
                    //Add Variable to VariableList
                    if (tmpCodeBlock is VariableDeclaration)
                    {
                        tmpVariableList.Add(tmpCodeBlock as VariableDeclaration);
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// Convert all Same Types into the same Type
        /// </summary>
        /// <param name="inDictionary"></param>
        /// <param name="inTypeContainer"></param>
        /// <param name="inClass"></param>
        private void ManageTypeContainer(Dictionary<string, List<BaseType>> inDictionary, TypeContainer inTypeContainer, ClassContainer inClass)
        {
            if (inDictionary.TryGetValue(inTypeContainer.Name, inItem => inClass.FullUsingList.Contains(inItem.Namespace), out var tmpResult))
            {
                inTypeContainer.Type = tmpResult;
            }
            else
            {
                //TODO checke if Unknown Type is used elsewhere with matching Namespaces

                //Create new Unknown Type
                var tmpUnknownType = new UnknownTypeClass(inTypeContainer.Name);
                tmpUnknownType.PossibleNamespace.AddRange(inClass.FullUsingList);
                UnknownTypes.Add(tmpUnknownType);
                //TODO? Run Warning Event to Settings?
            }
            //Generic sub-Types handling
            foreach (var tmpGeneric in inTypeContainer.GenericTypes)
            {
                ManageTypeContainer(inDictionary, tmpGeneric, inClass);
            }
        }
    }
}
