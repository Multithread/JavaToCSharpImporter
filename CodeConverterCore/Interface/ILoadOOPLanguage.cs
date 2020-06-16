using CodeConverterCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeConverterCore.Interface
{
    public interface ILoadOOPLanguage
    {
        ProjectInformation CreateObjectInformation(List<string> inFileContents, IniParser.Model.IniData inConfiguration);
    }
}
