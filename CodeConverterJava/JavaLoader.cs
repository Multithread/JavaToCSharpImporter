using CodeConverterCore.Interface;
using CodeConverterCore.Model;
using MoreLinq;
using System.Collections.Generic;
using System.Linq;

namespace CodeConverterJava.Model
{
    public class JavaLoader : ILoadOOPLanguage
    {
        public ProjectInformation CreateObjectInformation(List<string> inFileContents, IniParser.Model.IniData inConfiguration)
        {
            var tmpClassList = new List<ClassContainer>();
            foreach (var tmpFile in inFileContents)
            {
                //tmpClassList.AddRange(JavaClassLoader.LoadFile(tmpFile));
                tmpClassList.AddRange(JavaAntlrClassLoader.LoaderOptimization(tmpFile));
            }
            var tmpObjectInformation = new ProjectInformation()
                .FillClasses(tmpClassList);
            //Add Mapped Methodes to Class List (So we don't need String oä as a Class List
            var tmpAdditionalClasses = new List<ClassContainer>
            {
                //new ClassContainer
                //{
                //    Type="int",
                //    Namespace=""
                //},new ClassContainer
                //{
                //    Type="String",
                //    Namespace=""
                //},new ClassContainer
                //{
                //    Type="File",
                //    Namespace="java.io"
                //}
            };

            //Load all Classes, with Methodes we might need
            foreach (var tmpMap in inConfiguration["Methode"])
            {
                var tmpLeftSplit = tmpMap.KeyName.Split('.');
                var tmpNamespace = string.Join(".", tmpLeftSplit.SkipLast(2));
                var tmpName = (TypeContainer)tmpLeftSplit.SkipLast(1).Last();
                var tmpMethodeName = tmpLeftSplit.Last();

                var tmpMethode = tmpAdditionalClasses.FirstOrDefault(inItem =>
                    inItem.Namespace == tmpNamespace && inItem.Type == tmpName);
                if (tmpMethode == null)
                {
                    tmpMethode = new ClassContainer
                    {
                        Type = tmpName,
                        Namespace = tmpNamespace
                    };
                    tmpAdditionalClasses.Add(tmpMethode);
                }

                if (!tmpMethode.MethodeList.Any(inItem => inItem.Name == tmpMethodeName))
                {
                    //TODO Check for Param Equality
                    var tmpNewMethode = new MethodeContainer();
                    tmpNewMethode.Name = tmpMethodeName;
                    tmpNewMethode.ModifierList = new List<string> { "public" };
                    tmpMethode.MethodeList.Add(tmpNewMethode);
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
