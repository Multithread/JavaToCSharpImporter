using CodeConverterCore.Enum;
using CodeConverterCore.Helper;
using CodeConverterCore.Interface;
using CodeConverterCore.Model;

namespace JavaToCSharpConverter.Model
{
    public class NamespaceCodeStepper : ICodeStepperEvents
    {
        public NamespaceCodeStepper(ClassContainer inCurrentClass)
        {
            _currentClass = inCurrentClass;
        }
        private ClassContainer _currentClass;

        public void CodeEntryStep(ICodeEntry inCodeEntry)
        {
            if (inCodeEntry is StatementCode)
            {
                var tmpStatement = inCodeEntry as StatementCode;
                if (tmpStatement.StatementType == StatementTypeEnum.Assert)
                {
                    AddToUsingIfRequired(_currentClass, Modifiers.SystemDiagnosis);
                }
            }
            else if (inCodeEntry is VariableDeclaration)
            {
                var tmpVarDecl = inCodeEntry as VariableDeclaration;
                AddToUsingIfRequired(_currentClass, tmpVarDecl.Type);
            }
            else
            {

            }
        }

        /// <summary>
        /// AddUsing to ClassContainer if Required
        /// </summary>
        /// <param name="inClass"></param>
        /// <param name="inUsingToCheck"></param>
        public static void AddToUsingIfRequired(ClassContainer inClass, TypeContainer inType)
        {
            var tmpType = inClass.Parent.ClassFromBaseType(inType);
            AddToUsingIfRequired(inClass, tmpType.Namespace);
        }

        /// <summary>
        /// AddUsing to ClassContainer if Required
        /// </summary>
        /// <param name="inClass"></param>
        /// <param name="inUsingToCheck"></param>
        private static void AddToUsingIfRequired(ClassContainer inClass, string inUsingToCheck)
        {
            if (!inClass.FullUsingList.Contains(inUsingToCheck))
            {
                inClass.UsingList.Add(inUsingToCheck);
            }
        }
    }
}
