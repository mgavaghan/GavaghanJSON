using System;

namespace Gavaghan.JSON
{
    /// <summary>
    /// Superclass of classes that embed their concrete type class in the json
    /// output.This enables reconstituting the proper type when reading from
    /// serialized form.
    /// </summary>
    public class TypedJSONObject : JSONObject
    {
        /// <summary>
        /// The key to the type value of a <code>TypedJSONObject</code>.
        /// </summary>
        static public readonly String TYPE_KEY = "jsonObjectSubclass";

        /// <summary>
        /// Only allow instantiation by subtypes.
        /// </summary>
        protected TypedJSONObject()
        {
            Add(TYPE_KEY, new JSONString(GetType().AssemblyQualifiedName));
        }

        /// <summary>
        /// Get the type name of this instance.
        /// </summary>
        public string Type 
        { 
            get
            { 
                if (!TryGetValue(TYPE_KEY, out IJSONValue type))
                {
                    throw new Exception("Type information is missing");
                }

                if (!(type is JSONString))
                {
                    throw new Exception(String.Format("Type value of '{0}' is not an instance of a 'JSONString'", type.GetType().Name));
                }

                return ((JSONString)type).StringValue;
            }
        }
    }
}
