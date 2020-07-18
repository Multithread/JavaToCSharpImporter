using CodeConverterCore.Converter;
using CodeConverterCore.Enum;
using CodeConverterCore.Interface;
using CodeConverterCore.Model;
using CodeConverterCSharp.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeConverterCSharp
{
    public class CSharpClassWriter
    {

        /// <summary>
        /// Crete ClassFileInfo for a single Class
        /// </summary>
        /// <param name="inClass"></param>
        /// <returns></returns>
        public FileWriteInfo CreateClassFile(ClassContainer inClass)
        {
            if (!inClass.IsAnalyzed)
            {
                throw new Exception("Cannot create String from a Class that has not been typecleaned");
            }
            var tmpInfo = new FileWriteInfo()
            {
                FullName = inClass.Type.Type.Name,
                RelativePath = inClass.Namespace?.Replace('.', Path.PathSeparator),
            };
            var tmpStringBuilder = new StringBuilder();
            tmpStringBuilder.AppendLine(CreateImports(inClass));
            tmpStringBuilder.AppendLine();

            //Namespace
            AddComment(tmpStringBuilder, inClass.NamespaceComment, 0, true);
            tmpStringBuilder.AppendLine($"namespace {inClass.Namespace}");
            tmpStringBuilder.AppendLine("{");
            var tmpIndentDepth = 1;

            AddClassContainerString(inClass, tmpStringBuilder, tmpIndentDepth);

            tmpStringBuilder.AppendLine("}");
            tmpInfo.Content = tmpStringBuilder.ToString();
            return tmpInfo;
        }

        private void AddClassContainerString(ClassContainer inClass, StringBuilder tmpStringBuilder, int tmpIndentDepth)
        {
            //Create Class Header
            AddComment(tmpStringBuilder, inClass.Comment, 1, true);
            tmpStringBuilder.AppendLine(CreateIndent(tmpIndentDepth) + CreateClassDefinition(inClass)); ;
            tmpStringBuilder.AppendLine(CreateIndent(tmpIndentDepth) + "{");

            for (var tmpI = 0; tmpI < inClass.FieldList.Count; tmpI++)
            {
                var tmpField = inClass.FieldList[tmpI];
                AddFieldtoString(tmpStringBuilder, tmpField, tmpIndentDepth + 1);
                tmpStringBuilder.AppendLine("");
            }

            for (var tmpI = 0; tmpI < inClass.MethodeList.Count; tmpI++)
            {
                var tmpMethode = inClass.MethodeList[tmpI];
                AddMethodeToString(tmpStringBuilder, tmpMethode, tmpIndentDepth + 1);
                if (tmpI < inClass.MethodeList.Count - 1)
                {
                    tmpStringBuilder.AppendLine("");
                }
            }

            for (var tmpI = 0; tmpI < inClass.InnerClasses.Count; tmpI++)
            {
                AddClassContainerString(inClass.InnerClasses[tmpI], tmpStringBuilder, tmpIndentDepth + 1);
            }

            //Create Class end and return FileWriteInfo
            tmpStringBuilder.AppendLine(CreateIndent(tmpIndentDepth) + "}");
        }

        private void AddFieldtoString(StringBuilder inOutput, FieldContainer inField, int inIndentDepth)
        {
            AddComment(inOutput, inField.Comment, inIndentDepth, true);
            var tmpFieldString = $"{ReturnModifiersOrdered(inField.ModifierList)}{CreateStringFromType(inField.Type)}{inField.Name}";
            if (inField.DefaultValue != null)
            {
                var tmpSb = new StringBuilder();
                AddCodeBlockToString(tmpSb, inField.DefaultValue, 0);
                tmpFieldString = $"{tmpFieldString} = {tmpSb}";
                inOutput.Append(CreateIndent(inIndentDepth) + tmpFieldString);
            }
            else
            {
                tmpFieldString += ";";
                inOutput.AppendLine(CreateIndent(inIndentDepth) + tmpFieldString);
            }
        }

        private void AddMethodeToString(StringBuilder inOutput, MethodeContainer inMethode, int inIndentDepth)
        {
            AddComment(inOutput, inMethode.Comment, inIndentDepth, true);
            var tmpMethodeString = $"{ReturnModifiersOrdered(inMethode.ModifierList)}{CreateStringFromType(inMethode.ReturnType)}{inMethode.Name}(";

            tmpMethodeString += string.Join(", ", inMethode.Parameter.Select(inItem => $"{CreateStringFromType(inItem.Type)}{inItem.Name}{(inItem.DefaultValue != null ? " = " + inItem.DefaultValue : "")}")) + ")";

            inOutput.AppendLine(CreateIndent(inIndentDepth) + tmpMethodeString);

            if (inMethode.Code == null)
            {
                inOutput.AppendLine(";");
            }
            else
            {
                inOutput.AppendLine(CreateIndent(inIndentDepth) + "{");
                AddCodeBlockToString(inOutput, inMethode.Code, inIndentDepth + 1);
                inOutput.AppendLine(CreateIndent(inIndentDepth) + "}");
            }
        }

        private void AddCodeBlockToString(StringBuilder inOutput, CodeBlock inCode, int inIndentDepth)
        {
            foreach (var tmpEntry in inCode.CodeEntries)
            {
                inOutput.Append(CreateIndent(inIndentDepth));
                AddCodeEntryToString(inOutput, tmpEntry, inIndentDepth);
                var tmpLastChar = inOutput[inOutput.Length - 1];
                if (tmpLastChar == ')' ||
                    tmpLastChar == ']' ||
                    char.IsLetterOrDigit(tmpLastChar))
                {
                    inOutput.AppendLine(";");
                }
            }
        }

        private string CreateStatementCodeBlock(CodeBlock inCode)
        {
            var tmpSb = new StringBuilder();
            foreach (var tmpEntry in inCode.CodeEntries)
            {
                AddCodeEntryToString(tmpSb, tmpEntry, 0);
            }
            return tmpSb.ToString();
        }

        /// <summary>
        /// Write Code-Entry into C# Code
        /// </summary>
        /// <param name="inOutput"></param>
        /// <param name="inCodeEntry"></param>
        private void AddCodeEntryToString(StringBuilder inOutput, ICodeEntry inCodeEntry, int inIndentDepth = 0)
        {
            if (inCodeEntry == null) { return; }
            if (inCodeEntry is VariableDeclaration)
            {
                var tmpVar = inCodeEntry as VariableDeclaration;
                inOutput.Append($"{tmpVar.Type.Name} {tmpVar.Name}");
            }
            else if (inCodeEntry is ConstantValue)
            {
                var tmpConstant = inCodeEntry as ConstantValue;
                if (tmpConstant.Value is BaseType)
                {
                    inOutput.Append($"{(tmpConstant.Value as BaseType).Name}");
                }
                else if (tmpConstant.Value is IName)
                {
                    inOutput.Append($"{(tmpConstant.Value as IName).Name}");
                }
                else if (tmpConstant.Value is ClassContainer)
                {
                    inOutput.Append($"{tmpConstant.Type.Type.Name}");
                    if (tmpConstant.Type.IsArray)
                    {
                        inOutput.Append("[");
                        if (tmpConstant.Type.ArrayInizialiser != null)
                        {
                            var tmpSb = new StringBuilder();
                            AddCodeEntryToString(tmpSb, tmpConstant.Type.ArrayInizialiser);
                            inOutput.Append(tmpSb.ToString());
                        }
                        inOutput.Append("]");
                    }
                }
                else if (tmpConstant.Value is TypeContainer)
                {
                    inOutput.Append($"{tmpConstant.Value}");
                    if (tmpConstant.Type?.ArrayInizialiser != null)
                    {
                        inOutput.Append("[");
                        AddCodeEntryToString(inOutput, tmpConstant.Type.ArrayInizialiser);
                        inOutput.Append("]");
                    }
                }
                else
                {
                    inOutput.Append($"{tmpConstant.Value}");
                }
            }
            else if (inCodeEntry is StatementCode)
            {
                AddStatementToString(inOutput, inCodeEntry as StatementCode, inIndentDepth);
            }
            else if (inCodeEntry is SetFieldWithValue)
            {
                var tmpFieldVal = inCodeEntry as SetFieldWithValue;

                foreach (var tmpEntry in tmpFieldVal.VariableToAccess.CodeEntries)
                {
                    AddCodeEntryToString(inOutput, tmpEntry);
                }
                inOutput.Append(" = ");
                foreach (var tmpEntry in tmpFieldVal.ValueToSet.CodeEntries)
                {
                    AddCodeEntryToString(inOutput, tmpEntry);
                }
            }
            else if (inCodeEntry is VariableAccess)
            {
                var tmpVarAccess = inCodeEntry as VariableAccess;
                AddCodeEntryToString(inOutput, tmpVarAccess.Access);
                if (tmpVarAccess.Child != null)
                {
                    inOutput.Append(".");
                    AddCodeEntryToString(inOutput, tmpVarAccess.Child);
                }
                else if (tmpVarAccess.BaseDataSource != null)
                {
                    inOutput.Append(" = ");
                    AddCodeEntryToString(inOutput, tmpVarAccess.BaseDataSource);
                }
            }
            else if (inCodeEntry is ReturnCodeEntry)
            {
                var tmpReturn = inCodeEntry as ReturnCodeEntry;
                inOutput.Append($"{(tmpReturn.IsYield ? "yield " : "")}return ");
                foreach (var tmpEntry in tmpReturn.CodeEntries)
                {
                    AddCodeEntryToString(inOutput, tmpEntry);
                }
            }
            else if (inCodeEntry is NewObjectDeclaration)
            {
                var tmpReturn = inCodeEntry as NewObjectDeclaration;
                inOutput.Append(" new ");
                AddCodeEntryToString(inOutput, tmpReturn.InnerCode);
            }
            else if (inCodeEntry is MethodeCall)
            {
                var tmpMethodeCall = inCodeEntry as MethodeCall;
                inOutput.Append($"{tmpMethodeCall.Name}({string.Join(",", tmpMethodeCall.Parameter.Select(inItem => AddCodeBlockToString(inItem)))})");
            }
            else if (inCodeEntry is CodeExpression)
            {
                var tmpExpr = inCodeEntry as CodeExpression;
                inOutput.Append("(");
                inOutput.Append(string.Join($" {CSharpStaticInfo.GetOperatorString(tmpExpr.Manipulator)} ",
                    tmpExpr.SubClauseEntries.Select(inItem => CreateStatementCodeBlock(inItem))
                    ));
                inOutput.Append(")");
            }
            else
            {
                throw new Exception("Code Entry Type not Implement");
            }
        }

        private string AddCodeBlockToString(CodeBlock inCodeEntry)
        {
            var tmpSb = new StringBuilder();
            AddCodeBlockToString(tmpSb, inCodeEntry, 0);
            return tmpSb.ToString();
        }

        private void AddStatementToString(StringBuilder inOutput, StatementCode inStatement, int inIndentDepth)
        {
            switch (inStatement.StatementType)
            {
                case StatementTypeEnum.If:
                    var tmpStatementCodeString = CreateStatementCodeBlock(inStatement.StatementCodeBlocks[0]);
                    if (tmpStatementCodeString.StartsWith("("))
                    {
                        inOutput.AppendLine($"if{tmpStatementCodeString}");
                    }
                    else
                    {
                        inOutput.AppendLine($"if({tmpStatementCodeString})");
                    }
                    inOutput.AppendLine(CreateIndent(inIndentDepth) + "{");
                    AddCodeBlockToString(inOutput, inStatement.InnerContent, inIndentDepth + 1);
                    inOutput.AppendLine(CreateIndent(inIndentDepth) + "}");
                    break;
                case StatementTypeEnum.Else:
                    inOutput.AppendLine("else {");
                    AddCodeBlockToString(inOutput, inStatement.InnerContent, inIndentDepth + 1);
                    inOutput.AppendLine(CreateIndent(inIndentDepth) + "}");
                    break;
                default:
                    throw new Exception("Unhandlet Statement Type");
                    break;
            }
        }

        /// <summary>
        /// Create C# String from TypeContainer
        /// This createsa type with all the required elements
        /// </summary>
        /// <param name="inType"></param>
        /// <returns></returns>
        private string CreateStringFromType(TypeContainer inType, bool inAddEndEntry = true)
        {
            if (inType == null)
            {
                return "";
            }
            if (inType.GenericTypes.Count > 0)
            {
                return $"{inType.Type?.Name ?? inType.Name}<{string.Join(" ,", inType.GenericTypes.Select(inItem => CreateStringFromType(inItem, false)))}>{(inType.IsArray ? "[]" : "")}{(inAddEndEntry ? " " : "")}";
            }
            return $"{inType.Type?.Name ?? inType.Name}{(inType.IsArray ? "[]" : "")}{(inAddEndEntry ? " " : "")}";
        }

        /// <summary>
        /// Create Comment from Comment-String
        /// </summary>
        /// <param name="inOutput">Output StringBuilder</param>
        /// <param name="inComment">Comment string (single or Multiline)</param>
        /// <param name="inIndentDepth">How far to Intend</param>
        /// <param name="inDefinitionCommennt">Simple Comment, or Methode/Class definition Comment</param>
        private void AddComment(StringBuilder inOutput, string inComment, int inIndentDepth, bool inDefinitionCommennt = false)
        {
            if (string.IsNullOrWhiteSpace(inComment))
            {
                return;
            }
            var tmpConverted = inComment;
            if (inDefinitionCommennt)
            {
                tmpConverted = CreateIndent(inIndentDepth) + tmpConverted.Replace(Environment.NewLine, Environment.NewLine + CreateIndent(inIndentDepth));
            }

            inOutput.AppendLine(tmpConverted);
        }

        /// <summary>
        /// Create the usings at top of the class
        /// </summary>
        /// <param name="inClass"></param>
        /// <returns></returns>
        private string CreateImports(ClassContainer inClass)
        {
            return string.Join(Environment.NewLine, inClass.UsingList
                .OrderBy(inItem => inItem)
                .Select(inItem => $"using {inItem};"));
        }

        /// <summary>
        /// Create the usings at top of the class
        /// </summary>
        /// <param name="inClass"></param>
        /// <returns></returns>
        private string CreateClassDefinition(ClassContainer inClass)
        {
            return $"{ReturnModifiersOrdered(inClass.ModifierList)}{inClass.Type.Type.Name}{CreateClassInterfaceData(inClass)}";
        }
        /// <summary>
        /// Create the usings at top of the class
        /// </summary>
        /// <param name="inClass"></param>
        /// <returns></returns>
        private string CreateClassInterfaceData(ClassContainer inClass)
        {
            if (inClass.InterfaceList.Count == 0)
            {
                return "";
            }
            var tmpTypes = string.Join(", ", inClass.InterfaceList.Select(inItem => CreateStringFromType(inItem, false)));
            var tmpExtendsList = inClass.InterfaceList.Where(inItem => inItem.Extends.Count > 0);
            var tmpExtends = string.Join(", ", tmpExtendsList);
            if (tmpExtends.Length > 0)
            {
                tmpExtends = " where " + tmpExtends;
                throw new NotImplementedException("Interfaces with Generics with Extends not Implemented yet");
                return $" : {tmpTypes} {tmpExtends}";
            }
            return $" : {tmpTypes}";
        }

        private static string ReturnModifiersOrdered(List<string> inModifierList)
        {
            if (inModifierList.Count == 0)
            {
                return "";
            }
            return string.Join(" ", inModifierList.OrderBy(inItem =>
           {
               switch (inItem)
               {
                   case "public":
                       return 10;
                   case "private":
                       return 12;
                   case "internal":
                       return 13;
                   case "protected":
                       return 14;
                   case "abstract":
                       return 20;
                   case "override":
                       return 30;
                   case "default":
                       return 55;
                   case "readonly":
                       return 60;
                   case "sealed":
                       return 65;
                   case "static":
                       return 80;
                   case "class":
                       return 100;
                   case "interface":
                       return 101;
                   default:
                       throw new NotImplementedException("Unknown Attribute");
               }
           }).ToList()) + " ";
        }

        /// <summary>
        /// Indent Creation
        /// </summary>
        /// <param name="inDepth"></param>
        /// <returns></returns>
        private static string CreateIndent(int inDepth)
        {
            if (!_indent.TryGetValue(inDepth, out var tmpString))
            {
                tmpString = "";
                for (var tmpI = 0; tmpI < inDepth; tmpI++)
                {
                    tmpString += "    ";
                }
                _indent.Add(inDepth, tmpString);
            }
            return tmpString;
        }
        private static Dictionary<int, string> _indent = new Dictionary<int, string>();
    }
}
