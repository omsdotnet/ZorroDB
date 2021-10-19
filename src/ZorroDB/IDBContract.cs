using System;

namespace ZorroDB
{
  public interface IDBContract
  {
    Guid? Set<T>(T entity, Guid id = new Guid());

    T Get<T>(Guid id = new Guid());

    void Reset<T>(Guid id = new Guid());

    IDBBulkContract Bulk { get; }
  }
}
