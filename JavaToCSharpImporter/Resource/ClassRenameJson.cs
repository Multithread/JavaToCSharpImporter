namespace JavaToCSharpConverter.Resources
{
    public static class ClassRenameJson
    {
        public static string SystemAliasJson = @"[
{'Source':{'Namespace':'java.lang','ClassName':'Object','MethodeName':'super'},'Target':{'Namespace':'System','ClassName':'object','MethodeName':'base'}},
{'Source':{'Namespace':'java.lang','ClassName':'Object','MethodeName':'getClass'},'Target':{'Namespace':'System','ClassName':'object','MethodeName':'GetType'}},
{'Source':{'Namespace':'java.lang','ClassName':'Object','MethodeName':'hashCode'},'Target':{'Namespace':'System','ClassName':'object','MethodeName':'GetHashCode'}},
{'Source':{'Namespace':'java.lang','ClassName':'Object'},'Target':{'Namespace':'System','ClassName':'object'}},
{'Source':{'Namespace':'java.lang','ClassName':'Boolean'},'Target':{'Namespace':'System','ClassName':'bool'}},
{'Source':{'Namespace':'java.lang','ClassName':'Integer'},'Target':{'Namespace':'System','ClassName':'int'}},
{'Source':{'Namespace':'java.lang','ClassName':'Void'},'Target':{'Namespace':'System','ClassName':'void'}},
{'Source':{'Namespace':'java.lang','ClassName':'True'},'Target':{'Namespace':'System','ClassName':'true'}},
{'Source':{'Namespace':'java.lang','ClassName':'False'},'Target':{'Namespace':'System','ClassName':'false'}},
{'Source':{'Namespace':'java.lang','ClassName':'Int64'},'Target':{'Namespace':'System','ClassName':'long'}},
{'Source':{'Namespace':'java.lang','ClassName':'String','MethodeName':'compareTo'},'Target':{'Namespace':'System','ClassName':'String','MethodeName':'CompareTo'}},
{'Source':{'Namespace':'java.lang','ClassName':'String'},'Target':{'Namespace':'System','ClassName':'String'}},
{'Source':{'Namespace':'java.lang','ClassName':'Collection'},'Target':{'Namespace':'System.Collections.ObjectModel','ClassName':'Collection'}},
{'Source':{'Namespace':'java.lang','ClassName':'Class','MethodeName':'getCanonicalName'},'Target':{'Namespace':'System','ClassName':'Type','MethodeName':'FullName','MethodeAsProperty':true}},
{'Source':{'Namespace':'java.lang','ClassName':'Class'},'Target':{'Namespace':'System','ClassName':'Type'}},
{'Source':{'Namespace':'java.lang','ClassName':'Comparable','MethodeName':'compareTo'},'Target':{'Namespace':'System','ClassName':'IComparable','MethodeName':'CompareTo'}},
{'Source':{'Namespace':'java.lang','ClassName':'Comparable'},'Target':{'Namespace':'System','ClassName':'IComparable'}},
]";
    }
}
