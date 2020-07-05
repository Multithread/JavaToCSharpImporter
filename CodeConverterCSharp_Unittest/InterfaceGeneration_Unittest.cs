using CodeConverterCore.Converter;
using CodeConverterCore.Model;
using CodeConverterCSharp;
using NUnit.Framework;
using System.Linq;
using CodeConverterCore.Helper;
using CodeConverterCore.Analyzer;

namespace CodeConverterCSharp_Unittest
{
    public class InterfaceGeneration_Unittest
    {
        [Test]
        public void InterfaceCheck()
        {
            var tmpProject = new ProjectInformation();
            var tmpClass = Create.AddClass("IAttribute");
            tmpClass.ModifierList.Add("public");
            tmpClass.ModifierList.Add("interface");

            tmpProject.ClassList.Add(tmpClass);
            new AnalyzerCore().LinkProjectInformation(tmpProject);
            var tmpObjectInformation = CSharpWriter.CreateClassesFromObjectInformation(tmpProject, new ConverterBase()).ToList();

            Assert.AreEqual(1, tmpObjectInformation.Count);
            Assert.AreEqual(true, tmpObjectInformation[0].Content.Contains("public interface IAttribute"));
        }

        [Test]
        public void InterfaceWithBaseInterfaces()
        {
            var tmpProject = new ProjectInformation();
            var tmpClass = Create.AddClass("IAttribute");
            tmpClass.ModifierList.Add("public");
            tmpClass.ModifierList.Add("interface");
            tmpClass.InterfaceList.Add("IAttr");
            tmpClass.InterfaceList.Add("IBaseAttribute");

            tmpProject.ClassList.Add(tmpClass);
            new AnalyzerCore().LinkProjectInformation(tmpProject);
            var tmpObjectInformation = CSharpWriter.CreateClassesFromObjectInformation(tmpProject, new ConverterBase()).ToList();

            Assert.AreEqual(1, tmpObjectInformation.Count);
            Assert.AreEqual(true, tmpObjectInformation[0].Content.Contains("public interface IAttribute: IAttr, IBaseAttribute"));
        }

        [Test]
        public void InterfaceWithInnerClassCheck()
        {
            var tmpProject = new ProjectInformation();
            var tmpClass = Create.AddClass("IAttribute");
            tmpClass.ModifierList.Add("public");
            tmpClass.ModifierList.Add("interface");

            var tmpInnerClass = Create.AddClass("AttributeEmpty");
            tmpInnerClass.ModifierList.Add("internal");
            tmpInnerClass.ModifierList.Add("class");
            tmpClass.InnerClasses.Add(tmpInnerClass);

            tmpProject.ClassList.Add(tmpClass);
            new AnalyzerCore().LinkProjectInformation(tmpProject);
            var tmpObjectInformation = CSharpWriter.CreateClassesFromObjectInformation(tmpProject, new ConverterBase()).ToList();

            Assert.AreEqual(1, tmpObjectInformation.Count);
            Assert.AreEqual(true, tmpObjectInformation[0].Content.Contains("public interface IAttribute"));
            Assert.AreEqual(true, tmpObjectInformation[0].Content.Contains("internal class AttributeEmpty"));
        }

        [Test]
        public void InnerClassWithInterfaceWhoWasRenamed()
        {
            var tmpProject = new ProjectInformation();
            var tmpClass = Create.AddClass("IAttribute");
            tmpClass.ModifierList.Add("public");
            tmpClass.ModifierList.Add("interface");

            var tmpInnerClass = Create.AddClass("AttributeEmpty");
            tmpInnerClass.ModifierList.Add("internal");
            tmpInnerClass.ModifierList.Add("class");
            tmpInnerClass.InterfaceList.Add("IAttribute");
            tmpClass.InnerClasses.Add(tmpInnerClass);

            tmpProject.ClassList.Add(tmpClass);
            new AnalyzerCore().LinkProjectInformation(tmpProject);
            tmpClass.Type.Type.Name = "ISpezialAttribute";

            var tmpObjectInformation = CSharpWriter.CreateClassesFromObjectInformation(tmpProject, new ConverterBase()).ToList();

            Assert.AreEqual(1, tmpObjectInformation.Count);
            Assert.AreEqual(true, tmpObjectInformation[0].Content.Contains("public interface ISpezialAttribute"));
            Assert.AreEqual(true, tmpObjectInformation[0].Content.Contains("internal class AttributeEmpty: ISpezialAttribute"));
        }

    }
}
