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
