using CodeConverterCore.Enum;
using CodeConverterCore.Helper;
using CodeConverterCore.Model;
using CodeConverterJava.Model;
using NUnit.Framework;
using System.Collections.Generic;

namespace CodeConverterJava_Unittest.Methode
{
    /// <summary>
    /// Checking Field Write Access
    /// </summary>
    public class MethodeCall_Unittest
    {
        [Test]
        public void CallFunctionWithoutParameter()
        {
            var tmpClass = @"
package org;
public class Class1 {
public void Run(){
Run();
}
}
";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpMethodeContent = tmpObjectInformation.ClassList[0].MethodeList[0];
            var tmpCodeLine1 = (tmpMethodeContent.Code.CodeEntries[0]) as MethodeCall;
            Assert.AreEqual("Run", tmpCodeLine1.Name);
        }

        [Test]
        public void CallFunctionWithParameter()
        {
            var tmpClass = @"
package org;
public class Class1 {
public void Run(string inData){
Run(inData);
}
}
";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpMethodeContent = tmpObjectInformation.ClassList[0].MethodeList[0];
            var tmpCodeLine1 = (tmpMethodeContent.Code.CodeEntries[0] as MethodeCall);
            Assert.AreEqual("Run", tmpCodeLine1.Name);
            var tmpEntry = tmpCodeLine1.Parameter[0].CodeEntries[0] as ConstantValue;
            Assert.AreEqual("inData", tmpEntry.Value);
        }

        [Test]
        public void CallFunctionWithThisParameter()
        {
            var tmpClass = @"
package org;
public class Class1 {
public void Run(){
this.Run();
}
}
";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpMethodeContent = tmpObjectInformation.ClassList[0].MethodeList[0];
            var tmpVarAccess = (tmpMethodeContent.Code.CodeEntries[0]) as VariableAccess;
            Assert.AreEqual("this", (tmpVarAccess.Access as ConstantValue).Value);
            var tmpMethode = (tmpVarAccess.Child as VariableAccess).Access as MethodeCall;
            Assert.AreEqual("Run", tmpMethode.Name);
        }
    }
}
