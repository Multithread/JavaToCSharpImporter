using CodeConverterCore.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeConverterCore.ImportExport
{
    public static class ExportHelper
    {
        private static JsonSerializerSettings Settings()
        {
            return new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                ContractResolver = ShouldSerializeContractResolver.Instance,
            };
        }

        public static string CreateJsonFromClassList(List<ClassContainer> inClassList)
        {
            return JsonConvert.SerializeObject(inClassList, Settings());
        }
        public static string SaveAliasList(List<AliasObject> inAliasList)
        {
            return JsonConvert.SerializeObject(inAliasList, Settings());
        }
        public static string SaveMappingList(List<LanguageMappingObject> inMappingObjectList)
        {
            return JsonConvert.SerializeObject(inMappingObjectList, Settings());
        }
    }

    public class ShouldSerializeContractResolver : DefaultContractResolver
    {
        public static readonly ShouldSerializeContractResolver Instance = new ShouldSerializeContractResolver();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (property.PropertyType != typeof(string))
            {
                if (property.PropertyType.GetInterface(nameof(IEnumerable)) != null)
                    property.ShouldSerialize =
                        instance => (instance?.GetType().GetProperty(property.PropertyName).GetValue(instance) as IEnumerable<object>)?.Count() > 0;
            }
            return property;
        }
    }
}
