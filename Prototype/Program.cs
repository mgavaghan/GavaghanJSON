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
      StringReader rdr = new StringReader("ABCDEFGHIJ");
      PushbackReader pbr = new PushbackReader(rdr, 2);
      char[] buf = new char[3];
      int got;

      Console.WriteLine(pbr.ReadLine());

      Console.ReadLine();
    }
  }
}
