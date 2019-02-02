using Antlr4.Runtime.Tree;
using JavaToCSharpConverter.Helper;
using JavaToCSharpConverter.Interface;
using JavaToCSharpConverter.Model.OOP;
using JavaToCSharpConverter.Model.Splitter;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static JavaToCSharpConverter.Model.Java.JavaParser;

namespace JavaToCSharpConverter.Model.CSharp
{
    public static class CSharpClassWriter
    {
        /// <summary>
        /// Create C# Class File from Class
        /// </summary>
        /// <param name="inClass"></param>
        /// <returns></returns>
        public static string CreateFile(ClassContainer inClass, INameConverter inConverter)
        {
            var tmpStringBuilder = new StringBuilder();

            //TODO Check what new Namespaces are Required for the Class, not onyl use the existing?
            foreach (var tmpUsing in inClass.UsingList)
            {
                //tmpStringBuilder.AppendLine($"using {inConverter.ChangeNamespace(tmpUsing)};");
            }

            var tmpRequiredUsings = new List<string>();

            tmpStringBuilder.AppendLine($"namespace {inConverter.ChangeNamespace(inClass.Namespace)}{Environment.NewLine}{{");

            tmpStringBuilder.AppendLine(inClass.Comment);

            tmpStringBuilder.Append(string.Join(" ", inConverter.MapAndSortAttributes(inClass.AttributeList)));
            tmpStringBuilder.Append($" {inClass.Name} ");
            if (inClass.InterfaceList.Count > 0)
            {
                //Interface Type Mapping
                tmpStringBuilder.Append(" : " + string.Join(" ", inClass.InterfaceList.Select(inItem
                    => inConverter.DoTypeMap(inItem, inClass, tmpRequiredUsings))));
            }
            tmpStringBuilder.AppendLine("{");

            var tmpDepth = 1;
            //Set Fields
            foreach (var tmpField in inClass.FieldList)
            {
                tmpStringBuilder.AppendLine();
                tmpStringBuilder.AppendLine();

                //Map Type to New and Manage Usings
                var tmpTypeString = inConverter.DoTypeMap(tmpField.Type, inClass, tmpRequiredUsings);

                tmpStringBuilder.Append(GetLeftSpace(tmpDepth) + $"{string.Join(" ", inConverter.MapAndSortAttributes(tmpField.ModifierList, true))} {tmpTypeString} {tmpField.Name}");
                if (tmpField.HasDefaultValue)
                {
                    if (string.IsNullOrEmpty(tmpField.DefaultValue))
                    {
                        //TODO Methode runner for ANTLR
                        tmpStringBuilder.Append($" = ");

                        var tmpCodeState = new CodeState(inConverter, inClass.UsingList);
                        RewriteAntlrFunctionCode(tmpStringBuilder, tmpField.AntlrDefaultValue, inClass, inConverter, tmpRequiredUsings, tmpCodeState);

                        //tmpStringBuilder.Append($" = ");
                    }
                    else
                    {
                        tmpStringBuilder.Append($" = {tmpField.DefaultValue}");
                    }
                }
                else
                {
                    tmpStringBuilder.Append(";");
                }
            }

            //Set Methodes
            foreach (var tmpMethode in inClass.MethodeList)
            {
                tmpStringBuilder.AppendLine();
                tmpStringBuilder.AppendLine();

                var tmpNewReturnType = string.IsNullOrEmpty(tmpMethode.ReturnType) ? "void" : inConverter.DoTypeMap(tmpMethode.ReturnType, inClass, tmpRequiredUsings);
                if (tmpMethode.IsConstructor)
                {
                    tmpNewReturnType = "";
                }

                var tmpNewMethodeName = inConverter.MapFunction(tmpMethode.Name, inClass.Name, inClass.FullUsingList).Split('.')
                    .Last().PascalCase();
                var tmpMethodeModifier = inConverter.MapAndSortAttributes(tmpMethode.ModifierList);

                //Check if Override ist used Correctly
                if (tmpMethodeModifier.Contains("override"))
                {
                    bool tmpOverridenFromInterface = IsOverrideRequiredForMethode(inClass, inConverter, tmpMethode);

                    //Overrides from methode do not need an override
                    if (!tmpOverridenFromInterface)
                    {
                        tmpMethodeModifier.Remove("override");
                    }
                }

                tmpStringBuilder.Append(GetLeftSpace(tmpDepth) + $"{string.Join(" ", tmpMethodeModifier)} {tmpNewReturnType} {tmpNewMethodeName}");

                //TODO ref und out ergänzen
                tmpStringBuilder.Append($"({string.Join(", ", tmpMethode.Parameter.Select(inItem => inConverter.DoTypeMap(inItem.Type, inClass, tmpRequiredUsings) + " " + inItem.Name))})");

                if (tmpMethode.AntlrCode != null)
                {
                    //TODO Methode Parsing
                    tmpStringBuilder.AppendLine(GetLeftSpace(tmpDepth) + $"{{}}");
                }
                else if (!string.IsNullOrEmpty(tmpMethode.Code))
                {
                    tmpStringBuilder.AppendLine(GetLeftSpace(tmpDepth) + $"{{");

                    //Rewrite the Linear Code
                    RewriteFunctionCode(tmpStringBuilder, tmpMethode, inClass, inConverter, tmpRequiredUsings);

                    if (!tmpMethode.Code.EndsWith("}"))
                    {
                        tmpStringBuilder.AppendLine(GetLeftSpace(tmpDepth) + $"}}");
                    }
                }
                else
                {
                    tmpStringBuilder.Append($";");
                }
            }

            tmpStringBuilder.AppendLine(GetLeftSpace(1) + $"}}{Environment.NewLine}}}");

            var tmpMoreUsings = "";
            if (tmpRequiredUsings.Count > 0)
            {
                tmpMoreUsings = string.Join(Environment.NewLine, tmpRequiredUsings.Select(inItem => $"using {inConverter.ChangeNamespace(inItem)};"));
                tmpMoreUsings += Environment.NewLine;
            }

            return tmpMoreUsings + tmpStringBuilder.ToString();
        }

