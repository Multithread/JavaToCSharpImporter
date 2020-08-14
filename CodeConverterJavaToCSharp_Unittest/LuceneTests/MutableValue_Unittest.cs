using CodeConverterCore.Analyzer;
using CodeConverterCore.Converter;
using CodeConverterCore.Helper;
using CodeConverterCore.ImportExport;
using CodeConverterCSharp;
using CodeConverterCSharp.Lucenene;
using CodeConverterJava.Model;
using JavaToCSharpConverter.Resources;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CodeConverterJavaToCSharp_Unittest.LuceneTests
{

    class MutableValue_Unittest
    {
        [Test]
        public void CheckAccountableInterfaceComments()
        {

            var tmpObjectInformation = ProjectInformationHelper.DoFullRun(
                ImportHelper.ImportMappingList(ClassRenameJson.SystemAliasJson), new ConverterLucene(), new JavaLoader() { LoadDefaultData = true },
                JavaBits);

            var tmpResult = CSharpWriter.CreateClassesFromObjectInformation(tmpObjectInformation, new ConverterLucene()).ToList().Last();

            Assert.AreEqual("MutableValue", tmpResult.FullName);
            
            Assert.AreEqual(true, tmpResult.Content.Contains("Type c1 =             this.GetType()"));
        }

        private string JavaBits = @"
package org.apache.lucene.util.mutable;

        /**
         * Base class for all mutable values.
         *  
         * @lucene.internal 
         */
        public abstract class MutableValue implements Comparable<MutableValue> {
  public boolean exists = true;

        public abstract void copy(MutableValue source);
        public abstract boolean equalsSameType(Object other);
  public int compareTo(MutableValue other)
        {
            Class <? extends MutableValue > c1 = this.getClass();
            Class <? extends MutableValue > c2 = other.getClass();
            if (c1 != c2)
            {
                int c = c1.hashCode() - c2.hashCode();
                if (c == 0)
                {
                    c = c1.getCanonicalName().compareTo(c2.getCanonicalName());
                }
                return c;
            }
            return compareSameType(other);
        }

  public boolean equals(Object other)
        {
            return (getClass() == other.getClass()) && this.equalsSameType(other);
        }

  public String toString()
        {
            return exists() ? toObject().toString() : ""(null)"";
        }
    }";
    }
}
