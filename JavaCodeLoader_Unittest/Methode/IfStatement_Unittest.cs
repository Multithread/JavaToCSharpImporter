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
    public class IfStatement_Unittest
    {
        [Test]
        public void SimpleIfWithReturns()
        {
            var tmpClass = @"
package org;
public class Class1 {
public void OneSmallerThanTwo(int in1, int in2){
if(in1<in2){
return true;
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

        [Test]
        public void SimpleIfWithMultiInnerData()
        {
            var tmpClass = @"
package org;
public class Class1 {
public void OneSmallerThanTwo(int in1, int in2){
if(in1<in2){
int1=in2;
return true;
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
            Assert.AreEqual(2, tmpStatement.InnerContent.CodeEntries.Count);

            //IF Statement (in1<in2)
            Assert.AreNotEqual(null, tmpStatement.StatementCodeBlocks[0]);
        }

        [Test]
        public void SimpleIfWithReturnsWithoutBrackets()
        {
            var tmpClass = @"
package org;
public class Class1 {
public void OneSmallerThanTwo(int in1, int in2){
if(in1<in2)
return true;

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
            Assert.AreEqual(1, tmpStatement.InnerContent.CodeEntries.Count);

            //IF Statement (in1<in2)
            Assert.AreNotEqual(null, tmpStatement.StatementCodeBlocks[0]);
        }

        [Test]
        public void IfWithElse()
        {
            var tmpClass = @"
package org;
public class Class1 {
public void OneSmallerThanTwo(int in1, int in2){
if(in1<in2)
return true;
else
return false;
}
";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpMethodeContent = tmpObjectInformation.ClassList[0].MethodeList[0];
            var tmpStatementId = (tmpMethodeContent.Code.CodeEntries[0]) as StatementCode;
            var tmpStatementElse = (tmpMethodeContent.Code.CodeEntries[1]) as StatementCode;

            //Statement IF
            Assert.AreEqual(StatementTypeEnum.If, tmpStatementId.StatementType);
            Assert.AreEqual(StatementTypeEnum.Else, tmpStatementElse.StatementType);

            //Inner Content (return true);
            Assert.AreEqual(1, tmpStatementId.InnerContent.CodeEntries.Count);
            Assert.AreEqual(1, tmpStatementElse.InnerContent.CodeEntries.Count);

            //IF Statement (in1<in2)
            Assert.AreNotEqual(null, tmpStatementId.StatementCodeBlocks[0]);
            Assert.AreNotEqual(null, tmpStatementElse.StatementCodeBlocks[0]);
        }

        [Test]
        public void IfWithElseifAndElse()
        {
            var tmpClass = @"
package org;
public class Class1 {
public void OneSmallerThanTwo(int in1, int in2){
if(in1<in2)
return 1;
else if(in1<in2)
return 2;
else
return 3;
}
";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpMethodeContent = tmpObjectInformation.ClassList[0].MethodeList[0];
            var tmpStatementId = (tmpMethodeContent.Code.CodeEntries[0]) as StatementCode;
            var tmpStatementElse = (tmpMethodeContent.Code.CodeEntries[1]) as StatementCode;

            //Statement IF
            Assert.AreEqual(StatementTypeEnum.If, tmpStatementId.StatementType);
            Assert.AreEqual(StatementTypeEnum.Else, tmpStatementElse.StatementType);

            //Inner Content (return true);
            Assert.AreEqual(1, tmpStatementId.InnerContent.CodeEntries.Count);
            Assert.AreEqual(2, tmpStatementElse.InnerContent.CodeEntries.Count);

            //IF Statement (in1<in2)
            Assert.AreNotEqual(null, tmpStatementId.StatementCodeBlocks[0]);
            Assert.AreNotEqual(null, tmpStatementElse.StatementCodeBlocks[0]);
        }
    }
}
