using System;
using System.IO;

namespace ZorroDB.Example
{
  class TestDto
  {
    public string Name;

    public string name { set; get; }

    public override string ToString()
    {
      return $"{Name} : {name}";
    }
  }

  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("ZorroDB Testing");

      var db = new DBEngine(Path.Combine("D:\\", "testdb"));

      var dto = new TestDto()
      {
        Name = "Getting started",
        name = "ororo"
      };

      db.Set(dto);

      var item = db.Get<TestDto>();

      Console.WriteLine("Rezult:");

      Console.WriteLine(item);

      Console.ReadKey();
    }
  }
}
