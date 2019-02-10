# JavaToCSharpImporter
An Importer for Java Code. It is Designed to Import .java Classes into C#. 
The Importer does not try to create a 100% Working copy of the Java class, but rather do a lot of the Work required to change the Code from Java to C#.

The main target is to port Lucene 7.6 or 8 to C#, since IKVM might be dead.

> The whole Importer is written with the help of ANTLR 4

**Current Work Parts**

- Support for Gerrics in Function Return-Type
- Support if/for/foreach to be checked correctly
- Convert <T extends RollingBuffer.Resettable> into C# where T:RollingBuffer.Resettable
- A lot of other stuff
- Parent Object Type Function Recognition.


**Working Parts so far**

- Convert an Interface from Java to C#
- Convert simple classes from Java to C#
- Support for Generics in Class-Fields
- Support for Gerrics in Function Propertiesn 
- Recognition for Array Type Definitions


**Current State of COnversion**

```java
/**
 * Licence Comment
 */
package org.apache.lucene.util;

import java.util.Collections;

/**
 * Description of Class
 */
public final class GenericMethode<C, D extends C> {

  private final Class<C> baseClass;
  private final Class<?>[] parameters;
  //string from Constructor
  private final String method;
  
  /**
  * Constructor information
  */
  public GenericMethode(string method){
	this.method = method;
  }
  
  /** Generic Testmethode
  */
  int methode1(final Class<? extends D> subclazz, Class<C> dataClazz) {
    if (!baseClass.isAssignableFrom(subclazz))
      throw new IllegalArgumentException(subclazz.getName() + " is not a subclass of " + baseClass.getName());
    boolean overridden = false;
    int distance = 0;
	
      // increment distance if overridden
      if (overridden) distance++;
    return distance;
  }
  
  /** Normal Testmethode
  */
  public boolean methodCompare(String method) {
    return this.method==method;
 }}

```

```C#
/// <summary>
/// Licence Comment
/// </summary>
using LuceNET.Util;
using System;
namespace LuceNET.Util
{
    /// <summary>
    /// Description of Class
    /// </summary>
    public sealed class GenericMethode<C, D>
    {
        private readonly C baseClass;
        private readonly object[] parameters;
        private readonly string method;
        public  GenericMethode(string inMethod)   
        {
            thisthis._method = inMethod;            
        }
        Int32 Methode1<OtherType>(OtherType inSubclazz, C inDataClazz)where OtherType : D    
        {
            if (!_baseClass.isAssignableFrom(inSubclazz))throw new IllegalArgumentException(subclazz.getName() + " is not a subclass of " + baseClass.getName());
            bool overridden = false;
            Int32 distance = 0;
            if (overridden)distance ++;
            return distance;            
        }
        public bool MethodCompare(string inMethod)   
        {
            return thisthis._method == _method;            
        }
    }
}

```
