﻿using CodeConverterCore.Helper;
using CodeConverterCore.ImportExport;
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
    public class SystemMerging_Unittest
    {
        [Test]
        public void SuperCallModifiedToBase()
        {
            var tmpClass = @"
package org;
public final class ThreadInterruptedException extends RuntimeException {
  public ThreadInterruptedException(InterruptedException ie) {
    super(ie);
  }}";
            var tmpObjectInformation = ProjectInformationHelper.DoFullRun(ImportHelper.ImportMappingList(ClassRenameJson.SystemAliasJson), new ConverterLucene(), new JavaLoader() { LoadDefaultData = true }, tmpClass);

            var tmpMethodeContent = tmpObjectInformation.ClassList.Last().MethodeList[0];
            Assert.AreEqual("base", tmpMethodeContent.ConstructorCall.Name);
        }

        [Test]
        public void SuperCallModifiedToBaseToCSharp()
        {
            var tmpClass = @"
package org;
public final class ThreadInterruptedException extends RuntimeException {
  public ThreadInterruptedException(InterruptedException ie) {
    super(ie);
  }}";
            var tmpObjectInformation = ProjectInformationHelper.DoFullRun(ImportHelper.ImportMappingList(ClassRenameJson.SystemAliasJson), new ConverterLucene(), new JavaLoader() { LoadDefaultData = true }, tmpClass);

            var tmpResult = CSharpWriter.CreateClassesFromObjectInformation(tmpObjectInformation, new ConverterLucene()).ToList();

            var tmpExpectedResult = @"using System;

namespace org
{
    public sealed class ThreadInterruptedException : RuntimeException
    {
        public ThreadInterruptedException(InterruptedException inIe)
            :base(inIe)
        {
        }
    }
}
";
            var tmpResultText = tmpResult.Last().Content;
            Assert.AreEqual(tmpExpectedResult, tmpResultText);
        }

        [Test]
        public void Test()
        {
            var tmpClass = @"
package java.lang;
public class Collection<T>{
}";
            var tmpObjectInformation = ProjectInformationHelper.DoFullRun(ImportHelper.ImportMappingList(ClassRenameJson.SystemAliasJson), new ConverterLucene(), new JavaLoader() { LoadDefaultData = true }, tmpClass);

            var tmpResult = CSharpWriter.CreateClassesFromObjectInformation(tmpObjectInformation, new ConverterLucene()).ToList();

            ///Write to JSON to add additional Java Base Classes
            var b= ExportHelper.CreateJsonFromClassList(tmpObjectInformation.ClassList.Where(inItem=> inItem.ClassType == CodeConverterCore.Enum.ClassTypeEnum.Normal).ToList());
        }
    }
}
