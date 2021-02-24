using System;
using System.Reflection;

namespace Gavaghan.JSON
{
    /// <summary>
    /// A <code>JSONValueFactory</code> implementation that can read and write
    /// <code>JSONObject</code> subtypes.All types derived from
    /// <code>TypedJSONObject</code> include a type value of the concrete derived
    /// class name when converting to a String.When reading a String, the type value
    /// is used to instantiate the proper subtype.
    /// </summary>
    public class TypedJSONValueFactory : JSONValueFactory
    {
        /// <summary>
        /// The default implementation
        /// </summary>
        static public readonly TypedJSONValueFactory TYPED_DEFAULT = new TypedJSONValueFactory();

        /// <summary>
        /// Look for the 'type' value in a populated <code>JSONObject</code> and create a
        /// default instance of it.  If 'value' is not a <code>JSONObject</code> or if
        /// 'type' value is not found, defer to the super class.
        /// </summary>
        /// <param name="path">JSON path to the value we're reading</param>
        /// <param name="value">the value to possibly recast</param>
        /// <returns>the recast value or 'null' if no recast was required</returns>
        /// <exception cref="Gavaghan.JSON.JSONException" />
        protected override IJSONValue Recast(string path, IJSONValue value)
        {
            TypedJSONObject retval;

            // if it's not a JSONObject, we have nothing to do
            if (!(value is JSONObject)) return base.Recast(path, value);

            // look for a type value
            JSONObject valueObj = (JSONObject)value;
            valueObj.TryGetValue(TypedJSONObject.TYPE_KEY, out IJSONValue typeValue);

            // if no type found, defer to superclass
            if (typeValue == null) return base.Recast(path, value);

            // ensure typeValue is a string
            if (!(typeValue is JSONString))
            {
                throw new JSONException(path, String.Format("'type' value is a '{0}' but a JSONString was expected", typeValue.GetType().Name));
            }

            // load the new type
            String typeName = ((JSONString)typeValue).StringValue;
            Type type = Type.GetType(typeName);

            if (type == null) throw new JSONException(path, String.Format("Read a JSON object with type attribute '{0}' but that class could not be found", typeName));

            // ensure the class is an appropriate subtype
            if (!typeof(TypedJSONObject).IsAssignableFrom(type))
            {
                throw new JSONException(path, String.Format("Read an object of type '{0}' but that class is not assignable to 'TypedJSONObject'", typeName));
            }

            // instantiate a default instance
            try
            {
                ConstructorInfo ctx = type.GetConstructor(NO_PARAMS);
                if (ctx == null) throw new Exception("'" + type.Name + "' does not have a public default constructor");

                IJSONValue newJSON = (IJSONValue)ctx.Invoke(NO_ARGS);
                retval = (TypedJSONObject)newJSON;
            }
            catch (TargetInvocationException exc)
            {
                throw new Exception("Constructor for '" + type.Name + "' threw an exception", exc.InnerException);
            }

            return retval;
        }
    }
}
