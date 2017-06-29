using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Gavaghan.JSON
{
  /// <summary>
  /// A JSON String.
  /// </summary>
  public class JSONString : AbstractJSONValue
  {
    /// <summary>
    /// The underlying value.
    /// </summary>
    private string mValue;

    /// <summary>
    /// Read a string value.
    /// </summary>
    /// <param name="path">path to the value being read</param>
    /// <param name="pbr"></param>
    /// <returns></returns>
    /// <exception cref="System.IO.IOOException"/>
    /// <exception cref="Gavaghan.JSON.JSNException"/>
    static public string ReadString(string path, PushbackReader pbr)
    {
      StringBuilder builder = new StringBuilder();

      char c = JSONValueFactory.Demand(pbr);
      if (c != '\"') throw new JSONException(path, "Leading quote expected at start of string.");

      for (; ; )
      {
        c = JSONValueFactory.Demand(pbr);

        // if closing quote
        if (c == '\"') break;

        // if escape
        if (c == '\\')
        {
          c = JSONValueFactory.Demand(pbr);

          switch (c)
          {
            case '\"':
            case '/':
            case '\\':
              builder.Append(c);
              break;
            case 'b':
              builder.Append('\b');
              break;
            case 'f':
              builder.Append('\f');
              break;
            case 'n':
              builder.Append('\n');
              break;
            case 'r':
              builder.Append('\r');
              break;
            case 't':
              builder.Append('\t');
              break;

            case 'u':
              StringBuilder hex = new StringBuilder();
              hex.Append(JSONValueFactory.Demand(pbr));
              hex.Append(JSONValueFactory.Demand(pbr));
              hex.Append(JSONValueFactory.Demand(pbr));
              hex.Append(JSONValueFactory.Demand(pbr));
              try
              {
                int uchar = Int32.Parse(hex.ToString(), NumberStyles.HexNumber);
                builder.Append((char)uchar);
              }
              catch (FormatException)
              {
                throw new JSONException(path, "Illegal unicode value: " + hex.ToString());
              }
              break;

            default:
              throw new JSONException(path, "Illegal escape value in string: " + c);
          }
        }
        else
        {
          builder.Append(c);
        }
      }

      return builder.ToString();
    }

    /// <summary>
    /// Create a new JSONString.
    /// </summary>
    /// <param name="value"></param>
    public JSONString(string value)
    {
      if (value == null) throw new ArgumentNullException("Null value not allowed.  Use JSONNull instead.");
      mValue = value;
    }

    /// <summary>
    /// Create a new JSONString.
    /// </summary>
    public JSONString()
    {
      mValue = "";
    }

    /// <summary>
    /// Get the underlying value (as a Decimal, a string, a Boolean, etc.)
    /// </summary>
    public override object Value
    {
      get { return mValue; }
    }
    /// <summary>
    /// Set the underlying value.
    /// </summary>
    /// <param name="value"></param>
    public virtual void SetValue(string value)
    {
      mValue = value;
    }

    /// <summary>
    /// Create a prototype instance of the same type.
    /// </summary>
    public override IJSONValue CreatePrototype()
    {
      return new JSONString();
    }

    /// <summary>
    /// Copy the value of another IJSONValue into our underlying value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public override void CopyValue(IJSONValue value)
    {
      if (!GetType().IsAssignableFrom(value.GetType())) throw new Exception("Can't assign a " + value.GetType().Name + " to a " + GetType().Name);

      mValue = (string)value.Value;
    }

    /// <summary>
    /// Read a JSON value (presumes the key has already been read) and set the
    /// underlying value. There's generally no reason to call this method
    /// directly. It is intended to be overridden by an extended type.
    /// </summary>
    /// <param name="path">path to the value being read</param>
    /// <param name="pbr">source reader</param>
    /// <exception cref="Gavaghan.JSON.JSONException">on grammar error</exception>
    /// <exception cref="System.IO.IOException">on read failure</exception>
    public override void Read(string path, PushbackReader pbr)
    {
      mValue = ReadString(path, pbr);
    }

    /// <summary>
    /// Render this JSON value to a Writer. There's generally no reason to call
    /// this method directly. It is intended to be overridden by an extended type.
    /// </summary>
    /// <param name="indent">indent padding</param>
    /// <param name="writer">target writer</param>
    /// <param name="pretty">'true' for pretty-print, 'false' for flat</param>
    /// <exception cref="System.IO.IOException">on read failure</exception>
    public override void Write(string indent, TextWriter writer, bool pretty)
    {
      StringBuilder builder = new StringBuilder();

      for (int i = 0; i < mValue.Length; i++)
      {
        char c = mValue[i];

        if (c == '\"') builder.Append("\\\"");
        else if (c == '\\') builder.Append("\\\\");
        else if ((c >= 32) && (c <= 126)) builder.Append(c);
        else if (c == '\b') builder.Append("\\b");
        else if (c == '\f') builder.Append("\\f");
        else if (c == '\n') builder.Append("\\n");
        else if (c == '\r') builder.Append("\\r");
        else if (c == '\t') builder.Append("\\t");
        else
        {
          string hex = ((int)c).ToString("X4").ToLower();
          hex = hex.Substring(hex.Length - 4);

          builder.Append("\\u");
          builder.Append(hex);
        }
      }

      writer.Write('\"');
      writer.Write(builder.ToString());
      writer.Write('\"');
    }
  }
}
