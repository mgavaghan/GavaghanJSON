using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Gavaghan.JSON.Test
{
    /// <summary>
    /// This is a test class for JSONNumberTest and is intended
    /// to contain all JSONNumberTest Unit Tests
    /// </summary>
    [TestClass()]
    public class JSONNumberTest
    {
        [TestMethod()]
        public void TestZero()
        {
            using (StringReader rdr = new StringReader("0 "))
            using (PushbackReader pbr = new PushbackReader(rdr, 1))
            {
                JSONNumber number = new JSONNumber();
                number.Read("$", pbr);

                Assert.AreEqual("0", number.Value.ToString());
            }
        }

        [TestMethod()]
        public void TestWhole()
        {
            using (StringReader rdr = new StringReader("123 "))
            using (PushbackReader pbr = new PushbackReader(rdr, 1))
            {
                JSONNumber number = new JSONNumber();
                number.Read("$", pbr);

                Assert.AreEqual("123", number.Value.ToString());
            }
        }

        [TestMethod()]
        public void TestJustNegative()
        {
            using (StringReader rdr = new StringReader("- "))
            using (PushbackReader pbr = new PushbackReader(rdr, 1))
            {
                try
                {
                    JSONNumber number = new JSONNumber();
                    number.Read("$", pbr);

                    Assert.Fail("Exception expected");
                }
                catch (JSONException)
                {
                }
            }
        }

        [TestMethod()]
        public void TestNegativeWhole()
        {
            using (StringReader rdr = new StringReader("-123 "))
            using (PushbackReader pbr = new PushbackReader(rdr, 1))
            {
                JSONNumber number = new JSONNumber();
                number.Read("$", pbr);

                Assert.AreEqual("-123", number.Value.ToString());
            }
        }

        [TestMethod()]
        public void TestNoDecimal()
        {
            using (StringReader rdr = new StringReader("123. "))
            using (PushbackReader pbr = new PushbackReader(rdr, 1))
            {
                try
                {
                    JSONNumber number = new JSONNumber();
                    number.Read("$", pbr);

                    Assert.Fail("Exception expected");
                }
                catch (JSONException)
                {
                }
            }
        }

        [TestMethod()]
        public void TestDecimal()
        {
            using (StringReader rdr = new StringReader("-123.456 "))
            using (PushbackReader pbr = new PushbackReader(rdr, 1))
            {
                JSONNumber number = new JSONNumber();
                number.Read("$", pbr);

                Assert.AreEqual("-123.456", number.Value.ToString());
            }
        }

        [TestMethod()]
        public void TestExponent()
        {
            using (StringReader rdr = new StringReader("123.456E+2 "))
            using (PushbackReader pbr = new PushbackReader(rdr, 1))
            {
                JSONNumber number = new JSONNumber();
                number.Read("$", pbr);

                Assert.AreEqual("12345.6", number.Value.ToString());
            }

            using (StringReader rdr = new StringReader("123.456E-2 "))
            using (PushbackReader pbr = new PushbackReader(rdr, 1))
            {
                JSONNumber number = new JSONNumber();
                number.Read("$", pbr);

                Assert.AreEqual("1.23456", number.Value.ToString());
            }

            using (StringReader rdr = new StringReader("123.456E2 "))
            using (PushbackReader pbr = new PushbackReader(rdr, 1))
            {
                JSONNumber number = new JSONNumber();
                number.Read("$", pbr);

                Assert.AreEqual("12345.6", number.Value.ToString());
            }

            using (StringReader rdr = new StringReader("123e2 "))
            using (PushbackReader pbr = new PushbackReader(rdr, 1))
            {
                JSONNumber number = new JSONNumber();
                number.Read("$", pbr);

                Assert.AreEqual("12300", number.Value.ToString());
            }
        }

        [TestMethod()]
        public void TestExponentFail()
        {
            using (StringReader rdr = new StringReader("123E "))
            using (PushbackReader pbr = new PushbackReader(rdr, 1))
            {
                try
                {
                    JSONNumber number = new JSONNumber();
                    number.Read("$", pbr);

                    Assert.Fail("Exception expected");
                }
                catch (JSONException)
                {
                }
            }
            using (StringReader rdr = new StringReader("123E+ "))
            using (PushbackReader pbr = new PushbackReader(rdr, 1))
            {
                try
                {
                    JSONNumber number = new JSONNumber();
                    number.Read("$", pbr);

                    Assert.Fail("Exception expected");
                }
                catch (JSONException)
                {
                }
            }
        }
    }
}
