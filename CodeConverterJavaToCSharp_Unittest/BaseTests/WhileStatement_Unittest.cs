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
    public class WhileStatement_Unittest
    {
        [Test]
        public void SimpleWhile()
        {
            var tmpClass = @"
package org;
public class Class1 {
public void OneSmallerThanTwo(){
while(true){
return true;
}
}
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
        public void OneSmallerThanTwo()
        {
            while (true)
            {
                return true;
            }
        }
    }
}
";
            //Check Elvis Result
            Assert.AreEqual(tmpExpectedResult, tmpResult.Content);
        }
    }
}
