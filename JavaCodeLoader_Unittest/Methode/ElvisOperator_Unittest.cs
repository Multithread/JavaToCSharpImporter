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
    public class ElvisOperator_Unittest
    {
        [Test]
        public void SimpleReturnElvis()
        {
            var tmpClass = @"
package org;
public class Class1 {
public void OneSmallerThanTwo(bool inFirst,int in1, int in2){
return inFirst? in1:in2;
}
";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpMethodeContent = tmpObjectInformation.ClassList[0].MethodeList[0];
            var tmpStatement = ((tmpMethodeContent.Code.CodeEntries[0]) as ReturnCodeEntry).CodeEntries[0] as StatementCode;

            //Statement Elvis
            Assert.AreEqual(StatementTypeEnum.Elvis, tmpStatement.StatementType);
        }
    }
}
