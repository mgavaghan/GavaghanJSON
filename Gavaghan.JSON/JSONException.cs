using System;

namespace Gavaghan.JSON
{
    /// <summary>
    /// Exception thrown when a document does not conform to the JSON grammar.
    /// </summary>
    public class JSONException : Exception
    {
        /// <summary>
        /// Create a new JSONException.
        /// </summary>
        /// <param name="path">path to the value being read</param>
        /// <param name="message">a description of the exception</param>
        public JSONException(string path, string message)
          : base(path + ": " + message)
        {
            Path = path;
        }

        /// <summary>
        /// Create a new JSONException with a specified cause.
        /// </summary>
        /// <param name="path">path to the value being read</param>
        /// <param name="message">a description of the exception</param>
        /// <param name="cause">cause of the failure</param>
        public JSONException(string path, string message, Exception cause)
          : base(path + ": " + message, cause)
        {
            Path = path;
        }

        /// <summary>
        /// Get path to the offending content.
        /// </summary>
        public string Path { get; private set; }
    }
}
