using JavaToCSharpConverter.Helper;
using JavaToCSharpConverter.Interface;
using System.Collections.Generic;
using System.Linq;

namespace JavaToCSharpConverter.Model.CSharp
{
    public class JavaToCSharpNameConverter : INameConverter
    {
        public JavaToCSharpNameConverter(ObjectInformation inOjectInformation)
        {
            _objectInformation = inOjectInformation;

            _iniData = DataHelper.LoadIni(@"C:\Data\ini\mapper.ini");

        }
        private IniParser.Model.IniData _iniData;

        private ObjectInformation _objectInformation;

        /// <summary>
        /// Change the Name of a Methode
        /// Consistency Required for the Code to work Propertly
        /// </summary>
        public string ChangeMethodeName(string inMethodeName)
        {
            return inMethodeName.PascalCase();
        }


        /// <summary>
        /// Change the Namespace
        /// </summary>
        public string ChangeNamespace(string inNamespace)
        {
            var tmpReplaceNamespace = _iniData["Namespace"][inNamespace];

            if (!string.IsNullOrEmpty(tmpReplaceNamespace))
            {
                return tmpReplaceNamespace;
            }

            foreach (var tmpData in _iniData["TypeStartsWith"])
            {
                if (inNamespace.StartsWith(tmpData.KeyName))
                {
                    inNamespace = tmpData.Value + inNamespace.Substring(tmpData.KeyName.Length);
                }
            }

            //First Char to Upper
            var tmpNamespaceSplit = inNamespace.Split('.');
            inNamespace = string.Join(".", tmpNamespaceSplit.Select(inItem => inItem.PascalCase()));

            return inNamespace;
        }

        private HashSet<string> _accessModifier = new HashSet<string> { "private", "protected", "internal", "public" };
        private HashSet<string> _typeModifier = new HashSet<string> { "sealed", "static", "abstract", "override", "readonly" };

        /// <summary>
        /// Mapp and Sort the Attributes from Java to C#
        /// </summary>
        /// <param name="inAttributeList"></param>
        /// <param name="inProperty"></param>
        /// <returns></returns>
        public List<string> MapAndSortAttributes(List<string> inAttributeList, bool inProperty = false)
        {
            //Attribute Mappen, welche anderst heissen
            for (var tmpI = 0; tmpI < inAttributeList.Count; tmpI++)
            {
                if (inAttributeList[tmpI] == "@Override")
                {
                    inAttributeList[tmpI] = "override";
                }
                if (inAttributeList[tmpI] == "final")
                {
                    if (inProperty)
                    {
                        inAttributeList[tmpI] = "readonly";
                    }
                    else
                    {
                        inAttributeList[tmpI] = "sealed";
                    }
                }
            }

            //Attribute Sortieren
            return inAttributeList.OrderBy(inItem =>
            {
                if (_accessModifier.Contains(inItem))
                {
                    return -1;
                }
                if (_typeModifier.Contains(inItem))
                {
                    return 0;
                }

                return 1;
            })
            .ToList();
        }

        /// <summary>
        /// Information for System Namespace.Type Mapping
        /// </summary>
        private Dictionary<string, string> _typeMapping = new Dictionary<string, string>
        {
            {"","" }
        };

        public string MapType(string inType, List<string> inNamespaces)
        {
            inType = inType.Trim(' ');
            var tmpClassInformation = _objectInformation.GetClassForType(inType, inNamespaces);
            if (tmpClassInformation == null)
            {
                //If FrameworkTypes
                if (inType == "boolean")
                {
                    return "bool";
                }
                if (inType == "int")
                {
                    return "System.Int32";
                }

                return inType;
            }

            var tmpQualifiedType = tmpClassInformation.Namespace + "." + tmpClassInformation.Name;
            if (_typeMapping.TryGetValue(tmpQualifiedType, out var tmpType))
            {
                return tmpType;
            }

            return tmpQualifiedType;
        }

        public ClassContainer GetClassForType(string inType, List<string> inNamespaces)
        {
            return _objectInformation.GetClassForType(inType.Trim(' '), inNamespaces);

        }

        public string MapFunction(string inFunction, string inType, List<string> inNamespaces)
        {
            inType = inType.Trim(' ');
            inFunction = inFunction.Trim(' ');
            var tmpClassInformation = _objectInformation.GetClassForType(inType, inNamespaces);

            if (tmpClassInformation == null)
            {
                return inFunction;
            }

            var tmpQualifiedType = tmpClassInformation.Namespace + "." + tmpClassInformation.Name + "." + inFunction.PascalCase();
            //if (_typeMapping.TryGetValue(tmpQualifiedType, out var tmpType))
            //{
            //    return tmpType;
            //}

            return tmpQualifiedType;
        }
    }
}

