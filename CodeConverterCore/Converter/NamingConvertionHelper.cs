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
