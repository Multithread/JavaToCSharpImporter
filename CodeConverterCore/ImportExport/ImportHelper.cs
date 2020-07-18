using CodeConverterCore.Enum;
using CodeConverterCore.Model;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CodeConverterCore.ImportExport
{
    public static class ImportHelper
    {
        public static List<ClassContainer> ImportClasses(string inClassJson)
        {
            var tmpClassList = JsonConvert.DeserializeObject<List<ClassContainer>>(inClassJson);
            tmpClassList.ForEach(inItem => inItem.ClassType = ClassTypeEnum.System);
            return tmpClassList;
        }

        public static List<AliasObject> ImportAliasList(string inClassJson)
        {
            return JsonConvert.DeserializeObject<List<AliasObject>>(inClassJson);
        }

        public static List<LanguageMappingObject> ImportMappingList(string inMappingJson)
        {
            return JsonConvert.DeserializeObject<List<LanguageMappingObject>>(inMappingJson);
        }
    }
}
