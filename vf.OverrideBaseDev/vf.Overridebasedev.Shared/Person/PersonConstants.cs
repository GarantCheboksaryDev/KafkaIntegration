using System;
using Sungero.Core;

namespace vf.OverrideBaseDev.Constants.Parties
{
  public static class Person
  {

    /// <summary>
    /// Признаки полов в интеграции с 1С.
    /// </summary>
    public static class Genders
    {
      /// <summary>
      /// Признак мужского пола.
      /// </summary>
      [Public]
      public const string Male = "М";
      
      /// <summary>
      /// Признак женского пола.
      /// </summary>
      [Public]
      public const string Female = "Ж";
    }
    
    [Public]
    public const string PersonReferenceName = "Person";
  }
}