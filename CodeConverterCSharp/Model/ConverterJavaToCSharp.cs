using CodeConverterCore.Converter;
using CodeConverterCore.Helper;
using CodeConverterCore.Model;
using System;
using System.Text.RegularExpressions;

namespace JavaToCSharpConverter.Model
{
    public class ConverterJavaToCSharp : ConverterBase
    {
        private Regex ReplaceJavaLineComments = new Regex(Environment.NewLine + "( )*(\\*|//)( )?");
        private Regex EmptyLinesStart = new Regex($"^(( )*{Environment.NewLine})*");
        private Regex EmptyLinesEnd = new Regex($"({Environment.NewLine}( )*)*$");
        /// <summary>
        /// Create Comment from Comment-String
        /// </summary>
        /// <param name="inComment">Comment string (single or Multiline)</param>
        /// <param name="inDefinitionCommennt">Simple Comment, or Methode/Class definition Comment</param>
        public override string Comment(string inOldComment, bool inDefinitionCommennt = false)
        {
            inOldComment = inOldComment.Trim(' ');
            if (inOldComment.Contains(Environment.NewLine))
            {
                inOldComment = inOldComment.Trim('/').Trim('*');
                inOldComment = inOldComment.Trim('/').Trim('*');
                inOldComment = ReplaceJavaLineComments.Replace(inOldComment, Environment.NewLine);
                inOldComment = EmptyLinesStart.Replace(inOldComment, "");
                inOldComment = EmptyLinesEnd.Replace(inOldComment, "");
            }

            if (inDefinitionCommennt)
            {
                inOldComment = inOldComment.Replace(Environment.NewLine, Environment.NewLine + $"/// ");
                return $"/// <summary>{Environment.NewLine}/// {inOldComment}{Environment.NewLine}/// </summary>";
            }
            else
            {
                if (inOldComment.Contains(Environment.NewLine))
                {
                    return "/*" + inOldComment + "*/";
                }
                else
                {
                    return "//" + inOldComment;
                }
            }
        }

        /// <summary>
        /// Change Modifier from Java to C# (ie. final)
        /// </summary>
        /// <param name="inOldmodifier"></param>
        /// <returns></returns>
        public override string Modifier(string inOldmodifier)
        {
            if (inOldmodifier == "final")
            {
                return "readonly";
            }
            return base.Modifier(inOldmodifier);
        }

        /// <summary>
        /// MethodeParameter Handling (normal starting with "in", out param starting wit "out")
        /// </summary>
        public override string MethodeInParameter(FieldContainer inMethodeParameter)
        {
            var tmpName = inMethodeParameter.Name;
            if (RegexHelper.NameStartsWith_In.IsMatch(tmpName))
            {
                tmpName = tmpName.Substring(2);
            }
            else if (RegexHelper.NameStartsWith_Out.IsMatch(tmpName))
            {
                tmpName = tmpName.Substring(3);
            }
            else if (RegexHelper.NameStartsWith_Tmp.IsMatch(tmpName))
            {
                tmpName = tmpName.Substring(3);
            }
            if (!RegexHelper.IsFirstCharUpper(tmpName))
            {
                tmpName = tmpName[0].ToString().ToUpper() + tmpName.Substring(1);
            }

            if (inMethodeParameter.ModifierList.Contains("out"))
            {
                return "out" + tmpName;
            }
            return "in" + tmpName;
        }
    }
}
