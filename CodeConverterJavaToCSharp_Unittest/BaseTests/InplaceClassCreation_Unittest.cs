using CodeConverterCore.Analyzer;
using CodeConverterCore.Converter;
using CodeConverterCore.Helper;
using CodeConverterCSharp;
using CodeConverterCSharp.Lucenene;
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
    public class InplaceClassCreation_Unittest
    {
        [Test]
        public void ClassCreationWithOveride()
        {
            var tmpClass = @"public abstract class LongValues  {

  /** Get value at <code>index</code>. */
  public abstract long get(long index);

  /** An instance that returns the provided value. */
  public static final LongValues IDENTITY = new LongValues() {

    @Override
    public long get(long index) {
      return index;
    }
  };
}";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            new AnalyzerCore().LinkProjectInformation(tmpObjectInformation);
            new NamingConvertionHelper(new ConverterLucene()).ConvertProject(tmpObjectInformation);

            var tmpResult = CSharpWriter.CreateClassesFromObjectInformation(tmpObjectInformation, new ConverterJavaToCSharp()).ToList();

            var tmpExpectedResult = @"

namespace 
{
    public abstract class LongValues
    {
        /// <summary>
        /// An instance that returns the provided value.
        /// </summary>
        public readonly static LongValues IDENTITY =  new LongValues_0();

        public abstract long Get(long inIndex);
        public class LongValues_0 : LongValues
        {
            public override long Get(long inIndex)
            {
                return inIndex;
            }
        }
    }
}
";
            //Check Elvis Result
            Assert.AreEqual(tmpExpectedResult, tmpResult[0].Content);
        }
    }
}
