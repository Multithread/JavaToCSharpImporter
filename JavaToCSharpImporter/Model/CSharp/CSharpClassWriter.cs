using Antlr4.Runtime.Tree;
using JavaToCSharpConverter.Helper;
using JavaToCSharpConverter.Interface;
using JavaToCSharpConverter.Model.OOP;
//using MoreLinq;
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
            var tmpCodeState = new CodeState(inConverter, inClass.UsingList);
            var tmpStringBuilder = new StringBuilder();

            //Usings need to be added at the very end.
            var tmpRequiredUsings = new List<string>();
            tmpStringBuilder.AppendLine($"namespace {inConverter.ChangeNamespace(inClass.Namespace)}{Environment.NewLine}{{");

            if (!string.IsNullOrEmpty(inClass.Comment))
            {
                tmpStringBuilder.AppendLine(inClass.Comment);
            }

            tmpStringBuilder.Append(string.Join(" ", inConverter.MapAndSortAttributes(inClass.AttributeList)));

            //Resolve Type to String
            tmpStringBuilder.Append($" {TypeToString(inClass, inConverter, tmpRequiredUsings, inClass.Type)}");
            if (inClass.InterfaceList.Count > 0)
            {
                //Interface Type Mapping
                tmpStringBuilder.Append(" : " + string.Join(" ", inClass.InterfaceList.Select(inItem
                    => inConverter.DoTypeMap(inItem, inClass, tmpRequiredUsings))));
            }

            var tmpClassExtends = new List<Tuple<string, string>>();
            ManageTypeExtends(inClass, inConverter, tmpClassExtends, inClass.Type);

            if (tmpClassExtends.Count > 0)
            {
                tmpStringBuilder.Append($"where {string.Join(",", tmpClassExtends.Select(inItem => $"{(inItem.Item1 == "?" ? "OtherType" : inItem.Item1)} : {inItem.Item2}"))}");
            }

            tmpStringBuilder.AppendLine("");
            tmpStringBuilder.AppendLine("{");

            //Set Fields
            foreach (var tmpField in inClass.FieldList)
            {
                var tmpType = tmpField.Type;
                if (tmpType.Name == "Class")
                {
                    tmpType.Name = tmpType.GenericTypes.First().Name;
                    tmpType.GenericTypes.Clear();
                    if (tmpType.Name == "?")
                    {
                        tmpType.Name = "object";
                    }
                }
                //Map Type to New and Manage Usings
                var tmpTypeString = TypeToString(inClass, inConverter, tmpRequiredUsings, tmpType);

                tmpStringBuilder.Append($"{string.Join(" ", inConverter.MapAndSortAttributes(tmpField.ModifierList, true))} {tmpTypeString} {tmpField.Name}");
                if (tmpField.HasDefaultValue)
                {
                    if (string.IsNullOrEmpty(tmpField.DefaultValue))
                    {
                        //TODO Methode runner for ANTLR
                        tmpStringBuilder.Append($" = ");

                        RewriteAntlrFunctionCode(tmpStringBuilder, tmpField.AntlrDefaultValue, inClass, inConverter, tmpRequiredUsings, tmpCodeState);
                        tmpStringBuilder.Append(";");
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
                tmpStringBuilder.AppendLine();
                tmpStringBuilder.AppendLine();
            }

            //Set Methodes
            foreach (var tmpMethode in inClass.MethodeList)
            {
                var tmpNewReturnType = string.IsNullOrEmpty(tmpMethode.ReturnType.ToString()) ? "void" : inConverter.DoTypeMap(tmpMethode.ReturnType.ToString(), inClass, tmpRequiredUsings);
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

                tmpStringBuilder.Append($"{string.Join(" ", tmpMethodeModifier)} {tmpNewReturnType} {tmpNewMethodeName}");

                tmpCodeState.ClearVariableList();

                var tmpMethodeGenericWhere = new List<Tuple<string, string>>();
                var tmpGenericList = new List<string>();
                foreach (var tmpParam in tmpMethode.Parameter)
                {
                    //Add Parameter Name to current Variable List
                    tmpCodeState.AddMethodeParam(inClass.Namespace, tmpParam.Name);

                    var tmpType = tmpParam.Type.ToString();
                    if (tmpParam.Type.Name == "Class" && tmpParam.Type.GenericTypes.Count > 0)
                    {
                        var tmpName = tmpParam.Type.GenericTypes.First().Name;
                        if (tmpName == "?")
                        {
                            tmpName = "OtherType";
                            if (tmpMethode.IsConstructor)
                            {
                                //Move Type to object
                                tmpParam.Type.Name = "object";
                                tmpParam.Type.GenericTypes.Clear();

                                tmpName = "object";
                            }
                        }
                        //No known Type
                        if (inConverter.GetClassForType(tmpName, inClass.FullUsingList) == null)
                        {
                            //Also notr definied in Class
                            if (!inClass.Type.GenericTypes.Any(inItem => inItem.Name == tmpName))
                            {
                                tmpGenericList.Add(tmpName);
                            }
                        }
                    }
                    var tmpParamType = tmpParam.Type;
                    ManageTypeExtends(inClass, inConverter, tmpMethodeGenericWhere, tmpParamType);
                }

                if (tmpGenericList.Count > 0 && !tmpMethode.IsConstructor)
                {
                    tmpStringBuilder.Append($"<{string.Join(", ", tmpGenericList)}>");
                }

                //TODO ref und out ergänzen
                tmpStringBuilder.Append($"({string.Join(", ", tmpMethode.Parameter.Select(inItem => TypeToString(inClass, inConverter, tmpRequiredUsings, inItem.Type) + " " + inConverter.ChangeMethodeParameterName(inItem.Name)))})");

                if (tmpMethodeGenericWhere.Count > 0)
                {
                    tmpStringBuilder.Append($"where {string.Join(",", tmpMethodeGenericWhere.Select(inItem => $"{(inItem.Item1 == "?" ? "OtherType" : inItem.Item1)} : {inItem.Item2}"))}");
                }
                tmpStringBuilder.AppendLine();
                if (tmpMethode.AntlrCode != null)
                {
                    //Parse Antlr Function Code to Java
                    RewriteAntlrFunctionCode(tmpStringBuilder, tmpMethode.AntlrCode, inClass, inConverter, tmpRequiredUsings, tmpCodeState);
                }
                //else if (!string.IsNullOrEmpty(tmpMethode.Code))
                //{
                //    tmpStringBuilder.AppendLine($"{{");

                //    //Rewrite the Linear Code
                //    RewriteFunctionCode(tmpStringBuilder, tmpMethode, inClass, inConverter, tmpRequiredUsings, tmpCodeState);

                //    if (!tmpMethode.Code.EndsWith("}"))
                //    {
                //        tmpStringBuilder.AppendLine($"}}");
                //    }
                //}
                else
                {
                    tmpStringBuilder.Append($";");
                }
                tmpStringBuilder.AppendLine();
                tmpStringBuilder.AppendLine();
            }
            tmpStringBuilder.AppendLine("");
            tmpStringBuilder.AppendLine($"}}{Environment.NewLine}}}");

            var tmpMoreUsings = "";
            if (tmpRequiredUsings.Count > 0)
            {
                tmpMoreUsings = string.Join(Environment.NewLine, tmpRequiredUsings.Select(inItem => $"using {inConverter.ChangeNamespace(inItem)};"));
                tmpMoreUsings += Environment.NewLine;
            }

            return inClass.NamespaceComment + Environment.NewLine + tmpMoreUsings + tmpStringBuilder.ToString();
        }

        private static void ManageTypeExtends(ClassContainer inClass, INameConverter inConverter, List<Tuple<string, string>> tmpMethodeGenericWhere, TypeContainer tmpParamType)
        {
            if (tmpParamType.Extends.Count > 0)
            {
                tmpMethodeGenericWhere.AddRange(tmpParamType.Extends.Select(inItem => new Tuple<string, string>(tmpParamType.Name, inItem)));
            }
            foreach (var tmpExtendType in tmpParamType.GenericTypes.Append(tmpParamType))
            {
                foreach (var tmpExtends in tmpExtendType.Extends)
                {
                    if (inClass.Type.GenericTypes.Any(inItem => inItem.Name == tmpExtends))
                    {
                        //continue;
                    }
                    if (inConverter.GetClassForType(tmpExtends, inClass.FullUsingList) == null)
                    {
                        tmpMethodeGenericWhere.Add(new Tuple<string, string>(tmpExtendType.Name, tmpExtends));
                    }
                }
            }
        }

        private static string TypeToString(ClassContainer inClass, INameConverter inConverter, List<string> tmpRequiredUsings, TypeContainer tmpType)
        {
            if (tmpType == null)
            {
                return "TypenUll";
            }
            if (tmpType.Name == "Class" && tmpType.GenericTypes.Count > 0)
            {
                return TypeToString(inClass, inConverter, tmpRequiredUsings, tmpType.GenericTypes.First());
            }

            //? Type Mapping to something else. Might be done better later
            if (tmpType.Name == "?")
            {
                tmpType.Name = "OtherType";
            }

            var tmpTypeString2 = DoTypeMap(inConverter, tmpType.Name, inClass, tmpRequiredUsings);
            if (tmpType.GenericTypes.Count > 0)
            {
                tmpTypeString2 += "<";
                bool tmpIsFirst = true;
                foreach (var tmpIdentifier in tmpType.GenericTypes)
                {
                    if (!tmpIsFirst)
                    {
                        tmpTypeString2 += ",";
                    }
                    tmpTypeString2 += TypeToString(inClass, inConverter, tmpRequiredUsings, tmpIdentifier);
                    tmpIsFirst = false;
                }
                tmpTypeString2 += ">";
            }
            if (tmpType.Extends.Count > 0)
            {
                //Handle Extends somehow
            }
            if (tmpType.IsArray)
            {
                tmpTypeString2 += "[]";
            }
            return tmpTypeString2;
        }

        /// <summary>
        /// Rewrite Linear code from Functions
        /// </summary>
        private static void RewriteAntlrFunctionCode(StringBuilder inStringBuilder, IParseTree inTreeElement, ClassContainer inClass, INameConverter inConverter, List<string> inRequiredUsings, CodeState inCodeState = null)
        {
            if (inTreeElement == null)
            {
                return;
            }
            if (inTreeElement.GetText() == ("booleanoverridden=false;"))
            {

            }
            var tmpType = inTreeElement.GetType().Name;
            if (inTreeElement is VariableInitializerContext)
            {
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is CreatedNameContext)
            {
                var tmpNameContext = inTreeElement as CreatedNameContext;
                ManageNamedContext(inStringBuilder, tmpNameContext, inClass, inConverter, inRequiredUsings, inCodeState);
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
                    else if (tmpLiteralContext.BOOL_LITERAL() != null)
                    {
                        inStringBuilder.Append(tmpLiteralContext.BOOL_LITERAL().GetText());
                    }
                    else if (tmpLiteralContext.NULL_LITERAL() != null)
                    {
                        inStringBuilder.Append("null");
                    }
                    else
                    {
                        throw new NotImplementedException("Missing");
                    }
                }
                else if (tmpPrimaryContext.IDENTIFIER() != null)
                {
                    inStringBuilder.Append(tmpPrimaryContext.IDENTIFIER());
                }
                else if (tmpPrimaryContext.THIS() != null)
                {
                    inStringBuilder.Append("this.");
                }
                else
                {
                    throw new NotImplementedException("Missing");

                }
                return;
            }
            else if (inTreeElement is CreatorContext)
            {
                var tmpCreator = inTreeElement as CreatorContext;

                var tmpNameContext = tmpCreator.createdName();
                ManageNamedContext(inStringBuilder, tmpNameContext, inClass, inConverter, inRequiredUsings, inCodeState);

                if (tmpCreator.classCreatorRest() != null)
                {
                    var tmpClassCreator = tmpCreator.classCreatorRest();
                    //TODO Handle creation of class

                    inStringBuilder.Append(tmpClassCreator.GetText());
                    return;
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
                //inStringBuilder.AppendLine(";");
            }
            else if (inTreeElement is ExpressionContext)
            {
                var tmpExpr = inTreeElement as ExpressionContext;
                HandleExpressionContext(inStringBuilder, tmpExpr, inClass, inConverter, inRequiredUsings, inCodeState);
            }
            else if (inTreeElement is MethodCallContext)
            {
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is ExpressionListContext)
            {
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            //Write Comments with Newline at end
            else if (inTreeElement is ErrorNodeImpl)
            {
                inStringBuilder.AppendLine(inTreeElement.GetText());
            }
            else if (inTreeElement is ITerminalNode)
            {
                if (inTreeElement.GetText() == "{")
                {
                    inCodeState.AddSpacing();
                }
                if (inTreeElement.GetText() == "}")
                {
                    inCodeState.ReduceSpacing();
                }
                //Just for output
                inStringBuilder.Append(inTreeElement.GetText());
                if (inTreeElement.GetText() == "{")
                {
                    //inStringBuilder.AppendLine("");
                }
            }
            else if (inTreeElement is BlockContext)
            {
                var tmpBlock = inTreeElement as BlockContext;
                foreach (var tmpElement in tmpBlock.GetChildren())
                {
                    inStringBuilder.Append(inCodeState.GetSpacing());
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is StatementContext)
            {
                var tmpStatement = inTreeElement as StatementContext;
                //Assert remove. Closed might be #if (DEDBUG)
                if (tmpStatement.ASSERT() != null)
                {
                    inStringBuilder.AppendLine("//" + tmpStatement.GetText());
                    return;
                }
                var tmpAddSpace = false;
                if (tmpStatement.THROW() != null || tmpStatement.RETURN() != null)
                {
                    tmpAddSpace = true;
                }
                if (tmpStatement.FOR() != null)
                {
                    //inStringBuilder.Append("for(");

                    //RewriteAntlrFunctionCode(inStringBuilder, tmpStatement.forControl(), inClass, inConverter, inRequiredUsings, inCodeState);

                    //inStringBuilder.AppendLine(")");
                    //foreach (var tmpElement in tmpStatement.statement())
                    //{
                    //    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                    //}
                    //    return;
                }
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                    if (tmpAddSpace)
                    {
                        tmpAddSpace = false;
                        inStringBuilder.Append(" ");
                    }
                }
                inStringBuilder.AppendLine("");
            }
            else if (inTreeElement is ParExpressionContext)
            {
                inStringBuilder.Append("(");
                foreach (var tmpElement in (inTreeElement as ParExpressionContext).expression().GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
                inStringBuilder.Append(")");
            }
            else if (inTreeElement is CatchClauseContext)
            {
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is CatchTypeContext)
            {
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is MethodBodyContext)
            {
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is LocalVariableDeclarationContext)
            {
                var tmpVardec = inTreeElement as LocalVariableDeclarationContext;
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is TypeTypeContext)
            {
                inCodeState.CurrentType = inTreeElement.GetText();
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is VariableDeclaratorsContext)
            {
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is VariableDeclaratorContext)
            {
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is QualifiedNameContext)
            {
                inStringBuilder.Append(inTreeElement.GetText() + " ");
            }
            else if (inTreeElement is PrimitiveTypeContext)
            {
                var tmpTypeString = inTreeElement.GetText();

                tmpTypeString = inConverter.DoTypeMap(tmpTypeString, inClass);

                inStringBuilder.Append(tmpTypeString + " ");
            }
            else if (inTreeElement is BlockStatementContext)
            {
                var tmpBlockStatement = inTreeElement as BlockStatementContext;
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is VariableDeclaratorIdContext)
            {
                if (inCodeState.CurrentType != null)
                {
                    inCodeState.AddVariable(inCodeState.CurrentType.ToString(), inTreeElement.GetText());
                    inCodeState.CurrentType = null;
                }
                inStringBuilder.Append(inTreeElement.GetText() + " ");
            }
            else if (inTreeElement is ForControlContext)
            {
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is ForInitContext)
            {
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is ClassOrInterfaceTypeContext)
            {
                if (inTreeElement.GetText() == "Class<?>")
                {
                    inStringBuilder.Append("object ");
                    inCodeState.CurrentType = "object";
                    return;
                }
                var tmpTypeContext = inTreeElement as ClassOrInterfaceTypeContext;
                foreach (var tmpIdentifier in tmpTypeContext.IDENTIFIER())
                {
                    var tmpIdentifierText = tmpIdentifier.GetText();
                    tmpIdentifierText = inConverter.DoTypeMap(tmpIdentifierText, inClass);

                    inStringBuilder.Append(tmpIdentifierText);

                    inCodeState.CurrentType = tmpIdentifierText;
                }

                foreach (var tmpElement in tmpTypeContext.typeArguments())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
                inStringBuilder.Append(" ");
            }
            else if (inTreeElement is TypeArgumentsContext)
            {
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is TypeArgumentContext)
            {
                HandleTypeArgumentContext(inStringBuilder, inTreeElement, inClass, inConverter, inRequiredUsings, inCodeState);
            }
            else if (inTreeElement is VariableModifierContext)
            {
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is EnhancedForControlContext)
            {
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is AnnotationContext)
            {
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is ElementValueContext)
            {
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is ExplicitGenericInvocationContext)
            {
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is NonWildcardTypeArgumentsContext)
            {
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is TypeListContext)
            {
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is ExplicitGenericInvocationSuffixContext)
            {
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is ArgumentsContext)
            {
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is LambdaExpressionContext)
            {
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is LambdaParametersContext)
            {
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is LambdaBodyContext)
            {
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is ElementValueArrayInitializerContext)
            {
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is ArrayInitializerContext)
            {
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is SwitchBlockStatementGroupContext)
            {
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is SwitchLabelContext)
            {
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is FinallyBlockContext)
            {
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is LocalTypeDeclarationContext)
            {
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is ClassOrInterfaceModifierContext)
            {
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is FormalParameterListContext)
            {
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is FormalParameterContext)
            {
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is ResourceSpecificationContext)
            {
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is ResourcesContext)
            {
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is ResourceContext)
            {
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else
            {
                throw new NotImplementedException("Missing");
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    //RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
        }

        private static void HandleTypeArgumentContext(StringBuilder inStringBuilder, IParseTree inTreeElement, ClassContainer inClass, INameConverter inConverter, List<string> inRequiredUsings, CodeState inCodeState)
        {
            foreach (var tmpElement in inTreeElement.GetChildren())
            {
                RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
            }
        }

        private static void ManageNamedContext(StringBuilder inStringBuilder, CreatedNameContext tmpNameContext, ClassContainer inClass, INameConverter inConverter, List<string> inRequiredUsings, CodeState inCodeState)
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
            if (tmpNameContext.typeArgumentsOrDiamond().Length > 0)
            {
                inStringBuilder.Append("<");
                foreach (var tmpIdentifier in tmpNameContext.typeArgumentsOrDiamond().Where(inItem => inItem.typeArguments() != null).SelectMany(inItem => inItem.typeArguments().typeArgument()))
                {
                    if (tmpIdentifier is TypeArgumentContext)
                    {
                        HandleTypeArgumentContext(inStringBuilder, tmpIdentifier, inClass, inConverter, inRequiredUsings, inCodeState);
                    }
                    else
                    {
                        throw new NotImplementedException("Unahndled NamedContext State");
                    }
                }
                inStringBuilder.Append(">");
            }
        }

        private static void HandleExpressionContext(StringBuilder inStringBuilder, ExpressionContext tmpExpr, ClassContainer inClass, INameConverter inConverter, List<string> inRequiredUsings, CodeState inCodeState)
        {
            foreach (var tmpElement in tmpExpr.GetChildren())
            {
                if (tmpElement is PrimaryContext)
                {
                    var tmpText = tmpElement.GetText();
                    if (tmpElement.GetChild(0) is LiteralContext)
                    {
                        inStringBuilder.Append(tmpText);
                    }
                    else
                    {
                        if (tmpText == "this")
                        {
                            inStringBuilder.Append("this");
                            inCodeState.CurrentType = inConverter.GetClassForType(inClass.Name, inClass.FullUsingList).Type;
                        }
                        else if (tmpText.StartsWith("\""))
                        {
                            //Add String Primary Context to Result
                            inStringBuilder.Append(tmpText);
                        }
                        else
                        {
                            tmpText = ManageNames(inStringBuilder, inClass, inConverter, inCodeState, tmpText, inRequiredUsings);
                        }
                    }
                }
                else if (tmpElement is TerminalNodeImpl)
                {
                    if (tmpExpr.NEW() != null)
                    {
                        inStringBuilder.Append("new ");
                        inCodeState.CurrentType = null;
                    }
                    else if (tmpExpr.SUPER() != null)
                    {
                        inStringBuilder.Append("base");
                        var tmpClass = inConverter.GetClassForType(inClass.Name, inClass.UsingList);
                        //TODO Get Parent Type,
                        throw new NotImplementedException("Base not handled");
                        inCodeState.CurrentType = null;
                    }
                    else if (tmpElement.GetText() == ".")
                    {
                        inStringBuilder.Append(".");
                    }
                    else
                    {
                        //var tmpElementText = tmpElement.GetText();
                        ////if (new[] { "==", "!=", "<=", ">=", "||", "&&", }.Contains(tmpElement.GetText()))
                        //if (!WordRegex.IsMatch(tmpElementText))
                        //{
                        //    inCodeState.CurrentType = null;
                        //    inStringBuilder.Append(tmpElementText);
                        //}
                        //else
                        {
                            var tmpText = tmpElement.GetText();
                            tmpText = ManageNames(inStringBuilder, inClass, inConverter, inCodeState, tmpText, inRequiredUsings);
                        }
                    }
                }
                else
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
        }

        private static string ManageNames(StringBuilder inStringBuilder, ClassContainer inClass, INameConverter inConverter, CodeState inCodeState, string tmpText, List<string> inRequiredUsings)
        {
            if (WordRegex.IsMatch(tmpText))
            {
                if (inCodeState.CurrentType == null && inCodeState.HasVariable(tmpText))
                {
                    if (inCodeState.IsVariableMethodeParam(tmpText))
                    {
                        tmpText = inConverter.ChangeMethodeParameterName(tmpText);
                    }
                    inCodeState.CurrentType = null;
                }
                else
                {
                    //Get Type for Recognition
                    var tmpType = inCodeState.CurrentType;
                    if (tmpType == null)
                    {
                        tmpType = inClass.Type;
                    }

                    //Correct Type Handling required
                    var tmpClass = inConverter.GetClassForType(tmpType.Name, inClass.FullUsingList);
                    if (tmpClass != null && tmpClass.MethodeList.Any(inItem => inItem.Name == tmpText))
                    {
                        tmpText = inConverter.MapFunction(tmpText, tmpType.Name, inClass.FullUsingList);
                    }
                    else if (tmpClass != null && tmpClass.FieldList.Any(inItem => inItem.Name == tmpText))
                    {
                        tmpText = inConverter.ChangeFieldName(tmpText);
                    }
                    else if (inConverter.GetClassForType(tmpText, inClass.FullUsingList) != null)
                    {
                        //Type Name Mapping
                        tmpText = inConverter.DoTypeMap(tmpText, inClass);
                    }
                    else if (inCodeState.HasVariable(tmpText))
                    {
                        //Nothing todo when we have a Variable
                    }
                    else if (inClass.FieldList.Any(inItem => inItem.Name == tmpText))
                    {
                        //Nothing todo when we have a Field-Value
                    }
                    else if (inClass.MethodeList.Any(inItem => inItem.Name == tmpText))
                    {
                        //Nothing todo when we have a Field-Value
                    }
                    else
                    {
                        //Maybe nothing to to?
                        //Might need to Create the Missing Type, if Required

                        //TODO Find out how to only add real Types and not Variable or methode Names (maybe by Javas Name Conventions?)
                        if (inConverter is IMissingTypes)
                        {
                            (inConverter as IMissingTypes).AddMissingClass(tmpText);
                            AddUsingIfRequired(inRequiredUsings, "MigrationHelper");
                        }
                    }
                }
            }
            else
            {
                if (tmpText != ".")
                {
                    inCodeState.CurrentType = null;
                }
            }

            inStringBuilder.Append(tmpText);

            return tmpText;
        }

        private static Regex WordRegex = new Regex("\\w", RegexOptions.IgnoreCase);

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
        private static string DoTypeMap(this INameConverter inConverter, string inOldType, ClassContainer inClass, List<string> tmpRequiredUsings = null)
        {
            if (tmpRequiredUsings == null)
            {
                tmpRequiredUsings = inClass.FullUsingList;
            }
            //TODO MultiType Mapping (KeyVal<TKey,TVal>)

            var tmpNewType = inConverter.MapType(inOldType, inClass.FullUsingList).Split('.');

            var tmpNewUsing = string.Join(".", MoreLinq.Extensions.SkipLastExtension.SkipLast(tmpNewType, 1));
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
