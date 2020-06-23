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
    public class VariableCreation_Unittest
    {
        [Test]
        public void CreateVariable()
        {
            var tmpClass = @"
package org;
public class Class1 {
public void Run(){
var tmpValue=4;
}
}
";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpMethodeContent = tmpObjectInformation.ClassList[0].MethodeList[0];
            var tmpCodeLine1 = (tmpMethodeContent.Code.CodeEntries[0] as StatementCode).StatementCodeBlocks[0].CodeEntries[0] as SetFieldWithValue;
            Assert.AreEqual("tmpValue", (tmpCodeLine1.VariableToAccess.CodeEntries[0] as ConstantValue).Value);
            Assert.AreEqual("4", (tmpCodeLine1.ValueToSet.CodeEntries[0] as ConstantValue).Value);
        }
    }
}
