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
    public class TypeConvertion_Unittest
    {
        [Test]
        public void SimpleSingleTypeConversionExplizit()
        {
            var tmpClass = @"
package org;
public class Class1 {
public void Run(){
var tmpValue=(String)Value;
}
}
";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpMethodeContent = tmpObjectInformation.ClassList[0].MethodeList[0];
            var tmpCodeLine1 = (tmpMethodeContent.Code.CodeEntries[0] as VariableDeclaration);
            Assert.AreEqual("tmpValue", tmpCodeLine1.Name);

            var tmpFirstConversion = tmpMethodeContent.Code.CodeEntries[1] as TypeConversion;
            Assert.AreEqual("String", tmpFirstConversion.Type.ToString());
            Assert.AreEqual("Value", (tmpFirstConversion.PreconversionValue.CodeEntries[0] as ConstantValue).Value);
        }

        [Test]
        public void DoubleConversionExplizit()
        {
            var tmpClass = @"
package org;
public class Class1 {
public void Run(){
var tmpValue=(Int32)(String)Value;
}
}
";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpMethodeContent = tmpObjectInformation.ClassList[0].MethodeList[0];
            var tmpCodeLine1 = (tmpMethodeContent.Code.CodeEntries[0] as VariableDeclaration);
            Assert.AreEqual("tmpValue", tmpCodeLine1.Name);

            var tmpFirstConversion = tmpMethodeContent.Code.CodeEntries[1] as TypeConversion;
            var tmpSecondConversion = (tmpFirstConversion.PreconversionValue.CodeEntries[0] as TypeConversion);

            Assert.AreEqual("Int32", tmpFirstConversion.Type.ToString());
            Assert.AreEqual("String", tmpSecondConversion.Type.ToString());
            Assert.AreEqual("Value", (tmpSecondConversion.PreconversionValue.CodeEntries[0] as ConstantValue).Value);
        }
    }
}
