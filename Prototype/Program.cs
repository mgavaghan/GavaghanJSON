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
      JSONObject json = new JSONObject();

      json.Add("special", new JSONBoolean(true));
      json.Add("number", new JSONNull());
      json.Add("color", new JSONString("red"));

      Console.WriteLine(json.ToString());

      Console.ReadLine();
    }
  }
}
