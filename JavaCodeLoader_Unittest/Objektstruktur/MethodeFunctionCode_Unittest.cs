using CodeConverterCore.Enum;
using CodeConverterCore.Helper;
using CodeConverterJava.Model;
using CodeConverterCore.Model;
using NUnit.Framework;
using System.Collections.Generic;

namespace CodeConverterJava_Unittest.Objektstruktur
{
    public class MethodeFunctionCode_Unittest
    {
        [Test]
        public void IfWIthConstant()
        {
            var tmpClass = @"
package org;
public class Class1 {
public void Run(){
if(true){}
}}";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpMethodeContent = tmpObjectInformation.ClassList[0].MethodeList[0].Code.CodeEntries[0];
            Assert.AreEqual(StatementTypeEnum.If, (tmpMethodeContent as StatementCode).StatementType);
        }

    }
}
