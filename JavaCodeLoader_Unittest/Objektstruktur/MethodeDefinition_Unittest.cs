using JavaToCSharpConverter.Helper;
using JavaToCSharpConverter.Model.Java;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace JavaCodeLoader_Unittest.Objektstruktur
{
    public class MethodeDefinition_Unittest
    {
        [Test]
        public void SimpleMethode()
        {
            var tmpClass = @"
package org;
public class Class1 {
    public void Run()
    {}
}";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpClass1 = tmpObjectInformation.ClassList[0];
            Assert.AreEqual("Run", tmpClass1.MethodeList[0].Name);
        }
    }
}
