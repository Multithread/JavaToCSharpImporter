namespace JavaToCSharpConverter.Resource
{
    public static class StringReplacementJson
    {
        public static string ReplacementJson = @"[
  {
    ""NameRegex"": ""."",
    ""SourceText"": ""double.compare"",
    ""ReplacementText"": ""FLoatCompareHelper.DoubleCompare"",
    ""RequiredUsing"": ""JavaCoreReplacer""
  },
  {
    ""NameRegex"": ""."",
    ""SourceText"": ""float.compare"",
    ""ReplacementText"": ""FLoatCompareHelper.FloatCompare"",
    ""RequiredUsing"": ""JavaCoreReplacer""
  },
  {
    ""NameRegex"": ""."",
    ""SourceText"": ""double.doubleToLongBits"",
    ""ReplacementText"": ""FLoatCompareHelper.FloatToIntBits"",
    ""RequiredUsing"": ""JavaCoreReplacer""
  },
  {
    ""NameRegex"": ""."",
    ""SourceText"": ""float.floatToIntBits"",
    ""ReplacementText"": ""FLoatCompareHelper.DoubleToLongBits"",
    ""RequiredUsing"": ""JavaCoreReplacer""
  }
]";
    }
}
