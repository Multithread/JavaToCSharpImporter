using JavaToCSharpConverter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaToCSharpConverter.Interface
{
    public interface ILoadOOPLanguage
    {
        ObjectInformation CreateObjectInformation(List<string> inFileContents, IniParser.Model.IniData inConfiguration);
    }
}
