using CodeConverterCore.Helper;
using CodeConverterJava.Model;
using CodeConverterCore.Model;
using NUnit.Framework;
using System.Collections.Generic;

namespace CodeConverterJava_Unittest.Objektstruktur
{
    public class MethodeCall_Unittest
    {
        [Test]
        public void CallKnownMethodeOnItselfNoAnalyzer()
        {
            var tmpClass = @"
package org;
public class Class1 {
public void Run(){
Run();
}}";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpMethodeContent = tmpObjectInformation.ClassList[0].MethodeList[0];
            Assert.AreEqual(1, tmpMethodeContent.Code.CodeEntries.Count);
            Assert.AreEqual("Run", (tmpMethodeContent.Code.CodeEntries[0] as MethodeCall).Name);
            Assert.AreEqual(0, (tmpMethodeContent.Code.CodeEntries[0] as MethodeCall).Parameter.Count);
        }

        [Test]
        public void CallUnknownMethodeOnItselfNoAnalyzer()
        {
            var tmpClass = @"
package org;
public class Class1 {
public void Run(){
NotRun();
}}";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpMethodeContent = tmpObjectInformation.ClassList[0].MethodeList[0];
            Assert.AreEqual(1, tmpMethodeContent.Code.CodeEntries.Count);
            Assert.AreEqual("NotRun", (tmpMethodeContent.Code.CodeEntries[0] as MethodeCall).Name);
            Assert.AreEqual(0, (tmpMethodeContent.Code.CodeEntries[0] as MethodeCall).Parameter.Count);
        }

    }
}
