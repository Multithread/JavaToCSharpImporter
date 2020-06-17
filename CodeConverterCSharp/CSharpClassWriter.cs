using CodeConverterCore.Model;
using CodeConverterCSharp.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeConverterCSharp
{
    public static class CSharpClassWriter
    {
        /// <summary>
        /// Crete ClassFileInfo for a single Class
        /// </summary>
        /// <param name="inClass"></param>
        /// <returns></returns>
        public static FileWriteInfo CreateClassFile(ClassContainer inClass)
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
            tmpStringBuilder.AppendLine(CreateIndent(tmpIndentDepth) + CreateClassDefinition(inClass)); ;
            tmpStringBuilder.AppendLine(CreateIndent(tmpIndentDepth) + "{");

            foreach (var tmpField in inClass.FieldList)
            {
                AddFieldtoString(tmpStringBuilder, tmpField, tmpIndentDepth + 1);
            }


            //Create Class end and return FileWriteInfo
            tmpStringBuilder.AppendLine(CreateIndent(tmpIndentDepth) + "}");
            tmpStringBuilder.AppendLine("}");
            tmpInfo.Content = tmpStringBuilder.ToString();
            return tmpInfo;
        }

        private static void AddFieldtoString(StringBuilder inOutput, FieldContainer inField, int inIndentDepth)
        {
            inOutput.AppendLine(inField.Comment);
        }

        private static void AddComment(StringBuilder inOutput, string inComment, int inIndentDepth, bool inDefinitionCommennt = false)
        {
            if (inComment == null)
            {
                return;
            }
            if (inDefinitionCommennt)
            {

            }
            if (inComment.Contains(Environment.NewLine))
            {

            }
            else
            {

            }
        }

        /// <summary>
        /// Create the usings at top of the class
        /// </summary>
        /// <param name="inClass"></param>
        /// <returns></returns>
        private static string CreateImports(ClassContainer inClass)
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
        private static string CreateClassDefinition(ClassContainer inClass)
        {
            var tmpClassModifier = inClass.AttributeList.OrderBy(inItem =>
            {
                switch (inItem)
                {
                    case "public":
                        return 10;
                    case "abstract":
                        return 20;
                    case "override":
                        return 25;
                    case "static":
                        return 33;
                    case "class":
                        return 100;
                    default:
                        throw new NotImplementedException("Unknown Attribute");
                }
            });
            return $"{string.Join(" ", tmpClassModifier)} {inClass.Type.Type.Name}";
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
