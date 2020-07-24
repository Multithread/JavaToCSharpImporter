namespace JavaToCSharpConverter.Resources
{
    public static class ClassRenameJson
    {
        public static string SystemAliasJson = @"[
{'Source':{'Namespace':'java.lang','ClassName':'Object'},'Target':{'Namespace':'','ClassName':'object'}},
{'Source':{'Namespace':'java.lang','ClassName':'Object','MethodeName':'super'},'Target':{'Namespace':'','ClassName':'object','MethodeName':'base'}},
{'Source':{'Namespace':'java.lang','ClassName':'Boolean'},'Target':{'Namespace':'','ClassName':'bool'}},
{'Source':{'Namespace':'java.lang','ClassName':'Integer'},'Target':{'Namespace':'','ClassName':'int'}},
{'Source':{'Namespace':'java.lang','ClassName':'Void'},'Target':{'Namespace':'','ClassName':'void'}},
{'Source':{'Namespace':'java.lang','ClassName':'True'},'Target':{'Namespace':'','ClassName':'true'}},
{'Source':{'Namespace':'java.lang','ClassName':'False'},'Target':{'Namespace':'','ClassName':'false'}},
{'Source':{'Namespace':'java.lang','ClassName':'Int64'},'Target':{'Namespace':'','ClassName':'long'}},
{'Source':{'Namespace':'java.lang','ClassName':'Collection'},'Target':{'Namespace':'System.Collections.ObjectModel','ClassName':'Collection'}},
]";
    }
}
