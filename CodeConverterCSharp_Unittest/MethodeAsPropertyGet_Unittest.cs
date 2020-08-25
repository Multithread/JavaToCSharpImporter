using CodeConverterCore.Converter;
using CodeConverterCore.Helper;
using CodeConverterCore.Model;
using CodeConverterCSharp;
using NUnit.Framework;
using System.Linq;

namespace CodeConverterCSharp_Unittest
{
    public class MethodeAsPropertyGet_Unittest
    {
        [Test]
        public void PropertyGet()
        {
            var tmpProject = new ProjectInformation();
            var tmpClass = Create.AddClass("v1");
            var tmpMethode=tmpClass.AddMethode("Name", new TypeContainer("string"));
            tmpMethode.IsProperty = true;
            tmpProject.FillClasses(new System.Collections.Generic.List<ClassContainer> { tmpClass });
            var tmpObjectInformation = CSharpWriter.CreateClassesFromObjectInformation(tmpProject,new ConverterBase()).ToList();


            Assert.AreEqual(1, tmpObjectInformation.Count);
            Assert.AreEqual(true, tmpObjectInformation[0].Content.Contains("string Name{"));
            Assert.AreEqual(false, tmpObjectInformation[0].Content.Contains("string Name()"));
        }
    }
}
