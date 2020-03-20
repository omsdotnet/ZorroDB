using System;
using System.IO;
using System.Text;

namespace ZorroDB
{
  internal static class DBHelper
  {
    public static string GetTypePath(string basePath, Type typeValue)
    {
      var typeName = Base64Encode(typeValue.FullName);
      var typePath = Path.Combine(basePath, typeName);
      return typePath;
    }

    public static string Base64Encode(string plainText)
    {
      var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
      return Convert.ToBase64String(plainTextBytes, Base64FormattingOptions.None).Replace('/', '_');
    }

    public static string Base64Decode(string base64EncodedData)
    {
      var base64EncodedBytes = Convert.FromBase64String(base64EncodedData.Replace('_', '/'));
      return Encoding.UTF8.GetString(base64EncodedBytes);
    }
  }
}
