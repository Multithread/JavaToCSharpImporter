using CodeConverterCore.Helper;
using CodeConverterJava.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace JavaCodeLoader_Unittest.Objektstruktur
{
    public class MethodeDefinition_Unittest
    {
        [Test]
        public void SimpleMethode()
        {
            var tmpClass = @"
package org;
public class Class1 {
    public void Run()
    {}
}";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpClass1 = tmpObjectInformation.ClassList[0];
            Assert.AreEqual("Run", tmpClass1.MethodeList[0].Name);
        }

        [Test]
        public void MultipleMethods()
        {
            var tmpClass = @"
package org;
public class Class1 {
    public void Run()    {}
    public void OtherMethode(){}
}";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpClass1 = tmpObjectInformation.ClassList[0];
            Assert.AreEqual(2, tmpClass1.MethodeList.Count);
            Assert.AreEqual("OtherMethode", tmpClass1.MethodeList[1].Name);
        }

        [Test]
        public void MethodeReturnVoid()
        {
            var tmpClass = @"
package org;
public class Class1 {
    public void Run(){}
}";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpClass1 = tmpObjectInformation.ClassList[0];
            Assert.AreEqual("void", tmpClass1.MethodeList[0].ReturnType.Name);
        }

        [Test]
        public void MethodeReturnType()
        {
            var tmpClass = @"
package org;
public class Class1 {
    public Class1 Run(){}
}";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpClass1 = tmpObjectInformation.ClassList[0];
            Assert.AreEqual("Class1", tmpClass1.MethodeList[0].ReturnType.Name);
        }

        [Test]
        public void MethodeGenericType()
        {
            var tmpClass = @"
package org;
public class Class1 {
    public <IClassType> void Run(){}
}";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpClass1 = tmpObjectInformation.ClassList[0];
            Assert.AreEqual("IClassType", tmpClass1.MethodeList[0].GenericTypes[0].Name);
        }

        [Test]
        public void MethodeGenericTypeArray()
        {
            var tmpClass = @"
package org;
public class Class1 {
    public <IClassType[]> void Run(){}
}";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpClass1 = tmpObjectInformation.ClassList[0];
            Assert.AreEqual("IClassType", tmpClass1.MethodeList[0].GenericTypes[0].Name);
            Assert.AreEqual(true, tmpClass1.MethodeList[0].GenericTypes[0].IsArray);
        }

        /// <summary>
        /// Waiting for Implementation: Generic with Generic Type //TODO
        /// z.B. List<list<Object>>
        /// </summary>
        //        [Test]
        //        public void MethodeGenericTypeWithGenericType()
        //        {
        //            var tmpClass = @"
        //package org;
        //public class Class1 {
        //    public <IClassType<IParseTree>> void Run(){}
        //}";
        //            var tmpIniData = DataHelper.LoadIni("");
        //            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

        //            var tmpClass1 = tmpObjectInformation.ClassList[0];
        //            Assert.AreEqual("IClassType", tmpClass1.MethodeList[0].GenericTypes[0].Name);
        //            Assert.AreEqual(1, tmpClass1.MethodeList[0].GenericTypes[0].GenericTypes.Count);
        //        }

        [Test]
        public void MethodeGenericTypeMultiple()
        {
            var tmpClass = @"
package org;
public class Class1 {
    public <IClassType,Array[]> void Run(){}
}";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpClass1 = tmpObjectInformation.ClassList[0];
            Assert.AreEqual(2, tmpClass1.MethodeList[0].GenericTypes.Count);
        }

        [Test]
        public void MethodeReturnTypeGeneric()
        {
            var tmpClass = @"
package org;
public class Class1 {
    public Class1<T> Run(){}
}";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpClass1 = tmpObjectInformation.ClassList[0];
            Assert.AreEqual("Class1", tmpClass1.MethodeList[0].ReturnType.Name);
            Assert.AreEqual("T", tmpClass1.MethodeList[0].ReturnType.GenericTypes[0].Name);
        }
        [Test]
        public void MethodeWithSingleParameter()
        {
            var tmpClass = @"
package org;
public class Class1 {
    public void Run(int inValue){}
}";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpMethode1 = tmpObjectInformation.ClassList[0].MethodeList[0];
            Assert.AreEqual("inValue", tmpMethode1.Parameter[0].Name);
            Assert.AreEqual("int", tmpMethode1.Parameter[0].Type.Name);
        }
        [Test]
        public void MethodeWithMultipleParameter()
        {
            var tmpClass = @"
package org;
public class Class1 {
    public void Run(int inValue1,string inStirng, Class1 inCLass){}
}";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpMethode1 = tmpObjectInformation.ClassList[0].MethodeList[0];
            Assert.AreEqual(3, tmpMethode1.Parameter.Count);
        }

        [Test]
        public void MethodeWithGenericParameter()
        {
            var tmpClass = @"
package org;
public class Class1 {
    public void Run(Class1<int> inValue){}
}";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpMethode1 = tmpObjectInformation.ClassList[0].MethodeList[0];
            Assert.AreEqual("int", tmpMethode1.Parameter[0].Type.GenericTypes[0].Name);
        }
    }
}
