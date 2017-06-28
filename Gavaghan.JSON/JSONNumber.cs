using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Gavaghan.JSON
{
  /// <summary>
  /// A JSON number represented as a Decimal.
  /// </summary>
  public class JSONNumber : AbstractJSONValue
  {
    /// <summary>
    /// The underlying value.
    /// </summary>
    private Decimal mValue;

    /// <summary>
    /// Read the whole portion of a number.
    /// </summary>
    /// <param name="pbr"></param>
    /// <param name="builder"></param>
    /// <exception cref="System.IO.IOException"/>
    /// <exception cref="Gavaghan.JSON.JSONException"/>
    private void ReadWholePart(PushbackReader pbr, StringBuilder builder)
    {
      char c;
      for (; ; )
      {
        c = JSONValueFactory.Demand(pbr);
        if (Char.IsDigit(c))
        {
          builder.Append(c);
        }
        else
        {
          pbr.Unread(c);
          break;
        }
      }
    }

    /// <summary>
    /// Read the fractional part of the number.
    /// </summary>
    /// <param name="path">path to the value being read</param>
    /// <param name="pbr"></param>
    /// <param name="builder"></param>
    /// <exception cref="System.IO.IOException"/>
    /// <exception cref="Gavaghan.JSON.JSONException"/>
    private void ReadFractionalPart(string path, PushbackReader pbr, StringBuilder builder)
    {
      char c;
      c = JSONValueFactory.Demand(pbr);
      if (c == '.')
      {
        builder.Append(c);

        for (; ; )
        {
          c = JSONValueFactory.Demand(pbr);
          if (!Char.IsDigit(c))
          {
            if (builder.ToString().EndsWith(".")) throw new JSONException(path, "Digits expected after decimal points.");
            pbr.Unread(c);
            break;
          }

          builder.Append(c);
        }
      }
      else
      {
        pbr.Unread(c);
      }
    }

    /// <summary>
    /// Read the exponent.
    /// </summary>
    /// <param name="path">path to the value being read</param>
    /// <param name="pbr"></param>
    /// <param name="builder"></param>
    /// <exception cref="System.IO.IOException"/>
    /// <exception cref="Gavaghan.JSON.JSONException"/>
    private void ReadExponent(string path, PushbackReader pbr, StringBuilder builder)
    {
      char c;
      c = JSONValueFactory.Demand(pbr);
      if (c == 'e' || (c == 'E'))
      {
        builder.Append(c);

        c = JSONValueFactory.Demand(pbr);

        if (Char.IsDigit(c) || (c == '+') || (c == '-'))
        {
          builder.Append(c);

          for (; ; )
          {
            c = JSONValueFactory.Demand(pbr);
            if (!Char.IsDigit(c))
            {
              pbr.Unread(c);
              break;
            }

            builder.Append(c);
          }
        }
        else throw new JSONException(path, "Content does not appear to be a number");
      }
      else
      {
        pbr.Unread(c);
      }
    }
    
    /// <summary>
    /// Create a new JSONNumber.
    /// </summary>
    /// <param name="value"></param>
    public JSONNumber(Decimal value)
    {
      mValue = value;
    }

    /// <summary>
    /// Create a new JSONNumber.
    /// </summary>
    /// <param name="value"></param>
    public JSONNumber(long value)
    {
      mValue = new Decimal(value);
    }

    /// <summary>
    /// Create a new JSONNumber.
    /// </summary>
    /// <param name="value"></param>
    public JSONNumber(double value)
    {
      mValue = new Decimal(value);
    }

    /// <summary>
    /// Create a new JSONNumber.
    /// </summary>
    /// <param name="value"></param>
    public JSONNumber(string value)
    {
      mValue = Decimal.Parse(value);
    }

    /// <summary>
    /// Create a new JSONNumber.
    /// </summary>
    public JSONNumber()
    {
      mValue = Decimal.Zero;
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
    public virtual void SetValue(Decimal value)
    {
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
      StringBuilder builder = new StringBuilder();

      char c = JSONValueFactory.Demand(pbr);
      if (!Char.IsDigit(c) && (c != '-')) throw new JSONException(path, "Content does not appear to be a number.");

      builder.Append(c);

      // read the number
      if (c != '0') ReadWholePart(pbr, builder);
      ReadFractionalPart(path, pbr, builder);
      ReadExponent(path, pbr, builder);

      // parse and set value
      try
      {
        mValue = Decimal.Parse(builder.ToString(), NumberStyles.AllowExponent | NumberStyles.Float);
      }
      catch (FormatException)
      {
        throw new JSONException(path, "Illegal number format: " + builder.ToString());
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
      writer.Write(mValue.ToString());
    }
  }
}
