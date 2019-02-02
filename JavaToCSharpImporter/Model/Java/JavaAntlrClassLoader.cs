using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using JavaToCSharpConverter.Helper;
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
                            ResolveInterfaceDeclaration(tmpClass, tmpComment, tmpSubChild);
                            //An Interface has no Implementation of an Methode. 
                            tmpClass.MethodeList.ForEach(inItem => inItem.AntlrCode = null);
                        }
                        else if (tmpSubChild is ErrorNodeImpl)
                        {
                            tmpComment = tmpSubChild.GetText();
                        }
                        else if (tmpSubChild is TerminalNodeImpl)
                        {

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
            }
        }

        /// <summary>
        /// Resolve an Interface Declaration
        /// </summary>
        /// <param name="tmpClass"></param>
        /// <param name="tmpComment"></param>
        /// <param name="tmpSubChild"></param>
        private static void ResolveInterfaceDeclaration(ClassContainer tmpClass, string tmpComment, IParseTree tmpSubChild)
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
                }
                tmpClass.MethodeList.Add(tmpMethode);
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
                var tmpSubClass = new ClassContainer
                {
                    UsingList = tmpClass.UsingList,
                    Namespace = tmpClass.Namespace,
                };
                FillClassContext(tmpSubClass, tmpBodyPart.classDeclaration());
                tmpClass.InnerClasses.Add(tmpSubClass);
            }
            else
            {

            }
        }

        /// <summary>
        /// Fill Class from ClassDeclarationContext
        /// </summary>
        /// <param name="inClass"></param>
        /// <param name="inClassContext"></param>
        private static void FillClassContext(ClassContainer inClass, ClassDeclarationContext inClassContext)
        {
            inClass.Name = inClassContext.IDENTIFIER().GetText();
            inClass.InterfaceList.AddRange(inClassContext.typeList().GetChildren().Select(inItem => inItem.GetText()));
            inClass.AttributeList.Add("class");

            foreach (var tmpItem in inClassContext.classBody().GetChildren())
            {
                var tmpType = tmpItem.GetType().Name;
                if (tmpItem is ClassBodyDeclarationContext)
                {
                    var tmpClassBody = tmpItem as ClassBodyDeclarationContext;
                    var tmpDeclaration = tmpClassBody.memberDeclaration();

                    var tmpModifierList = new List<string>();
                    //get modifiers together
                    foreach (var tmpClassBodyContextModifier in tmpClassBody.modifier())
                    {
                        var tmpModifierText = tmpClassBodyContextModifier.GetText();
                        //Fuck it: If it starts with @ it probably is an override. If Not -> Fix the C# Code:D
                        if (tmpModifierText.StartsWith("@"))
                        {
                            tmpModifierText = tmpModifierText.Substring(1).ToLower();
                        }
                        tmpModifierList.Add(tmpModifierText);
                    }

                    if (tmpDeclaration.methodDeclaration() != null)
                    {
                        AddMethodeDeclaration(inClass, tmpDeclaration.methodDeclaration(), tmpModifierList);
                    }
                    else if (tmpDeclaration.fieldDeclaration() != null)
                    {
                        //Load field declaration into the container
                        var tmpModifier = tmpDeclaration.fieldDeclaration();
                        var tmpFieldContainer = new FieldContainer
                        {
                            Name = tmpModifier.variableDeclarators().variableDeclarator(0).variableDeclaratorId().IDENTIFIER().GetText(),
                            Type = tmpModifier.typeType().GetText(),
                        };
                        if (tmpModifier.variableDeclarators().variableDeclarator().Length > 0)
                        {
                            tmpFieldContainer.AntlrDefaultValue = tmpModifier.variableDeclarators().variableDeclarator()[0].variableInitializer();
                            tmpFieldContainer.HasDefaultValue = tmpFieldContainer.AntlrDefaultValue != null;
                        }
                        tmpFieldContainer.ModifierList = tmpModifierList;
                        inClass.FieldList.Add(tmpFieldContainer);
                    }
                    else if (tmpDeclaration.constructorDeclaration() != null)
                    {
                        var tmpConstructor = tmpDeclaration.constructorDeclaration();
                        var tmpMethode = new MethodeContainer
                        {
                            Name = tmpConstructor.IDENTIFIER().GetText(),
                            AntlrCode = tmpConstructor.block(),
                            IsConstructor = true,
                        };
                        var tmpParams = tmpConstructor.formalParameters()?.formalParameterList()?.formalParameter();
                        if (tmpParams != null)
                        {
                            foreach (var tmpParam in tmpParams)
                            {
                                //Set parameters
                                var tmpNewMethode = new FieldContainer
                                {
                                    Name = tmpParam.variableDeclaratorId().IDENTIFIER().GetText(),
                                    Type = tmpParam.typeType().GetText(),
                                };

                                if (tmpParam.variableModifier().Length > 0)
                                {
                                    foreach (var tmpModifierContext in tmpParam.variableModifier())
                                    {
                                        var tmpParamModifier = tmpModifierContext.GetText();
                                        //Fuck it: If it starts with @ it probably is an override. If Not -> Fix the C# Code:D
                                        if (tmpParamModifier.StartsWith("@"))
                                        {
                                            tmpParamModifier = tmpParamModifier.Substring(1).ToLower();
                                        }
                                        tmpNewMethode.ModifierList.Add(tmpParamModifier);
                                    }
                                }
                                tmpMethode.Parameter.Add(tmpNewMethode);
                            }
                        }
                        tmpMethode.ModifierList = tmpModifierList;
                        inClass.MethodeList.Add(tmpMethode);
                    }
                    else if (tmpDeclaration.genericMethodDeclaration() != null)
                    {
                        var tmpMethodeDeclaration = tmpDeclaration.genericMethodDeclaration();

                        var tmpMethodeContainer = AddMethodeDeclaration(inClass, tmpMethodeDeclaration.methodDeclaration(), tmpModifierList);
                    }
                    else
                    {

                    }

                }
                else if (tmpItem is TerminalNodeImpl)
                {
                }
                else
                {

                }
            }
        }

        /// <summary>
        /// Add Methode Declaration to Class
        /// </summary>
        /// <param name="inClass"></param>
        /// <param name="tmpDeclaration"></param>
        /// <param name="tmpModifierList"></param>
        /// <returns></returns>
        private static MethodeContainer AddMethodeDeclaration(ClassContainer inClass, MethodDeclarationContext inMethodeContext, List<string> tmpModifierList)
        {
            var tmpMethode = new MethodeContainer
            {
                Name = inMethodeContext.IDENTIFIER().GetText(),
                AntlrCode = inMethodeContext.methodBody(),
                ReturnType = inMethodeContext.typeTypeOrVoid().GetText(),
            };
            var tmpParams = inMethodeContext.formalParameters()?.formalParameterList()?.formalParameter();
            if (tmpParams != null)
            {
                foreach (var tmpParam in tmpParams)
                {
                    var tmpNewMethode = new FieldContainer
                    {
                        Name = tmpParam.variableDeclaratorId().IDENTIFIER().GetText(),
                        Type = tmpParam.typeType().GetText(),
                    };
                    if (tmpParam.variableModifier().Length > 0)
                    {
                        foreach (var tmpModifierContext in tmpParam.variableModifier())
                        {
                            var tmpParamModifier = tmpModifierContext.GetText();
                            //Fuck it: If it starts with @ it probably is an override. If Not -> Fix the C# Code:D
                            if (tmpParamModifier.StartsWith("@"))
                            {
                                tmpParamModifier = tmpParamModifier.Substring(1).ToLower();
                            }
                            tmpNewMethode.ModifierList.Add(tmpParamModifier);
                        }
                    }
                    tmpMethode.Parameter.Add(tmpNewMethode);
                }
            }
            tmpMethode.ModifierList = tmpModifierList;
            inClass.MethodeList.Add(tmpMethode);
            return tmpMethode;
        }
    }
}
