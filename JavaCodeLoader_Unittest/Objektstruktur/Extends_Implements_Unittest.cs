using CodeConverterCore.Helper;
using CodeConverterJava.Model;
using NUnit.Framework;
using System.Collections.Generic;

namespace CodeConverterJava_Unittest.Objektstruktur
{
    public  class Extends_Implements_Unittest
    {
        [Test]
        public void Interface_Implements1()
        {
            var tmpClass = @"
package org.apache.lucene.util;

public interface Accountable<T> implements object{
bool Get(int inIndex)
}";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpProjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpClass1 = tmpProjectInformation.ClassList[0];
            Assert.AreEqual(1, tmpClass1.InterfaceList.Count);

            Assert.AreEqual("object", tmpClass1.InterfaceList[0].Name);
        }

        [Test]
        public void Interface_ImplementMultiple()
        {
            var tmpClass = @"
package org.apache.lucene.util;

public interface Accountable<T> implements object, IAccountable{
bool Get(int inIndex)
}";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpProjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpClass1 = tmpProjectInformation.ClassList[0];
            Assert.AreEqual(2, tmpClass1.InterfaceList.Count);

            Assert.AreEqual("object", tmpClass1.InterfaceList[0].Name);
        }

        [Test]
        public void Interface_Extends1()
        {
            var tmpClass = @"
package org.apache.lucene.util;

public interface Accountable<T> extends object<T>{
bool Get(int inIndex)
}";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpProjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpClass1 = tmpProjectInformation.ClassList[0];
            Assert.AreEqual(1, tmpClass1.InterfaceList.Count);

            Assert.AreEqual("object", tmpClass1.InterfaceList[0].Name);
        }

        [Test]
        public void Class_ImplementMultiple()
        {
            var tmpClass = @"
package org.apache.lucene.util;

public class Accountable<T> implements object, IAccountable{
bool Get(int inIndex)
}";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpProjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpClass1 = tmpProjectInformation.ClassList[0];
            Assert.AreEqual(2, tmpClass1.InterfaceList.Count);

            Assert.AreEqual("object", tmpClass1.InterfaceList[0].Name);
        }

        [Test]
        public void Class_Extends1()
        {
            var tmpClass = @"
package org.apache.lucene.util;

public class Accountable<T> extends object<T>{
bool Get(int inIndex)
}";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpProjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            var tmpClass1 = tmpProjectInformation.ClassList[0];
            Assert.AreEqual(1, tmpClass1.InterfaceList.Count);

            Assert.AreEqual("object", tmpClass1.InterfaceList[0].Name);
        }
    }
}
