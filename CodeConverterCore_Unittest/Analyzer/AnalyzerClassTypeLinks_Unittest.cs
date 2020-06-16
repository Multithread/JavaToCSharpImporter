using CodeConverterCore.Analyzer;
using CodeConverterCore.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace CodeConverterCore_Unittest
{
    public class AnalyzerClassTypeLinks_Unittest
    {

        [Test]
        public void CheckInterface()
        {
            var tmpProject = new ProjectInformation();
            tmpProject.FillClasses(new List<ClassContainer>
            {
                Class_TestX(),
                Class_TestY(inItem=> inItem.InterfaceList.Add(new TypeContainer{Name=Class_TestX().Name })),
            });
            new AnalyzerCore().LinkProjectInformation(tmpProject);

            Assert.AreEqual(tmpProject.ClassList[0].Type.Type, tmpProject.ClassList[1].InterfaceList[0].Type);
        }

        private ClassContainer Class_TestX(Action<ClassContainer> inChanges = null)
        {
            var tmpClass = new ClassContainer
            {
                Type = new TypeContainer { Name = "TestX" },
                Namespace = "Base",
            };
            inChanges?.Invoke(tmpClass);
            return tmpClass;
        }

        private ClassContainer Class_TestY(Action<ClassContainer> inChanges = null)
        {
            var tmpClass = new ClassContainer
            {
                Type = new TypeContainer { Name = "TestY" },
                Namespace = "Base",
            };
            inChanges?.Invoke(tmpClass);
            return tmpClass;
        }
    }
}