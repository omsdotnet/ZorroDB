using System;
using System.Collections.Generic;

namespace ZorroDB
{
  public interface IDBBulkContract
  {
    Dictionary<Guid, T> Get<T>();

    void Reset();
  }
}
