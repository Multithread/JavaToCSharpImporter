# JavaToCSharpImporter
An Importer for Java Code. It is Designed to Import .java Classes into C#. 
The Importer does not try to create a 100% Working copy of the Java class, but rather do a lot of the Work required to change the Code from Java to C#.

The main target is to port Lucene 7.6 or 8 to C#, since IKVM might be dead.

> The whole Importer is written with the help of ANTLR 4

**Current Work Items**

- Java: loading of if/for/foreach methode functions
- Java: loading of methode calls inside function code 
- Core: type cleaning inside of methode code
- C#: Writing of Class Field Definitions
- Convert <T extends RollingBuffer.Resettable> into C# where T:RollingBuffer.Resettable
- Even better Support for Generics in all the places
 

**Working Parts so far**

- Java: Loading of Code files with ANTLR
- Java: Loading of Class definition information into IL
- Java: Loading of Methode header information into IL
- IL for Generic Classes and methods
- C#: Writing of Function Headers
- C#: Writing of Class headers


**Current State of Conversion (2019.02.10)**

```Java
package org.apache.lucene.util;

import java.util.Collections;

public class Class1 {
private string Value;
public Class1<int> CreateInstance(){
return null;
}
public void SetValue(string inValue){
Value=inValue;
}
}}
```
gets converted to:

```C#
using java.util.Collections;

namespace org.apache.lucene.util
{
    public class Class1
    {
        private string Value;

        public Class1<int> CreateInstance()
        {
        }

        public void SetValue(string inValue)
        {
        }
    }
}
```
