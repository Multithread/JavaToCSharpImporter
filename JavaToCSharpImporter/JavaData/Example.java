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
  }
  
  
  
}
