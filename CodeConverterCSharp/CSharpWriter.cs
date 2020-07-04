using CodeConverterCore.Converter;
using CodeConverterCore.Model;
using CodeConverterCSharp.Model;
using System;
using System.Collections.Generic;

namespace CodeConverterCSharp
{
    public static class CSharpWriter
    {
        /// <summary>
        /// Create C# Classes for the spezified Project
        /// </summary>
        /// <param name="inObjectInformation"></param>
        /// <returns></returns>
        public static IEnumerable<FileWriteInfo> CreateClassesFromObjectInformation(ProjectInformation inObjectInformation, IConverter inConverter)
        {
            var tmpWriter = new CSharpClassWriter(inConverter);
            foreach (var tmpClass in inObjectInformation.ClassList)
            {
                yield return tmpWriter.CreateClassFile(tmpClass);
            }
        }
    }
}
