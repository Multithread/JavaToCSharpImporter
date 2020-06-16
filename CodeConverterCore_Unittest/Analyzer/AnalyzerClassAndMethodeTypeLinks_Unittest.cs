using CodeConverterCore.Analyzer;
using CodeConverterCore.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace CodeConverterCore_Unittest
{
    /// <summary>
    /// Text Analyzer linking of Types
    /// </summary>
    public class AnalyzerClassAndMethodeTypeLinks_Unittest
    {

        [Test]
        public void MethodeReturnType()
        {
            var tmpProject = new ProjectInformation();
            tmpProject.FillClasses(new List<ClassContainer>
            {
                Class_TestX(),
                Class_TestY(),
            });
            new AnalyzerCore().LinkProjectInformation(tmpProject);

            Assert.AreEqual(tmpProject.ClassList[0].Type.Type, tmpProject.ClassList[0].MethodeList[0].ReturnType.Type);
        }

        [Test]
        public void MethodeParam1Type()
        {
            var tmpProject = new ProjectInformation();
            tmpProject.FillClasses(new List<ClassContainer>
            {
                Class_TestX(),
                Class_TestY(),
            });
            new AnalyzerCore().LinkProjectInformation(tmpProject);

            Assert.AreEqual(tmpProject.ClassList[1].Type.Type, tmpProject.ClassList[0].MethodeList[0].Parameter[0].Type.Type);
        }


        private ClassContainer Class_TestX(Action<ClassContainer> inChanges = null)
        {
            var tmpClass = new ClassContainer
            {
                Type = new TypeContainer { Name = "TestX" },
                Namespace = "Base",
                MethodeList = new List<MethodeContainer>
                {
                    new MethodeContainer
                    {
                        Name ="TestXFunc",
                        ReturnType ="TestX",
                        ModifierList =new List<string>{ "public","static"},
                        Parameter =new List<FieldContainer>(){
                            new FieldContainer
                            {
                                Name ="inTestY",
                                Type ="TestY",
                            }
                        },
                    }
                }
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
                MethodeList = new List<MethodeContainer>
                {
                    new MethodeContainer
                    {
                        Name ="TestXFunc",
                        ReturnType ="TestX",
                    }
                }
            };
            inChanges?.Invoke(tmpClass);
            return tmpClass;
        }
    }
}