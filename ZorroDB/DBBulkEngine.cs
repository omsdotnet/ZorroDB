using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ZorroDB
{
  internal class DBBulkEngine : IDBBulkContract
  {
    private readonly string basePath;
    private readonly IDBContract singleEngine;

    public DBBulkEngine(string basePath, IDBContract singleEngine)
    {
      this.basePath = basePath;
      this.singleEngine = singleEngine;
    }

    public void Reset()
    {
      var indexes = Directory.EnumerateDirectories(basePath);

      foreach (string index in indexes)
      {
        Directory.Delete(index, true);
      }
    }

    public Dictionary<Guid, T> Get<T>()
    {
      var comeType = typeof(T);
      var typePath = DBHelper.GetTypePath(basePath, comeType);

      var ret = new Dictionary<Guid, T>();

      var indexes = Directory.EnumerateFiles(typePath).Select(Path.GetFileName);

      foreach (string index in indexes)
      {
        var id = Guid.Parse(index);

        T entity = singleEngine.Get<T>(id);

        if (entity != null)
        {
          ret.Add(id, entity);
        }
      }

      return ret;
    }
  }
}
