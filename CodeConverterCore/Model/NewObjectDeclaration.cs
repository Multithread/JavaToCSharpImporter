using CodeConverterCore.Interface;

namespace CodeConverterCore.Model
{
    public class NewObjectDeclaration : ICodeEntry
    {
        public ICodeEntry InnerCode { get; set; }
    }
}
