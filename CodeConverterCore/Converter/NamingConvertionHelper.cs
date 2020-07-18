using CodeConverterCore.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CodeConverterCore.Converter
{
    public class NamingConvertionHelper
    {
        private IConverter Converter;
        private ProjectInformation _projectInfo;

        public NamingConvertionHelper(IConverter inConverter)
        {
            Converter = inConverter;
        }

        public void ConvertProject(ProjectInformation inProject)
        {
            _projectInfo = inProject;
            foreach (var tmpClass in inProject.ClassList)
            {
                ConvertClass(tmpClass);
            }
        }

        public void ConvertClass(ClassContainer inClass)
        {
            inClass.Type.Type.Name = Converter.ClassName(inClass);
            inClass.ModifierList = Converter.MapAndSortAttributes(inClass.ModifierList);
            inClass.Namespace = Converter.Namespace(inClass.Namespace);
            inClass.NamespaceComment = Converter.Comment(inClass.NamespaceComment, true);
            inClass.Comment = Converter.Comment(inClass.Comment, true);

            foreach (var tmpInnerClass in inClass.InnerClasses)
            {
                ConvertClass(tmpInnerClass);
            }

            foreach (var tmpField in inClass.FieldList)
            {
                ConvertField(tmpField, true, true);
            }

            foreach (var tmpMethode in inClass.MethodeList)
            {
                ConvertMethode(tmpMethode, inClass);
            }

            for (var tmpI = 0; tmpI < inClass.UsingList.Count; tmpI++)
            {
                inClass.UsingList[tmpI] = Converter.Namespace(inClass.UsingList[tmpI]);
            }
        }

        /// <summary>
        /// Convert Methode to C# methode
        /// </summary>
        /// <param name="tmpMethode"></param>
        /// <param name="inClass"></param>
        private void ConvertMethode(MethodeContainer tmpMethode, ClassContainer inClass)
        {
            var tmpOldName = tmpMethode.Name;
            tmpMethode.Name = Converter.MethodeName(tmpMethode);
            tmpMethode.ModifierList = Converter.MapAndSortAttributes(tmpMethode.ModifierList);
            tmpMethode.Comment = Converter.Comment(tmpMethode.Comment, true);

            foreach (var tmpField in tmpMethode.Parameter)
            {
                ConvertField(tmpField);
                tmpField.Name = Converter.MethodeInParameter(tmpField);
            }

            //checking override, which changes from java to C# if the parent methode ist in a interface
            if (tmpMethode.ModifierList.Contains("override"))
            {
                var tmpParentClassQueue = new Queue<ClassContainer>();
                tmpParentClassQueue.Enqueue(inClass);
                var tmpHandled = false;
                while (tmpParentClassQueue.Count > 0 && !tmpHandled)
                {
                    var tmpClass = tmpParentClassQueue.Dequeue();
                    foreach (var tmpSubclass in tmpClass.InterfaceList)
                    {
                        var tmpSubClassContainer = _projectInfo.GetClassForType(tmpSubclass.Name, tmpClass.FullUsingList);
                        tmpParentClassQueue.Enqueue(tmpSubClassContainer);

                        //Check if Methode with same parameter Typers exist
                        if (tmpSubClassContainer.MethodeList.Any(inItem => inItem.Name == tmpMethode.Name || inItem.Name == tmpOldName))
                        {
                            if (tmpSubClassContainer.IsInterface())
                            {
                                tmpMethode.ModifierList = tmpMethode.ModifierList.Where(inItem => inItem != "override").ToList();
                                tmpHandled = true;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void ConvertField(FieldContainer inField, bool inIsProperty = false, bool inDefiningComment = false)
        {
            inField.Comment = Converter.Comment(inField.Comment, inDefiningComment);
            inField.ModifierList = Converter.MapAndSortAttributes(inField.ModifierList, inIsProperty);
        }

    }
}
