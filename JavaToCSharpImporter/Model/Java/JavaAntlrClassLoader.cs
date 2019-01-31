using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System.Collections.Generic;
using System.Linq;
using static JavaToCSharpConverter.Model.Java.JavaParser;

namespace JavaToCSharpConverter.Model.Java
{
    public static class JavaAntlrClassLoader
    {
        public static List<ClassContainer> LoaderOptimization(string inFile)
        {
            var tmpClassList = new List<ClassContainer>();

            var tmpPackage = "";
            var tmpUsingList = new List<string>();

            ICharStream stream = new AntlrInputStream(inFile);
            ITokenSource lexer = new JavaLexerLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            var parser = new JavaParser(tokens);
            parser.BuildParseTree = true;
            IParseTree tree = parser.compilationUnit();
            var tmpClasses = new List<TypeDeclarationContext>();
            var tmpComments = new List<IParseTree>();
            for (var tmpI = 0; tmpI < tree.ChildCount; tmpI++)
            {
                var tmpChild = tree.GetChild(tmpI);
                var tmpType = tmpChild.GetType().Name;
                if (tmpChild is PackageDeclarationContext)
                {
                    tmpPackage = (tmpChild as PackageDeclarationContext).qualifiedName().GetText();
                }
                else if (tmpChild is ImportDeclarationContext)
                {
                    tmpUsingList.Add((tmpChild as ImportDeclarationContext).qualifiedName().GetText());
                }
                else if (tmpChild is TypeDeclarationContext)
                {
                    tmpClasses.Add(tmpChild as TypeDeclarationContext);
                }
                else
                {

                }
            }
            //Load all Classes from File
            foreach (var tmpElement in tmpClasses)
            {
                var tmpClass = new ClassContainer();
                tmpClassList.Add(tmpClass);
                tmpClass.Namespace = tmpPackage;
                tmpClass.UsingList = tmpUsingList;
                if (tmpComments.Count > 0)
                {
                    tmpClass.Comment = tmpComments[0].GetText();
                    tmpComments.RemoveAt(0);
                }

                ExtractClassFromTree(tmpElement, tmpClass);
            }

            return tmpClassList;
        }

        private static void ExtractClassFromTree(TypeDeclarationContext tmpElement, ClassContainer tmpClass)
        {
            for (var tmpI = 0; tmpI < tmpElement.ChildCount; tmpI++)
            {
                var tmpChild = tmpElement.GetChild(tmpI);
                var tmpType = tmpChild.GetType().Name;
                if (tmpChild is IClassAndInterfaceBaseData)
                {
                    var tmpContext = tmpChild as IClassAndInterfaceBaseData;
                    tmpClass.Name = tmpContext.IDENTIFIER().GetText();
                    var tmpExtends = tmpContext.EXTENDS()?.GetText();
                    if (tmpExtends != null)
                    {
                        tmpClass.InterfaceList.Add(tmpExtends);
                    }
                }
                if (tmpChild is ClassOrInterfaceModifierContext)
                {
                    //var tmpContext = tmpChild as ClassOrInterfaceModifierContext;
                    tmpClass.AttributeList.AddRange(tmpChild.GetChildren().Select(inItem => inItem.GetText()));
                }
                else if (tmpChild is InterfaceDeclarationContext)
                {
                    tmpClass.AttributeList.Add("interface");
                    var tmpContext = tmpChild as InterfaceDeclarationContext;

                    var tmpComment = "";
                    for (var tmpI2 = 0; tmpI2 < tmpContext.interfaceBody().ChildCount; tmpI2++)
                    {
                        var tmpSubChild = tmpContext.interfaceBody().GetChild(tmpI2);
                        if (tmpSubChild is InterfaceBodyDeclarationContext)
                        {
                            var tmpBodyPart = (tmpSubChild as InterfaceBodyDeclarationContext).interfaceMemberDeclaration();
                            if (tmpBodyPart.interfaceMethodDeclaration() != null)
                            {
                                var tmpIntDec = tmpBodyPart.interfaceMethodDeclaration();

                                var tmpMethode = new MethodeContainer
                                {
                                    Name = tmpIntDec.IDENTIFIER().GetText(),
                                    ReturnType = tmpIntDec.typeTypeOrVoid().GetText(),
                                    Comment = tmpComment,
                                    AntlrCode = tmpIntDec.methodBody(),
                                };
                                var tmpParams = tmpIntDec.formalParameters()?.formalParameterList()?.formalParameter();
                                if (tmpParams != null)
                                {
                                    foreach (var tmpParam in tmpParams)
                                    {
                                        var tmpModifier = tmpParam.variableModifier();
                                        var tmpFieldContainer = new FieldContainer
                                        {
                                            Name = tmpParam.variableDeclaratorId().IDENTIFIER().GetText(),
                                            Type = tmpParam.typeType().GetText(),
                                        };
                                        foreach (var tmpInfo in tmpParam.variableModifier())
                                        {
                                            tmpFieldContainer.ModifierList.Add(tmpInfo.GetText());
                                        }
                                        tmpMethode.Parameter.Add(tmpFieldContainer);
                                    }
                                    tmpClass.MethodeList.Add(tmpMethode);
                                }
                            }
                            else if (tmpBodyPart.constDeclaration() != null)
                            {
                                var tmpConstDef = tmpBodyPart.constDeclaration();
                                var tmpDeclarator = tmpConstDef.constantDeclarator()[0];

                                var tmpFieldContainer = new FieldContainer
                                {
                                    Name = tmpDeclarator.IDENTIFIER().GetText(),
                                    Type = tmpConstDef.typeType().GetText(),
                                    Comment = tmpComment,
                                    AntlrDefaultValue = tmpDeclarator.variableInitializer(),
                                };
                                tmpFieldContainer.HasDefaultValue = tmpFieldContainer.AntlrDefaultValue != null;
                                tmpClass.FieldList.Add(tmpFieldContainer);

                            }
                            else if (tmpBodyPart.classDeclaration() != null)
                            {
                                var tmpSubClass = new ClassContainer();
                                FillClassContext(tmpSubClass, tmpBodyPart.classDeclaration());
                                tmpClass.InnerClasses.Add(tmpSubClass);
                            }
                            else
                            {

                            }
                        }
                        else if (tmpSubChild is ErrorNodeImpl)
                        {
                            tmpComment = tmpSubChild.GetText();
                        }
                        else
                        {

                        }
                    }
                }
                else if (tmpChild is ClassDeclarationContext)
                {
                    FillClassContext(tmpClass, tmpChild as ClassDeclarationContext);
                }
                else
                {

                }
                tmpClass.Name = tmpChild.GetText();
            }
        }

        /// <summary>
        /// Fill Class from ClassDeclarationContext
        /// </summary>
        /// <param name="tmpClass"></param>
        /// <param name="inClassContext"></param>
        private static void FillClassContext(ClassContainer tmpClass, ClassDeclarationContext inClassContext)
        {
            var tmpImplements = inClassContext.IMPLEMENTS()?.GetText();
            if (tmpImplements != null)
            {
                tmpClass.InterfaceList.Add(tmpImplements);
            }
        }

        private static IEnumerable<IParseTree> GetChildren(this IParseTree inTree)
        {
            for (var tmpI = 0; tmpI < inTree.ChildCount; tmpI++)
            {
                var tmpChild = inTree.GetChild(tmpI);
                yield return tmpChild;
            }
        }
    }
}
