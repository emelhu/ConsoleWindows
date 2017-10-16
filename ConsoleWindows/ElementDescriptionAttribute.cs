using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eMeL.ConsoleWindows
{
  //[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  //public class ElementDescriptionAttribute : Attribute
  //{
  //  public readonly ElementDescriptionInfo description;

  //  public ElementDescriptionAttribute(bool editable, bool useViewmodel)
  //  {
  //    description.editable      = editable;
  //    description.useViewmodel  = useViewmodel;
  //  }
  //}

  //public struct ElementDescriptionInfo
  //{
  //  public bool editable;
  //  public bool useViewmodel;
  //}
}

/*
var a = GetDescription(typeof(TextElement));
var b = GetDescription(typeof(LabelElement));

static ElementDescriptionInfo? GetDescription(Type type)
{ 
  object[] attrs = type.GetCustomAttributes(typeof(ElementDescriptionAttribute), true);
  if (attrs.Length == 0)
      return null;
  else
      return ((ElementDescriptionAttribute)attrs[0]).description;
}
*/