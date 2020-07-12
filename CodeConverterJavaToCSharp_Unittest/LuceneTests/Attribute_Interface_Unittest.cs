using CodeConverterCore.Analyzer;
using CodeConverterCore.Helper;
using CodeConverterCSharp;
using CodeConverterCSharp.Lucenene;
using CodeConverterJava.Model;
using JavaToCSharpConverter.Model;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CodeConverterJavaToCSharp_Unittest.LuceneTests
{
    public class Attribute_Interface_Unittest
    {
        [Test]
        public void CheckAttributeInterfaceComments()
        {
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { JavaBits }, tmpIniData);
            new AnalyzerCore().LinkProjectInformation(tmpObjectInformation);

            var tmpResult = CSharpWriter.CreateClassesFromObjectInformation(tmpObjectInformation, new ConverterLucene()).ToList();
            Assert.AreEqual(1, tmpResult.Count);
            Assert.AreEqual("Attribute", tmpResult[0].FullName);
            //Check for no double Comments
            Assert.AreEqual(false, tmpResult[0].Content.Contains("/*/*"));
            //Check for no double Comments
            int tmpCommentLineCount = new Regex(Regex.Escape("///")).Matches(tmpResult[0].Content).Count;
            Assert.AreEqual(19, tmpCommentLineCount);
        }

        private string JavaBits = @"/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the ""License""); you may not use this file except in compliance with
 * the License.You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an ""AS IS"" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
package org.apache.lucene.util;

/**
 * Base interface for attributes.
 */
public interface Attribute
{
}";
    }
}
