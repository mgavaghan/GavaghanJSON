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
      return "\t\n\u000B\f\r\u001C\u001D\u001E\u001F\u000C\u000D\u000E".IndexOf((char)c) >= 0;
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
  }
}
