using CodeConverterCore.Analyzer;
using CodeConverterCore.Converter;
using CodeConverterCore.Enum;
using CodeConverterCore.Helper;
using CodeConverterCore.ImportExport;
using CodeConverterCore.Interface;
using CodeConverterCore.Model;
using CodeConverterCSharp;
using CodeConverterCSharp.Lucenene;
using CodeConverterJava.Model;
using JavaToCSharpConverter.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JavaToCSharpConverter
{
    public static class ConverterHelper
    {
        public static string JavaMapperPath;
        public static string LuceneReplacerPath;

        public static void ConvertFiles(string inSourcePath, string inOutPath)
        {
            JavaMapperPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\JavaData\\JavaMapper.ini";
            LuceneReplacerPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\JavaData\\LuceneReplacer.ini";

            var tmpObjectInformation = ProjectInformationHelper.DoFullRun(
                ImportHelper.ImportMappingList(ClassRenameJson.SystemAliasJson), new ConverterLucene(), new JavaLoader() { LoadDefaultData = true },
                LoadFIleContents(inSourcePath).ToArray());

            Directory.CreateDirectory(inOutPath);

            //var tmpReplacer = new IniParser.Parser.IniDataParser().Parse(File.ReadAllText(LuceneReplacerPath));

            if (tmpObjectInformation.MissingMethodes.Count > 0)
            {
                throw new Exception("Missing Methodes Class to be Implemented");
            }
            WriteCSharpCode(inOutPath, tmpObjectInformation, null);

            CreateCSharpSLNFile(tmpObjectInformation, inOutPath);
        }

        /// <summary>
        /// Write the Classes as C# Code 
        /// </summary>
        /// <param name="inOutPath"></param>
        /// <param name="tmpClassList"></param>
        /// <param name="inProjectInformation"></param>
        /// <param name="tmpReplacer"></param>
        private static void WriteCSharpCode(string inOutPath, ProjectInformation inProjectInformation, IniParser.Model.IniData tmpReplacer)
        {
            var tmpWriter = new CSharpClassWriter();
            foreach (var tmpClass in inProjectInformation.ClassList)
            {
                if (tmpClass.ClassType == ClassTypeEnum.System)
                {
                    continue;
                }
                if (!tmpClass.UsingList.Contains("UnknownTypes"))
                {
                    tmpClass.UsingList.Add("UnknownTypes");
                }
                if (!tmpClass.UsingList.Contains("System"))
                {
                    tmpClass.UsingList.Add("System");
                }
                var tmpCSharp = tmpWriter.CreateClassFile(tmpClass).Content;

                //Do Replacements for non-Fixable Code Changes
                if (tmpReplacer != null)
                {
                    foreach (var tmpKV in tmpReplacer[tmpClass.Name])
                    {
                        tmpCSharp = tmpCSharp.Replace(tmpKV.KeyName, tmpKV.Value);
                    }
                }

                var tmpNewNamespace = tmpClass.Namespace.Split('.');
                var tmpNewPath = Path.Combine(inOutPath, Path.Combine(tmpNewNamespace));

                Directory.CreateDirectory(tmpNewPath);
                File.WriteAllText(Path.Combine(tmpNewPath, tmpClass.Name + ".cs"), tmpCSharp);
            }

            //Write UnknownType File
            var tmpUnknownFilePaths = new List<string>();
            var tmpUsings = new List<string>();
            foreach (var tmpClass in inProjectInformation.GetAllUnknownTypes())
            {
                var tmpCSharp = new StringBuilder();
                tmpClass.ModifierList.Add("public");
                tmpClass.ModifierList.Add("class");
                new CSharpClassWriter().AddClassContainerString(tmpClass, tmpCSharp, 1);

                tmpUnknownFilePaths.Add(tmpCSharp.ToString());
            }

            //Write unknown Types into File
            var tmpUnknownFile = $@"
{string.Join(Environment.NewLine, tmpUsings.OrderBy(inItem => inItem).Select(inItem => $"using {inItem};"))}

namespace UnknownTypes
{{
{string.Join(Environment.NewLine + Environment.NewLine, tmpUnknownFilePaths)}
}}";
            Directory.CreateDirectory(Path.Combine(inOutPath));
            File.WriteAllText(Path.Combine(inOutPath, "UnknownTypes.cs"), tmpUnknownFile);
        }

        private static List<string> LoadFIleContents(string inSourcePath)
        {
            var tmpFileList = Directory.EnumerateFiles(inSourcePath, "*", SearchOption.AllDirectories).ToList();
            for (var tmpI = 0; tmpI < tmpFileList.Count; tmpI++)
            {
                var tmpFileText = File.ReadAllText(tmpFileList[tmpI]);

                //TODO Load Replaces from Ini File.
                tmpFileText = tmpFileText.Replace(">...", ">[]");

                tmpFileList[tmpI] = tmpFileText;
            }
            return tmpFileList;
        }

        /// <summary>
        /// Create Project and sln file for VIsual Studio
        /// </summary>
        /// <param name="tmpClassList"></param>
        /// <param name="tmpObjectInformation"></param>
        /// <param name="tmpIniData"></param>
        private static void CreateCSharpSLNFile(ProjectInformation tmpObjectInformation, string inPath)
        {
            //Create C# Solution with all created Files
            File.WriteAllText(Path.Combine(inPath, "TestProject.csproj"), $@"<?xml version=""1.0"" encoding=""utf-8""?>
<Project ToolsVersion=""15.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />
  <PropertyGroup>
    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
    <ProjectGuid>{{74ED4AF5-38DF-4589-927B-0A391C4D0AD0}}</ProjectGuid>
    <OutputType>Exe</OutputType>
    <RootNamespace>JavaToCSharpConverter</RootNamespace>
    <AssemblyName>JavaToCSharpConverter</AssemblyName>
    <TargetFrameworkVersion>v4.8</TargetFrameworkVersion>
    <LangVersion>8.0</LangVersion>
    <FileAlignment>512</FileAlignment>
    <AutoGenerateBindingRedirects>true</AutoGenerateBindingRedirects>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "">
    <PlatformTarget>AnyCPU</PlatformTarget>
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
    <OutputPath>bin\Debug\</OutputPath>
    <DefineConstants>DEBUG;TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' "">
    <PlatformTarget>AnyCPU</PlatformTarget>
    <DebugType>pdbonly</DebugType>
    <Optimize>true</Optimize>
    <OutputPath>bin\Release\</OutputPath>
    <DefineConstants>TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <PropertyGroup>
    <StartupObject>JavaToCSharpConverter.Program</StartupObject>
  </PropertyGroup>
  <PropertyGroup Condition=""'$(Configuration)|$(Platform)' == 'Debug64|AnyCPU'"">
    <DebugSymbols>true</DebugSymbols>
    <OutputPath>bin\Debug64\</OutputPath>
    <DefineConstants>DEBUG;TRACE</DefineConstants>
    <DebugType>full</DebugType>
    <PlatformTarget>AnyCPU</PlatformTarget>
    <ErrorReport>prompt</ErrorReport>
    <CodeAnalysisRuleSet>MinimumRecommendedRules.ruleset</CodeAnalysisRuleSet>
    <Prefer32Bit>true</Prefer32Bit>
  </PropertyGroup>
  <PropertyGroup Condition=""'$(Configuration)|$(Platform)' == 'Debug|x64'"">
    <PlatformTarget>x64</PlatformTarget>
    <OutputPath>bin\x64\Debug\</OutputPath>
    <DefineConstants>TRACE;DEBUG</DefineConstants>
  </PropertyGroup>
  <PropertyGroup Condition=""'$(Configuration)|$(Platform)' == 'Release|x64'"">
    <PlatformTarget>x64</PlatformTarget>
    <OutputPath>bin\x64\Release\</OutputPath>
  </PropertyGroup>
  <PropertyGroup Condition=""'$(Configuration)|$(Platform)' == 'Debug64|x64'"">
    <PlatformTarget>x64</PlatformTarget>
    <OutputPath>bin\x64\Debug64\</OutputPath>
    <DefineConstants>TRACE;DEBUG</DefineConstants>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include=""MoreLinq, Version=3.1.0.0, Culture=neutral, PublicKeyToken=384d532d7e88985d, processorArchitecture=MSIL"">
      <HintPath>..\packages\morelinq.3.1.0\lib\net451\MoreLinq.dll</HintPath>
    </Reference>
    <Reference Include=""Multithreading, Version=1.0.0.0, Culture=neutral, processorArchitecture=AMD64"">
      <SpecificVersion>False</SpecificVersion>
      <HintPath>..\..\FindFileCopys\FindFileCopys\bin\x64\Debug\Multithreading.dll</HintPath>
    </Reference>
    <Reference Include=""System"" />
    <Reference Include=""System.Core"" />
    <Reference Include=""System.ValueTuple, Version=4.0.2.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51, processorArchitecture=MSIL"">
      <HintPath>..\packages\System.ValueTuple.4.4.0\lib\net461\System.ValueTuple.dll</HintPath>
    </Reference>
    <Reference Include=""System.Xml.Linq"" />
    <Reference Include=""System.Data.DataSetExtensions"" />
    <Reference Include=""Microsoft.CSharp"" />
    <Reference Include=""System.Data"" />
    <Reference Include=""System.Net.Http"" />
    <Reference Include=""System.Xml"" />
  </ItemGroup>
  <ItemGroup>
{string.Join(Environment.NewLine, tmpObjectInformation.ClassList.Where(inItem => inItem.ClassType == ClassTypeEnum.Normal).Select(inItem => $"<Compile Include=\"{Path.Combine(inItem.Namespace.Split('.'))}\\{inItem.Name}.cs\" />"))}
<Compile Include=""UnknownTypes.cs"" />
</ItemGroup>
  <ItemGroup>
{""    /*<None Include=""App.config"" />
    <None Include=""packages.config"" />*/
    }
  </ItemGroup>
  <ItemGroup />
  <Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />
</Project>
");

            File.WriteAllText(Path.Combine(inPath, "Solution.sln"), $@"Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio 15
VisualStudioVersion = 15.0.26730.16
MinimumVisualStudioVersion = 10.0.40219.1
Project(""{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}"") = ""TestProject"", ""TestProject.csproj"", ""{{74ED4AF5-38DF-4589-927B-0A391C4D0AD0}}""
EndProject
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Debug|x64 = Debug|x64
		Debug64|Any CPU = Debug64|Any CPU
		Debug64|x64 = Debug64|x64
		Release|Any CPU = Release|Any CPU
		Release|x64 = Release|x64
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{{74ED4AF5-38DF-4589-927B-0A391C4D0AD0}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{{74ED4AF5-38DF-4589-927B-0A391C4D0AD0}}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{{74ED4AF5-38DF-4589-927B-0A391C4D0AD0}}.Debug|x64.ActiveCfg = Debug|x64
		{{74ED4AF5-38DF-4589-927B-0A391C4D0AD0}}.Debug|x64.Build.0 = Debug|x64
		{{74ED4AF5-38DF-4589-927B-0A391C4D0AD0}}.Debug64|Any CPU.ActiveCfg = Debug64|Any CPU
		{{74ED4AF5-38DF-4589-927B-0A391C4D0AD0}}.Debug64|Any CPU.Build.0 = Debug64|Any CPU
		{{74ED4AF5-38DF-4589-927B-0A391C4D0AD0}}.Debug64|x64.ActiveCfg = Debug64|x64
		{{74ED4AF5-38DF-4589-927B-0A391C4D0AD0}}.Debug64|x64.Build.0 = Debug64|x64
		{{74ED4AF5-38DF-4589-927B-0A391C4D0AD0}}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{{74ED4AF5-38DF-4589-927B-0A391C4D0AD0}}.Release|Any CPU.Build.0 = Release|Any CPU
		{{74ED4AF5-38DF-4589-927B-0A391C4D0AD0}}.Release|x64.ActiveCfg = Release|x64
		{{74ED4AF5-38DF-4589-927B-0A391C4D0AD0}}.Release|x64.Build.0 = Release|x64
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
	GlobalSection(ExtensibilityGlobals) = postSolution
		SolutionGuid = {{3535A1B6-00D7-40A9-A145-5F8EAF85103A}}
	EndGlobalSection
EndGlobal
");
        }
    }
}
