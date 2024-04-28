using System;
using DukeOfThieves.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;


namespace DukeOfThieves.Data
{
  public static class DataExtensions
  {
    public static float SqrMagnitudeTo(this Vector3 from, Vector3 to)
    {
      return Vector3.SqrMagnitude(to - from);
    }

    public static string ToJson(this object obj)
    {
      try
      {
        var savedString = JsonConvert.SerializeObject(obj, Formatting.None, GlobalVariables.SerializerSettings);
        return savedString;
      }
      catch (Exception e)
      {
        Debug.LogError($"Failed to serialize object to JSON: {e.Message}");
        return null; 
      }
    }

    public static T ToDeserialized<T>(this string json)
    {
        var convertedData = JsonConvert.DeserializeObject<T>(json, GlobalVariables.SerializerSettings);
        return convertedData;
    }
  }
}