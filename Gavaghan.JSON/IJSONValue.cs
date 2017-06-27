using System;
using System.IO;

namespace Gavaghan.JSON
{
  /// <summary>
  /// Interface to all JSON types.
  /// </summary>
  public interface IJSONValue
  {
    /// <summary>
    /// Get the underlying value (as a Decimal, a string, a Boolean, etc.)
    /// </summary>
    object Value { get; }

    /// <summary>
    /// Read a JSON value (presumes the key has already been read) and set the
    /// underlying value. There's generally no reason to call this method
    /// directly. It is intended to be overridden by an extended type.
    /// </summary>
    /// <param name="path">path to the value being read</param>
    /// <param name="pbr">source reader</param>
    /// <exception cref="Gavaghan.JSON.JSONException">on grammar error</exception>
    /// <exception cref="System.IO.IOException">on read failure</exception>
    void Read(string path, PushbackReader pbr);

    /// <summary>
    /// Render this JSON value to a Writer. There's generally no reason to call
    /// this method directly. It is intended to be overridden by an extended type.
    /// </summary>
    /// <param name="indent">indent padding</param>
    /// <param name="writer">target writer</param>
    /// <param name="pretty">'true' for pretty-print, 'false' for flat</param>
    /// <exception cref="System.IO.IOException">on read failure</exception>
    void Write(string indent, TextWriter writer, bool pretty);

    /// <summary>
    /// Render this object as a pretty-printed string.
    /// </summary>
    /// <returns></returns>
    string ToPrettyString();

    /// <summary>
    /// Render this object as a flattened string.
    /// </summary>
    /// <returns></returns>
    string ToFlatString();
  }
}
