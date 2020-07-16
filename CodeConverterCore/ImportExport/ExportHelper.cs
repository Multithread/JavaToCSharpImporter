using CodeConverterCore.Model;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CodeConverterCore.ImportExport
{
    public static class ExportHelper
    {
        public static string CreateJsonFromClassList(List<ClassContainer> inClassList)
        {
            return JsonConvert.SerializeObject(inClassList);
        }
        public static string SaveAliasList(List<AliasObject> inAliasList)
        {
            return JsonConvert.SerializeObject(inAliasList);
        }
    }
}
