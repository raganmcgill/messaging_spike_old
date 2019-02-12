using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace monitor.service.models
{
    internal static class TableComparer
    {
        public static bool HasSchemaChanged(string sourceJsonString, string targetJsonString)
        {
            var sourceJObject = JsonConvert.DeserializeObject<JObject>(sourceJsonString);
            var targetJObject = JsonConvert.DeserializeObject<JObject>(targetJsonString);

            if (!JToken.DeepEquals(sourceJObject, targetJObject))
            {
                foreach (KeyValuePair<string, JToken> sourceProperty in sourceJObject)
                {
                    var targetProp = targetJObject.Property(sourceProperty.Key);

                    if (!JToken.DeepEquals(sourceProperty.Value, targetProp.Value))
                    {
//                        Console.WriteLine($"{sourceProperty.Key} property value is changed");
                    }
//                    else
//                    {
//                        Console.WriteLine($"{sourceProperty.Key} property value didn't change");
//                    }
                }

                return true;
            }
            else
            {
//                Console.WriteLine("Objects are same");
                return false;
            }
        }
    }
}