        private static string GetLeftSpace(int inDepth)
        {
            var tmpString = "";

            for (var tmpI = 0; tmpI < inDepth; tmpI++)
            {
                tmpString += "    ";
            }
            return tmpString;
        }


        /// <summary>
        /// Rewrite Linnear code from Functions
        /// </summary>
        private static void RewriteAntlrFunctionCode(StringBuilder inStringBuilder, IParseTree inTreeElement, ClassContainer inClass, INameConverter inConverter, List<string> inRequiredUsings, CodeState inCodeState = null)
        {
            if (inTreeElement == null)
            {
                return;
            }
            var tmpType = inTreeElement.GetType().Name;
            if (inTreeElement is VariableInitializerContext)
            {
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
                inStringBuilder.AppendLine(";");
            }
            else if (inTreeElement is CreatedNameContext)
            {
                var tmpNameContext = inTreeElement as CreatedNameContext;
                ManageNamedContext(inStringBuilder, tmpNameContext);
            }
            else if (inTreeElement is PrimaryContext)
            {
                var tmpPrimaryContext = inTreeElement as PrimaryContext;
                if (tmpPrimaryContext.literal() != null)
                {
                    var tmpLiteralContext = tmpPrimaryContext.literal();
                    if (tmpLiteralContext.integerLiteral() != null)
                    {
                        inStringBuilder.Append(tmpLiteralContext.integerLiteral().GetText());
                    }
                    else
                    {

                    }
                }
                else if (tmpPrimaryContext.IDENTIFIER() != null)
                {
                    inStringBuilder.Append(tmpPrimaryContext.IDENTIFIER());
                }
                else
                {


                }
            }
            else if (inTreeElement is CreatorContext)
            {
                var tmpCreator = inTreeElement as CreatorContext;

                var tmpNameContext = tmpCreator.createdName();
                ManageNamedContext(inStringBuilder, tmpNameContext);

                if (tmpCreator.classCreatorRest() != null)
                {
                    var tmpClassCreator = tmpCreator.classCreatorRest();
                    //TODO Handle creation of class
                }
                if (tmpCreator.arrayCreatorRest() != null)
                {
                    inStringBuilder.Append("[");
                    var tmpArray = tmpCreator.arrayCreatorRest();

                    bool tmpFirstInArray = true;
                    foreach (var tmpExpr in tmpArray.expression())
                    {
                        if (!tmpFirstInArray)
                        {
                            inStringBuilder.Append(", ");
                        }
                        tmpFirstInArray = false;
                        HandleExpressionContext(inStringBuilder, tmpExpr, inClass, inConverter, inRequiredUsings, inCodeState);
                    }
                    inStringBuilder.Append("]");
                    return;
                }

                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
                inStringBuilder.AppendLine(";");

                return;
            }
            if (inTreeElement is ExpressionContext)
            {
                var tmpExpr = inTreeElement as ExpressionContext;
                HandleExpressionContext(inStringBuilder, tmpExpr, inClass, inConverter, inRequiredUsings, inCodeState);
            }
            //else if (inTreeElement is TerminalNodeImpl && WordRegex.IsMatch(inTreeElement.GetText()))
            //{
            //    //Ist ein Wort
            //    inStringBuilder.Append(inTreeElement.GetText() + " ");
            //}
            else
            {
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    //RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
        }

        private static void ManageNamedContext(StringBuilder inStringBuilder, CreatedNameContext tmpNameContext)
        {
            bool tmpIsFirst = true;
            foreach (var tmpIdentifier in tmpNameContext.IDENTIFIER())
            {
                var tmpName = tmpIdentifier.GetText();
                if (!tmpIsFirst)
                {
                    inStringBuilder.Append(".");
                }
                inStringBuilder.Append(tmpName);
                tmpIsFirst = false;
                //TODO Handle Usings
            }
        }

        private static void HandleExpressionContext(StringBuilder inStringBuilder, ExpressionContext tmpExpr, ClassContainer inClass, INameConverter inConverter, List<string> inRequiredUsings, CodeState inCodeState)
        {
            if (tmpExpr.NEW() != null)
            {
                inStringBuilder.Append("new ");
            }
            else if (tmpExpr.THIS() != null)
            {
                inStringBuilder.Append("this.");
            }
            else if (tmpExpr.SUPER() != null)
            {
                inStringBuilder.Append("base.");
            }
            foreach (var tmpElement in tmpExpr.GetChildren())
            {
                if (tmpElement is TerminalNodeImpl)
                {
                    if (tmpElement.GetText() == ".")
                    {
                        inStringBuilder.Append(tmpElement.GetText());
                        continue;
                    }
                }
                RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
            }
        }

        private static Regex WordRegex = new Regex("\\w");


        /// <summary>
        /// Rewrite Linnear code from Functions
        /// </summary>
        private static void RewriteFunctionCode(StringBuilder inStringBuilder, MethodeContainer inMethode, ClassContainer inClass, INameConverter inConverter, List<string> inRequiredUsings, CodeState inCodeState = null)
        {
            if (inCodeState == null)
            {
                inCodeState = new CodeState(inConverter, inClass.FullUsingList);
                //Add Methode params to State
                foreach (var tmpParam in inMethode.Parameter)
                {
                    inCodeState.AddVariable(tmpParam.Type, tmpParam.Name);
                }
                //Add Fields of Class
                foreach (var tmpParam in inClass.FieldList)
                {
                    inCodeState.AddVariable(tmpParam.Type, tmpParam.Name, false);
                }
            }

            var tmpCodeParts = CodeSplitter.FileDataSplitter(inMethode.Code, new CodeLineSplitter());
            foreach (var tmpCodePart in tmpCodeParts)
            {
                var tmpInfo = (CodeLineResultType?)tmpCodePart.Item1;
                if (tmpInfo == CodeLineResultType.Comment)
                {
                    inStringBuilder.Append(tmpCodePart.Item2);
                    continue;
                }
                if (tmpInfo == CodeLineResultType.EndOfLine)
                {
                    //Clean the Code
                    var tmpString = tmpCodePart.Item2.TrimEnd(';').RemoveNewlines().Trim(' ');

                    //Ignore Asserts, as they have no equal in C# except maybe #if DEBUG
                    if (tmpString.StartsWith("assert"))
                    {
                        inStringBuilder.Append("//" + tmpString.RemoveNewlines());
                        continue;
                    }

                    if (tmpString.Contains("="))
                    {
                        var tmpRightPart = "";
                        tmpRightPart = tmpString.Substring(tmpString.IndexOf("="));
                        tmpString = tmpString.Substring(0, tmpString.IndexOf("="));


                        var tmpLeftParts = CodeSplitter.FileDataSplitter(tmpString, new DefinitionSplitter())
                            .Select(inItem => inItem.Item2.RemoveNewlines().Trim(' '))
                            .Where(inItem => !string.IsNullOrEmpty(inItem))
                            .ToList();


                        if (tmpLeftParts.First() == "final")
                        {
                            tmpLeftParts = tmpLeftParts.Skip(1).ToList();
                        }
                        if (tmpLeftParts.Count > 1)
                        {
                            //Map Type to new Type
                            var tmpType = inConverter.DoTypeMap(tmpLeftParts.First(), inClass, inClass.FullUsingList).Split('.').Last();
                            //TODO: Resolve var Types (from Return Type of Right side)
                            inCodeState.AddVariable(tmpType, tmpLeftParts.Last().Trim(' ').TrimEnd(';'));
                            inStringBuilder.Append(tmpType + " ");
                        }

                        if (tmpLeftParts.Contains("."))
                        {
                            //Do something? Maybe not
                        }

                        inStringBuilder.Append(tmpLeftParts.Last());

                        //TODO Resolve Right side
                        inStringBuilder.Append(tmpRightPart);

                        inStringBuilder.AppendLine(";");
                    }
                    else
                    {
                        //TODO Resolve Methode Calls

                        var tmpCurrentPos = 0;
                        var tmpRemainingText = tmpCodePart.Item2;
                        var tmpCurrentPropertyText = "";
                        ClassContainer tmpType = null;
                        while (tmpRemainingText.Length > 0)
                        {
                            var tmpFirstIndex = tmpRemainingText.FirstIndexofAny(new char[] { '.', '(', ')' });
                            var tmpSplitChar = ' ';
                            //Save removed Char somewhere
                            if (tmpFirstIndex == -1)
                            {
                                tmpCurrentPropertyText = tmpRemainingText.RemoveNewlines().Trim(' ');
                                tmpRemainingText = "";
                            }
                            else
                            {
                                tmpSplitChar = tmpRemainingText[tmpFirstIndex];
                                tmpCurrentPropertyText = tmpRemainingText.Substring(0, tmpFirstIndex).RemoveNewlines().Trim(' ');
                            }

                            var tmpIsFunction = false;
                            var tmpIndex = 1;
                            while (tmpRemainingText.Length > tmpIndex)
                            {
                                if (tmpRemainingText[tmpIndex] == '(')
                                {
                                    tmpIsFunction = true;
                                    break;
                                }
                                if (tmpRemainingText[tmpIndex] == '.')
                                {
                                    break;
                                }
                                tmpIndex++;
                            }
                            tmpRemainingText = tmpRemainingText.Substring(tmpFirstIndex + 1);

                            //Return Param abfangen
                            if (tmpCurrentPropertyText.StartsWith("return "))
                            {
                                tmpCurrentPropertyText = tmpCurrentPropertyText.Substring(7).TrimStart(' ');
                                inStringBuilder.Append("return ");
                            }

                            if (tmpCurrentPropertyText.StartsWith("throw new "))
                            {
                                tmpCurrentPropertyText = tmpCurrentPropertyText.Substring(7).TrimStart(' ');
                                //TODO Throw new with inner Methodes usage
                                //AddUsingIfRequired(inRequiredUsings, "System");
                                inStringBuilder.Append(tmpCodePart.Item2);
                                break;
                            }

                            if (tmpCurrentPropertyText == "continue")
                            {
                                tmpCurrentPropertyText = tmpCurrentPropertyText.Substring(7);
                                inStringBuilder.AppendLine("continue;");
                                break;
                            }
                            if (tmpCurrentPropertyText == "return")
                            {
                                tmpCurrentPropertyText = tmpCurrentPropertyText.Substring(7);
                                inStringBuilder.AppendLine("return;");
                                break;
                            }

                            if (tmpIsFunction)
                            {
                                if (tmpType == null)
                                {
                                    tmpType = inClass;
                                }
                                var tmpNewFunctionNameList = inConverter.MapFunction(tmpCurrentPropertyText, tmpType.Name, new List<string> { tmpType.Namespace });
                                var tmpNewFunctionName = tmpNewFunctionNameList.Split('.').Last();

                                inStringBuilder.Append(tmpNewFunctionName + tmpSplitChar + tmpRemainingText);
                                break;
                            }
                            else
                            {
                                //Type laden
                                if (tmpType == null)
                                {
                                    tmpType = inCodeState.GetType(tmpCurrentPropertyText);
                                    //Es handelt sich wohl um einen Statischen Typen
                                    if (tmpType == null)
                                    {
                                        tmpType = inConverter.GetClassForType(tmpCurrentPropertyText, inClass.FullUsingList);
                                    }
                                    if (tmpType == null)
                                    {
                                        inStringBuilder.Append(tmpCurrentPropertyText + tmpSplitChar + tmpRemainingText);
                                        break;
                                    }
                                    AddUsingIfRequired(inRequiredUsings, tmpType.Namespace);
                                    inStringBuilder.Append(tmpCurrentPropertyText + tmpSplitChar);
                                    continue;
                                }
                                else
                                {
                                    inStringBuilder.Append(tmpCurrentPropertyText + tmpSplitChar);
                                }
                            }

                            if (tmpSplitChar == '(')
                            {

                            }
                        }

                        inStringBuilder.AppendLine("");
                        //inStringBuilder.Append(tmpCodePart.Item2);
                    }

                    continue;
                }
                else
                {
                    inStringBuilder.Append(tmpCodePart.Item2);
                }
            }
        }

        /// <summary>
        /// Check if this methode needs an 'override'
        /// </summary>
        /// <param name="inClass"></param>
        /// <param name="inConverter"></param>
        /// <param name="tmpMethode"></param>
        /// <returns></returns>
        private static bool IsOverrideRequiredForMethode(ClassContainer inClass, INameConverter inConverter, MethodeContainer tmpMethode)
        {
            bool? tmpOverridenFromInterface = null;
            foreach (var tmpClass in inClass.InterfaceList)
            {
                if (!inConverter.MapFunction(tmpMethode.Name, tmpClass, inClass.FullUsingList)
                    .Contains("."))
                {
                    continue;
                }
                var tmpInterface = inConverter.GetClassForType(tmpClass, inClass.FullUsingList);
                if (tmpInterface == null)
                {
                    continue;
                }
                if (tmpInterface.IsInterface())
                {
                    tmpOverridenFromInterface = false;
                    break;
                }
                else if (inConverter.MapAndSortAttributes(
                   tmpInterface.MethodeList
                   .First(inItem => inItem.Name == tmpMethode.Name)
                   .ModifierList).Contains("override"))
                {
                    tmpOverridenFromInterface = true;
                    break;
                }
            }
            //Check Children of Children
            if (tmpOverridenFromInterface == null)
            {
                foreach (var tmpClass in inClass.InterfaceList)
                {
                    var tmpInterface = inConverter.GetClassForType(tmpClass, inClass.FullUsingList);
                    if (tmpInterface == null)
                    {
                        continue;
                    }
                    tmpOverridenFromInterface = IsOverrideRequiredForMethode(tmpInterface, inConverter, tmpMethode);
                    if (tmpOverridenFromInterface != null)
                    {
                        break;
                    }
                }
            }
            return tmpOverridenFromInterface ?? true;
        }

        /// <summary>
        /// Add Using to List if not already Added
        /// </summary>
        /// <param name="tmpRequiredUsings"></param>
        /// <param name="inNewUsing"></param>
        private static string DoTypeMap(this INameConverter inConverter, string inOldType, ClassContainer inClass, List<string> tmpRequiredUsings)
        {
            //TODO MultiType Mapping (KeyVal<TKey,TVal>)

            var tmpNewType = inConverter.MapType(inOldType, inClass.UsingList).Split('.');

            var tmpNewUsing = string.Join(".", tmpNewType.SkipLast(1));
            AddUsingIfRequired(tmpRequiredUsings, tmpNewUsing);

            return tmpNewType.Last();
        }

        private static void AddUsingIfRequired(List<string> tmpRequiredUsings, string tmpNewUsing)
        {
            if (!string.IsNullOrEmpty(tmpNewUsing) && !tmpRequiredUsings.Contains(tmpNewUsing))
            {
                tmpRequiredUsings.Add(tmpNewUsing);
            }
        }
    }
}
