# JavaToCSharpImporter
An Importer for Java Code. It is Designed to Import .java Classes into C#. 
The Importer does not try to create a 100% Working copy of the Java class, but rather do a lot of the Work required to change the Code from Java to C#.

The main target is to port Lucene 7.6 or 8 to C#, since IKVM might be dead.

> The whole Importer is written with the help of ANTLR 4

**Current Work Items**

- Java: loading of if/for/foreach methode functions and do let the IL understand it
- Convert <T extends RollingBuffer.Resettable> into C# where T:RollingBuffer.Resettable
- IL: Understandment for missing classes methods to be written in a seperate file so that there will be no Compile errors on the C# output
 

**Working Parts so far**

- Java: Loading of Code files with ANTLR
- Java: Loading of Class definition information into IL
- Java: Loading of Methode header information into IL
- Java: loading of methode calls inside function code into IL
- IL for Generic Classes and methods
- IL: Rewriting Comments to match C# Comments
- IL: Renaming Methods to match C# Names
- IL: Renaming Method-params to match C# Parameternames 
- C#: Writing of Class headers
- C#: Writing of Function Headers
- C#: Writing of Class Field Definitions


**Current State of Conversion (2020.07.18)**

Conversion of the Lucene 7 Interface "Bits" into Valid C# code

```Java
/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
package org.apache.lucene.util;


/**
 * Interface for Bitset-like structures.
 * @lucene.experimental
 */

public interface  Bits {
  /** 
   * Returns the value of the bit with the specified <code>index</code>.
   * @param index index, should be non-negative and &lt; {@link #length()}.
   *        The result of passing negative or out of bounds values is undefined
   *        by this interface, <b>just don't do it!</b>
   * @return <code>true</code> if the bit is set, <code>false</code> otherwise.
   */
  boolean get(int index);
  
  /** Returns the number of bits in this set */
  int length();

  Bits[] EMPTY_ARRAY = new Bits[0];
  
  /**
   * Bits impl of the specified length with all bits set. 
   */
  class MatchAllBits implements Bits {
    final int len;
    
    public MatchAllBits(int len) {
      this.len = len;
    }

    @Override
    public boolean get(int index) {
      return true;
    }

    @Override
    public int length() {
      return len;
    }
  }

  /**
   * Bits impl of the specified length with no bits set. 
   */
  class MatchNoBits implements Bits {
    final int len;
    
    public MatchNoBits(int len) {
      this.len = len;
    }

    @Override
    public boolean get(int index) {
      return false;
    }

    @Override
    public int length() {
      return len;
    }
  }
}

```
gets converted to:

```C#


/// <summary>
/// Licensed to the Apache Software Foundation (ASF) under one or more
/// contributor license agreements.  See the NOTICE file distributed with
/// this work for additional information regarding copyright ownership.
/// The ASF licenses this file to You under the Apache License, Version 2.0
/// (the "License"); you may not use this file except in compliance with
/// the License.  You may obtain a copy of the License at
/// 
///     http://www.apache.org/licenses/LICENSE-2.0
/// 
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.
/// </summary>
namespace LuceNET.util
{
    /// <summary>
    /// Interface for Bitset-like structures.
    /// @lucene.experimental
    /// </summary>
    public interface Bits
    {
        //Bits[] EMPTY_ARRAY =  new Bits[0];

        /// <summary>
        /// Returns the value of the bit with the specified <code>index</code>.
        /// @param index index, should be non-negative and &lt; {@link #length()}.
        ///        The result of passing negative or out of bounds values is undefined
        ///        by this interface, <b>just don't do it!</b>
        /// @return <code>true</code> if the bit is set, <code>false</code> otherwise.
        /// </summary>
        bool Get(int inIndex)
;

        /// <summary>
        /// Returns the number of bits in this set
        /// </summary>
        int Length()
;
        /// <summary>
        /// Bits impl of the specified length with all bits set. 
        /// </summary>
        class MatchAllBits : Bits
        {
            readonly int len;

            public MatchAllBits(int inLen)
            {
                this.len = inLen;
            }

            public bool Get(int inIndex)
            {
                return true;
            }

            public int Length()
            {
                return len;
            }
        }

        /// <summary>
        /// Bits impl of the specified length with no bits set. 
        /// </summary>
        class MatchNoBits : Bits
        {
            readonly int len;

            public MatchNoBits(int inLen)
            {
                this.len = inLen;
            }

            public bool Get(int inIndex)
            {
                return false;
            }

            public int Length()
            {
                return len;
            }
        }
    }
}

```
