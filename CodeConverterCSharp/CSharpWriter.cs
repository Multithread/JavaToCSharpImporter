using CodeConverterCore.Model;
using CodeConverterCSharp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeConverterCSharp
{
    public static class CSharpWriter
    {
        /// <summary>
        /// Create C# Classes for the spezified Project
        /// </summary>
        /// <param name="inObjectInformation"></param>
        /// <returns></returns>
        public static IEnumerable<FileWriteInfo> CreateClassesFromObjectInformation(ProjectInformation inObjectInformation)
        {
            foreach(var tmpClass in inObjectInformation.ClassList)
            {
                yield return CSharpClassWriter.CreateClassFile(tmpClass);
            }
        }
    }
}
