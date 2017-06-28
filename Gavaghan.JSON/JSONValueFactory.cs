using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gavaghan.JSON
{
  /// <summary>
  /// Factory for determining the proper IJSONValue implementation based on the
  /// incoming stream.
  /// </summary>
  public class JSONValueFactory
  {
    /// <summary>
    /// The default implementation
    /// </summary>
    static public readonly JSONValueFactory DEFAULT = new JSONValueFactory();

    /// <summary>
    /// Determine if a character is whitespace.
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    static public bool IsWhitespace(int c)
    {
      return " \t\n\u000B\f\r\u001C\u001D\u001E\u001F\u000C\u000D\u000E".IndexOf((char)c) >= 0;
    }

    /// <summary>
    /// Skip to first non-whitespace character.
    /// </summary>
    /// <param name="pbr">a pushback reader</param>
    /// <exception cref="System.IO.IOException"/>
    static public void SkipWhitespace(PushbackReader pbr)
    {
      for (; ; )
      {
        int c = pbr.Read();

        if (c < 0) break; // bail on EOF

        // if non-whitespace found, push it back and exit
        if (!IsWhitespace(c))
        {
          pbr.Unread(c);
          break;
        }
      }
    }

    /// <summary>
    /// Demand a character and throw a JSONException if EOF.
    /// </summary>
    /// <param name="rdr">a reader</param>
    /// <returns></returns>
    /// <exception cref="System.IO.IOException"/>
    /// <exception cref="Gavaghan.JSON.JSONException"/>
    static public char Demand(TextReader rdr)
    {
      int c = rdr.Read();
      if (c < 0) throw new JSONException("$", "Out of data while reading JSON object.");
      return (char)c;
    }

    /// <summary>
    /// Callback when a string is encountered.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="pbr"></param>
    /// <returns></returns>
    /// <exception cref="System.IO.IOException" />
    /// <exception cref="Gavaghan.JSON.JSONException" />
    protected IJSONValue OnString(String path, PushbackReader pbr)
    {
      return new JSONString();
    }

    /// <summary>
    /// Callback when a number is encountered.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="pbr"></param>
    /// <returns></returns>
    /// <exception cref="System.IO.IOException" />
    /// <exception cref="Gavaghan.JSON.JSONException" />
    protected IJSONValue OnNumber(String path, PushbackReader pbr)
    {
      return new JSONNumber();
    }

    /// <summary>
    /// Callback when an array is encountered.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="pbr"></param>
    /// <returns></returns>
    /// <exception cref="System.IO.IOException" />
    /// <exception cref="Gavaghan.JSON.JSONException" />
    protected IJSONValue OnArray(String path, PushbackReader pbr)
    {
      return new JSONArray(this);
    }

    /// <summary>
    /// Callback when an object is encountered.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="pbr"></param>
    /// <returns></returns>
    /// <exception cref="System.IO.IOException" />
    /// <exception cref="Gavaghan.JSON.JSONException" />
    protected IJSONValue OnObject(String path, PushbackReader pbr)
    {
      return new JSONObject(this);
    }

    /// <summary>
    /// Callback when a boolean is encountered.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="pbr"></param>
    /// <returns></returns>
    /// <exception cref="System.IO.IOException" />
    /// <exception cref="Gavaghan.JSON.JSONException" />
    protected IJSONValue OnBoolean(String path, PushbackReader pbr)
    {
      return new JSONBoolean();
    }

    /// <summary>
    /// Callback when a null is encountered.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="pbr"></param>
    /// <returns></returns>
    /// <exception cref="System.IO.IOException" />
    /// <exception cref="Gavaghan.JSON.JSONException" />
    protected IJSONValue OnNull(String path, PushbackReader pbr)
    {
      return new JSONNull();
    }

    /// <summary>
    /// Callback for the start of an unknown type.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="pbr"></param>
    /// <returns></returns>
    /// <exception cref="System.IO.IOException" />
    /// <exception cref="Gavaghan.JSON.JSONException" />
    protected IJSONValue OnUnknown(String path, PushbackReader pbr, char c)
    {
      throw new JSONException(path, "Illegal start of JSON value: " + c);
    }

    /// <summary>
    /// Create a new JSONValueFactory.
    /// </summary>
    public JSONValueFactory()
    {
    }

    /// <summary>
    /// Get the minimum size of the pushback buffer.
    /// </summary>
    /// <returns></returns>
    public virtual int PushbackBufferSize
    {
      get { return 1; }
    }

    /// <summary>
    /// Read the JSON value that comes after the whitespace (if any).
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    /// <exception cref="System.IO.IOException" />
    /// <exception cref="Gavaghan.JSON.JSONException" />
    public IJSONValue Read(TextReader reader)
    {
      PushbackReader pbr = new PushbackReader(reader, PushbackBufferSize);

      // look for start of value
      SkipWhitespace(pbr);
      int c = pbr.Read();

      // bail out early if EOF
      if (c < 0) return null;

      pbr.Unread(c);

      return Read("$", pbr);
    }

    /// <summary>
    /// Read a JSON value.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="pbr"></param>
    /// <returns></returns>
    /// <exception cref="System.IO.IOException" />
    /// <exception cref="Gavaghan.JSON.JSONException" />
    public IJSONValue Read(string path, PushbackReader pbr)
    {
      IJSONValue value;
      char c = Demand(pbr);

      // is it a string?
      if (c == '\"')
      {
        value = OnString(path, pbr);
      }
      // is it a number?
      else if (Char.IsDigit(c) || (c == '-'))
      {
        value = OnNumber(path, pbr);
      }
      // is it an array?
      else if (c == '[')
      {
        value = OnArray(path, pbr);
      }
      // is it an object?
      else if (c == '{')
      {
        value = OnObject(path, pbr);
      }
      // is it a boolean?
      else if ((c == 't') || (c == 'f'))
      {
        value = OnBoolean(path, pbr);
      }
      // is it a null?
      else if (c == 'n')
      {
        value = OnNull(path, pbr);
      }
      // else, value type
      else
      {
        value = OnUnknown(path, pbr, c);
      }

      // unread trigger character
      pbr.Unread(c);

      // implementation specific read
      value.Read(path, pbr);

      return value;
    }
  }
}
