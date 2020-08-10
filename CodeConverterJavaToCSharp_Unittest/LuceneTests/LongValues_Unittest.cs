using CodeConverterCore.Analyzer;
using CodeConverterCore.Converter;
using CodeConverterCore.Helper;
using CodeConverterCSharp;
using CodeConverterCSharp.Lucenene;
using CodeConverterJava.Model;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CodeConverterJavaToCSharp_Unittest.LuceneTests
{
//    public abstract class LongValues
//    {


//        /** Get value at <code>index</code>. */
//        public abstract long get(long index);

//        /** An instance that returns the provided value. */
//        public static LongValues IDENTITY = new LongValues()
//        {

//          public override long get(long index)
//        {
//            return index;
//        }

//    };

//    public static LongValues ZEROES = new LongValues()
//    {



//          public override long get(long index)
//    {
//        return 0;
//    }

//};
//    }

    class LongValues_Unittest
    {
        [Test]
        public void CheckAccountableInterfaceComments()
        {
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { JavaBits }, tmpIniData);
            new AnalyzerCore().LinkProjectInformation(tmpObjectInformation);

            new NamingConvertionHelper(new ConverterLucene()).ConvertProject(tmpObjectInformation);

            var tmpResult = CSharpWriter.CreateClassesFromObjectInformation(tmpObjectInformation, new ConverterLucene()).ToList();

            Assert.AreEqual(1, tmpResult.Count);
            Assert.AreEqual("Accountable", tmpResult[0].FullName);
            //Check for no double Comments
            Assert.AreEqual(false, tmpResult[0].Content.Contains("/*/*"));
            //Check for no double Comments
            int tmpCommentLineCount = new Regex(Regex.Escape("///")).Matches(tmpResult[0].Content).Count;
            Assert.AreEqual(29, tmpCommentLineCount);

            Assert.AreEqual(true, tmpResult[0].Content.Contains("Collections.emptyList();"));
            Assert.AreEqual(true, tmpResult[0].Content.Contains("public Collection<Accountable>"));
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


        import java.util.Collection;
        import java.util.Collections;

        /**
         * An object whose RAM usage can be computed.
         *
         * @lucene.internal
         */
        public interface Accountable
        {

            /**
             * Return the memory usage of this object in bytes. Negative values are illegal.
             */
            long ramBytesUsed();

  /**
   * Returns nested resources of this class. 
   * The result should be a point-in-time snapshot (to avoid race conditions).
   * @see Accountables
   */
  default Collection<Accountable> getChildResources()
            {
                return Collections.emptyList();
            }

        }
";
    }
}
