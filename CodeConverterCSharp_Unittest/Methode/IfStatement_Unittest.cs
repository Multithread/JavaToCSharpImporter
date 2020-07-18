using CodeConverterCore.Analyzer;
using CodeConverterCore.Converter;
using CodeConverterCore.Enum;
using CodeConverterCore.Helper;
using CodeConverterCore.Model;
using CodeConverterCSharp;
using NUnit.Framework;
using System.Linq;

namespace CodeConverterCSharp_Unittest.Methode
{
    public class IfStatement_Unittest
    {
        [Test]
        public void SimpleIfWithReturns()
        {
            var tmpIniData = DataHelper.LoadIni("");
            var tmpProject = new ProjectInformation();
            var tmpClass = Create.AddClass("IAttribute");
            tmpClass.ModifierList.Add("public");
            tmpClass.ModifierList.Add("interface");

            tmpProject.ClassList.Add(tmpClass);
            var tmpMethode = Create.AddMethode(tmpClass, "m1", TypeContainer.Void
                , new FieldContainer { Name = "in1", Type = new TypeContainer { Name = "int" } }
                , new FieldContainer { Name = "in2", Type = new TypeContainer { Name = "int" } });

            tmpMethode.Code = new CodeBlock();

            tmpMethode.Code.AddIfStatement(
                Create.CreateComparisionBlock("in1", VariableOperatorType.LessOrEquals, "in2"),
                Create.AddReturnStatement(new CodeBlock(), "true"));

            Create.AddReturnStatement(tmpMethode.Code, "false");

            new AnalyzerCore().LinkProjectInformation(tmpProject);

            var tmpResult = CSharpWriter.CreateClassesFromObjectInformation(tmpProject, new ConverterBase()).ToList();

            var tmpExpectedResult = @"

namespace 
{
    public interface IAttribute
    {
         void m1(int in1int in2)
        {
            if(in1 <= in2)
            {
                return true;
            }
            return false;
        }
    }
}
";
            Assert.AreEqual(tmpExpectedResult, tmpResult[0].Content);
        }


        [Test]
        public void IfWithElse()
        {
            var tmpIniData = DataHelper.LoadIni("");
            var tmpProject = new ProjectInformation();
            var tmpClass = Create.AddClass("IAttribute");
            tmpClass.ModifierList.Add("public");
            tmpClass.ModifierList.Add("interface");

            tmpProject.ClassList.Add(tmpClass);
            var tmpMethode = Create.AddMethode(tmpClass, "m1", TypeContainer.Void
                , new FieldContainer { Name = "in1", Type = new TypeContainer { Name = "int" } }
                , new FieldContainer { Name = "in2", Type = new TypeContainer { Name = "int" } });

            tmpMethode.Code = new CodeBlock();

            tmpMethode.Code.AddIfStatement(
                Create.CreateComparisionBlock("in1", VariableOperatorType.LessOrEquals, "in2"),
                Create.AddReturnStatement(new CodeBlock(), "true"),
                Create.AddReturnStatement(new CodeBlock(), "false"));


            new AnalyzerCore().LinkProjectInformation(tmpProject);

            var tmpResult = CSharpWriter.CreateClassesFromObjectInformation(tmpProject, new ConverterBase()).ToList();

            var tmpExpectedResult = @"

namespace 
{
    public interface IAttribute
    {
         void m1(int in1int in2)
        {
            if(in1 <= in2)
            {
                return true;
            }
            else {
                return false;
            }
        }
    }
}
";
            Assert.AreEqual(tmpExpectedResult, tmpResult[0].Content);
        }
    }
}
