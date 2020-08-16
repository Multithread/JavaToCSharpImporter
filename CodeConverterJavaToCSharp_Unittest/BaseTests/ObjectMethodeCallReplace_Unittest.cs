using CodeConverterCore.Analyzer;
using CodeConverterCore.Converter;
using CodeConverterCore.Helper;
using CodeConverterCore.ImportExport;
using CodeConverterCSharp;
using CodeConverterCSharp.Lucenene;
using CodeConverterJava.Model;
using JavaToCSharpConverter.Resources;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CodeConverterJavaToCSharp_Unittest.LuceneTests
{

    public class ObjectMethodeCallReplace_Unittest
    {
        [Test]
        public void CheckAccountableInterfaceComments()
        {

            var tmpObjectInformation = ProjectInformationHelper.DoFullRun(
                ImportHelper.ImportMappingList(ClassRenameJson.SystemAliasJson), new ConverterLucene(), new JavaLoader() { LoadDefaultData = true },
                JavaBits);

            var tmpResult = CSharpWriter.CreateClassesFromObjectInformation(tmpObjectInformation, new ConverterLucene()).ToList().Last();

            Assert.AreEqual("MutableValue", tmpResult.FullName);

            Assert.AreEqual(true, tmpResult.Content.Contains("Type c1 =             this.GetType()"));
            Assert.AreEqual(true, tmpResult.Content.Contains(" - c2.GetHashCode()"));
        }

        private string JavaBits = @"
package org.apache.lucene.util.mutable;

public abstract class MutableValue implements Comparable<MutableValue> {

    public void compareTo(MutableValue other)
    {
        Class <? extends MutableValue > c1 = this.getClass();
        Class <? extends MutableValue > c2 = other.getClass();
        int c = c1.hashCode() -c2.hashCode();            
    }
}";
    }
}
