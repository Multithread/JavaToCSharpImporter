using CodeConverterCore.Model;

namespace CodeConverterCore.Converter
{
    public class NamingConvertionHelper
    {
        private IConverter Converter;
        public NamingConvertionHelper(IConverter inConverter)
        {
            Converter = inConverter;
        }

        public void ConvertProject(ProjectInformation inProject)
        {
            foreach (var tmpClass in inProject.ClassList)
            {
                ConvertClass(tmpClass);
            }
        }

        public void ConvertClass(ClassContainer inClass)
        {
            inClass.Type.Type.Name = Converter.ClassName(inClass);
            for (var tmpI = 0; tmpI < inClass.ModifierList.Count; tmpI++)
            {
                inClass.ModifierList[tmpI] = Converter.Modifier(inClass.ModifierList[tmpI]);
            }
            foreach (var tmpInnerClass in inClass.InnerClasses)
            {
                ConvertClass(tmpInnerClass);
            }

            foreach (var tmpField in inClass.FieldList)
            {
                ConvertField(tmpField);
            }

            foreach (var tmpMethode in inClass.MethodeList)
            {
                for (var tmpI = 0; tmpI < tmpMethode.ModifierList.Count; tmpI++)
                {
                    tmpMethode.ModifierList[tmpI] = Converter.Modifier(tmpMethode.ModifierList[tmpI]);
                }

                foreach (var tmpField in tmpMethode.Parameter)
                {
                    ConvertField(tmpField);
                    tmpField.Name = Converter.MethodeInParameter(tmpField);
                }
            }

            inClass.Namespace = Converter.Namespace(inClass.Namespace);
            for(var tmpI = 0; tmpI < inClass.UsingList.Count; tmpI++)
            {
                inClass.UsingList[tmpI] = Converter.Namespace(inClass.UsingList[tmpI]);
            }
        }

        private void ConvertField(FieldContainer inField)
        {
            for (var tmpI = 0; tmpI < inField.ModifierList.Count; tmpI++)
            {
                inField.ModifierList[tmpI] = Converter.Modifier(inField.ModifierList[tmpI]);
            }
        }
    }
}
