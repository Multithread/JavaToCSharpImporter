using CodeConverterCore.Analyzer;
using CodeConverterCore.Helper;
using CodeConverterCore.Model;
using NUnit.Framework;
using NUnit.Framework.Internal.Execution;
using System.Collections.Generic;

namespace CodeConverterCore_Unittest
{
    /// <summary>
    /// Text Analyzer linking of Types
    /// </summary>
    public class PropertyStackingAccess_Unittest
    {

        [Test]
        public void PropertysThis_FullName_ToLower()
        {
            var tmpProject = new ProjectInformation();

            var tmpClassString = Create.AddClass("String");
            var tmpGetFullName = "getFullName";
            var tmpFullNameMethode = tmpClassString.AddMethode(tmpGetFullName, new TypeContainer { Name = "String" });

            var tmpClass1 = Create.AddClass("Class1");
            var tmpMethodeName = "getChildResources";
            tmpClass1.AddMethode(tmpMethodeName, new TypeContainer { Name = "void" });
            var tmpMethode = tmpClass1.MethodeList[0];
            Create.AddField(tmpClass1, "Text", new BaseType("String"));

            tmpMethode.Code = new CodeBlock();

            var tmpReturn = new ReturnCodeEntry
            {
                CodeEntries = new List<CodeConverterCore.Interface.ICodeEntry>
                {
                    new VariableAccess
                    {
                        Access= new VariableAccess
                        {
                            Child= new VariableAccess
                            {
                                Access = new ConstantValue
                                {
                                    Value="Text",
                                }
                            },
                            Access= new ConstantValue
                            {
                                Value = "this"
                            }
                        },
                        Child= new VariableAccess
                        {
                            Access= new MethodeCall
                            {
                                Name =tmpGetFullName
                            }
                        }
                    }
                }
            };
            tmpMethode.Code.CodeEntries.Add(tmpReturn);

            var tmpText = tmpReturn.ToString();
            Assert.AreEqual("return this  Text     getFullName ()  ", tmpText);

            tmpProject.FillClasses(new List<ClassContainer> { tmpClass1, tmpClassString });

            new AnalyzerCore().LinkProjectInformation(tmpProject);

            tmpFullNameMethode.IsProperty = true;
            tmpFullNameMethode.Name = "FullName";

            Assert.AreEqual(tmpFullNameMethode.Name, (((tmpReturn.CodeEntries[0] as VariableAccess).Child as VariableAccess).Access as MethodeCall).Name);
        }
    }
}