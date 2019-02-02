# JavaToCSharpImporter
An Importer for Java Code. It is Designed to Import .java Classes into C#. 
The Importer does not try to create a 100% Working copy of the Java class, but rather do a lot of the Work required to change the Code from Java to C#.

The main target is to port Lucene 7.6 or 8 to C#, since IKVM might be dead.

> The whole Importer is not written on a AST Compiler, since there was none, except for the generic ANTLR.


**Current Work Parts**

- Support for Generics in Class-Fields
- Support for Gerrics in Function Properties and Function Return-Type
- Support if/for/foreach to be checked correctly
- Split and Check Data inside ().
- Convert <T extends RollingBuffer.Resettable> into C# where T:RollingBuffer.Resettable
- Recognition for Array Type Definitions
- A lot of other stuff


**Working Parts so far**

- Convert an Interface from Java to C#
- Convert simple classes from Java to C#
