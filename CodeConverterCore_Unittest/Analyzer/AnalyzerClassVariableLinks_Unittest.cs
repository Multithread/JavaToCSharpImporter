using CodeConverterCore.Analyzer;
using CodeConverterCore.Helper;
using CodeConverterCore.Model;
using NUnit.Framework;

namespace CodeConverterCore_Unittest
{
    /// <summary>
    /// Text Analyzer linking of Types
    /// </summary>
    public class AnalyzerClassVariableLinks_Unittest
    {

        [Test]
        public void CheckInterface()
        {
            var tmpProject = new ProjectInformation();
            var tmpClass1 = Create.AddClass("Class1");
            tmpClass1.AddMethode("M1", new TypeContainer { Name = "void" });
            var tmpMethode = tmpClass1.MethodeList[0];
            var tmpVar = Create.AddVariable(tmpMethode.Code, "tmpVar", new TypeContainer { Name = "string" });
            Create.SetFieldValue(tmpMethode.Code, tmpVar.CodeEntries[0], new ConstantValue { Value = "\"BBBB\"" });
            new AnalyzerCore().LinkProjectInformation(tmpProject);

            Assert.AreEqual(false, true);
        }

    }
}