using CodeConverterCore.Converter;
using CodeConverterCore.Helper;
using CodeConverterCore.Interface;
using CodeConverterCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
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
            if (string.IsNullOrWhiteSpace(inOldComment))
            {
                return inOldComment;
            }
            inOldComment = inOldComment.Trim(' ');
            //Fixing newlines from files with only \n modifier
            if (!inOldComment.Contains(Environment.NewLine)
                && inOldComment.Contains("\n"))
            {
                inOldComment = inOldComment.Replace("\n", Environment.NewLine);
            }

            inOldComment = inOldComment.Trim('/').Trim('*');
            inOldComment = inOldComment.Trim('/').Trim('*').Trim(' ');
            if (inOldComment.Contains(Environment.NewLine))
            {
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
        /// Mapp and Sort the Attributes from Java to C#
        /// </summary>
        /// <param name="inAttributeList"></param>
        /// <param name="inProperty"></param>
        /// <returns></returns>
        public override List<string> MapAndSortAttributes(List<string> inAttributeList, bool inProperty = false)
        {
            //Attribute Mappen, welche anderst heissen
            for (var tmpI = 0; tmpI < inAttributeList.Count; tmpI++)
            {
                if (inAttributeList[tmpI] == "@Override")
                {
                    inAttributeList[tmpI] = "override";
                }
                if (inAttributeList[tmpI] == "final")
                {
                    if (inProperty)
                    {
                        inAttributeList[tmpI] = "readonly";
                    }
                    else
                    {
                        inAttributeList[tmpI] = "sealed";
                    }
                }
                else if (inAttributeList[tmpI] == "default")
                {
                    if (inAttributeList.Count > 1)
                    {
                        throw new NotImplementedException("Interface default implemenation with mupltiple attributes not handled");
                    }
                    inAttributeList[tmpI] = "public";
                }
            }

            //Attribute Sortieren
            inAttributeList = inAttributeList.OrderBy(inItem =>
            {
                if (_accessModifier.Contains(inItem))
                {
                    return -1;
                }
                if (_typeModifier.Contains(inItem))
                {
                    return 0;
                }

                return 1;
            })
            .ToList();

            //Remove all other @ elements
            return inAttributeList
                .Where(inItem => !inItem.StartsWith("@"))
                .ToList();
        }
        private HashSet<string> _accessModifier = new HashSet<string> { "private", "protected", "internal", "public" };
        private HashSet<string> _typeModifier = new HashSet<string> { "sealed", "static", "abstract", "new", "override", "readonly" };

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

        /// <summary>
        /// Change Methode Names to be matching C# names
        /// </summary>
        /// <param name="inMethode"></param>
        /// <returns></returns>
        public override string MethodeName(MethodeContainer inMethode)
        {
            if (inMethode.ModifierList.Contains("private"))
            {
                return $"_{inMethode.Name}";
            }
            return inMethode.Name.PascalCase();
        }

        /// <summary>
        /// Handle Things, not handled inside other code
        /// This is the last methode to be called on conversion
        /// </summary>
        /// <param name="inClass"></param>
        public override void AnalyzerClassModifier(ClassContainer inClass)
        {
            //Check all Constructors for SUPER Calls
            foreach (var tmpMethode in inClass.MethodeList.Where(inItem => inItem.Name == inClass.Name))
            {
                HandleCodeBlock(tmpMethode.Code, (inItem) =>
                {
                    if (inItem is VariableDeclaration)
                    {
                        var tmpVarDec = inItem as VariableDeclaration;
                        if (tmpVarDec.Name == "Class")
                        {
                            tmpVarDec.Type.GenericTypes.Clear();
                        }
                    }
                }
                );
                //Check Code for base calls
                for (var tmpI = 0; tmpI < tmpMethode.Code.CodeEntries.Count; tmpI++)
                {
                    var tmpCall = tmpMethode.Code.CodeEntries[tmpI] as MethodeCall;
                    if (tmpCall != null)
                    {
                        if (tmpCall.MethodeLink.Name == "base")
                        {
                            //Change Methodecall to constructor call
                            tmpMethode.ConstructorCall = tmpCall;
                            tmpMethode.ConstructorCall.Name = "base";
                            tmpMethode.Code.CodeEntries.RemoveAt(tmpI);
                            break;
                        }
                    }
                }
            }

            //Handle Methode modifiers override/new/abstract to be C# compatible
            foreach (var tmpMethode in inClass.MethodeList)
            {
                //ParentClass Methode
                var tmpParentClass = inClass;
                while (true)
                {
                    tmpParentClass = tmpParentClass.GetParentClass();
                    if (tmpParentClass != null)
                    {
                        var tmpMethode2 = tmpMethode.FindMatchingMethode(tmpParentClass);
                        if (tmpMethode2 != null)
                        {
                            tmpMethode.ModifierList.HandleListContent(Modifiers.Override);
                            break;
                        }
                    }
                    else
                    {
                        tmpMethode.ModifierList.HandleListContent(Modifiers.Override, false);
                        break;
                    }
                }

                //Handle abstract
                if (tmpMethode.Code == null)
                {
                    tmpMethode.ModifierList.HandleListContent(Modifiers.Abstract);
                }
                else
                {
                    tmpMethode.ModifierList.HandleListContent(Modifiers.Abstract, false);
                }
            }
        }

        /// <summary>
        /// Handle Code Block Content with a 
        /// </summary>
        /// <param name="inBlock"></param>
        /// <param name="inCodeEntryAction"></param>
        private static void HandleCodeBlock(CodeBlock inBlock, Action<ICodeEntry> inCodeEntryAction)
        {
            if (inBlock == null)
            {
                return;
            }
            foreach (var tmpEntry in inBlock.CodeEntries)
            {
                inCodeEntryAction(tmpEntry);

                if (tmpEntry is StatementCode)
                {
                    HandleCodeBlock((tmpEntry as StatementCode).InnerContent, inCodeEntryAction);
                    foreach (var tmpBlock in (tmpEntry as StatementCode).StatementCodeBlocks)
                    {
                        HandleCodeBlock(tmpBlock, inCodeEntryAction);
                    }
                }
            }
        }
    }
}
