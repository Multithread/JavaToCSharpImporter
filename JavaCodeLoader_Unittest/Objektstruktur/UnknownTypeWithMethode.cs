using CodeConverterCore.Analyzer;
using CodeConverterCore.Helper;
using CodeConverterJava.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeConverterJava_Unittest.Objektstruktur
{
   public  class UnknownTypeWithMethode
    {
        [Test]
        public void SimpleMethode()
        {
            var tmpClass = @"
package org.apache.lucene.util;

public interface Accountable{
    default Collection<Accountable> getChildResources()
    {
        return Collections.emptyList();
    }
}";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpProjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            new AnalyzerCore().LinkProjectInformation(tmpProjectInformation);

            var tmpClass1 = tmpProjectInformation.ClassList[0];
            Assert.AreEqual("getChildResources", tmpClass1.MethodeList[0].Name);

            Assert.AreEqual(2, tmpProjectInformation.GetAllUnknownTypes().Count);
        }
    }
}
