using CodeConverterCore.Model;

namespace CodeConverterCore.Analyzer
{
    public class AnalyzerSettings
    {

        public int MaxAmountOfParallelism { get; set; } = 7;

        /// <summary>
        /// An Unknown Type has been added
        /// </summary>
        public event UnknownTypeHandler UnknownTypeAdded;

        internal void InvokeUnknownTypeAdded(UnknownTypeClass inUnknownType)
        {
            UnknownTypeAdded?.Invoke(inUnknownType);
        }
        public delegate void UnknownTypeHandler(UnknownTypeClass inUnknownType);
    }
}
