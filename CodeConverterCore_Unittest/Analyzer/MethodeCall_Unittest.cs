using CodeConverterCore.Analyzer;
using CodeConverterCore.Helper;
using CodeConverterCore.Model;
using NUnit.Framework;
using System.Collections.Generic;

namespace CodeConverterCore_Unittest
{
    /// <summary>
    /// Text Analyzer linking of Types
    /// </summary>
    public class MethodeCall_Unittest
    {

        [Test]
        public void CallKnownMethodeOnItself()
        {
            var tmpProject = new ProjectInformation();
            var tmpClass1 = Create.AddClass("Class1");
            tmpProject.FillClasses(new List<ClassContainer> { tmpClass1 });
            var tmpMethodeName = "inStr";
            tmpClass1.AddMethode(tmpMethodeName, new TypeContainer { Name = "void" });
            var tmpMethode = tmpClass1.MethodeList[0];
            tmpMethode.Code = new CodeBlock();
            var tmpMethodeCall = Create.CallMethode(tmpMethode.Code, tmpMethodeName);
            new AnalyzerCore().LinkProjectInformation(tmpProject);


            Assert.AreEqual(tmpMethode, tmpMethodeCall.MethodeLink);
        }

        //[Test]
        //public void CallKnownMethodeOnItself()
        //{
        //    var tmpProject = new ProjectInformation();
        //    var tmpClass1 = Create.AddClass("Class1");
        //    var tmpMethodeName = "inStr";
        //    tmpClass1.AddMethode(tmpMethodeName, new TypeContainer { Name = "void" }, Create.AddField(tmpClass1, tmpMethodeName, new BaseType("string")));
        //    var tmpMethode = tmpClass1.MethodeList[0];
        //    tmpMethode.Code = new CodeBlock();
        //    var tmpSet = Create.SetFieldValue(tmpMethode.Code, tmpClass1.MethodeList[0].Parameter[0], new ConstantValue { Value = "\"BBBB\"" });
        // 
        //    new AnalyzerCore().LinkProjectInformation(tmpProject);

        //    Assert.AreEqual(tmpClass1.MethodeList[0].Parameter[0], (tmpSet.CodeEntries[0] as SetFieldWithValue).VariableToAccess.CodeEntries[0]);
        //}

    }
}