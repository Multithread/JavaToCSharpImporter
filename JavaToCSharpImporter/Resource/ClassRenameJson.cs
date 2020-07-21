namespace JavaToCSharpConverter.Resources
{
    public static class ClassRenameJson
    {
        public static string SystemAliasJson = @"[
{'Source':{'Namespace':'java.lang','ClassName':'Object'},'Target':{'Namespace':'','ClassName':'object'}},
{'Source':{'Namespace':'java.lang','ClassName':'Object','MethodeName':'super'},'Target':{'Namespace':'','ClassName':'object','MethodeName':'base'}},
{'Source':{'Namespace':'java.lang','ClassName':'Boolean'},'Target':{'Namespace':'','ClassName':'bool'}},
{'Source':{'Namespace':'java.lang','ClassName':'Integer'},'Target':{'Namespace':'','ClassName':'int'}},
]";
    }
}
