using CodeConverterCore.Enum;
using CodeConverterCore.Helper;
using CodeConverterCore.Model;
using CodeConverterJava.Model;
using NUnit.Framework;
using System.Collections.Generic;

namespace CodeConverterJava_Unittest.Objektstruktur
{
    /// <summary>
    /// Checking Field Write Access
    /// </summary>
    public class FieldAccess_Unittest
    {
        [Test]
        public void MethodeVariableDeclaration()
        {
            var tmpClass = @"
package org;
public class Class1 {
public int Value;
public void Run(){
Value=4;
}
}
";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpMethodeContent = tmpObjectInformation.ClassList[0].MethodeList[0];
            var tmpCodeLine1 = (tmpMethodeContent.Code.CodeEntries[0] as StatementCode).StatementCodeBlocks[0].CodeEntries[0] as SetFieldWithValue;
            Assert.AreEqual("Value", (tmpCodeLine1.VariableToAccess.CodeEntries[0] as ConstantValue).Value);
            Assert.AreEqual("4", (tmpCodeLine1.ValueToSet.CodeEntries[0] as ConstantValue).Value);
        }
    }
}
