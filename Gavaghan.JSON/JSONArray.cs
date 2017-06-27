using System;
using System.Collections.Generic;
using System.IO;

namespace Gavaghan.JSON
{
  /// <summary>
  ///  A JSON array represented as a IList&lt;JSONValue&gt;
  /// </summary>
  public class JSONArray : AbstractJSONValue
  {
    /// <summary>
    /// The underlying value.
    /// </summary>
    private IList<IJSONValue> mValue;

    /// <summary>
    /// JSONValueFactory for reading from a Reader.
    /// </summary>
    private JSONValueFactory mFactory;

    /// <summary>
    /// Create a new JSONArray.
    /// </summary>
    /// <param name="value"></param>
    public JSONArray(IList<IJSONValue> value)
    {
      if (value == null) throw new ArgumentNullException("Null value not allowed.  Use JSONNull instead.");
      mValue = value;
    }

    /// <summary>
    /// Create a new JSONArray.
    /// </summary>
    /// <param name="factory"></param>
    public JSONArray(JSONValueFactory factory)
    {
      mValue = new List<IJSONValue>();
      mFactory = factory;
    }

    /// <summary>
    /// Create a new JSONArray.
    /// </summary>
    public JSONArray()
    {
      mValue = new List<IJSONValue>();
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
    public virtual void SetValue(IList<IJSONValue> value)
    {
      if (value == null) throw new ArgumentNullException("Null value not allowed.  Use JSONNull instead.");
      mValue = value;
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
      if (c != '[') throw new JSONException(path, "Content does not appear to be an array.");

      // empty array is an easy out
      JSONValueFactory.SkipWhitespace(pbr);
      c = JSONValueFactory.Demand(pbr);
      if (c == ']') return;
      pbr.Unread(c);

      // loop through values
      try
      {
        for (; ; )
        {
          IJSONValue value = mFactory.Read(path, pbr);
          mValue.Add(value);

          // get next non-whitespace
          JSONValueFactory.SkipWhitespace(pbr);
          c = JSONValueFactory.Demand(pbr);

          // is end?
          if (c == ']') return;

          // is more
          if (c == ',')
          {
            JSONValueFactory.SkipWhitespace(pbr);
            continue;
          }

          throw new JSONException(path, "Incorrectly formatted array: " + c);
        }
      }
      finally
      {
        mFactory = null;
      }
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
      string newIndent = indent + "   ";

      if (mValue.Count == 0)
      {
        writer.Write("[]");
      }
      else
      {
        int count = 1;

        writer.Write("[");
        writer.Write(JSONObject.EOL);

        foreach (IJSONValue value in mValue)
        {
          writer.Write(newIndent);

          value.Write(newIndent, writer, pretty);

          if (count != mValue.Count) writer.Write(',');

          writer.Write(JSONObject.EOL);
          count++;
        }

        writer.Write(indent);
        writer.Write("]");
      }
    }
  }
}
