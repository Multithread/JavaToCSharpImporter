using JavaToCSharpConverter.Helper;
using System;
using System.Collections.Generic;

namespace JavaToCSharpConverter.Interface
{
    public abstract class CodeSplitterEvents
    {
        /// <summary>
        /// Something Started
        /// </summary>
        /// <param name="inKey"></param>
        public abstract object Started(Checker inElement);

        /// <summary>
        /// For Spezial Characters
        /// </summary>
        /// <param name="inKey"></param>
        /// <returns>Shall there be a Split at this Position?</returns>
        public abstract object CurrentCharacter(char inChar);

        /// <summary>
        /// Something ended
        /// </summary>
        /// <param name="inKey"></param>
        /// <param name="inPosition"></param>
        /// <returns>Shall there be a Split at this Position?</returns>
        public abstract object Ended(Checker inElement);

        /// <summary>
        /// Get the Current Stack of the Splitter
        /// </summary>
        public Func<List<Checker>> GetCurrentStack { get; internal set; }
    }
}
