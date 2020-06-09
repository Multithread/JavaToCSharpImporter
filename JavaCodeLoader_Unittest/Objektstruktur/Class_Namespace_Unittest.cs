using JavaCodeLoader_Unittest.TextHelpData;
using JavaToCSharpConverter.Helper;
using JavaToCSharpConverter.Model;
using JavaToCSharpConverter.Model.Java;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace JavaCodeLoader_Unittest.Objektstruktur
{
    public class Class_Namespace_Unittest
    {
        [Test]
        public void ClassListWithOneClass()
        {
            var tmpClass = @"
package org;
public class Class1 {}
";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            Assert.AreEqual(1, tmpObjectInformation.ClassList.Count);
            Assert.AreEqual("Class1", tmpObjectInformation.ClassList[0].Name);
        }

        [Test]
        public void ClassWithCOmplexNamespace()
        {
            var tmpClass = @"
package org.apache.lucene.util;
public class Class1 {}
";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            Assert.AreEqual(1, tmpObjectInformation.ClassList.Count);
            Assert.AreEqual("org.apache.lucene.util", tmpObjectInformation.ClassList[0].Namespace);
        }

        [Test]
        public void NamespaceComment()
        {
            var tmpClass = @"
/*Namespace Comment*/
package org.apache.lucene.util;
public class Class1 {}
";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            Assert.AreEqual(1, tmpObjectInformation.ClassList.Count);
            Assert.AreEqual("/*Namespace Comment*/", tmpObjectInformation.ClassList[0].NamespaceComment);
        }
        [Test]
        public void ClassWithImport()
        {
            var tmpClass = @"
package org.apache.lucene.util;
import System.IO;
public class Class1 {}
";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            Assert.AreEqual(1, tmpObjectInformation.ClassList.Count);
            Assert.AreEqual("System.IO", tmpObjectInformation.ClassList[0].UsingList[0]);
        }
        [Test]
        public void ClassWithMultipleImport()
        {
            var tmpClass = @"
package org.apache.lucene.util;
import System.IO;
import Testing;
import Linq;
public class Class1 {}
";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            Assert.AreEqual(1, tmpObjectInformation.ClassList.Count);
            Assert.AreEqual(3, tmpObjectInformation.ClassList[0].UsingList.Count);
        }

        [Test]
        public void ClassWithGeneric()
        {
            var tmpClass = @"
package org.apache.lucene.util;
public class Class1<object> {}
";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            Assert.AreEqual(1, tmpObjectInformation.ClassList.Count);
            Assert.AreEqual("object", tmpObjectInformation.ClassList[0].Type.GenericTypes[0].Name);
        }

        [Test]
        public void ClassWithGenericArray()
        {
            var tmpClass = @"
package org.apache.lucene.util;
public class Class1<object[]> {}
";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            Assert.AreEqual("object", tmpObjectInformation.ClassList[0].Type.GenericTypes[0].Name);
            Assert.AreEqual(true, tmpObjectInformation.ClassList[0].Type.GenericTypes[0].IsArray);
        }

        [Test]
        public void ClassWithMultipleGenericTypes()
        {
            var tmpClass = @"
package org.apache.lucene.util;
public class Class1<Type,object[]> {}
";
            var tmpIniData = DataHelper.LoadIni("");
            var tmpObjectInformation = new JavaLoader().CreateObjectInformation(new List<string> { tmpClass }, tmpIniData);

            Assert.AreEqual(2, tmpObjectInformation.ClassList[0].Type.GenericTypes.Count);
            Assert.AreEqual("Type", tmpObjectInformation.ClassList[0].Type.GenericTypes[0].Name);
            Assert.AreEqual(true, tmpObjectInformation.ClassList[0].Type.GenericTypes[1].IsArray);
        }
    }
}
