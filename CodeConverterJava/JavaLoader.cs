using CodeConverterCore.Helper;
using CodeConverterCore.ImportExport;
using CodeConverterCore.Interface;
using CodeConverterCore.Model;
using CodeConverterJava.Resources;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeConverterJava.Model
{
    public class JavaLoader : ILoadOOPLanguage
    {
        public bool LoadDefaultData { get; set; } = false;

        public ProjectInformation CreateObjectInformation(List<string> inFileContents, IniParser.Model.IniData inConfiguration)
        {
            var tmpClassList = new List<ClassContainer>();
            ProjectInformation tmpObjectInformation = new ProjectInformation();
            if (LoadDefaultData)
            {
                tmpObjectInformation = ProjectInformationHelper.CreateSystemProjectInformation(ImportHelper.ImportClasses(JavaLangClassJson.JavaLang), ImportHelper.ImportAliasList(CompilerAliasHelper.SystemAliasJson), "java.lang");
            }

            foreach (var tmpFile in inFileContents)
            {
                //tmpClassList.AddRange(JavaClassLoader.LoadFile(tmpFile));
                tmpClassList.AddRange(JavaAntlrClassLoader.LoaderOptimization(tmpFile));
            }
            tmpObjectInformation.FillClasses(tmpClassList);
            //Add Mapped Methodes to Class List (So we don't need String oä as a Class List
            var tmpAdditionalClasses = new List<ClassContainer>();

            //Load all Classes, with Methodes we might need
            if (inConfiguration != null)
            {
                foreach (var tmpMap in inConfiguration["Methode"])
                {
                    var tmpLeftSplit = tmpMap.KeyName.Split('.');
                    var tmpNamespace = string.Join(".", tmpLeftSplit.SkipLast(2));
                    var tmpName = (TypeContainer)tmpLeftSplit.SkipLast(1).Last();
                    var tmpMethodeName = tmpLeftSplit.Last();

                    var tmpClass = tmpAdditionalClasses.FirstOrDefault(inItem =>
                        inItem.Namespace == tmpNamespace && inItem.Type == tmpName);
                    if (tmpClass == null)
                    {
                        tmpClass = new ClassContainer
                        {
                            Type = tmpName,
                            Namespace = tmpNamespace
                        };
                        tmpAdditionalClasses.Add(tmpClass);
                    }

                    if (!tmpClass.MethodeList.Any(inItem => inItem.Name == tmpMethodeName))
                    {
                        //TODO Check for Param Equality
                        if (tmpClass.MethodeList.Count(inItem => inItem.Name == tmpMethodeName) > 1)
                        {
                            throw new NotImplementedException("Methode differenting with params not implemented");
                        }

                        var tmpNewMethode = new MethodeContainer();
                        tmpNewMethode.Name = tmpMethodeName;
                        tmpNewMethode.ModifierList = new List<string> { "public" };
                        tmpClass.MethodeList.Add(tmpNewMethode);
                    }
                }
            }

            IResolveMethodeContentToIL tmpCodeHandler = new JavaMethodeCodeResolver();
            foreach (var tmpClass in tmpClassList)
            {
                foreach (var tmpMethode in tmpClass.MethodeList)
                {
                    tmpCodeHandler.Resolve(tmpMethode);
                }
            }

            foreach (var tmpClassouter in tmpClassList)
            {
                foreach (var tmpClass in tmpClassouter.InnerClasses)
                {
                    foreach (var tmpMethode in tmpClass.MethodeList)
                    {
                        tmpCodeHandler.Resolve(tmpMethode);
                    }
                }
            }

            //Fill them into the object Information
            tmpObjectInformation.FillClasses(tmpAdditionalClasses);
            return tmpObjectInformation;
        }
    }
}
