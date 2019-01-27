using JavaToCSharpConverter.Interface;
using JavaToCSharpConverter.Model;
using JavaToCSharpConverter.Model.Splitter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace JavaToCSharpConverter.Helper
{
    /// <summary>
    /// CodeDatei Splitten
    /// </summary>
    public static class CodeSplitter
    {
        /// <summary>
        /// Split for Usings
        /// </summary>
        /// <param name="inFile"></param>
        /// <returns></returns>
        public static IEnumerable<Tuple<object, string>> SplitForUsings(string inFile)
        {
            return FileDataSplitter(inFile, new PreClassSplitter());
        }

        /// <summary>
        /// Split Inputstring
        /// </summary>
        /// <param name="inData"></param>
        /// <returns></returns>
        public static IEnumerable<Tuple<object, string>> FileDataSplitter(string inData, CodeSplitterEvents inEvents)
        {
            var tmpCurrentIndex = 0;
            var tmpStringBuilder = new StringBuilder();

            var tmpDataStack = new Stack<Checker>();
            Checker tmpTop = null;

            //Aktuellen Stack zurückgeben
            inEvents.GetCurrentStack = () =>
             {
                 var tmpList = tmpDataStack.Select(inItem => inItem.Clone()).ToList();
                 if (tmpTop != null)
                 {
                     tmpList.Insert(0, tmpTop.Clone());
                 }
                 return tmpList;
             };

            var tmpFound = false;
            //Iterate for every Char
            while (tmpCurrentIndex < inData.Length - 1)
            {
                tmpFound = false;
                var tmpResult = inEvents.CurrentCharacter(inData[tmpCurrentIndex]);
                if (tmpResult != null)
                {
                    tmpStringBuilder.Append(inData[tmpCurrentIndex]);
                    tmpCurrentIndex++;
                    yield return new Tuple<object, string>(tmpResult, tmpStringBuilder.ToString());
                    tmpStringBuilder.Clear();
                    continue;
                }

                //Prüfen ob ein Ende Erreicht ist
                string tmpString = inData[tmpCurrentIndex].ToString() + inData[tmpCurrentIndex + 1];
                if (tmpTop != null)
                {
                    if (tmpTop.Escape != null && tmpString.StartsWith(tmpTop.Escape))
                    {
                        tmpStringBuilder.Append(tmpTop.End);
                        tmpCurrentIndex += tmpTop.End.Length + 1;
                        tmpStringBuilder.Append(inData[tmpCurrentIndex - 1]);

                        tmpResult = inEvents.Ended(tmpTop.Clone());
                        if (tmpResult != null)
                        {
                            yield return new Tuple<object, string>(tmpResult, tmpStringBuilder.ToString());
                            tmpStringBuilder.Clear();
                        }
                        continue;
                    }
                    if (tmpTop.End != null && tmpString.StartsWith(tmpTop.End))
                    {
                        tmpStringBuilder.Append(tmpTop.End);
                        tmpCurrentIndex += tmpTop.End.Length;

                        tmpResult = inEvents.Ended(tmpTop?.Clone());
                        if (tmpResult != null)
                        {
                            yield return new Tuple<object, string>(tmpResult, tmpStringBuilder.ToString());
                            tmpStringBuilder.Clear();
                        }

                        tmpTop = GetTop(tmpDataStack);
                        continue;
                    }
                    if (!tmpTop.SubAllowed)
                    {
                        if (tmpString.StartsWith(tmpTop.End))
                        {
                            tmpStringBuilder.Append(tmpTop.End);
                            tmpCurrentIndex += tmpTop.End.Length;

                            tmpResult = inEvents.Ended(tmpTop.Clone());
                            if (tmpResult != null)
                            {
                                yield return new Tuple<object, string>(tmpResult, tmpStringBuilder.ToString());
                                tmpStringBuilder.Clear();
                            }

                            tmpTop = GetTop(tmpDataStack);
                            continue;
                        }
                        tmpStringBuilder.Append(inData[tmpCurrentIndex]);
                        tmpCurrentIndex += 1;
                        continue;
                    }
                }
                //Anfügen
                foreach (var tmpCheck in CheckList)
                {
                    if (tmpString.StartsWith(tmpCheck.Start))
                    {
                        tmpStringBuilder.Append(tmpCheck.Start);
                        tmpCurrentIndex += tmpCheck.Start.Length;
                        if (tmpTop != null)
                        {
                            tmpDataStack.Push(tmpTop);
                        }
                        tmpTop = tmpCheck;
                        tmpFound = true;

                        tmpResult = inEvents.Started(tmpTop.Clone());
                        if (tmpResult != null)
                        {
                            yield return new Tuple<object, string>(tmpResult, tmpStringBuilder.ToString());
                            tmpStringBuilder.Clear();
                        }
                        break;
                    }
                }
                if (tmpFound)
                {
                    continue;
                }
                tmpStringBuilder.Append(inData[tmpCurrentIndex]);
                tmpCurrentIndex++;
            }

            if (inData.Length > tmpCurrentIndex)
            {
                tmpStringBuilder.Append(inData[tmpCurrentIndex]);
            }

            //Wenn das letzte Zeichen noch dem Ende Entspricht: top
            if (tmpTop != null && inData[tmpCurrentIndex].ToString().StartsWith(tmpTop.End))
            {
                var tmpResult = inEvents.Ended(tmpTop.Clone());
                if (tmpResult != null)
                {
                    yield return new Tuple<object, string>(tmpResult, tmpStringBuilder.ToString());
                    yield break;
                }
            }

            yield return new Tuple<object, string>(null, tmpStringBuilder.ToString());
        }

        private static Checker GetTop(Stack<Checker> inData)
        {
            if (inData.Count > 0)
            {
                return inData.Pop();
            }
            return null;
        }

        private static List<Checker> CheckList = new List<Checker>
        {
            new Checker
            {
                Start="//",
                End="\n"
            //},new Checker
            //{
            //    Start="REM",
            //    End="\n"
            },new Checker
            {
                Start="/*",
                End="*/"
            },new Checker
            {
                Start="{",
                End="}",
                SubAllowed=true
            },new Checker
            {
                Start="<",
                End=">",
                SubAllowed=true
            },new Checker
            {
                Start="\"",
                End="\"",
                Escape="\\",
            },new Checker
            {
                Start="(",
                End=")",
                SubAllowed=true
            },new Checker
            {
                Start="[",
                End="]",
                SubAllowed=true
            }
        };
    }

    [DebuggerDisplay("Start:{Start}, End:{End}, Escape:{Escape},SubAllowed:{SubAllowed}")]
    public class Checker
    {
        internal string Start;
        internal string End;
        internal string Escape;
        internal bool SubAllowed;

        /// <summary>
        /// Clone Checker for output
        /// </summary>
        /// <returns></returns>
        public Checker Clone()
        {
            return new Checker
            {
                Start = Start,
                End = End,
                Escape = Escape,
                SubAllowed = SubAllowed,
            };
        }
    }
}
