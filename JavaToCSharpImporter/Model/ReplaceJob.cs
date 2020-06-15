using Multithreading;

namespace JavaToCSharpConverter.Model
{
    public class ReplaceJob : Aufgabe
    {
        public ReplaceInFile ReplaceFile { get; set; }
        public override object Abarbeiten()
        {
            ReplaceFile.Search();
            return ReplaceFile;
        }
    }
}
