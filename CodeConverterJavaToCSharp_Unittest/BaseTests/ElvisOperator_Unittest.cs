using CodeConverterCore.Analyzer;
using CodeConverterCore.Enum;
using CodeConverterCore.Helper;
using CodeConverterCore.Model;
using CodeConverterCSharp;
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
    public class ElvisOperator_Unittest
    {
        [Test]
        public void SimpleReturnElvis()
        {
            var tmpClass = @"
package org;
public class Class1 {
public void OneSmallerThanTwo(bool inFirst,int in1, int in2){
return inFirst? in1:in2;
}
";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            new AnalyzerCore().LinkProjectInformation(tmpObjectInformation);

            var tmpResult = CSharpWriter.CreateClassesFromObjectInformation(tmpObjectInformation, new ConverterJavaToCSharp()).ToList();

            var tmpExpectedResult = @"

namespace org
{
    public class Class1
    {
        public void OneSmallerThanTwo(bool inFirst, int in1, int in2)
        {
            return inFirst ? in1 : in1;
        }
    }
}
";
            //Check Elvis Result
            Assert.AreEqual(tmpExpectedResult, tmpResult[0].Content);
        }
    }
}