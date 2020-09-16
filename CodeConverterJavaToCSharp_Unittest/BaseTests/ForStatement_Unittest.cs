using CodeConverterCore.Analyzer;
using CodeConverterCore.Converter;
using CodeConverterCore.Enum;
using CodeConverterCore.Helper;
using CodeConverterCore.Model;
using CodeConverterCSharp;
using CodeConverterCSharp.Lucenene;
using CodeConverterJava.Model;
using JavaToCSharpConverter.Model;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace CodeConverterJavaToCSharp_Unittest
{
    /// <summary>
    /// Checking Field Write Access
    /// </summary>
    public class ForStatement_Unittest
    {
        [Test]
        public void SimpleFor()
        {
            var tmpClass = @"
package org;
public class Class1 {
public void forIInLength(int len){
 for(int i=0;i<len;i++) {
       i--;
      }
return false;
}
";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);
            new AnalyzerCore().LinkProjectInformation(tmpObjectInformation);

            new NamingConvertionHelper(new ConverterLucene()).ConvertProject(tmpObjectInformation);

            var tmpResult = CSharpWriter.CreateClassesFromObjectInformation(tmpObjectInformation, new ConverterJavaToCSharp())
                .ToList()
                .Last();

            var tmpExpectedResult = @"

namespace org
{
    public class Class1
    {
        public void ForIInLength(int inLen)
        {
            for (int i = 0:(i < inLen):i++)
            {
                i--;
            }
            return false;
        }
    }
}
";
            //Check Elvis Result
            Assert.AreEqual(tmpExpectedResult, tmpResult.Content);
        }
    }
}
