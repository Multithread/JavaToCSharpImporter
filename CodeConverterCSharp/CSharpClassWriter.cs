using CodeConverterCore.Converter;
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
        private IConverter Converter;
        public CSharpClassWriter(IConverter inConverter)
        {
            Converter = inConverter;
        }

        /// <summary>
        /// Crete ClassFileInfo for a single Class
        /// </summary>
        /// <param name="inClass"></param>
        /// <returns></returns>
        public FileWriteInfo CreateClassFile(ClassContainer inClass)
        {
            if (!inClass.IsConverted)
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

            //Create Class end and return FileWriteInfo
            tmpStringBuilder.AppendLine(CreateIndent(tmpIndentDepth) + "}");
            tmpStringBuilder.AppendLine("}");
            tmpInfo.Content = tmpStringBuilder.ToString();
            return tmpInfo;
        }

        private void AddFieldtoString(StringBuilder inOutput, FieldContainer inField, int inIndentDepth)
        {
            AddComment(inOutput, inField.Comment, inIndentDepth, true);
            var tmpFieldString = $"{ReturnModifiersOrdered(inField.ModifierList)} {CreateStringFromType(inField.Type)} {inField.Name};";

            inOutput.AppendLine(CreateIndent(inIndentDepth) + tmpFieldString);
        }

        private void AddMethodeToString(StringBuilder inOutput, MethodeContainer inMethode, int inIndentDepth)
        {
            AddComment(inOutput, inMethode.Comment, inIndentDepth, true);
            var tmpMethodeString = $"{ReturnModifiersOrdered(inMethode.ModifierList)} {CreateStringFromType(inMethode.ReturnType)} {inMethode.Name}(";
            tmpMethodeString += string.Join("", inMethode.Parameter.Select(inItem => $"{CreateStringFromType(inItem.Type)} {inItem.Name}{(inItem.HasDefaultValue ? " = " + inItem.DefaultValue : "")}")) + ")";
            inOutput.AppendLine(CreateIndent(inIndentDepth) + tmpMethodeString);

            inOutput.AppendLine(CreateIndent(inIndentDepth) + "{");



            inOutput.AppendLine(CreateIndent(inIndentDepth) + "}");
        }

        /// <summary>
        /// Create C# String from TypeContainer
        /// This createsa type with all the required elements
        /// </summary>
        /// <param name="inType"></param>
        /// <returns></returns>
        private string CreateStringFromType(TypeContainer inType)
        {
            if (inType.GenericTypes.Count > 0)
            {
                return $"{inType.Type?.Name ?? inType.Name}<{string.Join(" ,", inType.GenericTypes.Select(inItem => CreateStringFromType(inItem)))}>{(inType.IsArray ? "[]" : "")}";
            }
            return $"{inType.Type?.Name ?? inType.Name}{(inType.IsArray ? "[]" : "")}";
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
            var tmpConverted = Converter.Comment(inComment, inDefinitionCommennt);
            tmpConverted = CreateIndent(inIndentDepth) + tmpConverted.Replace(Environment.NewLine, Environment.NewLine + CreateIndent(inIndentDepth));

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
            return $"{ReturnModifiersOrdered(inClass.AttributeList)} {inClass.Type.Type.Name}";
        }

        private static string ReturnModifiersOrdered(List<string> inModifierList)
        {
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
                       return 25;
                   case "static":
                       return 33;
                   case "class":
                       return 100;
                   case "interface":
                       return 101;
                   default:
                       throw new NotImplementedException("Unknown Attribute");
               }
           }).ToList());
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
