using System.IO;

namespace Gavaghan.JSON
{
  /// <summary>
  /// A JSON null.
  /// </summary>
  public class JSONNull : AbstractJSONValue
  {
    /// <summary>
    /// Get the underlying value (as a Decimal, a string, a Boolean, etc.)
    /// </summary>
    public override object Value
    {
      get { return null; }
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
      char c = JSONValueFactory.Demand(pbr);

      if (c == 'n')
      {
        if (JSONValueFactory.Demand(pbr) != 'u') throw new JSONException(path, "Content does not appear to be a null.");
        if (JSONValueFactory.Demand(pbr) != 'l') throw new JSONException(path, "Content does not appear to be a null.");
        if (JSONValueFactory.Demand(pbr) != 'l') throw new JSONException(path, "Content does not appear to be a null.");
      }

      else throw new JSONException(path, "Content does not appear to be a null.");
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
      writer.Write("null");
    }
  }
}
