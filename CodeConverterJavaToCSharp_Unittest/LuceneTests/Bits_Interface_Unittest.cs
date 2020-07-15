using CodeConverterCore.Analyzer;
using CodeConverterCore.Converter;
using CodeConverterCore.Helper;
using CodeConverterCore.Model;
using CodeConverterCSharp;
using CodeConverterCSharp.Lucenene;
using CodeConverterJava.Model;
using JavaToCSharpConverter.Model;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace CodeConverterJavaToCSharp_Unittest.LuceneTests
{
    public class Bits_Interface_Unittest
    {
        [Test]
        public void CheckBits1()
        {
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { JavaBits }, tmpIniData);
            var tmpAnalyerSettings = new AnalyzerSettings();
            var tmpUnknownTypeCounter = 0;
            tmpAnalyerSettings.UnknownTypeAdded += (UnknownTypeClass inItem) => { tmpUnknownTypeCounter++; };
            new AnalyzerCore().LinkProjectInformation(tmpObjectInformation, tmpAnalyerSettings);

            new NamingConvertionHelper(new ConverterLucene()).ConvertProject(tmpObjectInformation);
            var tmpResult = CSharpWriter.CreateClassesFromObjectInformation(tmpObjectInformation, new ConverterLucene()).ToList();
            Assert.AreEqual(1, tmpResult.Count);
            Assert.AreEqual("Bits", tmpResult[0].FullName);
            //Check for inner classes existing

            //Expecting four unknown Types: true, false, int and boolean, two which I have not yet added a Pre-Runtime class
            Assert.AreEqual(4, tmpUnknownTypeCounter);

            Assert.AreEqual(true, tmpResult[0].Content.Contains("MatchAllBits"));

            Assert.AreEqual(true, tmpResult[0].Content.Contains("this.len = inLen;"));

            Assert.AreEqual(true, tmpResult[0].Content.Contains("Bits[] EMPTY_ARRAY =  new Bits[0];"));
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
         * Interface for Bitset-like structures.
         * @lucene.experimental
         */

        public interface Bits
        {
            /** 
             * Returns the value of the bit with the specified <code>index</code>.
             * @param index index, should be non-negative and &lt; {@link #length()}.
             *        The result of passing negative or out of bounds values is undefined
             *        by this interface, <b>just don't do it!</b>
             * @return <code>true</code> if the bit is set, <code>false</code> otherwise.
             */
            boolean get(int index);

            /** Returns the number of bits in this set */
            int length();

            Bits[] EMPTY_ARRAY = new Bits[0];

            /**
             * Bits impl of the specified length with all bits set. 
             */
            class MatchAllBits implements Bits
            {
                final int len;

            public MatchAllBits(int len)
            {
                this.len = len;
            }

            @Override
            public boolean get(int index)
            {
                return true;
            }

            @Override
            public int length()
            {
                return len;
            }
        }

        /**
         * Bits impl of the specified length with no bits set. 
         */
        class MatchNoBits implements Bits
        {
            final int len;

        public MatchNoBits(int len)
        {
            this.len = len;
        }

        @Override
    public boolean get(int index)
        {
            return false;
        }

        @Override
    public int length()
        {
            return len;
        }
    }
}
";
    }
}
