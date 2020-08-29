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
    public class BitShifting_Unittest
    {
        [Test]
        public void ShiftBits()
        {
            var tmpClass = @"
package org;
public class Class1 {
public void Run(){
    return (int)(value>>32);
}
}
";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpMethodeContent = tmpObjectInformation.ClassList[0].MethodeList[0];
            var tmpCodeLine1 = (tmpMethodeContent.Code.CodeEntries[0] as ReturnCodeEntry).CodeEntries[0] as TypeConversion;
            var tmpExpr = (tmpCodeLine1.PreconversionValue.CodeEntries[0] as CodeBlocContainer).InnerBlock.CodeEntries[0] as CodeExpression;
            Assert.AreEqual(VariableOperatorType.BitShiftRight, tmpExpr.Manipulator);

        }
    }
}
