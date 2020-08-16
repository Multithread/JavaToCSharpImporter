namespace JavaToCSharpConverter.Resources
{
    public static class ClassRenameJson
    {
        public static string SystemAliasJson = @"[
{'Source':{'Namespace':'java.lang','ClassName':'Object','MethodeName':'super'},'Target':{'Namespace':'','ClassName':'object','MethodeName':'base'}},
{'Source':{'Namespace':'java.lang','ClassName':'Object','MethodeName':'getClass'},'Target':{'Namespace':'','ClassName':'object','MethodeName':'GetType'}},
{'Source':{'Namespace':'java.lang','ClassName':'Object','MethodeName':'hashCode'},'Target':{'Namespace':'','ClassName':'object','MethodeName':'GetHashCode'}},
{'Source':{'Namespace':'java.lang','ClassName':'Object'},'Target':{'Namespace':'','ClassName':'object'}},
{'Source':{'Namespace':'java.lang','ClassName':'Boolean'},'Target':{'Namespace':'','ClassName':'bool'}},
{'Source':{'Namespace':'java.lang','ClassName':'Integer'},'Target':{'Namespace':'','ClassName':'int'}},
{'Source':{'Namespace':'java.lang','ClassName':'Void'},'Target':{'Namespace':'','ClassName':'void'}},
{'Source':{'Namespace':'java.lang','ClassName':'True'},'Target':{'Namespace':'','ClassName':'true'}},
{'Source':{'Namespace':'java.lang','ClassName':'False'},'Target':{'Namespace':'','ClassName':'false'}},
{'Source':{'Namespace':'java.lang','ClassName':'Int64'},'Target':{'Namespace':'','ClassName':'long'}},
{'Source':{'Namespace':'java.lang','ClassName':'Collection'},'Target':{'Namespace':'System.Collections.ObjectModel','ClassName':'Collection'}},
{'Source':{'Namespace':'java.lang','ClassName':'Class','MethodeName':'getCanonicalName'},'Target':{'Namespace':'','ClassName':'Type','MethodeName':'getCanonicalName'}},
{'Source':{'Namespace':'java.lang','ClassName':'Class'},'Target':{'Namespace':'System','ClassName':'Type'}},
{'Source':{'Namespace':'java.lang','ClassName':'Comparable','MethodeName':'compareTo'},'Target':{'Namespace':'','ClassName':'IComparable','MethodeName':'CompareTo'}},
{'Source':{'Namespace':'java.lang','ClassName':'Comparable'},'Target':{'Namespace':'System','ClassName':'IComparable'}},
]";
    }
}
