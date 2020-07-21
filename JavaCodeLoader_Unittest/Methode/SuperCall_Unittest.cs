using CodeConverterCore.Converter;
using CodeConverterCore.Helper;
using CodeConverterCore.ImportExport;
using CodeConverterCore.Model;
using CodeConverterJava.Model;
using JavaToCSharpConverter.Resources;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace CodeConverterJava_Unittest.Methode
{
    /// <summary>
    /// Checking Field Write Access
    /// </summary>
    public class SuperCall_Unittest
    {
        [Test]
        public void SuperCallInCOnstructor()
        {
            var tmpClass = @"
package org;
public final class ThreadInterruptedException extends RuntimeException {
  public ThreadInterruptedException(InterruptedException ie) {
    super(ie);
  }}";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpMethodeContent = tmpObjectInformation.ClassList[0].MethodeList[0];
            var tmpCodeLine1 = (tmpMethodeContent.Code.CodeEntries[0]) as MethodeCall;
            Assert.AreEqual("super", tmpCodeLine1.Name);
        }

        [Test]
        public void SuperCallModifiedToBase()
        {
            var tmpClass = @"
package org;
public final class ThreadInterruptedException extends RuntimeException {
  public ThreadInterruptedException(InterruptedException ie) {
    super(ie);
  }}";
            var tmpObjectInformation = ProjectInformationHelper.DoFullRun(new List<LanguageMappingObject>(), new ConverterBase(), new JavaLoader() { LoadDefaultData = true }, tmpClass);

            var tmpMethodeContent = tmpObjectInformation.ClassList.Last().MethodeList[0];
            Assert.AreEqual("ThreadInterruptedException", tmpMethodeContent.Name);
            Assert.AreEqual("super", (tmpMethodeContent.Code.CodeEntries[0] as MethodeCall).Name);
        }

    }
}
