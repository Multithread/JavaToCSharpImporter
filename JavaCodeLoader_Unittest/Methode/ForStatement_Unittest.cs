using CodeConverterCore.Enum;
using CodeConverterCore.Helper;
using CodeConverterCore.Model;
using CodeConverterJava.Model;
using NUnit.Framework;
using System.Collections.Generic;

namespace CodeConverterJava_Unittest.Methode
{
    /// <summary>
    /// Checking Field Write Access
    /// </summary>
    public class ForStatement_Unittest
    {
        [Test]
        public void SimpleFor()
        {
            var tmpClass = @"
package org;
public class Class1 {
public void forIInLength(int len){
 for(int i=0;i<len;i++) {
       i--;
      }
return false;
}
";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpMethodeContent = tmpObjectInformation.ClassList[0].MethodeList[0];
            var tmpStatement = (tmpMethodeContent.Code.CodeEntries[0]) as StatementCode;

            //Statement IF
            Assert.AreEqual(StatementTypeEnum.If, tmpStatement.StatementType);

            //Inner Content (return true);
            Assert.AreNotEqual(null, tmpStatement.InnerContent);

            //IF Statement (in1<in2)
            Assert.AreNotEqual(null, tmpStatement.StatementCodeBlocks[0]);
        }

    }
}
