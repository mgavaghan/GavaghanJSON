using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Gavaghan.JSON.Test
{
    /// <summary>
    /// Summary description for JSNObjectTest
    /// </summary>
    [TestClass]
    public class JSONObjectTest
    {
        [TestMethod]
        public void TestRead()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream("Gavaghan.JSON.Test.JSONObjectTest.json"))
            using (StreamReader rdr = new StreamReader(stream))
            {
                //string line = rdr.ReadToEnd();
                //throw new Exception(line + "*");

                JSONObject json = (JSONObject)JSONValueFactory.DEFAULT.Read(rdr);

                Assert.AreEqual(Decimal.Parse("123"), json["number"].Value);
                Assert.AreEqual("Hello, World!", json["string"].Value);
                Assert.AreEqual("", json["emptystring"].Value);
                Assert.AreEqual(Boolean.Parse("true"), json["true"].Value);
                Assert.AreEqual(Boolean.Parse("false"), json["false"].Value);
                Assert.IsNull(json["null"].Value);

                IList<IJSONValue> empty = (List<IJSONValue>)json["emptyarray"].Value;
                Assert.AreEqual(0, empty.Count);

                IList<IJSONValue> array = (List<IJSONValue>)json["array"].Value;
                Assert.AreEqual(3, array.Count);

                JSONObject obj = (JSONObject)json["object"].Value;
                Assert.AreEqual("red", obj["color"].Value);
            }
        }
    }
}
