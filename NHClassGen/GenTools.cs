using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHClassGen {
  static class GenTools {
    public static string GenerateClass(string tableName, List<Column> columns, bool generateEnumerator = false) {
      string className = upperFirst(tableName);
      string result = "";

      result =
        "[NHibernate.Mapping.Attributes.Class(Table = \"" + tableName + "\")]\r\n" +
        "public class " + className + " " + (generateEnumerator ? ": System.Collections.Generic.IEnumerable<string> " : "") + "{\r\n";

      // Private Fields
      result += "  #region Private Fields\r\n";
      foreach (Column c in columns) {
        result += "  private " + c.Type.Name + " _" + lowerFirst(c.Name) + ";\r\n";
      }
      result += "  #endregion\r\n";

      // Properties
      result += "  #region Properties\r\n";

      if (generateEnumerator) {
        result += "public virtual event EventHandler<" + className + "EventArgs> OnChanged;\r\n";
      }

      foreach (Column c in columns) {
        if (c.IsAutoIncrement && c.IsId) {
          result += "  [NHibernate.Mapping.Attributes.Id(0, Name = \"" + c.Name + "\")]\r\n";
          result += "  [NHibernate.Mapping.Attributes.Generator(1, Class = \"native\"]\r\n";
        } else if (c.IsId) {
          result += "  [NHibernate.Mapping.Attributes.Id(Name = \"" + c.Name + "\")]\r\n";
        } else if (c.IsAutoIncrement) {
          result += "  [NHibernate.Mapping.Attributes.Generator(Class = \"native\"]\r\n";
        } else {
          result += "  [NHibernate.Mapping.Attributes.Property(";
          if (!c.IsNullable) {
            result += "NotNull = true, ";
          }

          if (c.Type == typeof(decimal)) {
            result += "Precision = " + c.NumericPrecision + ", ";
            result += "Scale = " + c.NumericScale + ", ";
          }

          if (c.Type == typeof(string) && c.CharacterMaximumLength != 0 && c.CharacterMaximumLength != 2147483647) {
            result += "Length = " + c.CharacterMaximumLength + ", ";
          }

          if (result[result.Length - 2] == ',') {
            result = result.Substring(0, result.Length - 2); // Trim last ,
          }

          result += ")]\r\n";
        }
        result += "  public virtual " + c.Type.Name + " " + upperFirst(c.Name) + " {\r\n";
        result += "    get { return _" + lowerFirst(c.Name) + "; }\r\n";
        if (generateEnumerator) {
          result += "    set {\r\n";
          result += "      " + c.Type.Name + " oldValue = _" + lowerFirst(c.Name) + ";\r\n";
          result += "      _" + lowerFirst(c.Name) + " = value;\r\n";
          result += "      OnChangedAction(\"" + upperFirst(c.Name) + "\", oldValue, value);\r\n";
          result += "    }\r\n";
        } else {
          result += "    set { _" + lowerFirst(c.Name) + " = value; }\r\n";
        }
        result += "  }\r\n";
      }
      result += "  #endregion\r\n";

      if (generateEnumerator) {
        result += "  #region Methods\r\n";
        result += "  public virtual System.Collections.Generic.IEnumerator<string> GetEnumerator() {\r\n";
        foreach (Column c in columns) {
          if (typeof(string).IsAssignableFrom(c.Type)) {
            result += "    yield return " + upperFirst(c.Name) + "; \r\n";
          } else {
            result += "    yield return " + upperFirst(c.Name) + ".ToString(); \r\n";
          }
        }

        result += "  }\r\n";
        result += "  System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {\r\n";
        result += "    return this.GetEnumerator();\r\n";
        result += "  }\r\n";

        result += "  protected virtual void OnChangedAction(string FieldName, object oldValue, object newValue) {\r\n";
        result += "    EventHandler<" + className + "EventArgs> handler = OnChanged;\r\n";
        result += "    if (handler != null) {\r\n";
        result += "      " + className + "EventArgs eventArgs = new " + className + "EventArgs();\r\n";
        result += "      eventArgs.FieldName = FieldName;\r\n";
        result += "      eventArgs.OldValue = oldValue;\r\n;";
        result += "      eventArgs.NewValue = newValue;\r\n";
        result += "      handler(this, eventArgs);\r\n";
        result += "    }";
        result += "  }";

        result += "  #endregion\r\n";
      }
      // Constructor
      result += "  #region Constructors\r\n";
      result += "  public " + className + "() {}\r\n";
      result += "  public " + className + "(";
      foreach (Column c in columns) {
        result += c.Type.Name + " " + lowerFirst(c.Name) + ", ";
      }

      if (result[result.Length - 2] == ',') {
        result = result.Substring(0, result.Length - 2); // Trim last ,
      }

      result += ") {\r\n";
      foreach (Column c in columns) {
        result += "    this._" + lowerFirst(c.Name) + " = " + lowerFirst(c.Name) + ";\r\n";
      }
      result += "  }\r\n";

      result += "  #endregion\r\n";
      result += "}";

      if (generateEnumerator) {
        result += "public class " + className + "EventArgs: EventArgs {\r\n";
        result += "  public string FieldName { get; set; }\r\n";
        result += "  public object OldValue { get; set; }\r\n";
        result += "  public object NewValue { get; set; }\r\n";
        result += "}";
      }
      return result;
    }


    private static string upperFirst(string s) {
      return s[0].ToString().ToUpper() + s.Substring(1);
    }

    private static string lowerFirst(string s) {
      return s[0].ToString().ToLower() + s.Substring(1);
    }
  }
}
