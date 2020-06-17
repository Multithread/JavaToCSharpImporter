# JavaToCSharpImporter
An Importer for Java Code. It is Designed to Import .java Classes into C#. 
The Importer does not try to create a 100% Working copy of the Java class, but rather do a lot of the Work required to change the Code from Java to C#.

The main target is to port Lucene 7.6 or 8 to C#, since IKVM might be dead.

> The whole Importer is written with the help of ANTLR 4

**Current Work Items**

- Java: loading of if/for/foreach methode functions
- Java: loading of methode calls inside function code 
- Core: type cleaning inside of methode code
- C#: Writing of Function Headers
- C#: Writing of Class Field Definitions
- Convert <T extends RollingBuffer.Resettable> into C# where T:RollingBuffer.Resettable
- Even better Support for Generics in all the places
 

**Working Parts so far**

- Convert an empty Interface from Java to C#
- Convert an empty classes from Java to C#
- Loading Java Class definitions into IL Structure
- IL for Generic Classes and methods


**Current State of Conversion (2019.02.10)**

```Java
package org.apache.lucene.util;

import java.util.Collections;

public class Class1 {
private string Value;
public Class1 CreateInstance(){
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

    }
}
```
