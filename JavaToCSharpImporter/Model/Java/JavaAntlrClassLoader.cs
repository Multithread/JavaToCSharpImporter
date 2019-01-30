using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System.Collections.Generic;
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


            return tmpClassList;
        }
    }
}
