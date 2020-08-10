using CodeConverterCore.Helper;
using CodeConverterJava.Model;
using NUnit.Framework;
using System.Collections.Generic;

namespace CodeConverterJava_Unittest.Methode
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

            var tmpInnerClassMethode = tmpObjectInformation.ClassList[0].InnerClasses[0].MethodeList[0];

            //Statement Elvis
            Assert.AreEqual("get", tmpInnerClassMethode.Name);
            Assert.AreEqual("LongValues_0", tmpObjectInformation.ClassList[0].InnerClasses[0].Name);
        }
    }
}
