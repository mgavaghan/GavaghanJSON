using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;

namespace Gavaghan.JSON.Test
{
    public class NoGoodConstructor : IJSONValue
    {
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "API Method")]
        public NoGoodConstructor(int a) { }

        public object Value => throw new NotImplementedException();

        public void CopyValue(IJSONValue value)
        {
            throw new NotImplementedException();
        }

        public IJSONValue CreatePrototype()
        {
            throw new NotImplementedException();
        }

        public IJSONValue DeepCopy()
        {
            throw new NotImplementedException();
        }

        public void Read(string path, PushbackReader pbr)
        {
            throw new NotImplementedException();
        }

        public string ToFlatString()
        {
            throw new NotImplementedException();
        }

        public string ToPrettyString()
        {
            throw new NotImplementedException();
        }

        public void Write(string indent, TextWriter writer, bool pretty)
        {
            throw new NotImplementedException();
        }
    }

    [TestClass()]
    public class JSONValueFactoryTest
    {
        static private JSONObject GetTestObject()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream("Gavaghan.JSON.Test.JSONObjectTest.json"))
            using (StreamReader rdr = new StreamReader(stream))
            {
                return (JSONObject)JSONValueFactory.DEFAULT.Read(rdr);
            }
        }

        [TestMethod()]
        public void TestKnownGet()
        {
            JSONObject doc = GetTestObject();

            decimal number = (decimal)JSONValueFactory.GetOrSet(doc, "number", typeof(JSONNumber));
            Assert.AreEqual(123m, number);

            string str = (string)JSONValueFactory.GetOrSet(doc, "string", typeof(JSONString));
            Assert.AreEqual("Hello, World!", str);

            try
            {
                JSONValueFactory.GetOrSet(doc, "string", typeof(JSONNumber));
                Assert.Fail("Exception expected");
            }
            catch (Exception)
            {
                // expected
            }
        }

        [TestMethod()]
        public void TestUnknownGet()
        {
            JSONObject doc = GetTestObject();

            decimal number = (decimal)JSONValueFactory.GetOrSet(doc, "number2", typeof(JSONNumber));
            Assert.AreEqual(0m, number);

            try
            {
                JSONValueFactory.GetOrSet(doc, "number2", typeof(JSONString));
                Assert.Fail("Exception expected");
            }
            catch (Exception)
            {
                // expected
            }

            try
            {
                JSONValueFactory.GetOrSet(doc, "number3", typeof(NoGoodConstructor));
            }
            catch (Exception)
            {
                // expected
            }
        }
    }
}
