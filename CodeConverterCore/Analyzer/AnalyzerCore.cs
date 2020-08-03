﻿using CodeConverterCore.Helper;
using CodeConverterCore.Interface;
using CodeConverterCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeConverterCore.Analyzer
{
    public class AnalyzerCore
    {
        private ProjectInformation ProjectInformation;

        private AnalyzerSettings Settings;

        /// <summary>
        /// Links the classes and Types inside the Project
        /// 
        /// TODO Remove tmpTypeDictionary from this Class, it does not seem to be required to achive everything required to map Types
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
                for (var tmpI = 0; tmpI < tmpClass.FieldList.Count; tmpI++)
                {
                    ManageField(tmpTypeDictionary, tmpClass.FieldList[tmpI], tmpClass);
                }
                foreach (var tmpInnerClass in tmpClass.InnerClasses)
                {
                    for (var tmpI = 0; tmpI < tmpInnerClass.FieldList.Count; tmpI++)
                    {
                        ManageField(tmpTypeDictionary, tmpInnerClass.FieldList[tmpI], tmpInnerClass);
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
        private void ManageField(Dictionary<string, List<BaseType>> inDictionary, FieldContainer inMethodeContainer, ClassContainer inClass)
        {
            ManageTypeContainer(inDictionary, inMethodeContainer.Type, inClass);
            if (inMethodeContainer.DefaultValue != null)
            {
                CodeEntryHandling(inMethodeContainer.DefaultValue.CodeEntries[0], new FieldNameFinder(inClass));
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
        private TypeContainer CodeEntryHandling(ICodeEntry inCodeEntry, FieldNameFinder inNameFinder, TypeContainer inReturnType = null)
        {
            if (inCodeEntry == null) { return null; }
            TypeContainer tmpReturnType = null;
            if (inCodeEntry is VariableDeclaration)
            {
                inNameFinder.VariableList.Add(inCodeEntry as VariableDeclaration);
            }
            else if (inCodeEntry is ConstantValue)
            {
                var tmpConstant = (inCodeEntry as ConstantValue);
                var tmpVal = tmpConstant.Value?.ToString() ?? tmpConstant.Type.Name;
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
                        //Check for in Variable List
                        var tmpVar = inNameFinder.VariableList?.FirstOrDefault(inItem => inItem.Name == tmpVal);
                        if (tmpVar != null)
                        {
                            tmpConstant.Value = tmpVar;
                            tmpConstant.Type = tmpVar.Type;
                            //TODO load class for Type
                            inNameFinder.Class = ProjectInformation.ClassFromBaseType(tmpVar.Type);
                        }
                        else
                        {
                            //TODO Check for Parent CLass FieldList
                            var tmpField = inNameFinder.Class?.FieldList?.FirstOrDefault(inItem => inItem.Name == tmpVal);
                            if (tmpField != null)
                            {
                                tmpConstant.Value = tmpField;
                                tmpConstant.Type = tmpField.Type;
                                //TODO load class for Type
                                inNameFinder.Class = ProjectInformation.ClassFromBaseType(tmpField.Type);

                            }
                            else
                            {
                                //Check for Static Types, System aliases or other Unknown Type
                                var tmpStaticClassOrUnknown = ProjectInformation.GetClassOrUnknownForType(tmpVal, inNameFinder.Class?.FullUsingList ?? new List<string>());
                                tmpConstant.Value = tmpStaticClassOrUnknown.Type;
                                if (tmpConstant.Type != null)
                                {
                                    tmpConstant.Type.Type = tmpStaticClassOrUnknown.Type.Type;
                                }
                                else
                                {
                                    tmpConstant.Type = tmpStaticClassOrUnknown.Type;
                                }

                                inNameFinder.Class = tmpStaticClassOrUnknown;

                            }
                        }
                    }
                    tmpReturnType = tmpConstant.Type;
                }
            }
            else if (inCodeEntry is StatementCode)
            {
                var tmpStatement = inCodeEntry as StatementCode;
                if (tmpStatement.StatementType == Enum.StatementTypeEnum.If
                    || tmpStatement.StatementType == Enum.StatementTypeEnum.Else)
                {
                    if (tmpStatement.InnerContent != null)
                    {
                        foreach (var tmpEntry in tmpStatement.InnerContent.CodeEntries)
                        {
                            CodeEntryHandling(tmpEntry, new FieldNameFinder(inNameFinder));
                        }
                    }
                    if (tmpStatement.StatementCodeBlocks != null)
                    {
                        foreach (var tmpEntry in tmpStatement.StatementCodeBlocks.SelectMany(inItem => inItem.CodeEntries))
                        {
                            CodeEntryHandling(tmpEntry, new FieldNameFinder(inNameFinder));
                        }
                    }
                }
                else if (tmpStatement.StatementType == Enum.StatementTypeEnum.Assert
                    || tmpStatement.StatementType == Enum.StatementTypeEnum.Elvis)
                {
                    foreach (var tmpCodeBlock in tmpStatement.StatementCodeBlocks)
                    {
                        foreach (var tmpEntry in tmpCodeBlock.CodeEntries)
                        {
                            CodeEntryHandling(tmpEntry, new FieldNameFinder(inNameFinder));
                        }
                    }
                }
                else
                {
                    throw new NotImplementedException("CodeEntryHandling: StatementCode Handling not Implemented");
                }
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
                var tmpNameFinder = new FieldNameFinder(inNameFinder);
                tmpReturnType = CodeEntryHandling(tmpVarAccess.Access, tmpNameFinder);
                //tmpNameFinder.Class = ProjectInformation.GetClassOrUnknownForType(tmpReturnType.Name, inNameFinder.Class.FullUsingList);
                if (tmpVarAccess.Child != null)
                {
                    CodeEntryHandling(tmpVarAccess.Child, tmpNameFinder, inReturnType);
                }
                else if (tmpVarAccess.BaseDataSource != null)
                {
                    CodeEntryHandling(tmpVarAccess.BaseDataSource, inNameFinder, inReturnType);
                }
            }
            else if (inCodeEntry is ReturnCodeEntry)
            {
                foreach (var tmpEntry in (inCodeEntry as ReturnCodeEntry).CodeEntries)
                {
                    CodeEntryHandling(tmpEntry, inNameFinder);
                }
            }
            else if (inCodeEntry is NewObjectDeclaration)
            {
                var tmpObjectDecl = (inCodeEntry as NewObjectDeclaration);
                CodeEntryHandling(tmpObjectDecl.InnerCode, inNameFinder);
                if (tmpObjectDecl.ArgumentList != null)
                {
                    foreach (var tmpArgument in tmpObjectDecl.ArgumentList)
                    {
                        foreach (var tmpEntry in tmpArgument.CodeEntries)
                        {
                            CodeEntryHandling(tmpEntry, inNameFinder);
                        }
                    }
                }
            }
            else if (inCodeEntry is MethodeCall)
            {
                var tmpMethodeCall = inCodeEntry as MethodeCall;
                var tmpParentClass = inNameFinder.Class;
                while (tmpParentClass != null)
                {
                    tmpMethodeCall.MethodeLink = tmpParentClass.MethodeList.FirstOrDefault(inItem => inItem.Name == tmpMethodeCall.Name);
                    if (tmpMethodeCall.MethodeLink != null)
                    {
                        break;
                    }
                    var tmpParentSelection = tmpParentClass.InterfaceList
                        .Select(inItem => ProjectInformation.GetClassForType(inItem.Type.Name, tmpParentClass.FullUsingList))
                        .FirstOrDefault(inItem => inItem != null && !inItem.ModifierList.Any(inModifier => inModifier == "interface"));
                    if (tmpParentSelection != null)
                    {
                        tmpParentClass = tmpParentSelection;
                    }
                    else if (tmpParentClass.Name != "Object")
                    {
                        //object type laden
                        tmpParentClass = ProjectInformation.GetClassForType("Object", new List<string> { ProjectInformation.SystemNamespace });
                    }
                    else
                    {
                        tmpParentClass = null;
                    }

                    foreach (var tmpParam in tmpMethodeCall.Parameter)
                    {
                        foreach (var tmpEntry in tmpParam.CodeEntries)
                        {
                            CodeEntryHandling(tmpEntry, new FieldNameFinder(inNameFinder));
                        }
                    }
                }
                if (tmpMethodeCall.MethodeLink == null)
                {
                    //TODO Implement Extension Methode finding
                    if (inNameFinder.Class is UnknownTypeClass)
                    {
                        if (tmpMethodeCall.Parameter.Count > 0)
                        {
                            throw new NotImplementedException("Unknown Methode on Class");
                        }
                        var tmpMethode = Create.AddMethode(inNameFinder.Class, tmpMethodeCall.Name, inReturnType ?? TypeContainer.Void);
                        tmpMethode.ReturnType = /*inReturnType ??*/ TypeContainer.Void;
                        tmpReturnType = tmpMethode.ReturnType;
                    }
                    else
                    {
                        throw new NotImplementedException("Unknown Methode on Class");
                    }
                }
            }
            else if (inCodeEntry is CodeExpression)
            {
                var tmpExpr = inCodeEntry as CodeExpression;
                tmpReturnType = inReturnType;
                foreach (var tmpSubClause in tmpExpr.SubClauseEntries)
                {
                    foreach (var tmpCodeEntry in tmpSubClause.CodeEntries)
                    {
                        tmpReturnType = CodeEntryHandling(tmpCodeEntry, inNameFinder, tmpReturnType);
                    }
                }
            }
            else
            {
                throw new NotImplementedException("Code Entry Type not Implement");
            }
            return tmpReturnType;
        }

        /// <summary>
        /// Convert all Same Types into the same Type
        /// </summary>
        /// <param name="inDictionary"></param>
        /// <param name="inTypeContainer"></param>
        /// <param name="inClass"></param>
        private void ManageTypeContainer(Dictionary<string, List<BaseType>> inDictionary, TypeContainer inTypeContainer, ClassContainer inClass)
        {
            if (inTypeContainer == null || string.IsNullOrEmpty(inTypeContainer.Name))
            {
                return;
            }
            var tmpResult = ProjectInformation.GetClassOrUnknownForType(inTypeContainer.Name, inClass.FullUsingList);
            inTypeContainer.Type = tmpResult.Type.Type;

            //Generic sub-Types handling
            foreach (var tmpGeneric in inTypeContainer.GenericTypes)
            {
                ManageTypeContainer(inDictionary, tmpGeneric, inClass);
            }
        }
    }
}
