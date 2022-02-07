using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System;
using System.IO;

namespace PetsHome.Business.Extensions
{
    public static class TempDataExtensions
    {
        public static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : class
        {
            var properties = value.GetType().GetProperties();
            foreach (var item in properties)
            {
                if (item.PropertyType == typeof(IFormFile) || item.PropertyType == typeof(Stream))
                {
                    item.SetValue(value, null);
                }
            }

            tempData[key] = JsonConvert.SerializeObject(value);
        }

        public static T Get<T>(this ITempDataDictionary tempData, string keyValue) where T : class
        {
            tempData.TryGetValue(keyValue, out object objeto);

            if (objeto == null)
            {
                return null;
            }
            else
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>((string)objeto);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}
