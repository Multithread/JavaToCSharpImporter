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
    public class NewObjectParamCall_Unittest
    {
        [Test]
        public void SimpleReturnNewObject()
        {
            var tmpClass = @"
package org;
public class Class1 {
public void OneSmallerThanTwo(int in1){
return new Date(in1);
}
";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpMethodeContent = tmpObjectInformation.ClassList[0].MethodeList[0];
            var tmpNewObject = ((tmpMethodeContent.Code.CodeEntries[0]) as ReturnCodeEntry).CodeEntries[0] as NewObjectDeclaration;

            //new Statement with Parameters
            Assert.AreNotEqual(null, tmpNewObject);
            Assert.AreEqual(1, tmpNewObject.ArgumentList.Count);
        }
        [Test]
        public void SetFieldTonewValue()
        {
            var tmpClass = @"
package org;
public class Class1 {
public void OneSmallerThanTwo(int in1){
var b= new Date(in1);
}
";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpMethodeContent = tmpObjectInformation.ClassList[0].MethodeList[0];
            var tmpNewObject = ((tmpMethodeContent.Code.CodeEntries[0]) as VariableDeclaration);

            //new Statement with Parameters
            Assert.AreEqual(2, tmpMethodeContent.Code.CodeEntries.Count);
        }
    }
}
