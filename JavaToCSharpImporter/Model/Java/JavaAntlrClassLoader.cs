using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using JavaToCSharpConverter.Helper;
using JavaToCSharpConverter.Interface;
using System;
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
            var tmpClassComments = new List<IParseTree>();
            var tmpPrePackageComments = new List<IParseTree>();
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
                else if (tmpChild is IErrorNode)
                {
                    if (string.IsNullOrEmpty(tmpPackage))
                    {
                        tmpPrePackageComments.Add(tmpChild);
                    }
                    else
                    {
                        tmpClassComments.Add(tmpChild);
                    }
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
                //Add Namespace-comment, if existing
                if (tmpPrePackageComments.Count > 0)
                {
                    tmpClass.NamespaceComment = tmpPrePackageComments[0].GetText();
                }
                //Add Class Comments
                if (tmpClassComments.Count > 0)
                {
                    tmpClass.Comment = tmpClassComments[0].GetText();
                    tmpClassComments.RemoveAt(0);
                }

                ExtractClassFromTreeTypeDeclaration(tmpElement, tmpClass);
            }

            return tmpClassList;
        }

        private static void ExtractClassFromTreeTypeDeclaration(TypeDeclarationContext tmpElement, ClassContainer tmpClass)
        {
            for (var tmpI = 0; tmpI < tmpElement.ChildCount; tmpI++)
            {
                var tmpChild = tmpElement.GetChild(tmpI);
                var tmpType = tmpChild.GetType().Name;
                if (tmpChild is IClassAndInterfaceBaseData)
                {
                    var tmpContext = tmpChild as IClassAndInterfaceBaseData;
                    tmpClass.Type = tmpContext.IDENTIFIER().GetText();

                    if (tmpContext.typeParameters() != null)
                    {
                        var tmpClassGenericTypes = GetGenericTypesFromGenericArgumentsChildren(tmpClass, tmpContext.typeParameters().GetChildren());
                        tmpClass.Type.GenericTypes.AddRange(tmpClassGenericTypes);
                        var tmpExtends = tmpContext.EXTENDS()?.GetText();
                        if (tmpExtends != null)
                        {
                            tmpClass.InterfaceList.Add(tmpExtends);
                        }
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
        /// Create Types from IParseTree Type arguments
        /// </summary>
        /// <param name="tmpClass"></param>
        /// <param name="inGenericTypeChildren"></param>
        /// <returns></returns>
        private static List<TypeContainer> GetGenericTypesFromGenericArgumentsChildren(ClassContainer tmpClass, IEnumerable<IParseTree> inGenericTypeChildren)
        {
            TypeContainer tmpCurrentGenericType = null;
            var tmpReturnTypes = new List<TypeContainer>();
            foreach (var tmpGenericClassDeclarationChild in inGenericTypeChildren)
            {
                if (tmpGenericClassDeclarationChild is TypeParameterContext)
                {
                    var tmpParam = tmpGenericClassDeclarationChild as TypeParameterContext;
                    tmpCurrentGenericType = new TypeContainer { Name = tmpParam.IDENTIFIER().GetText() };
                    tmpReturnTypes.Add(tmpCurrentGenericType);
                    if (tmpParam.annotation().Length > 0 || tmpParam.EXTENDS() != null)
                    {
                        throw new NotImplementedException("TypeParametersContext Extenmsion und Annotiations sind nicht Implementiert");
                    }
                }
                else if (tmpGenericClassDeclarationChild is TypeTypeOrVoidContext)
                {
                    var tmpTypeOrVoid = tmpGenericClassDeclarationChild as TypeTypeOrVoidContext;
                    tmpCurrentGenericType = new TypeContainer();
                    tmpReturnTypes.Add(tmpCurrentGenericType);
                    if (tmpTypeOrVoid.typeType() != null)
                    {
                        if (tmpTypeOrVoid.typeType().classOrInterfaceType() != null)
                        {
                            var tmpType = tmpTypeOrVoid.typeType().classOrInterfaceType();
                            if (tmpType.IDENTIFIER().Length > 1)
                            {
                                throw new NotImplementedException($"Identifier to Long at {tmpClass.Name}");
                            }
                            tmpCurrentGenericType.Name = tmpType.IDENTIFIER(0).GetText();
                            foreach (var tmpTypeArgument in tmpType.typeArguments().SelectMany(inItem => inItem.typeArgument()))
                            {
                                var tmpGenericInnerType = new TypeContainer();
                                tmpGenericInnerType.Name = tmpTypeArgument.GetText();
                                tmpCurrentGenericType.GenericTypes.Add(tmpGenericInnerType);
                                //TODO Handling of Stacked Generics
                            }
                        }
                        else
                        {
                            throw new NotImplementedException($"Unknown Type inside tmpGenericClassDeclarationChild of Class {tmpClass.Name}");
                        }
                    }
                    else
                    {
                        tmpCurrentGenericType.Name = tmpTypeOrVoid.GetText();
                    }
                }
                else if (tmpGenericClassDeclarationChild is ErrorNodeImpl)
                {
                    if (tmpCurrentGenericType != null)
                    {
                        var tmpGEnericText = tmpGenericClassDeclarationChild.GetText();
                        if (tmpGEnericText == "[" || tmpGEnericText == "]")
                        {
                            tmpCurrentGenericType.IsArray = true;
                        }
                        else if (tmpGEnericText == ">")
                        {
                            //TODO Handling of Stacked Generics
                        }
                        else if (tmpGEnericText == "<")
                        {
                            //TODO Handling of Stacked Generics
                        }
                        else if (tmpGEnericText == ",")
                        {
                            tmpCurrentGenericType = null;
                        }
                        else
                        {
                            throw new NotImplementedException($"Unknown Type inside Generic Class Header as Class {tmpClass.Name}");
                        }
                    }
                    else if (!TypeNonNumberChars.Contains(tmpGenericClassDeclarationChild.GetText()))
                    {
                        tmpCurrentGenericType = tmpGenericClassDeclarationChild.GetText();
                        tmpReturnTypes.Add(tmpCurrentGenericType);
                    }
                }
                else if (tmpGenericClassDeclarationChild is TerminalNodeImpl)
                {
                    if (tmpGenericClassDeclarationChild.GetText() == ",")
                    {
                        tmpCurrentGenericType = null;
                    }
                }
                else
                {
                    throw new NotImplementedException($"Unknown Type inside tmpGenericClassDeclarationChild of Class {tmpClass.Name}");
                }
            }
            return tmpReturnTypes;
        }
        private static List<string> TypeNonNumberChars = new List<string>() { "[", "]", "<", ">" };
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

                tmpSubClass.Type = tmpBodyPart.classDeclaration().IDENTIFIER().GetText();
                if (tmpBodyPart.classDeclaration().typeParameters() != null)
                {
                    //Fill Type Parameters
                    foreach (var tmpParam in tmpBodyPart.classDeclaration().typeParameters().typeParameter())
                    {
                        var tmpInnerType = (TypeContainer)tmpParam.IDENTIFIER().GetText();
                        if (tmpParam.annotation().Length > 0 || tmpParam.EXTENDS() != null)
                        {

                        }
                        tmpSubClass.Type.GenericTypes.Add(tmpInnerType);
                    }
                }

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
            inClass.InterfaceList.AddRange(inClassContext.typeList().GetChildren().Select(inItem => inItem.GetText()));
            inClass.AttributeList.Add("class");
            var tmpComment = "";

            var tmpChildList = inClassContext.classBody().GetChildren().ToList();
            for (var tmpI = 0; tmpI < tmpChildList.Count(); tmpI++)
            {
                var tmpItem = tmpChildList[tmpI];
                var tmpType = tmpItem.GetType().Name;
                if (tmpItem is ClassBodyDeclarationContext)
                {
                    var tmpClassBody = tmpItem as ClassBodyDeclarationContext;
                    var tmpDeclaration = tmpClassBody.memberDeclaration();
                    if (tmpDeclaration == null)
                    {
                        continue;
                    }
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
                        var tmpMethodeContainer = AddMethodeDeclaration(inClass, tmpDeclaration.methodDeclaration(), tmpModifierList);
                        //In this case, we have a Generic methode Constructor
                        if (tmpI > 0 && tmpChildList[tmpI - 1].GetText().EndsWith(">"))
                        {
                            var tmpDataList = new List<IParseTree>();
                            var tmpDepth = 0;
                            var tmpNegativeI = 0;
                            while (tmpDepth != 0 || tmpNegativeI == 0)
                            {
                                tmpNegativeI++;
                                var tmpPreviiousCHild = tmpChildList[tmpI - tmpNegativeI];
                                foreach (var tmpChildOfChildOfPrechild in tmpPreviiousCHild.GetChildren().Reverse().SelectMany(inItem => inItem.GetChildren().Reverse()))
                                {
                                    tmpDataList.Insert(0, tmpChildOfChildOfPrechild);
                                    if (tmpChildOfChildOfPrechild.GetText() == ">")
                                    {
                                        tmpDepth++;
                                    }
                                    else if (tmpChildOfChildOfPrechild.GetText() == "<")
                                    {
                                        tmpDepth--;
                                    }
                                }
                            }
                            var tmpGenericListForMethode = GetGenericTypesFromGenericArgumentsChildren(inClass, tmpDataList);
                            tmpMethodeContainer.GenericTypes.AddRange(tmpGenericListForMethode);
                        }
                    }
                    else if (tmpDeclaration.fieldDeclaration() != null)
                    {
                        //Load field declaration into the container
                        var tmpModifier = tmpDeclaration.fieldDeclaration();
                        var tmpFieldContainer = new FieldContainer
                        {
                            Name = tmpModifier.variableDeclarators().variableDeclarator(0).variableDeclaratorId().IDENTIFIER().GetText(),
                            Type = tmpModifier.typeType().GetText(),
                            Comment = tmpComment,
                        };
                        tmpComment = "";
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
                            Comment = tmpComment,
                        };
                        tmpComment = "";
                        var tmpParams = tmpConstructor.formalParameters()?.formalParameterList()?.formalParameter();
                        if (tmpParams != null)
                        {
                            foreach (IFormalParameterContext tmpParam in tmpParams)
                            {
                                HandlMethodeParameterContext(tmpMethode, tmpParam);
                            }
                            if (tmpConstructor.formalParameters()?.formalParameterList()?.lastFormalParameter() != null)
                            {
                                HandlMethodeParameterContext(tmpMethode, tmpConstructor.formalParameters().formalParameterList().lastFormalParameter());
                            }
                        }
                        tmpMethode.ModifierList = tmpModifierList;
                        inClass.MethodeList.Add(tmpMethode);
                    }
                    else if (tmpDeclaration.genericMethodDeclaration() != null)
                    {
                        var tmpMethodeDeclaration = tmpDeclaration.genericMethodDeclaration();

                        var tmpMethodeContainer = AddMethodeDeclaration(inClass, tmpMethodeDeclaration.methodDeclaration(), tmpModifierList);
                        var tmpGenericListForMethode = GetGenericTypesFromGenericArgumentsChildren(inClass, tmpMethodeDeclaration.typeParameters().GetChildren());
                        tmpMethodeContainer.GenericTypes.AddRange(tmpGenericListForMethode);
                    }
                    else
                    {
                    }

                }
                else if (tmpItem is TerminalNodeImpl)
                {
                    if (tmpItem.GetText().StartsWith("/"))
                    {
                        tmpComment = tmpItem.GetText();
                    }
                    else if (tmpItem.GetText() == "{")
                    {

                    }
                    else if (tmpItem.GetText() == "}")
                    {

                    }
                    else
                    {

                    }
                }
                else
                {

                }
            }
        }

        private static void HandlMethodeParameterContext(MethodeContainer tmpMethode, IFormalParameterContext tmpParam)
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

        public static TypeContainer CreateTypeContainerFromType(TypeTypeOrVoidContext inTypeOrVoid)
        {
            var inType = inTypeOrVoid.GetText();
            var tmpTypes = GetGenericTypesFromGenericArgumentsChildren(new ClassContainer(), new List<IParseTree> { inTypeOrVoid });
            return tmpTypes.First();
            //var tmpContainer = new TypeContainer();
            //if (inType.EndsWith("[]"))
            //{
            //    tmpContainer.IsArray = true;
            //    inType = inType.Substring(0, inType.Length - 2);
            //}
            //if (inType.Contains("extends"))
            //{
            //    tmpContainer.Name = inType.Split(' ')[0];
            //    var tmpExtends = inType.Substring(inType.IndexOf("extends"));

            //    tmpContainer.Extends = tmpExtends.Split(',').Select(inItem => inItem.Trim(' ')).ToList();
            //}
            //else if (inType.Contains("<"))
            //{
            //    tmpContainer.Name = inType.Substring(0, inType.IndexOf("<"));
            //    var tmpInnerData = inType.Substring(inType.IndexOf("<") + 1, inType.Length - inType.IndexOf("<") - 2);
            //    var tmpSplitted = CodeSplitter.FileDataSplitter(tmpInnerData, new ClassInterfaceSplitter()).ToList();
            //    foreach (var tmpSplit in tmpSplitted)
            //    {
            //        tmpContainer.GenericTypes.Add(tmpSplit.Item2);
            //    }
            //}
            //else
            //{
            //    tmpContainer.Name = inType;
            //}

            //return tmpContainer;
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
                ReturnType = CreateTypeContainerFromType(inMethodeContext.typeTypeOrVoid()),
            };
            var tmpParams = inMethodeContext.formalParameters()?.formalParameterList()?.formalParameter();
            if (tmpParams != null)
            {
                foreach (var tmpParam in tmpParams)
                {
                    var tmpNewMethode = new FieldContainer
                    {
                        Name = tmpParam.variableDeclaratorId().IDENTIFIER().GetText(),
                    }; if (tmpParam.typeType().classOrInterfaceType() != null)
                    {
                        tmpNewMethode.Type = tmpParam.typeType().classOrInterfaceType().IDENTIFIER(0).GetText();
                        foreach (var tmpGeneric in tmpParam.typeType().classOrInterfaceType().typeArguments().SelectMany(inItem => inItem.typeArgument()))
                        {
                            var tmpType = new TypeContainer
                            {
                                Name = tmpGeneric.GetChild(0).GetText(),
                            };
                            if (tmpGeneric.EXTENDS() != null)
                            {
                                tmpType.Extends.Add(tmpGeneric.typeType().GetText());
                            }
                            tmpNewMethode.Type.GenericTypes.Add(tmpType);
                        }

                        if (tmpParam.GetText().Contains("["))
                        {
                            tmpNewMethode.Type.IsArray = true;
                        }
                        if (tmpParam.typeType().classOrInterfaceType().IDENTIFIER().Length > 1)
                        {
                            throw new NotImplementedException("Multi-Identifier needs to be handled");
                        }
                    }
                    else if (tmpParam.typeType().primitiveType() != null)
                    {
                        tmpNewMethode.Type = tmpParam.typeType().primitiveType().GetText();
                    }
                    else
                    {
                        throw new NotImplementedException("Other Type Missing");
                    }
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
