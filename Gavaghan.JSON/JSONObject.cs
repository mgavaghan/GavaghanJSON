using System;
using System.Collections.Generic;
using System.IO;

namespace Gavaghan.JSON
{
    /// <summary>
    /// A JSON object as defined by http://www.json.org/
    /// NOTE do we need an answer to LinkedHashMap?
    /// </summary>
    public class JSONObject : Dictionary<string, IJSONValue>, IJSONValue
    {
        /// <summary>
        /// Windows EOL sequence.
        /// </summary>
        static public readonly string EOL = "\r\n";

        /// <summary>
        /// JSONValueFactory for reading from a Text.Reader
        /// </summary>
        private JSONValueFactory mFactory;

        /// <summary>
        /// Create a new JSONObject
        /// </summary>
        /// <param name="factory">the factory implementation used to read values in the object</param>
        public JSONObject(JSONValueFactory factory)
        {
            mFactory = factory;
        }

        /// <summary>
        /// Create a new JSONObject.
        /// </summary>
        public JSONObject()
        {
        }

        /// <summary>
        /// Get the underlying value (as a Decimal, a string, a Boolean, etc.)
        /// </summary>
        public virtual object Value
        {
            get { return this; }
        }

        /// <summary>
        /// Get the underlying value as a <code>JSONObject</code>.
        /// </summary>
        public JSONObject ObjectValue
        {
            get { return this; }
        }

        /// <summary>
        /// Create a prototype instance of the same type.
        /// </summary>
        public virtual IJSONValue CreatePrototype()
        {
            return new JSONObject();
        }

        /// <summary>
        /// Copy the value of another IJSONValue into our underlying value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual void CopyValue(IJSONValue value)
        {
            if (!GetType().IsAssignableFrom(value.GetType())) throw new Exception("Can't assign a " + value.GetType().Name + " to a " + GetType().Name);

            JSONObject source = (JSONObject)value.Value;

            foreach (string key in source.Keys)
            {
                Add(key, source[key]);
            }
        }

        /// <summary>
        /// Create a deep copy of this instance.
        /// </summary>
        public IJSONValue DeepCopy()
        {
            IJSONValue copy = CreatePrototype();
            copy.CopyValue(this);
            return copy;
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
        public virtual void Read(string path, PushbackReader pbr)
        {
            // assert we have an opening brace
            char c = JSONValueFactory.Demand(pbr);
            if (c != '{') throw new JSONException(path, "Failed to find '{' at start of JSON object.");

            for (; ; )
            {
                string key;

                // next is either a key or a closing brace
                JSONValueFactory.SkipWhitespace(pbr);
                c = JSONValueFactory.Demand(pbr);

                // is it a string?
                if (c == '\"')
                {
                    pbr.Unread(c);
                    key = JSONString.ReadString(path, pbr);
                }
                // is it a closing brace?
                else if (c == '}')
                {
                    break;
                }
                // else, it's poorly formed
                else
                {
                    throw new JSONException(path, "JSON object is not grammatically correct.  Unexpected: " + c);
                }

                // next ought to be a colon
                JSONValueFactory.SkipWhitespace(pbr);
                c = JSONValueFactory.Demand(pbr);
                if (c != ':') throw new JSONException(path + "." + key, "Expected ':' after key value");
                JSONValueFactory.SkipWhitespace(pbr);

                // next, read a JSONValue
                IJSONValue value = mFactory.Read(path + "." + key, pbr);

                // add it to the map
                Add(key, value);

                // next must be comma or close
                JSONValueFactory.SkipWhitespace(pbr);
                c = JSONValueFactory.Demand(pbr);

                if (c == ',') continue;
                if (c == '}') break;

                throw new JSONException(path, "JSON object is not grammatically correct.  Unexpected: " + c);
            }

            mFactory = null;
        }

        /// <summary>
        /// Render this JSON value to a Writer. There's generally no reason to call
        /// this method directly. It is intended to be overridden by an extended type.
        /// </summary>
        /// <param name="indent">indent padding</param>
        /// <param name="writer">target writer</param>
        /// <param name="pretty">'true' for pretty-print, 'false' for flat</param>
        /// <exception cref="System.IO.IOException">on read failure</exception>
        public virtual void Write(string indent, TextWriter writer, bool pretty)
        {
            string newIndent = indent + "   ";

            if (Count == 0)
            {
                writer.Write("{}");
            }
            else
            {
                writer.Write('{');

                int count = 1;

                if (pretty) writer.Write(EOL);

                foreach (string key in Keys)
                {
                    if (pretty) writer.Write(newIndent);
                    writer.Write('\"');
                    writer.Write(key);
                    writer.Write("\":");
                    if (pretty) writer.Write(" ");

                    TryGetValue(key, out IJSONValue value);

                    value.Write(newIndent, writer, pretty);

                    if (count != Count) writer.Write(',');

                    if (pretty) writer.Write(EOL);
                    count++;
                }

                writer.Write(indent);
                writer.Write('}');
            }
        }

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
            return AbstractJSONValue.ToString(this, true);
        }

        /// <summary>
        /// Render this object as a flattened string.
        /// </summary>
        /// <returns></returns>
        public string ToFlatString()
        {
            return AbstractJSONValue.ToString(this, false);
        }
    }
}
