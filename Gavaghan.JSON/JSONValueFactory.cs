using System;
using System.IO;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;

namespace Gavaghan.JSON
{
    /// <summary>
    /// Factory for determining the proper<code> JSONValue</code> implementation
    /// based on the incoming stream.
    /// 
    /// Overriding this class allows for custom JSON data types as well as the
    /// redefinition of whitespace.
    /// </summary>
    public class JSONValueFactory
    {
        /// <summary>
        /// The default implementation
        /// </summary>
        static public readonly JSONValueFactory DEFAULT = new JSONValueFactory();

        /// <summary>
        /// GC safe empty parameters.
        /// </summary>
        static protected readonly Type[] NO_PARAMS = new Type[0];

        /// <summary>
        /// GC safe empty arguments.
        /// </summary>
        static protected readonly Object[] NO_ARGS = new Object[0];

        /// <summary>
        /// Determine if a character is whitespace.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        static protected bool IsWhitespace(int c)
        {
            return " \t\n\u000B\f\r\u001C\u001D\u001E\u001F\u000C\u000D\u000E".IndexOf((char)c) >= 0;
        }

        /// <summary>
        /// Skip to first non-whitespace character.
        /// </summary>
        /// <param name="pbr">a pushback reader</param>
        /// <exception cref="System.IO.IOException"/>
        public virtual void SkipWhitespace(PushbackReader pbr)
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
        protected virtual IJSONValue OnString(String path, PushbackReader pbr) => new JSONString();

        /// <summary>
        /// Callback when a number is encountered.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pbr"></param>
        /// <returns></returns>
        /// <exception cref="System.IO.IOException" />
        /// <exception cref="Gavaghan.JSON.JSONException" />
        protected virtual IJSONValue OnNumber(String path, PushbackReader pbr) => new JSONNumber();

        /// <summary>
        /// Callback when an array is encountered.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pbr"></param>
        /// <returns></returns>
        /// <exception cref="System.IO.IOException" />
        /// <exception cref="Gavaghan.JSON.JSONException" />
        protected virtual IJSONValue OnArray(String path, PushbackReader pbr) => new JSONArray(this);

        /// <summary>
        /// Callback when an object is encountered.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pbr"></param>
        /// <returns></returns>
        /// <exception cref="System.IO.IOException" />
        /// <exception cref="Gavaghan.JSON.JSONException" />
        protected virtual IJSONValue OnObject(String path, PushbackReader pbr) => new JSONObject(this);

        /// <summary>
        /// Callback when a boolean is encountered.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pbr"></param>
        /// <returns></returns>
        /// <exception cref="System.IO.IOException" />
        /// <exception cref="Gavaghan.JSON.JSONException" />
        protected virtual IJSONValue OnBoolean(String path, PushbackReader pbr) => new JSONBoolean();

        /// <summary>
        /// Callback when a null is encountered.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pbr"></param>
        /// <returns></returns>
        /// <exception cref="System.IO.IOException" />
        /// <exception cref="Gavaghan.JSON.JSONException" />
        protected virtual IJSONValue OnNull(String path, PushbackReader pbr) => JSONNull.INSTANCE;

        /// <summary>
        /// Callback for the start of an unknown type.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pbr"></param>
        /// <returns></returns>
        /// <exception cref="System.IO.IOException" />
        /// <exception cref="Gavaghan.JSON.JSONException" />
        protected virtual IJSONValue OnUnknown(String path, PushbackReader pbr, char c) => throw new JSONException(path, "Illegal start of JSON value: " + c);

        /// <summary>
        /// Give subtypes a chance to recast the loaded value as an <code>IJSONValue</code>
        /// subtype.  Default implementation returns 'null' because no recast is needed.
        ///
        /// Subtypes only need to return a default instance.  The <code>read()</code>
        /// method handles copying of data.
        /// </summary>
        /// <param name="path">JSON path to the value we're reading</param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="Gavaghan.JSON.JSONException" />
        protected virtual IJSONValue Recast(string path, IJSONValue value) => null;

        /// <summary>
        /// Create a new JSONValueFactory.
        /// </summary>
        public JSONValueFactory()
        {
        }

        /// <summary>
        /// Get a named value from a <code>JSONObject</code>. If the value
        /// doesn't exist, make a default instance and add it.
        /// </summary>
        /// <param name="jsonObj">the <code>JSONObject</code> to get a value from</param>
        /// <param name="name"></param>
        /// <param name="jsonType"></param>
        /// <returns></returns>
        static public object GetOrSet(JSONObject jsonObj, String name, Type jsonType)
        {
            object retval;

            // look to see if the value already exists
            jsonObj.TryGetValue(name, out IJSONValue jsonValue);

            // if it exists, it's easy - just return it after a type check
            if (jsonValue != null)
            {
                // make sure we got the right object type
                if (!jsonType.IsAssignableFrom(jsonValue.GetType()))
                {
                    throw new Exception(String.Format("Value named '{0}' is of type '{1}' which is not assignable from '{2}'", name, jsonValue.GetType().Name, jsonType.Name));
                }

                retval = jsonValue.Value;
            }

            // otherwise, create a default
            else
            {
                // ensure property types
                if (!typeof(IJSONValue).IsAssignableFrom(jsonType))
                {
                    throw new Exception(String.Format("Type '{0}' is not assignable from '{1}'", typeof(IJSONValue).Name, jsonType.Name));
                }

                try
                {
                    ConstructorInfo ctx = jsonType.GetConstructor(NO_PARAMS);
                    if (ctx == null) throw new Exception("'" + jsonType.Name + "' does not have a public default constructor");

                    IJSONValue newJSON = (IJSONValue)ctx.Invoke(NO_ARGS);

                    jsonObj.Add(name, newJSON);
                    retval = newJSON.Value;
                }
                catch (TargetInvocationException exc)
                {
                    throw new Exception("Constructor for '" + jsonType.Name + "' threw an exception", exc.InnerException);
                }
            }

            return retval;
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
        /// <param name="path">JSON path to the value we're reading</param>
        /// <param name="pbr">a pushback reader</param>
        /// <returns>the next JSON value</returns>
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

            // give subtype a chance to select a different implementation
            IJSONValue recast = Recast(path, value);

            // if value was recast, copy over original data
            if (recast != null)
            {
                recast.CopyValue(value);
                value = recast;
            }

            return value;
        }
    }
}
