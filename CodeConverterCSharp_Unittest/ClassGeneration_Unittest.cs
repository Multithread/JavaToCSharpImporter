using CodeConverterCore.Model;
using CodeConverterCSharp;
using NUnit.Framework;
using System.Linq;

namespace CodeConverterCSharp_Unittest
{
    public class ClassGeneration_Unittest
    {
        [Test]
        public void CheckfoBaseErrors()
        {
            var tmpProject = new ProjectInformation();
            var tmpObjectInformation = CSharpWriter.CreateClassesFromObjectInformation(tmpProject).ToList();

            Assert.AreEqual(0, tmpObjectInformation.Count);
        }

    }
}
