using CodeConverterCore.Analyzer;
using CodeConverterCore.Converter;
using CodeConverterCore.Helper;
using CodeConverterCore.Model;
using CodeConverterCSharp;
using CodeConverterCSharp.Lucenene;
using CodeConverterJava.Model;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace CodeConverterJavaToCSharp_Unittest
{
    public class PropertyWithDefaultValue_Unittest
    {
        [Test]
        public void PropertyInInterfaceWithDefaultValue()
        {
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { JavaClass }, tmpIniData);
            var tmpAnalyerSettings = new AnalyzerSettings();
            var tmpUnknownTypeCounter = 0;
            tmpAnalyerSettings.UnknownTypeAdded += (UnknownTypeClass inItem) => { tmpUnknownTypeCounter++; };
            new AnalyzerCore().LinkProjectInformation(tmpObjectInformation, tmpAnalyerSettings);

            new NamingConvertionHelper(new ConverterLucene()).ConvertProject(tmpObjectInformation);
            var tmpResult = CSharpWriter.CreateClassesFromObjectInformation(tmpObjectInformation, new ConverterLucene()).ToList();
            
            Assert.AreEqual(1, tmpResult.Count);
            Assert.AreEqual("Bits", tmpResult[0].FullName);

            Assert.AreEqual(CSharpResult1, tmpResult[0].Content);
        }


        private string JavaClass = @"package org.apache.lucene.util;
        public interface Bits
        {
            Bits[] EMPTY_ARRAY = new Bits[0];
}";

        private string CSharpResult1 = @"

namespace LuceNET.util
{
    public interface Bits
    {
        Bits[] EMPTY_ARRAY =  new Bits[0];

    }
}
";
    }
}
