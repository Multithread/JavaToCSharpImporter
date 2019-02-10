using JavaToCSharpConverter.Helper;
using JavaToCSharpConverter.Interface;
using JavaToCSharpConverter.Model.Splitter;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace JavaToCSharpConverter.Model.CSharp
{
    public static class CSharpCodePrettyfier
    {
        /// <summary>
        /// Format C# code to be pretty
        /// </summary>
        /// <param name="inCode"></param>
        /// <returns></returns>
        public static string FormatCode(string inCode)
        {
            var tmpOutput = new StringBuilder();

            var tmpCurrentDept = 0;
            var tmpLastLineIsNewLine = false;
            var tmpPreviousText = "";

            foreach (var tmpPart in CodeSplitter.FileDataSplitter(inCode, new CodeFormatSplitter()))
            {
                if (tmpPart.Item1 == null)
                {
                    tmpOutput.Append(GetLeftSpace(tmpCurrentDept) + tmpPart.Item2.RemoveNewlines());
                    continue;
                }
                var tmpType = (FormatResultType)tmpPart.Item1;
                var tmpText = tmpPart.Item2;
                if (tmpType != FormatResultType.MultiLineCommentEnd)
                {
                    if (tmpType == FormatResultType.SingleLineCommentEnd)
                    {
                        tmpText = tmpText.RemoveNewlines();
                    }
                    else
                    {
                        tmpText = CleanupText(tmpText);
                    }
                }

                if (tmpType == FormatResultType.EndOfLine)
                {
                    if (tmpLastLineIsNewLine)
                    {
                        tmpOutput.AppendLine("");
                    }
                    tmpOutput.Append(GetLeftSpace(tmpCurrentDept) + tmpText);
                    tmpLastLineIsNewLine = true;
                    continue;
                }
                if (tmpType == FormatResultType.MultiLineCommentStart)
                {
                    tmpPreviousText = tmpText;
                    continue;
                }
                if (tmpType == FormatResultType.MultiLineCommentEnd)
                {
                    tmpPreviousText += tmpText;

                    tmpPreviousText = CleanupMultilineComment(tmpPreviousText, GetLeftSpace(tmpCurrentDept));

                    tmpOutput.AppendLine(tmpPreviousText);
                    tmpPreviousText = "";
                    continue;
                }
                if (!string.IsNullOrEmpty(tmpPreviousText))
                {
                    tmpOutput.Append(GetLeftSpace(tmpCurrentDept) + tmpPreviousText);
                    tmpPreviousText = "";
                }
                //Start of Curly Bracket
                if (tmpType == FormatResultType.CurlyStart)
                {
                    if (tmpLastLineIsNewLine)
                    {
                        tmpOutput.AppendLine("");
                    }
                    tmpText = tmpText.Substring(0, tmpText.Length - 1);
                    tmpOutput.AppendLine(GetLeftSpace(tmpCurrentDept) + tmpText);
                    tmpOutput.AppendLine(GetLeftSpace(tmpCurrentDept) + "{");
                    tmpCurrentDept++;
                    tmpLastLineIsNewLine = false;
                }
                //End of Curly Bracket
                if (tmpType == FormatResultType.CurlyEnd)
                {
                    tmpText = tmpText.Substring(1);
                    if (tmpText.Length > 0 || tmpLastLineIsNewLine)
                    {
                        tmpOutput.AppendLine(GetLeftSpace(tmpCurrentDept) + tmpText);
                        tmpLastLineIsNewLine = false;
                    }
                    tmpCurrentDept--;
                    tmpOutput.AppendLine(GetLeftSpace(tmpCurrentDept) + "}");

                }
            }

            if (tmpCurrentDept != 0)
            {
                //This happens when the input code has been broken
            }
            return tmpOutput.ToString();
        }

        /// <summary>
        /// Format C# Code to be pretty
        /// </summary>
        /// <param name="inText"></param>
        /// <returns></returns>
        private static string CleanupText(string inText)
        {
            //Remove Newlines and Text ad beginnning/end
            inText = inText.Replace("\n", " ").RemoveNewlines().Trim(' ');

            //Match all Spaces to be single space
            inText = SpaceRegex.Replace(inText, " ");

            //Remove all Spaces before or after Elements
            inText = SpaceDataRegex.Replace(inText, inItem => inItem.Groups["element"].Value);

            //Add new Spaces
            inText = SpaceAddBeforeDataRegex.Replace(inText, inItem => inItem.Groups["text"].Value + " " + inItem.Groups["element"].Value);
            inText = SpaceAddAfterDataRegex.Replace(inText, inItem => inItem.Groups["element"].Value + " " + inItem.Groups["text"].Value);
            inText = SpaceAddAfterKeyword.Replace(inText, inItem => inItem.Groups["element"].Value + " " + inItem.Groups["text"].Value);

            return inText;
        }
        /// <summary>
        /// Remove multiple spaces after each other 
        /// </summary>
        private static Regex SpaceRegex = new Regex("( ){1}");

        /// <summary>
        /// Remove all spaces before and after all spezial chars (Minify code)
        /// </summary>
        private static Regex SpaceDataRegex = new Regex(@"( ){0,1}(?<element>[\(\)\{\}\.\[\]\<\>=!|&,+\-:;])( ){0,1}");

        /// <summary>
        /// Add Spaces between text and spezial Chars (prettify)
        /// </summary>
        private static Regex SpaceAddBeforeDataRegex = new Regex(@"(?<text>[0-9a-zA-Z\""_\)\]])(?<element>[\=!|&+\-:])");

        /// <summary>
        /// Add Spaces between spezial chars and text (prettify)
        /// </summary>
        private static Regex SpaceAddAfterDataRegex = new Regex(@"(?<element>[\=|&,+\-\]:;\>])(?<text>[0-9a-zA-Z\""_])");

        /// <summary>
        /// Add Spaces between spezial chars and text (prettify)
        /// </summary>
        private static Regex SpaceAddAfterKeyword = new Regex(@"(?<element>(if|for|catch|finally))(?<text>\()");

        /// <summary>
        /// Format multiLine comment pretty
        /// </summary>
        /// <param name="inText"></param>
        /// <returns></returns>
        private static string CleanupMultilineComment(string inComment, string inLeftSpacing)
        {
            var tmpCommentList = inComment.Replace("\r", "").Split('\n').ToList();

            //Cleanup Old Comments
            for (var tmpI = 0; tmpI < tmpCommentList.Count; tmpI++)
            {
                //Trimm Chars at the beginning that we don't need anymore
                tmpCommentList[tmpI] = tmpCommentList[tmpI].Trim(' ').TrimStart('/').TrimStart('*').TrimStart(' ');
            }
            //remove */ from last Line of Comment
            tmpCommentList[tmpCommentList.Count - 1] = tmpCommentList[tmpCommentList.Count - 1].TrimEnd('/').TrimEnd('*');

            //Remove Existing Summary
            if (tmpCommentList[0] == "<summary>")
            {
                tmpCommentList.RemoveAt(0);
            }
            while (string.IsNullOrEmpty(tmpCommentList[0]))
            {
                tmpCommentList.RemoveAt(0);
                if (tmpCommentList.Count == 0)
                {
                    //No Comment to Create
                    return "";
                }
            }

            if (tmpCommentList.Last() == "</summary>")
            {
                tmpCommentList.RemoveAt(tmpCommentList.Count - 1);
            }
            while (string.IsNullOrEmpty(tmpCommentList.Last()))
            {
                tmpCommentList.RemoveAt(tmpCommentList.Count - 1);
                if (tmpCommentList.Count == 0)
                {
                    //No Comment to Create
                    return "";
                }
            }

            //Place Together C# Comment
            var tmpResult = inLeftSpacing + "/// <summary>" + Environment.NewLine;
            foreach (var tmpComment in tmpCommentList)
            {
                tmpResult += inLeftSpacing + "/// " + tmpComment + Environment.NewLine;
            }
            tmpResult += inLeftSpacing + "/// </summary>";


            return tmpResult;
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
    }
}
