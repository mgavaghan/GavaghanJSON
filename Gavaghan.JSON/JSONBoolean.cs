using System;
using System.IO;

namespace Gavaghan.JSON
{
  /// <summary>
  /// A JSON Boolean.
  /// </summary>
  public class JSONBoolean : AbstractJSONValue
  {
    /// <summary>
    /// The underlying value.
    /// </summary>
    private Boolean mValue;

    /// <summary>
    /// Create a new JSONBoolean.
    /// </summary>
    /// <param name="value"></param>
    public JSONBoolean(Boolean value)
    {
      mValue = value;
    }

    /// <summary>
    /// Create a new JSONBoolean.
    /// </summary>
    public JSONBoolean()
    {
      mValue = false;
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
    public virtual void SetValue(Boolean value)
    {
      mValue = value;
    }

    /// <summary>
    /// Create a prototype instance of the same type.
    /// </summary>
    public override IJSONValue CreatePrototype()
    {
      return new JSONBoolean();
    }

    /// <summary>
    /// Copy the value of another IJSONValue into our underlying value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public override void CopyValue(IJSONValue value)
    {
      if (!GetType().IsAssignableFrom(value.GetType())) throw new Exception("Can't assign a " + value.GetType().Name + " to a " + GetType().Name);

      mValue = (Boolean)value.Value;
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

      if (c == 't')
      {
        if (JSONValueFactory.Demand(pbr) != 'r') throw new JSONException(path, "Content does not appear to be a boolean.");
        if (JSONValueFactory.Demand(pbr) != 'u') throw new JSONException(path, "Content does not appear to be a boolean.");
        if (JSONValueFactory.Demand(pbr) != 'e') throw new JSONException(path, "Content does not appear to be a boolean.");
        mValue = true;
      }

      else if (c == 'f')
      {
        if (JSONValueFactory.Demand(pbr) != 'a') throw new JSONException(path, "Content does not appear to be a boolean.");
        if (JSONValueFactory.Demand(pbr) != 'l') throw new JSONException(path, "Content does not appear to be a boolean.");
        if (JSONValueFactory.Demand(pbr) != 's') throw new JSONException(path, "Content does not appear to be a boolean.");
        if (JSONValueFactory.Demand(pbr) != 'e') throw new JSONException(path, "Content does not appear to be a boolean.");
        mValue = false;
      }

      else throw new JSONException(path, "Content does not appear to be a boolean.");
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
      writer.Write(mValue.ToString().ToLower());
    }
  }
}
