namespace CodeConverterCore.Interface
{
    public interface ICodeStepperEvents
    {
        /// <summary>
        /// Called ever time the CodeStepper finds a ICodeEntry object
        /// </summary>
        /// <param name="inCodeEntry"></param>
        void CodeEntryStep(ICodeEntry inCodeEntry);
    }
}
