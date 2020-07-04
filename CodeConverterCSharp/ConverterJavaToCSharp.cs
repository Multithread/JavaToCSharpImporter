using CodeConverterCore.Converter;
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
    }
}
