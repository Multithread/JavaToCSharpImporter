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
    public class TypeConvertion_Unittest
    {
        [Test]
        public void SimpleSingleTypeConversionExplizit()
        {
            var tmpClass = @"
package java.lang;
public class Class1 {
public void Run(object value){
var tmpValue=(String)value;
}
}
";
            var tmpObjectInformation = ProjectInformationHelper.DoFullRun(ImportHelper.ImportMappingList(ClassRenameJson.SystemAliasJson), new ConverterLucene(), new JavaLoader() { LoadDefaultData = true }, tmpClass);

            var tmpMethodeContent = tmpObjectInformation.ClassList.Last().MethodeList[0];

            var tmpCodeLine1 = (tmpMethodeContent.Code.CodeEntries[0] as VariableDeclaration);
            Assert.AreEqual("tmpValue", tmpCodeLine1.Name);

            var tmpFirstConversion = tmpMethodeContent.Code.CodeEntries[1] as TypeConversion;
            Assert.AreEqual("String", tmpFirstConversion.Type.ToString());
            Assert.AreEqual("inValue", ((tmpFirstConversion.PreconversionValue.CodeEntries[0] as ConstantValue).Value as FieldContainer).Name);
        }

        [Test]
        public void SimpleSingleTypeConversionExplizitWithCSharpWrite()
        {
            var tmpClass = @"
package java.lang;
public class Class1 {
public void Run(object value){
var tmpValue=(String)value;
}
}
";
            var tmpObjectInformation = ProjectInformationHelper.DoFullRun(ImportHelper.ImportMappingList(ClassRenameJson.SystemAliasJson), new ConverterLucene(), new JavaLoader() { LoadDefaultData = true }, tmpClass);

            var tmpResult = CSharpWriter.CreateClassesFromObjectInformation(tmpObjectInformation, new ConverterLucene()).ToList();

            var tmpExpectedResult = @"using System;

namespace System
{
    public class Class1
    {
        public void Run(object inValue)
        {
            var tmpValue =             (String)inValue;
        }
    }
}
";
            Assert.AreEqual(tmpExpectedResult, tmpResult.Last().Content);
        }

        [Test]
        public void DoubleConversionExplizit()
        {
            var tmpClass = @"
package java.lang;
public class Class1 {
public void Run(object value){
var tmpValue=(Int32)(String)value;
}
}
";
            var tmpObjectInformation = ProjectInformationHelper.DoFullRun(ImportHelper.ImportMappingList(ClassRenameJson.SystemAliasJson), new ConverterLucene(), new JavaLoader() { LoadDefaultData = true }, tmpClass);


            var tmpResult = CSharpWriter.CreateClassesFromObjectInformation(tmpObjectInformation, new ConverterLucene()).ToList();

            var tmpExpectedResult = @"using System;

namespace System
{
    public class Class1
    {
        public void Run(object inValue)
        {
            var tmpValue =             (Int32)(String)inValue;
        }
    }
}
";
            Assert.AreEqual(tmpExpectedResult, tmpResult.Last().Content);
        }
    }
}
