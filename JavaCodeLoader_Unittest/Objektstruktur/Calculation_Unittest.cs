using CodeConverterCore.Helper;
using CodeConverterJava.Model;
using CodeConverterCore.Model;
using NUnit.Framework;
using System.Collections.Generic;

namespace CodeConverterJava_Unittest.Objektstruktur
{
    public class Calculation_Unittest
    {
        [Test]
        public void SetVariableWithMultiplyCalculation()
        {
            var tmpClass = @"
package org;
public class Class1 {
public void Run(int inResult){
inResult=4*6;
}}";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpMethodeContent = tmpObjectInformation.ClassList[0].MethodeList[0].Code.CodeEntries[0];
            var tmpVarSetter = (tmpMethodeContent as StatementCode).StatementCodeBlocks[0].CodeEntries[0] as SetFieldWithValue;
            Assert.AreEqual("inResult", tmpVarSetter.VariableToAccess.CodeEntries[0].ToString());
            Assert.AreEqual("(4 Multiplication 6)", tmpVarSetter.ValueToSet.ToString());
        }
        [Test]
        public void SetVariableWithTrippleMultiplyCalculation()
        {
            var tmpClass = @"
package org;
public class Class1 {
public void Run(int inResult){
inResult=4*6*8;
}}";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpMethodeContent = tmpObjectInformation.ClassList[0].MethodeList[0].Code.CodeEntries[0];
            var tmpVarSetter = (tmpMethodeContent as StatementCode).StatementCodeBlocks[0].CodeEntries[0] as SetFieldWithValue;
            Assert.AreEqual("inResult", tmpVarSetter.VariableToAccess.CodeEntries[0].ToString());
            Assert.AreEqual("((4 Multiplication 6) Multiplication 8)", tmpVarSetter.ValueToSet.ToString());
        }

        [Test]
        public void SetVariableWithMultiplyCalculationWithSubElementCheck()
        {
            var tmpClass = @"
package org;
public class Class1 {
public void Run(int inResult){
inResult=4*6;
}}";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpMethodeContent = tmpObjectInformation.ClassList[0].MethodeList[0].Code.CodeEntries[0];
            var tmpVarSetter = (tmpMethodeContent as StatementCode).StatementCodeBlocks[0].CodeEntries[0] as SetFieldWithValue;
            Assert.AreEqual("inResult", (tmpVarSetter.VariableToAccess.CodeEntries[0] as ConstantValue).Value);
            Assert.AreEqual(2, (tmpVarSetter.ValueToSet.CodeEntries[0] as CodeExpression).SubClauseEntries.Count);
        }

        [Test]
        public void CalculationWithoutOrderOfMath()
        {
            var tmpClass = @"
package org;
public class Class1 {
public void Run(int inResult){
inResult=4*6+6-3*2;
}}";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpMethodeContent = tmpObjectInformation.ClassList[0].MethodeList[0].Code.CodeEntries[0];
            var tmpVarSetter = (tmpMethodeContent as StatementCode).StatementCodeBlocks[0].CodeEntries[0] as SetFieldWithValue;
            Assert.AreEqual("inResult", tmpVarSetter.VariableToAccess.CodeEntries[0].ToString());
            Assert.AreEqual("(((4 Multiplication 6) Addition 6) Substraction (3 Multiplication 2))", tmpVarSetter.ValueToSet.ToString());
        }
    }
}
