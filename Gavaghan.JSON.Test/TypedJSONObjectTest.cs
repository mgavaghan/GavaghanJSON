using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Gavaghan.JSON.Test
{
    [TestClass]
    public class TypedJSONObjectTest
    {
        [TestMethod]
        public void TestSerDeser()
        {
            TestTypedJSONObject json = new TestTypedJSONObject();
            JSONValueFactory jsonFactory = new TypedJSONValueFactory();

            // populated the object
            json.SetString("Hello, World!");
            json.SetDecimal(123m);

            // serialize the object
            String asString = json.ToPrettyString();

            // try to deserialize
            TestTypedJSONObject json2;

            using (StringReader rdr = new StringReader(asString))
            {
                json2 = (TestTypedJSONObject)jsonFactory.Read(rdr);
            }

            Assert.AreEqual(json.GetString(), json2.GetString());
        }
    }
}
