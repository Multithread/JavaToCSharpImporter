using CodeConverterCore.Helper;
using CodeConverterCore.Interface;
using CodeConverterCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeConverterCore.Analyzer
{
    public class AnalyzerCore
    {
        private List<UnknownTypeClass> UnknownTypes = new List<UnknownTypeClass>();

        private ProjectInformation ProjectInformation;

        private AnalyzerSettings Settings;

        /// <summary>
        /// Links the classes and Types inside the Project
        /// </summary>
        /// <param name="inLoadedProject"></param>
        /// <param name="inSettings"></param>
        public void LinkProjectInformation(ProjectInformation inLoadedProject, AnalyzerSettings inSettings = null)
        {
            ProjectInformation = inLoadedProject;
            Settings = inSettings ?? new AnalyzerSettings();

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
                foreach (var tmpInnerClass in tmpClass.InnerClasses)
                {
                    for (var tmpI = 0; tmpI < tmpInnerClass.MethodeList.Count; tmpI++)
                    {
                        var tmpMethode = tmpInnerClass.MethodeList[tmpI];
                        ManageTypeContainer(tmpTypeDictionary, tmpMethode.ReturnType, tmpInnerClass);
                        foreach (var tmpParamType in tmpMethode.Parameter)
                        {
                            ManageTypeContainer(tmpTypeDictionary, tmpParamType.Type, tmpInnerClass);
                        }
                        foreach (var tmpGenericType in tmpMethode.GenericTypes)
                        {
                            ManageTypeContainer(tmpTypeDictionary, tmpGenericType, tmpInnerClass);
                        }
                    }
                }
            }
            //);

            //Run over all Methode Code to set Variable Types and Access inside the Methode Code 
            //Parallel.ForEach(inLoadedProject.ClassList, new ParallelOptions { MaxDegreeOfParallelism = inSettings.MaxAmountOfParallelism },
            //    tmpClass =>
            foreach (var tmpClass in inLoadedProject.ClassList)
            {
                for (var tmpI = 0; tmpI < tmpClass.MethodeList.Count; tmpI++)
                {
                    ManageCodeBlockOfMethode(tmpClass.MethodeList[tmpI], tmpClass);
                }
                foreach (var tmpInnerClass in tmpClass.InnerClasses)
                {
                    for (var tmpI = 0; tmpI < tmpInnerClass.MethodeList.Count; tmpI++)
                    {
                        ManageCodeBlockOfMethode(tmpInnerClass.MethodeList[tmpI], tmpInnerClass);
                    }
                }
            }
            //);

            //Last thing to do: set Classes as loaded Corectly
            foreach (var tmpClass in inLoadedProject.ClassList)
            {
                tmpClass.IsAnalyzed = true;
            }
        }

        /// <summary>
        /// Manage Code inside Methode 
        /// </summary>
        /// <param name="inMethodeContainer"></param>
        /// <param name="inClass"></param>
        private void ManageCodeBlockOfMethode(MethodeContainer inMethodeContainer, ClassContainer inClass)
        {
            var tmpVariableList = new List<VariableDeclaration>();
            tmpVariableList.AddRange(inMethodeContainer.Parameter);
            if (inMethodeContainer.Code != null)
            {
                for (var tmpI = 0; tmpI < inMethodeContainer.Code.CodeEntries.Count; tmpI++)
                {
                    var tmpCodeBlock = inMethodeContainer.Code.CodeEntries[tmpI];
                    CodeEntryHandling(tmpCodeBlock, new FieldNameFinder(inClass) { VariableList = tmpVariableList });
                }
            }
        }

        /// <summary>
        /// Write Code-Entry into C# Code
        /// </summary>
        /// <param name="inOutput"></param>
        /// <param name="inCodeEntry"></param>
        private void CodeEntryHandling(ICodeEntry inCodeEntry, FieldNameFinder inNameFinder)
        {
            if (inCodeEntry == null) { return; }
            if (inCodeEntry is VariableDeclaration)
            {
                inNameFinder.VariableList.Add(inCodeEntry as VariableDeclaration);
            }
            else if (inCodeEntry is ConstantValue)
            {
                var tmpConstant = (inCodeEntry as ConstantValue);
                var tmpVal = tmpConstant.Value?.ToString();
                if (tmpVal.EndsWith("\""))
                {
                    //Nichts zu Tun
                    throw new NotImplementedException("CodeEntryHandling: Text Handling missing");
                }
                else if (RegexHelper.NumberCheck.IsMatch(tmpVal))
                {
                    //It's a number, nothing to do
                    throw new NotImplementedException("CodeEntryHandling: Number Handling not done yet");
                }
                else
                {
                    //Access to Variable, Field, Param or static Class, so we need to do a lot here for Code-link
                    if (tmpVal == "this")
                    {
                        inNameFinder.VariableList = null;
                    }
                    else if (tmpVal == "base")
                    {
                        inNameFinder.Class = inNameFinder.Class.InterfaceList
                            .Select(inItem => ProjectInformation.GetClassForType(inItem.Type.Name, new List<string> { inItem.Type.Namespace }))
                            .FirstOrDefault(inItem => !inItem.IsInterface());
                        inNameFinder.VariableList = null;
                    }
                    else
                    {
                        //TODO Check for basic Types

                        //Check for in Variable List
                        var tmpVar = inNameFinder.VariableList?.FirstOrDefault(inItem => inItem.Name == tmpVal);
                        if (tmpVar != null)
                        {
                            tmpConstant.Value = tmpVar;
                            tmpConstant.Type = tmpVar.Type;
                        }
                        else
                        {
                            //TODO Check for Parent CLass FieldList
                            var tmpField = inNameFinder.Class.FieldList?.FirstOrDefault(inItem => inItem.Name == tmpVal);
                            if (tmpField != null)
                            {
                                tmpConstant.Value = tmpField;
                                tmpConstant.Type = tmpField.Type;
                            }
                            else
                            {
                                //TODO Check for Static Class and Namespaces with Class
                                //throw new NotImplementedException("CodeEntryHandling: ConstantValue Handling not Implemented");

                                //Check for other Unknown Type
                                var tmpUnknown = UnknownTypes.FirstOrDefault(inItem => inItem.Type.Name == tmpVal);
                                if (tmpUnknown == null)
                                {
                                    tmpUnknown = new UnknownTypeClass(tmpVal)
                                    {
                                        PossibleNamespace = inNameFinder.Class.FullUsingList
                                    };
                                    Settings.InvokeUnknownTypeAdded(tmpUnknown);
                                }
                                tmpConstant.Value = tmpUnknown.Type;
                            }
                        }
                    }
                }
            }
            else if (inCodeEntry is StatementCode)
            {
                throw new NotImplementedException("CodeEntryHandling: StatementCode Handling not Implemented");
            }
            else if (inCodeEntry is SetFieldWithValue)
            {
                var tmpFieldVal = inCodeEntry as SetFieldWithValue;
                foreach (var tmpEntry in tmpFieldVal.VariableToAccess.CodeEntries)
                {
                    CodeEntryHandling(tmpEntry, new FieldNameFinder(inNameFinder));
                }
                foreach (var tmpEntry in tmpFieldVal.ValueToSet.CodeEntries)
                {
                    CodeEntryHandling(tmpEntry, new FieldNameFinder(inNameFinder));
                }
            }
            else if (inCodeEntry is VariableAccess)
            {
                var tmpVarAccess = inCodeEntry as VariableAccess;
                CodeEntryHandling(tmpVarAccess.Access, inNameFinder);
                if (tmpVarAccess.Child != null)
                {
                    CodeEntryHandling(tmpVarAccess.Child, inNameFinder);
                }
                else if (tmpVarAccess.BaseDataSource != null)
                {
                    CodeEntryHandling(tmpVarAccess.BaseDataSource, inNameFinder);
                }
            }
            else if (inCodeEntry is ReturnCodeEntry)
            {
                foreach (var tmpEntry in (inCodeEntry as ReturnCodeEntry).CodeEntries)
                {
                    CodeEntryHandling(tmpEntry, inNameFinder);
                }
            }
            else
            {
                throw new NotImplementedException("Code Entry Type not Implement");
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
            if (inTypeContainer == null)
            {
                return;
            }
            if (inDictionary.TryGetValue(inTypeContainer.Name, inItem => inClass.FullUsingList.Contains(inItem.Namespace), out var tmpResult))
            {
                inTypeContainer.Type = tmpResult;
            }
            else
            {
                //TODO checke if Unknown Type is used elsewhere with matching Namespaces
                var tmpUnknownType = UnknownTypes.FirstOrDefault(
                    inItem=> inItem.Type.Name== inTypeContainer.Name);
                if (tmpUnknownType == null)
                {
                    //Create new Unknown Type
                    tmpUnknownType = new UnknownTypeClass(inTypeContainer.Name);
                    tmpUnknownType.PossibleNamespace.AddRange(inClass.FullUsingList);
                    UnknownTypes.Add(tmpUnknownType);
                    Settings.InvokeUnknownTypeAdded(tmpUnknownType);
                }
                inTypeContainer.Type = tmpUnknownType.Type;
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
