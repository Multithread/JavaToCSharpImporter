using CodeConverterCore.Analyzer;
using CodeConverterCore.Helper;
using CodeConverterCSharp;
using CodeConverterJava.Model;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace CodeConverterJavaToCSharp_Unittest
{
    public class ConverterGithubExample
    {
        [Test]
        public void IfWIthConstant()
        {
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { JavaClass }, tmpIniData);
            new AnalyzerCore().LinkProjectInformation(tmpObjectInformation);

            var tmpResult = CSharpWriter.CreateClassesFromObjectInformation(tmpObjectInformation).ToList();
            Assert.AreEqual(1, tmpResult.Count);
            Assert.AreEqual("Class1", tmpResult[0].FullName);
            Assert.AreEqual(CSharpResult1, tmpResult[0].Content);
        }


        private string JavaClass = @"
package org.apache.lucene.util;

import java.util.Collections;

public class Class1 {
private string Value;
public Class1<int> CreateInstance(){
return null;
}
public void SetValue(string inValue){
Value=inValue;
}
}}";

        private string CSharpResult1 = @"using java.util.Collections;

namespace org.apache.lucene.util
{
    public class Class1
    {
        private string Value;

        public Class1<int> CreateInstance()
        {
        }

        public void SetValue(string inValue)
        {
        }
    }
}
";
    }
}
