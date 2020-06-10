using JavaToCSharpConverter.Helper;
using JavaToCSharpConverter.Model.Java;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace JavaCodeLoader_Unittest.Objektstruktur
{
    public class MethodeCode_Unittest
    {
        [Test]
        public void ClassListWithOneClass()
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
            Assert.AreNotEqual(null, tmpMethodeContent.Code);
        }
    }
}
