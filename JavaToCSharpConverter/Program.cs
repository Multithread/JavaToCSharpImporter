using JavaToCSharpConverter.Model;
using JavaToCSharpConverter.Model.CSharp;
using JavaToCSharpConverter.Model.Java;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JavaToCSharpConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            //ReplaceHelper.Run();

            var tmpClassList = new List<ClassContainer>();

            var tmpFileList = Directory.EnumerateFiles(@"C:\Data\LucenTestData"/*,"*", SearchOption.AllDirectories*/).ToList();
            for (var tmpI = 0; tmpI < tmpFileList.Count; tmpI++)
            {
                tmpFileList[tmpI] = File.ReadAllText(tmpFileList[tmpI]);
            }

            //tmpFileList = TestData.Filelist;
            tmpFileList.AddRange(TestData.Filelist);
            foreach (var tmpFile in tmpFileList)
            {
                tmpClassList.AddRange(JavaClassLoader.LoadFile(tmpFile));
            }
            var tmpObjectInformation = new ObjectInformation()
                .FillClasses(tmpClassList);

            //Static Data (Framework Classes) Only Required for the Parsing part
            tmpObjectInformation.FillClasses(
                new List<ClassContainer>
                {
                    new ClassContainer
                    {
                        Name="int",
                        Namespace="System"
                    },new ClassContainer
                    {
                        Name="string",
                        Namespace="System"
                    },new ClassContainer
                    {
                        Name="File",
                        Namespace="java.io"
                    }
                });

            var tmpOutPath = @"Z:\Result\Code\";
            Directory.CreateDirectory(tmpOutPath);

            var tmpReplacer = new IniParser.Parser.IniDataParser().Parse(File.ReadAllText(@"C:\Data\ini\replacer.ini"));

            foreach (var tmpClass in tmpClassList)
            {
                var tmpCSharp = CSharpClassWriter.CreateFile(tmpClass, new JavaToCSharpNameConverter(tmpObjectInformation));

                //Do Replacements for non-Fixable Code Changes
                foreach (var tmpKV in tmpReplacer[tmpClass.Name])
                {
                    tmpCSharp = tmpCSharp.Replace(tmpKV.KeyName, tmpKV.Value);
                }

                File.WriteAllText(tmpOutPath + tmpClass.Name.Split('<')[0] + ".cs", tmpCSharp);
                // Console.Write(tmpCSharp);
            }

            File.WriteAllText(@"Z:\Result\TestProject.csproj", $@"<?xml version=""1.0"" encoding=""utf-8""?>
<Project ToolsVersion=""15.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />
  <PropertyGroup>
    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
    <ProjectGuid>{{74ED4AF5-38DF-4589-927B-0A391C4D0AD0}}</ProjectGuid>
    <OutputType>Exe</OutputType>
    <RootNamespace>JavaToCSharpConverter</RootNamespace>
    <AssemblyName>JavaToCSharpConverter</AssemblyName>
    <TargetFrameworkVersion>v4.6.1</TargetFrameworkVersion>
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
{string.Join(Environment.NewLine, tmpClassList.Select(inItem => $"<Compile Include=\"Code\\{inItem.Name.Split('<')[0]}.cs\" />"))}
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

            File.WriteAllText(@"Z:\Result\Solution.sln", $@"Microsoft Visual Studio Solution File, Format Version 12.00
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

            //Console.ReadLine();
        }

    }
}
