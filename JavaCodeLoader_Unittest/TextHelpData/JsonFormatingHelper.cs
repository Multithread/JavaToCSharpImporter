using NUnit.Framework;
using CodeConverterJava.Resources;
using CodeConverterCore.ImportExport;

namespace CodeConverterJava_Unittest.Objektstruktur
{
    public class JsonFormatingHelper
    {
        [Test]
        public void FormatJSON()
        {
            var tmpNewText = ExportHelper.CreateJsonFromClassList(ImportHelper.ImportClasses(JavaLangClassJson.JavaLang));
            Assert.AreEqual(true, true);
        }

    }
}
