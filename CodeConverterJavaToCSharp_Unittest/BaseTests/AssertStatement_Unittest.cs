using CodeConverterCore.Analyzer;
using CodeConverterCore.Helper;
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
    public class AssertStatement_Unittest
    {
        [Test]
        public void SimpleAssert()
        {
            var tmpClass = @"
package org;
public class Class1 {
public void OneSmallerThanTwo(int in1, int in2){
assert in1>0;
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
        public void OneSmallerThanTwo(int in1, int in2)
        {
            Trace.Assert (in1 > 0);
        }
    }
}
";
            //Statement IF
            Assert.AreEqual(tmpExpectedResult, tmpResult[0].Content);

        }
    }
}
