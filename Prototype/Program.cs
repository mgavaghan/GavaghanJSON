using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Gavaghan.JSON;

namespace Prototype
{
  class Program
  {
    static void Main(string[] args)
    {
      StringReader rdr = new StringReader("\"abc\u0058\"");
      PushbackReader pbr = new PushbackReader(rdr, 2);

      JSONString json = new JSONString("abc\u00F8");

      Console.WriteLine(json.ToString());

      Console.ReadLine();
    }
  }
}
