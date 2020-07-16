using CodeConverterCore.Model;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CodeConverterCore.ImportExport
{
    public static class ImportHelper
    {
        public static List<ClassContainer> ImportClasses(string inClassJson)
        {
            return JsonConvert.DeserializeObject<List<ClassContainer>>(inClassJson);
        }

        public static List<AliasObject> ImportAliasList(string inClassJson)
        {
            return JsonConvert.DeserializeObject<List<AliasObject>>(inClassJson);
        }
    }
}
