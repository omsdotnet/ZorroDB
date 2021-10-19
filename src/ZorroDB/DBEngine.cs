using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ZorroDB
{
  public class DBEngine : IDBContract
  {
    private readonly string basePath;

    public IDBBulkContract Bulk { get; }

    public DBEngine(string basePath)
    {
      this.basePath = basePath;

      Bulk = new DBBulkEngine(basePath, this);
    }

    public void Reset<T>(Guid id = new Guid())
    {
      var comeType = typeof(T);

      var typeName = DBHelper.Base64Encode(comeType.FullName);
      var typePath = Path.Combine(basePath, typeName);

      File.Delete(Path.Combine(typePath, id.ToString()));

      if (Directory.EnumerateFiles(typePath).Any())
      {
        var typeDataPath = Path.Combine(typePath, "C");
        var searchPattern = id.ToString().Replace("-", string.Empty) + "*";
        var values = Directory.EnumerateFiles(typeDataPath, searchPattern, SearchOption.AllDirectories);

        foreach (string value in values)
        {
          File.Delete(value);
        }
      }
      else
      {
        Directory.Delete(typePath, true);
      }
    }

    public Guid? Set<T>(T entity, Guid id = new Guid())
    {
      if (entity == null)
      {
        Reset<T>(id);
        return null;
      }

      var comeType = typeof(T);

      var typeName = DBHelper.Base64Encode(comeType.FullName);
      var typePath = Path.Combine(basePath, typeName);
      var typeDataPath = Path.Combine(typePath, "C");

      Directory.CreateDirectory(typePath);
      Directory.CreateDirectory(typeDataPath);

      var entityId = id.ToString();
      var entityIdKey = entityId.Replace("-", string.Empty);

      var knownType = typeof(string);

      var fieldsInfo = comeType.GetFields();
      foreach (FieldInfo fieldItem in fieldsInfo)
      {
        if (fieldItem.FieldType == knownType)
        {
          var name = DBHelper.Base64Encode(fieldItem.Name);
          var propertyDataPath = Path.Combine(typeDataPath, name);
          Directory.CreateDirectory(propertyDataPath);

          var value = DBHelper.Base64Encode(fieldItem.GetValue(entity).ToString());
          File.Create(Path.Combine(propertyDataPath, entityIdKey + value));
        }
      }

      var propertiesInfo = comeType.GetProperties();
      foreach (PropertyInfo propertyItem in propertiesInfo)
      {
        if (propertyItem.PropertyType == knownType)
        {
          var name = DBHelper.Base64Encode(propertyItem.Name);
          var propertyDataPath = Path.Combine(typeDataPath, name);
          Directory.CreateDirectory(propertyDataPath);

          var value = DBHelper.Base64Encode(propertyItem.GetValue(entity).ToString());
          File.Create(Path.Combine(propertyDataPath, entityIdKey + value));
        }
      }

      File.Create(Path.Combine(typePath, entityId));

      return id;
    }

    public T Get<T>(Guid id = new Guid())
    {
      var comeType = typeof(T);
      var typePath = DBHelper.GetTypePath(basePath, comeType);
      var typeDataPath = Path.Combine(typePath, "C");

      var entity = (T)Activator.CreateInstance(comeType);

      var searchPattern = id.ToString().Replace("-", string.Empty) + "*";

      var values = Directory.EnumerateFiles(typeDataPath, searchPattern, SearchOption.AllDirectories);

      var entityExists = false;

      if (values.Any())
      {
        foreach (string value in values)
        {
          string[] segments = value.Split(Path.DirectorySeparatorChar);
          int segmentsCount = segments.Length;

          string paramName = DBHelper.Base64Decode(segments[segmentsCount - 2]);
          string paramValue = DBHelper.Base64Decode(segments[segmentsCount - 1].Substring(32));

          FieldInfo fieldInfo = comeType.GetField(paramName);
          if (fieldInfo != null)
          {
            fieldInfo.SetValue(entity, paramValue);
            entityExists = true;
          }
          else
          {
            PropertyInfo propertyInfo = comeType.GetProperty(paramName);
            if (propertyInfo != null)
            {
              propertyInfo.SetValue(entity, paramValue);
              entityExists = true;
            }
          }
        }
      }

      if (entityExists)
      {
        return entity;
      }

      return default(T);
    }
  }
}
