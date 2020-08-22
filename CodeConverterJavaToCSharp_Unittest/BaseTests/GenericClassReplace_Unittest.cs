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
    public class GenericClassReplace_Unittest
    {
        [Test]
        public void SingleGenericClassReplace()
        {
            var tmpObjectInformation = ProjectInformationHelper.DoFullRun(
                ImportHelper.ImportMappingList(ClassRenameJson.SystemAliasJson), new ConverterLucene(), new JavaLoader() { LoadDefaultData = true },
                SingleReplacement);

            var tmpResult = CSharpWriter.CreateClassesFromObjectInformation(tmpObjectInformation, new ConverterLucene()).ToList().Last();
            
            Assert.AreEqual("Bits", tmpResult.FullName);

            Assert.AreEqual(CSharpResultSingle, tmpResult.Content);
        }


        private string SingleReplacement = @"package org.apache.lucene.util;
        public abstract class Bits
        {
            public abstract void AbstrMethode(){
                Class<? extends MutableValue> c1 = this.getClass();
            }
        }";

        private string CSharpResultSingle = @"using System;

namespace LuceNET.util
{
    public abstract class Bits
    {
        public void AbstrMethode()
        {
            Type c1 =             this.GetType();
        }
    }
}
";

        [Test]
        public void DoubleGenericClassReplace()
        {
            var tmpObjectInformation = ProjectInformationHelper.DoFullRun(
                ImportHelper.ImportMappingList(ClassRenameJson.SystemAliasJson), new ConverterLucene(), new JavaLoader() { LoadDefaultData = true },
                DoubleReplacement);

            var tmpResult = CSharpWriter.CreateClassesFromObjectInformation(tmpObjectInformation, new ConverterLucene()).ToList().Last();

            Assert.AreEqual("Bits", tmpResult.FullName);

            Assert.AreEqual(CSharpResultDouble, tmpResult.Content);
        }

        private string DoubleReplacement = @"package org.apache.lucene.util;
        public abstract class Bits
        {
            public abstract void AbstrMethode(Bits other){
                Class<? extends MutableValue> c1 = this.getClass();
                Class<? extends MutableValue> c2 = other.getClass();
            }
        }";


        private string CSharpResultDouble = @"using System;

namespace LuceNET.util
{
    public abstract class Bits
    {
        public void AbstrMethode(Bits inOther)
        {
            Type c1 =             this.GetType();
            Type c2 =             inOther.GetType();
        }
    }
}
";
    }
}
