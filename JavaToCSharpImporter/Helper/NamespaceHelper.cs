using CodeConverterCore.Enum;
using CodeConverterCore.Helper;
using CodeConverterCore.Interface;
using CodeConverterCore.Model;
using JavaToCSharpConverter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JavaToCSharpConverter.Helper
{
    public static class NamespaceHelper
    {
        /// <summary>
        /// Add all required Usings depending on the Classes
        /// </summary>
        /// <param name="inClass"></param>
        public static void HandleNamespaces(ClassContainer inClass)
        {
            if (!inClass.UsingList.Contains("UnknownTypes"))
            {
                inClass.UsingList.Add("UnknownTypes");
            }
            if (!inClass.UsingList.Contains("System"))
            {
                inClass.UsingList.Add("System");
            }

            var tmpNamespaceStepper = new NamespaceCodeStepper(inClass);
            CodeSteppingHelper.CheckClassCodeBlocks(tmpNamespaceStepper, inClass);

            //Check Methode Return Type if Using is Required
            foreach (var tmpMethode in inClass.MethodeList)
            {
                if (tmpMethode.ReturnType != null)
                {
                    NamespaceCodeStepper.AddToUsingIfRequired(inClass, tmpMethode.ReturnType);
                }
            }

            //Check Elvis Statements for struct with null Problem.
            var tmpElvisStepper = new ElvisStatementCodeStepper(inClass);
            CodeSteppingHelper.CheckClassCodeBlocks(tmpElvisStepper, inClass);
        }
    }
}
