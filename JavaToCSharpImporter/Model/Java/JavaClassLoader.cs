using JavaToCSharpConverter.Helper;
using JavaToCSharpConverter.Interface;
using JavaToCSharpConverter.Model.Splitter;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JavaToCSharpConverter.Model.Java
{
    public static class JavaClassLoader
    {
        /// <summary>
        /// Load a .java File into the Code
        /// </summary>
        /// <param name="inText"></param>
        /// <returns></returns>
        public static List<ClassContainer> LoadFile(string inText)
        {
            var tmpClassStringList = new List<string>();
            var tmpClassComments = new List<string>();

            var tmpUsingList = new List<string>();

            //Using, Namespace und Klassen auslesen
            var tmpPackage = GetClassesAndMetainformation(inText, tmpClassStringList, tmpClassComments, tmpUsingList);

            //Klassen Parsen
            var tmpClassList = new List<ClassContainer>();
            foreach (var tmpClassString in tmpClassStringList)
            {
                var tmpClass = new ClassContainer
                {
                    UsingList = tmpUsingList,
                    Namespace = tmpPackage,
                };

                var tmpComments = new List<string>();
                var tmpClassDefinition = "";
                var tmpClassCode = "";

                //TODO Add Methode Information to be 
                var tmpPartsWithChangedComments = CodeSplitter.FileDataSplitter(tmpClassString, new FullFlatSplitter()).ToList();
                foreach (var tmpSubPart in tmpPartsWithChangedComments)
                {
                    //Lere Daten abfangen
                    if (string.IsNullOrEmpty(tmpSubPart.Item2.RemoveNewlines().Trim(' ')))
                    {
                        continue;
                    }

                    var tmpType = (tmpSubPart.Item1 as CodeResultType?);

                    //Prüfen ob es sich um kommentare oder normalen Code handelt. Diese fügen wir am anfang an.
                    if (tmpType == CodeResultType.Comment)
                    {
                        tmpComments.Add(tmpSubPart.Item2);
                        continue;
                    }
                    //Prüfen ob es sich um kommentare oder normalen Code handelt. Diese fügen wir am anfang an.
                    if (tmpType == CodeResultType.None)
                    {
                        tmpClassDefinition += tmpSubPart.Item2;
                        continue;
                    }
                    //Prüfen ob es sich um kommentare oder normalen Code handelt. Diese fügen wir am anfang an.
                    if (tmpType == CodeResultType.InCurlyBracket)
                    {
                        tmpClassCode = tmpSubPart.Item2;
                        tmpClassCode = tmpClassCode.Substring(0, tmpClassCode.Length - 1);
                        FillClassWithDefinitionAndInterface(tmpClass, tmpClassDefinition);

                        //Set the Comment
                        tmpClass.Comment = string.Join("", tmpComments);

                        //TODO Fill Methode and Params from Inside Class
                        var tmpCLassFlatSplit = CodeSplitter.FileDataSplitter(tmpClassCode, new FullFlatSplitter()).ToList();
                        var tmpLastComment = "";
                        var tmpMethodeData = "";
                        for (var tmpI = 0; tmpI < tmpCLassFlatSplit.Count; tmpI++)
                        {
                            var tmpClassPart = tmpCLassFlatSplit[tmpI];
                            var tmpClassPartType = (tmpClassPart.Item1 as CodeResultType?);
                            if (tmpClassPartType == CodeResultType.Comment)
                            {
                                tmpLastComment += tmpClassPart.Item2;
                            }

                            if (tmpClassPartType == CodeResultType.LineEnd)
                            {
                                var tmpProperty = tmpClassPart.Item2.RemoveNewlines().Trim(' ');
                                //Sometimes someone adds a ; after a Curly Bracket.
                                if (tmpProperty == ";")
                                {
                                    continue;
                                }

                                if (tmpClass.AttributeList.Contains("interface") && tmpProperty.EndsWith(");"))
                                {
                                    var tmpMethodeContainer = new MethodeContainer
                                    {
                                        Comment = tmpLastComment,
                                    };
                                    tmpProperty = tmpProperty.Substring(0, tmpProperty.Length - 1);
                                    var tmpPropertyDef = CodeSplitter.FileDataSplitter(tmpProperty.Trim(' '), new DefinitionSplitter()).Where(inItem => !string.IsNullOrEmpty(inItem.Item2)).ToList();

                                    //Fill Fielddata
                                    tmpMethodeContainer.ReturnType = tmpPropertyDef.First().Item2;
                                    tmpMethodeContainer.Name = tmpPropertyDef.Skip(1).First().Item2.Split('(')[0];

                                    FillMethodeParams(tmpProperty, tmpMethodeContainer);
                                    tmpClass.MethodeList.Add(tmpMethodeContainer);
                                }
                                else
                                {

                                    var tmpFieldContainer = new FieldContainer
                                    {
                                        Comment = tmpLastComment,
                                    };

                                    var tmpLeftSide = tmpProperty;
                                    if (tmpProperty.IndexOf("=") != -1)
                                    {
                                        tmpLeftSide = tmpLeftSide.Substring(0, tmpLeftSide.IndexOf("="));
                                        tmpFieldContainer.HasDefaultValue = true;
                                        tmpFieldContainer.DefaultValue = tmpProperty.Substring(tmpProperty.IndexOf("=") + 1);
                                    }

                                    var tmpPropertyDef = CodeSplitter.FileDataSplitter(tmpLeftSide.Trim(' '), new DefinitionSplitter()).Where(inItem => !string.IsNullOrEmpty(inItem.Item2)).ToList();
                                    tmpPropertyDef.Reverse();

                                    //Fill Fielddata
                                    tmpFieldContainer.Name = tmpPropertyDef.First().Item2.TrimEnd(';').Trim(' ');
                                    tmpFieldContainer.Type = tmpPropertyDef.Skip(1).First().Item2.Trim(' ');
                                    tmpFieldContainer.ModifierList.AddRange(tmpPropertyDef.Skip(2).Select(inItem => inItem.Item2.Trim(' ')));

                                    tmpClass.FieldList.Add(tmpFieldContainer);

                                }
                                tmpLastComment = "";
                                continue;
                            }

                            if (tmpClassPartType == CodeResultType.None)
                            {
                                tmpMethodeData = tmpClassPart.Item2;
                            }

                            if (tmpClassPartType == CodeResultType.InCurlyBracket)
                            {
                                ExractInformationFothMethodeOrSubClass(tmpUsingList, tmpPackage, tmpClass, tmpLastComment, tmpMethodeData, tmpClassPart);
                                tmpLastComment = "";
                                tmpMethodeData = "";
                                continue;
                            }
                        }
                    }
                }
                tmpClassList.Add(tmpClass);
            }

            //Only Return nonEmpty Classes
            return tmpClassList.Where(inItem => !inItem.IsEmpty()).ToList();
        }

        /// <summary>
        /// Extract data for the Current Methode or Subclass (from outside they look the same)
        /// </summary>
        /// <param name="tmpUsingList"></param>
        /// <param name="tmpPackage"></param>
        /// <param name="tmpClass"></param>
        /// <param name="tmpLastComment"></param>
        /// <param name="tmpMethodeData"></param>
        /// <param name="tmpClassPart"></param>
        private static void ExractInformationFothMethodeOrSubClass(List<string> tmpUsingList, string tmpPackage, ClassContainer tmpClass, string tmpLastComment, string tmpMethodeData, Tuple<object, string> tmpClassPart)
        {
            var tmpMethodeDataCleaned = tmpMethodeData.RemoveNewlines().Trim(' ');
            //Manage Inner Classes
            if (tmpMethodeDataCleaned.Contains("class "))
            {
                //Patch Together the required Information for the Sub-Class
                var tmpPatchedText = $@"
{string.Join(Environment.NewLine, tmpUsingList.Select(inItem => $"inport {inItem};"))}
package {tmpPackage};
{tmpMethodeData}
{tmpClassPart.Item2}
";
                //Add Subclasses to the Inner Class-list
                tmpClass.InnerClasses.AddRange(LoadFile(tmpPatchedText));
            }
            else
            {
                //Manage normal Class Functions
                var tmpMethodeDef = CodeSplitter.FileDataSplitter(tmpMethodeDataCleaned.Split('(')[0], new DefinitionSplitter())
                    .Select(inItem => inItem.Item2.Trim(' '))
                    .Where(inItem => !string.IsNullOrEmpty(inItem)).ToList();

                tmpMethodeDef.Reverse();


                var tmpMethode = new MethodeContainer
                {
                    Comment = tmpLastComment,
                    Code = tmpClassPart.Item2,
                    Name = tmpMethodeDef.First(),
                    ReturnType = tmpMethodeDef.Skip(1).First(),
                    ModifierList = tmpMethodeDef.Skip(2).Select(inItem => inItem).ToList(),

                };
                if (tmpMethodeDataCleaned.Contains("("))
                {
                    FillMethodeParams(tmpMethodeDataCleaned, tmpMethode);
                }
                tmpClass.MethodeList.Add(tmpMethode);
            }
        }

        private static void FillMethodeParams(string tmpMethodeDataCleaned, MethodeContainer tmpMethode)
        {
            var tmpCleaned = tmpMethodeDataCleaned.TrimEnd('{').TrimEnd(' ');
            var tmpParameter = tmpCleaned.Substring(tmpCleaned.Split('(')[0].Length + 1, tmpCleaned.Length - tmpCleaned.Split('(')[0].Length - 2);

            if (!string.IsNullOrEmpty(tmpParameter))
            {
                //Methodenparameter aufbereiten
                var tmpParamList = CodeSplitter.FileDataSplitter(tmpParameter, new ClassInterfaceSplitter()).ToList();
                foreach (var tmpDataParam in tmpParamList)
                {
                    var tmpSplittedParam = CodeSplitter.FileDataSplitter(tmpDataParam.Item2.Trim(' ').Trim(','), new DefinitionSplitter())
                        .Where(inItem => !string.IsNullOrWhiteSpace(inItem.Item2.RemoveNewlines())).Select(inItem => inItem.Item2.Trim(' ')).ToList();

                    var tmpNewFieldContainer = new FieldContainer
                    {
                        Name = tmpSplittedParam.Last(),
                    };
                    if (tmpSplittedParam.Count == 3)
                    {
                        tmpNewFieldContainer.ModifierList.Add(tmpSplittedParam.First().Trim(' '));
                    }

                    tmpNewFieldContainer.Type = tmpSplittedParam.SkipLast(1).Last();
                    tmpMethode.Parameter.Add(tmpNewFieldContainer);
                }
            }
        }

        private static void FillClassWithDefinitionAndInterface(ClassContainer tmpClass, string tmpClassDefinition)
        {
            tmpClassDefinition = tmpClassDefinition.TrimEnd('{').RemoveNewlines();
            //Extends und Implements absplitten
            var tmpSplit = tmpClassDefinition.Split(new string[] { " extends " }, StringSplitOptions.None);
            tmpSplit = tmpSplit.SelectMany(inItem => inItem.Split(new string[] { " implements " }, StringSplitOptions.None)).ToArray();
            
            tmpClassDefinition = tmpSplit[0];

            //Set the Class Name
            var tmpClassParts = tmpClassDefinition.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            tmpClass.AttributeList = tmpClassParts.Reverse().Skip(1).ToList();
            tmpClass.Type = tmpClassParts.Last().RemoveNewlines().Trim(' ');

            //Add Generic Types of Class to Header
            if (tmpClass.Name.Contains("<"))
            {
                //Get All Generic Arguments
                tmpClass.GenericTypeParamList = CTSExtensions.GetGenericObjectsForType(tmpClass.Name);
                //Save the Name back
                tmpClass.Type = tmpClass.Name.Substring(0, tmpClass.Name.IndexOf("<"));
            }

            //Handle Extension and Implement of Java classes
            if (tmpSplit.Length > 1)
            {
                //TODO Handle Generic Interfaces?

                //Set All Interfaces of this Class 
                var tmpInterfaces = CodeSplitter.FileDataSplitter(string.Join(" ", tmpSplit.Skip(1)), new ClassInterfaceSplitter())
                    .Select(inItem => inItem.Item2)
                    .Foreach(inItem => inItem.RemoveNewlines().Trim(' '))
                    .Where(inItem => !string.IsNullOrEmpty(inItem))
                    .ToList();
                tmpClass.InterfaceList = tmpInterfaces;
            }
        }

        /// <summary>
        /// Returns the Heading Information( Usings, Namespace and Classes)
        /// </summary>
        /// <param name="inText"></param>
        /// <param name="tmpClassStringList"></param>
        /// <param name="tmpClassComments"></param>
        /// <param name="tmpUsingList"></param>
        /// <returns></returns>
        private static string GetClassesAndMetainformation(string inText, List<string> tmpClassStringList, List<string> tmpClassComments, List<string> tmpUsingList)
        {
            var tmpPackage = "";
            var tmpRestOfCode = "";
            foreach (var tmpSplittedFile in CodeSplitter.SplitForUsings(inText))
            {
                if ((tmpSplittedFile.Item1 as CodeResultType?) == CodeResultType.LineEnd)
                {
                    if (tmpSplittedFile.Item2.Contains("import"))
                    {
                        tmpUsingList.Add(tmpSplittedFile.Item2.Replace("import ", "").TrimEnd(';').RemoveNewlines());
                        continue;
                    }
                    if (tmpSplittedFile.Item2.Contains("package"))
                    {
                        tmpPackage = tmpSplittedFile.Item2.Replace("package ", "").TrimEnd(';').RemoveNewlines();
                        continue;
                    }
                }
                var tmpType = (tmpSplittedFile.Item1 as CodeResultType?);
                tmpRestOfCode += tmpSplittedFile.Item2;
                if (tmpType == CodeResultType.InCurlyBracket)
                {
                    tmpClassStringList.Add(tmpRestOfCode);
                    tmpRestOfCode = "";
                }
                continue;
            }
            tmpClassStringList.Add(tmpRestOfCode);
            return tmpPackage;
        }
    }
}
