using Antlr4.Runtime.Atn;
using CodeConverterCore.Enum;
using CodeConverterCore.Helper;
using CodeConverterCore.Interface;
using CodeConverterCore.Model;
using System;
using System.Collections.Generic;

namespace JavaToCSharpConverter.Model
{
    /// <summary>
    /// Used to check the Elvis-operator for C# problems with structs and nullable values
    /// </summary>
    public class ElvisStatementCodeStepper : ICodeStepperEvents
    {
        public ElvisStatementCodeStepper(ClassContainer inCurrentClass)
        {
            _currentClass = inCurrentClass;
        }
        private ClassContainer _currentClass;

        public void CodeEntryStep(ICodeEntry inCodeEntry)
        {
            if (inCodeEntry is StatementCode)
            {
                var tmpStatement = inCodeEntry as StatementCode;
                if (tmpStatement.StatementType == StatementTypeEnum.Elvis)
                {
                    if (IsCodeBlockNull(tmpStatement.StatementCodeBlocks[1]))
                    {
                        var tmpTypeConverter = new TypeConversion();
                        tmpTypeConverter.Type = CreateNullableType(GetTypeFromCodeBlock(tmpStatement.StatementCodeBlocks[2]));
                        var tmpType = _currentClass.Parent.ClassFromBaseType(tmpTypeConverter.Type);
                        
                        if (tmpType.ModifierList.Contains(Modifiers.Struct))
                        { 
                            tmpTypeConverter.PreconversionValue = tmpStatement.StatementCodeBlocks[2];

                            tmpStatement.StatementCodeBlocks[2] = new CodeBlock()
                            {
                                CodeEntries = new List<ICodeEntry>
                            {
                                tmpTypeConverter
                            }
                            };
                        }
                    }
                    else if (IsCodeBlockNull(tmpStatement.StatementCodeBlocks[2]))
                    {
                        var tmpTypeConverter = new TypeConversion();
                        tmpTypeConverter.Type = CreateNullableType(GetTypeFromCodeBlock(tmpStatement.StatementCodeBlocks[1]));
                        var tmpType = _currentClass.Parent.ClassFromBaseType(tmpTypeConverter.Type);

                        if (tmpType.ModifierList.Contains(Modifiers.Struct))
                        {
                            tmpTypeConverter.PreconversionValue = tmpStatement.StatementCodeBlocks[1];
                            tmpStatement.StatementCodeBlocks[1] = new CodeBlock()
                            {
                                CodeEntries = new List<ICodeEntry>
                            {
                                tmpTypeConverter
                            }
                            };
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Create Nullable type
        /// </summary>
        /// <param name="inBaseType"></param>
        /// <returns></returns>
        private TypeContainer CreateNullableType(TypeContainer inBaseType)
        {
            return new TypeContainer
            {
                Type = inBaseType.Type,
                Name = inBaseType.Name,
                GenericTypes = inBaseType.GenericTypes,
                Extends = inBaseType.Extends,
                IsArray = inBaseType.IsArray,
                IsNullable = true,
            };
        }

        private TypeContainer GetTypeFromCodeBlock(CodeBlock inCodeBlock)
        {
            foreach (var tmpEntry in inCodeBlock.CodeEntries)
            {
                if (tmpEntry is NewObjectDeclaration)
                {
                    var tmpObjectDecl = (tmpEntry as NewObjectDeclaration).InnerCode;
                    var tmpConstVal = (tmpObjectDecl as ConstantValue).Value;
                    if (tmpConstVal is TypeContainer)
                    {
                        return tmpConstVal as TypeContainer;
                    }
                    else
                    {
                        throw new NotImplementedException("Not implemented yet");
                    }
                }
                else if (tmpEntry is ConstantValue)
                {
                    var tmpConstValue = (tmpEntry as ConstantValue).Value;
                    if (tmpConstValue is VariableDeclaration)
                    {
                        var tmpAccess = tmpConstValue as VariableDeclaration;
                        return tmpAccess.Type;
                    }
                    else
                    {
                        throw new NotImplementedException("Not implemented yet");
                    }
                }
                else if (tmpEntry is VariableAccess)
                {
                    var tmpAccess = tmpEntry as VariableAccess;
                }
                else
                {
                    throw new NotImplementedException("Not implemented yet");
                }
            }
            return null;
        }
        private bool IsCodeBlockNull(CodeBlock inBlock)
        {
            if (((inBlock?.CodeEntries[0] as ConstantValue)?.Value as TypeContainer)?.Name == "null")
            {
                return true;
            }
            return false;
        }
    }
}
