using CodeConverterCore.Helper;
using CodeConverterJava.Model;
using CodeConverterCore.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeConverterJava_Unittest.Objektstruktur
{
    public class MethodeCode_Unittest
    {
        [Test]
        public void MethodeVariableDeclaration()
        {
            var tmpClass = @"
package org;
public class Class1 {
public void Run(){
int tmpInt;
}
}
";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpMethodeContent = tmpObjectInformation.ClassList[0].MethodeList[0];
            Assert.AreEqual(1, tmpMethodeContent.Code.CodeEntries.Count);
            Assert.AreEqual("int", (tmpMethodeContent.Code.CodeEntries[0] as VariableDeclaration).Type.Name);
        }

        [Test]
        public void MethodeVariableWithSimpleValueSetAsInt()
        {
            var tmpClass = @"
package org;
public class Class1 {
public void Run(){
var tmpInt=0;
}
}
";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpMethodeContent = tmpObjectInformation.ClassList[0].MethodeList[0];
            Assert.AreEqual("0", (tmpMethodeContent.Code.CodeEntries[1] as ConstantValue).Value);
        }

        [Test]
        public void MethodeVariableWithSimpleValueSetAsString()
        {
            var tmpClass = @"
package org;
public class Class1 {
public void Run(){
var tmpStr=""0"";
}
}
";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpMethodeContent = tmpObjectInformation.ClassList[0].MethodeList[0];
            Assert.AreEqual("\"0\"", (tmpMethodeContent.Code.CodeEntries[1] as ConstantValue).Value);
        }
    }
}
