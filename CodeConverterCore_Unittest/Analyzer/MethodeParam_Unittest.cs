using CodeConverterCore.Analyzer;
using CodeConverterCore.Helper;
using CodeConverterCore.Model;
using NUnit.Framework;

namespace CodeConverterCore_Unittest
{
    /// <summary>
    /// Text Analyzer linking of Types
    /// </summary>
    public class MethodeParam_Unittest
    {

        [Test]
        public void WriteIntToMethodeParam()
        {
            var tmpProject = new ProjectInformation();
            var tmpClass1 = Create.AddClass("Class1");
            var tmpFieldName = "inStr";
            tmpClass1.AddMethode("M1", new TypeContainer { Name = "void" }, Create.AddField(tmpClass1, tmpFieldName, new BaseType("string")));
            var tmpMethode = tmpClass1.MethodeList[0];
            tmpMethode.Code = new CodeBlock();
          var tmpSet= Create.SetFieldValue(tmpMethode.Code, tmpClass1.MethodeList[0].Parameter[0], new ConstantValue { Value = "\"BBBB\"" });
            new AnalyzerCore().LinkProjectInformation(tmpProject);

            Assert.AreEqual(tmpClass1.MethodeList[0].Parameter[0], (tmpSet.CodeEntries[0]  as SetFieldWithValue).VariableToAccess.CodeEntries[0]);
        }

    }
}