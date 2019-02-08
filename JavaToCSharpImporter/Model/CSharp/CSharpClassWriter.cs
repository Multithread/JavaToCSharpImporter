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
            tmpStringBuilder.AppendLine("");
            tmpStringBuilder.AppendLine("{");

            var tmpDepth = 1;
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

                tmpStringBuilder.Append(GetLeftSpace(tmpDepth) + $"{string.Join(" ", inConverter.MapAndSortAttributes(tmpField.ModifierList, true))} {tmpTypeString} {tmpField.Name}");
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

                tmpCodeState.ClearVariableList();

                var tmpMethodeGenericWhere = new List<string>();
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
                    if (tmpParam.Type.Extends.Count > 0)
                    {
                        tmpMethodeGenericWhere.AddRange(tmpParam.Type.Extends);
                    }
                    foreach (var tmpExtends in tmpParam.Type.GenericTypes.SelectMany(inItem => inItem.Extends).Concat(tmpParam.Type.Extends))
                    {
                        if (inClass.Type.GenericTypes.Any(inItem => inItem.Name == tmpExtends))
                        {
                            continue;
                        }
                        if (inConverter.GetClassForType(tmpExtends, inClass.FullUsingList) == null)
                        {
                            tmpMethodeGenericWhere.Add(tmpExtends);
                        }
                    }
                }

                if (tmpGenericList.Count > 0 && !tmpMethode.IsConstructor)
                {
                    tmpStringBuilder.Append($"<{string.Join(", ", tmpGenericList)}>");
                }

                //TODO ref und out ergänzen
                tmpStringBuilder.Append($"({string.Join(", ", tmpMethode.Parameter.Select(inItem => TypeToString(inClass, inConverter, tmpRequiredUsings, inItem.Type) + " " + inConverter.ChangeMethodeParameterName(inItem.Name)))})");

                if (tmpMethodeGenericWhere.Count > 0)
                {
                   // throw new NotImplementedException("Generic Where to be Implemented");
                }
                tmpStringBuilder.AppendLine();
                if (tmpMethode.AntlrCode != null)
                {
                    //Parse Antlr Function Code to Java
                    RewriteAntlrFunctionCode(tmpStringBuilder, tmpMethode.AntlrCode, inClass, inConverter, tmpRequiredUsings, tmpCodeState);
                }
                else if (!string.IsNullOrEmpty(tmpMethode.Code))
                {
                    tmpStringBuilder.AppendLine(GetLeftSpace(tmpDepth) + $"{{");

                    //Rewrite the Linear Code
                    RewriteFunctionCode(tmpStringBuilder, tmpMethode, inClass, inConverter, tmpRequiredUsings, tmpCodeState);

                    if (!tmpMethode.Code.EndsWith("}"))
                    {
                        tmpStringBuilder.AppendLine(GetLeftSpace(tmpDepth) + $"}}");
                    }
                }
                else
                {
                    tmpStringBuilder.Append($";");
                }
                tmpStringBuilder.AppendLine();
                tmpStringBuilder.AppendLine();
            }
            tmpStringBuilder.AppendLine("");
            tmpStringBuilder.AppendLine(GetLeftSpace(1) + $"}}{Environment.NewLine}}}");

            var tmpMoreUsings = "";
            if (tmpRequiredUsings.Count > 0)
            {
                tmpMoreUsings = string.Join(Environment.NewLine, tmpRequiredUsings.Select(inItem => $"using {inConverter.ChangeNamespace(inItem)};"));
                tmpMoreUsings += Environment.NewLine;
            }

            return tmpMoreUsings + tmpStringBuilder.ToString();
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
        /// Rewrite Linear code from Functions
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
                inStringBuilder.AppendLine(GetLeftSpace(1));
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
                foreach (var tmpElement in inTreeElement.GetChildren())
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
            else if (inTreeElement is TypeTypeContext)
            {
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
                    return;
                }
                var tmpTypeContext = inTreeElement as ClassOrInterfaceTypeContext;
                foreach (var tmpIdentifier in tmpTypeContext.IDENTIFIER())
                {
                    var tmpIdentifierText = tmpIdentifier.GetText();
                    tmpIdentifierText = inConverter.DoTypeMap(tmpIdentifierText, inClass);

                    inStringBuilder.Append(tmpIdentifierText);
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
                        tmpText = ManageNames(inStringBuilder, inClass, inConverter, inCodeState, tmpText);
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
                    else
                    {
                        var tmpText = tmpElement.GetText();
                        tmpText = ManageNames(inStringBuilder, inClass, inConverter, inCodeState, tmpText);
                    }
                }
                else
                {
                    RewriteAntlrFunctionCode(inStringBuilder, tmpElement, inClass, inConverter, inRequiredUsings, inCodeState);
                }
            }
        }

        private static string ManageNames(StringBuilder inStringBuilder, ClassContainer inClass, INameConverter inConverter, CodeState inCodeState, string tmpText)
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
                    if (tmpClass.MethodeList.Any(inItem => inItem.Name == tmpText))
                    {
                        tmpText = inConverter.MapFunction(tmpText, tmpType.Name, inClass.FullUsingList);
                    }
                    else
                    {
                        tmpText = inConverter.ChangeFieldName(tmpText);
                    }
                }
            }

            if (tmpText == "=")
            {
                inStringBuilder.Append(" = ");
                inCodeState.CurrentType = null;
            }
            else if (tmpText == ".")
            {
                inStringBuilder.Append(".");
            }
            else
            {
                inStringBuilder.Append(tmpText);
                if (WordRegex.IsMatch(tmpText))
                {
                    //inStringBuilder.Append(" ");
                }
            }

            return tmpText;
        }

        private static Regex WordRegex = new Regex("\\w");


        /// <summary>
        /// Rewrite Linnear code from Functions
        /// </summary>
        private static void RewriteFunctionCode(StringBuilder inStringBuilder, MethodeContainer inMethode, ClassContainer inClass, INameConverter inConverter, List<string> inRequiredUsings, CodeState inCodeState = null)
        {
            inCodeState.ClearVariableList();
            //Add Methode params to State
            foreach (var tmpParam in inMethode.Parameter)
            {
                inCodeState.AddVariable(tmpParam.Type.Name, tmpParam.Name);
            }
            //Add Fields of Class
            foreach (var tmpParam in inClass.FieldList)
            {
                inCodeState.AddVariable(tmpParam.Type.Name, tmpParam.Name, false);
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

                            var tmpIsFirstInFunction = false;
                            var tmpIndex = 1;
                            while (tmpRemainingText.Length > tmpIndex)
                            {
                                if (tmpRemainingText[tmpIndex] == '(')
                                {
                                    tmpIsFirstInFunction = true;
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

                            if (tmpIsFirstInFunction)
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
        private static string DoTypeMap(this INameConverter inConverter, string inOldType, ClassContainer inClass, List<string> tmpRequiredUsings = null)
        {
            if (tmpRequiredUsings == null)
            {
                tmpRequiredUsings = inClass.FullUsingList;
            }
            //TODO MultiType Mapping (KeyVal<TKey,TVal>)

            var tmpNewType = inConverter.MapType(inOldType, inClass.FullUsingList).Split('.');

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
