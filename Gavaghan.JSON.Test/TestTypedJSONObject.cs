using System;

namespace Gavaghan.JSON.Test
{
    public class TestTypedJSONObject : TypedJSONObject
    {
        public void SetString(String myString)
        {
            Add("string", new JSONString(myString));
        }

        public String GetString()
        {
            return ((String)JSONValueFactory.GetOrSet(this, "string", typeof(JSONString)));
        }

        public void SetDecimal(decimal myBigDecimal)
        {
            Add("decimal", new JSONNumber(myBigDecimal));
        }

        public decimal GetDecimal()
        {
            return ((decimal)JSONValueFactory.GetOrSet(this, "decimal", typeof(decimal)));
        }
    }
}
