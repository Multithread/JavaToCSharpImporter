using CodeConverterCore.Analyzer;
using CodeConverterCore.Converter;
using CodeConverterCore.Helper;
using CodeConverterCore.ImportExport;
using CodeConverterCore.Model;
using CodeConverterCSharp;
using CodeConverterCSharp.Lucenene;
using CodeConverterJava.Model;
using JavaToCSharpConverter.Resources;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace CodeConverterJavaToCSharp_Unittest
{
    public class JavaClassMapping_Unittest
    {
        [Test]
        public void PropertyInInterfaceWithDefaultValue()
        {
            var tmpAnalyerSettings = new AnalyzerSettings();

            var tmpObjectInformation = ProjectInformationHelper.DoFullRun(ImportHelper.ImportMappingList(ClassRenameJson.SystemAliasJson), new ConverterLucene(), new JavaLoader() { LoadDefaultData = true }, JavaClass);

            var tmpResult = CSharpWriter.CreateClassesFromObjectInformation(tmpObjectInformation, new ConverterLucene())
                .Where(inItem=> inItem.FullName == "MutableValue")
                .ToList();

            Assert.AreEqual(1, tmpResult.Count);
            Assert.AreEqual("MutableValue", tmpResult[0].FullName);

            Assert.AreEqual(CSharpResult1, tmpResult[0].Content);
        }


        private string JavaClass = @"package java.lang;
public abstract class MutableValue
{
    public int compareTo(MutableValue other) {
        Class<? extends MutableValue> c1 = other.getClass();
    }
}";

        private string CSharpResult1 = @"using System;

namespace System
{
    public class MutableValue
    {
        public int CompareTo(MutableValue inOther)
        {
            Type c1 =             inOther.GetType();
        }
    }
}
";
    }
}
