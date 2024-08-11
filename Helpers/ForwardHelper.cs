using Enricher.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Enricher.Helpers
{
    public class ForwardHelper
    {
        internal static object Forward(object requestBody)
        {
            var to = Environment.GetEnvironmentVariable("TO");
            using (WebClient client = new WebClient())
            {
                string jsonRequestBody = JsonSerializer.Serialize(requestBody);
                var jsonResponseBody = client.UploadString(to, jsonRequestBody);
                var responseBody = JsonSerializer.Deserialize<object>(jsonResponseBody);
                return responseBody;
            }
        }
    }
}
