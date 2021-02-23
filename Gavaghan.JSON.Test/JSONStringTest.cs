using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Gavaghan.JSON.Test
{
    /// <summary>
    /// Summary description for JSONStringTest
    /// </summary>
    [TestClass]
    public class JSONStringTest
    {
        [TestMethod]
        public void TestString()
        {
            using (StringReader rdr = new StringReader("\"ABC\\n\\\"DE\\u123DF\" "))
            using (PushbackReader pbr = new PushbackReader(rdr, 1))
            {
                JSONString str = new JSONString();
                str.Read("$", pbr);

                Assert.AreEqual("ABC\n\"DE\u123DF", str.Value.ToString());
            }
        }

        [TestMethod]
        public void TestLinefeedOutput()
        {
            using (StringReader rdr = new StringReader("\"\\n\" "))
            using (PushbackReader pbr = new PushbackReader(rdr, 1))
            using (StringWriter wrt = new StringWriter())
            {
                JSONString str = new JSONString();
                str.Read("$", pbr);

                str.Write("", wrt, true);

                string output = wrt.ToString();

                Assert.AreEqual("\"\\n\"", output);
            }
        }

        [TestMethod]
        public void TestQuoteOutput()
        {
            using (StringReader rdr = new StringReader("\"\\\"\" "))
            using (PushbackReader pbr = new PushbackReader(rdr, 1))
            using (StringWriter wrt = new StringWriter())
            {
                JSONString str = new JSONString();
                str.Read("$", pbr);

                Assert.AreEqual("\"", str.Value.ToString());

                str.Write("", wrt, true);

                String output = wrt.ToString();

                Assert.AreEqual("\"\\\"\"", output);
            }
        }

        [TestMethod]
        public void TestStringOutput()
        {
            using (StringReader rdr = new StringReader("\"ABC\\b\\n\\\"DE\\u123DF\\\\\" "))
            using (PushbackReader pbr = new PushbackReader(rdr, 1))
            using (StringWriter wrt = new StringWriter())
            {
                JSONString str = new JSONString();
                str.Read("$", pbr);

                str.Write("", wrt, true);

                String output = wrt.ToString();

                Assert.AreEqual("\"ABC\\b\\n\\\"DE\\u123dF\\\\\"", output);
            }
        }
    }
}
