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
    /// <summary>
    /// Checking Field Write Access
    /// </summary>
    public class TertiaerReturn_Unittest
    {
        [Test]
        public void SimpleSingleTypeConversionExplizitWithCSharpWrite()
        {
            var tmpClass = @"
package java.lang;
public abstract  class Class1 {
 public abstract boolean exists();
public abstract object toObject();
  @Override
  public String toString() {
    return exists() ? toObject().toString() : ""(null)"";
  }
    }
";
            var tmpObjectInformation = ProjectInformationHelper.DoFullRun(ImportHelper.ImportMappingList(ClassRenameJson.SystemAliasJson), new ConverterLucene(), new JavaLoader() { LoadDefaultData = true }, tmpClass);

            var tmpResult = CSharpWriter.CreateClassesFromObjectInformation(tmpObjectInformation, new ConverterLucene()).ToList().Last();

            var tmpExpectedResult = @"using System;

namespace System
{
    public abstract class Class1
    {
        public abstract bool Exists();

        public abstract object ToObject();

        public String ToString()
        {
            return Exists() ? ToObject().ToString() : ""(null)"";
        }
    }
}
";
            Assert.AreEqual(tmpExpectedResult, tmpResult.Content);
        }

    }
}
