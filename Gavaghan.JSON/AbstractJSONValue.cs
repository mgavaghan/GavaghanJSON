using System.IO;

namespace Gavaghan.JSON
{
  /// <summary>
  /// Base implementation of an IJSONValue.
  /// </summary>
  public abstract class AbstractJSONValue : IJSONValue
  {
    /// <summary>
    /// Render a JSONValue as a string.
    /// </summary>
    /// <param name="value">the IJSONValue to render</param>
    /// <param name="pretty">'true' to pretty-print with line feeds and indentation, 'false'
	  ///       to render on a single line.</param>
    /// <returns>the rendered value</returns>
    static public string ToString(IJSONValue value, bool pretty)
    {
      string str;

      using (StringWriter writer = new StringWriter())
      {
        value.Write("", writer, pretty);
        str = writer.ToString();
      }

      return str;
    }

    /// <summary>
    /// Get the underlying value (as a Decimal, a string, a Boolean, etc.)
    /// </summary>
    public abstract object Value { get; }

    /// <summary>
    /// Read a JSON value (presumes the key has already been read) and set the
    /// underlying value. There's generally no reason to call this method
    /// directly. It is intended to be overridden by an extended type.
    /// </summary>
    /// <param name="path">path to the value being read</param>
    /// <param name="pbr">source reader</param>
    /// <exception cref="Gavaghan.JSON.JSONException">on grammar error</exception>
    /// <exception cref="System.IO.IOException">on read failure</exception>
    public abstract void Read(string path, PushbackReader pbr);

    /// <summary>
    /// Render this JSON value to a Writer. There's generally no reason to call
    /// this method directly. It is intended to be overridden by an extended type.
    /// </summary>
    /// <param name="indent">indent padding</param>
    /// <param name="writer">target writer</param>
    /// <param name="pretty">'true' for pretty-print, 'false' for flat</param>
    /// <exception cref="System.IO.IOException">on read failure</exception>
    public abstract void Write(string indent, TextWriter writer, bool pretty);

    /// <summary>
    /// Render this object as a pretty-printed string.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return ToPrettyString();
    }

    /// <summary>
    /// Render this object as a pretty-printed string.
    /// </summary>
    /// <returns></returns>
    public string ToPrettyString()
    {
      return ToString(this, true);
    }

    /// <summary>
    /// Render this object as a flattened string.
    /// </summary>
    /// <returns></returns>
    public string ToFlatString()
    {
      return ToString(this, false);
    }
  }
}
