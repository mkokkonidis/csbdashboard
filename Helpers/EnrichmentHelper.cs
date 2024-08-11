using Enricher.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Enricher.Helpers
{
    public class EnrichmentHelper
    {
        internal static void EnrichInSitu(Dictionary<string, object> requestBody)
        {
            var reqList = requestBody["EnrichmentRequests"];
            if (reqList == null) return;

            var enrichmentRequests = JsonSerializer.Deserialize<EnrichmentRequest[]>(JsonSerializer.Serialize(reqList));


            foreach (var req in enrichmentRequests)
            {
                var url = req.Adapter.StartsWith("$")?
                            Environment.GetEnvironmentVariable(req.Adapter.Substring(1)) :
                            req.Adapter;

                try
                {
                    using (WebClient client = new WebClient())
                    {
                        client.Headers.Add("Accept", "application/json");
                        client.Headers.Add("Content-Type", "application/json");
                        client.Headers.Add("Authorization", "Bearer eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJNM3ZEaGYwcjRhamFsQmlaQVRPUjA5dmlpTmdGZlRqbjh5NVBrcTZFczVFIn0.eyJleHAiOjE3MjMzNjQwMjEsImlhdCI6MTcyMzM2MzcyMSwiYXV0aF90aW1lIjoxNzIzMzYzNzIwLCJqdGkiOiIxMmU5ODJhNi0yMzFmLTQ4ZDMtODk4Yy03MWNkZWIwNDUwOWMiLCJpc3MiOiJodHRwczovL3Rlc3QtcmV0ZW50aW9uLmJpb21lZC5udHVhLmdyL2F1dGgvcmVhbG1zL3JldGVudGlvbiIsImF1ZCI6ImFjY291bnQiLCJzdWIiOiJmZjBiZTliOS1hNDhhLTQ3MTItODBkYi05YmRkZWE1MzViZmUiLCJ0eXAiOiJCZWFyZXIiLCJhenAiOiJkYXNoYm9hcmQiLCJub25jZSI6IjA2Zjg4OTI0LWE1MTgtNGIzMS05ZTNmLTUxYjc2NWRlOWIzYSIsInNlc3Npb25fc3RhdGUiOiIwMDQ3ZjI1My0zYTljLTQyZTMtOTM0Ni02ODZjN2Q5ZmZkYTIiLCJhY3IiOiIxIiwiYWxsb3dlZC1vcmlnaW5zIjpbImh0dHBzOi8vdGVzdC1yZXRlbnRpb24uYmlvbWVkLm50dWEuZ3IiLCJodHRwOi8vbG9jYWxob3N0OjQyMDAiXSwicmVhbG1fYWNjZXNzIjp7InJvbGVzIjpbImNsaW5pY2FsX2Nhc2VfbWFuYWdlciIsImNzYl9hZG1pbmlzdHJhdG9yIiwib2ZmbGluZV9hY2Nlc3MiLCJ1bWFfYXV0aG9yaXphdGlvbiIsImRlZmF1bHQtcm9sZXMtcmV0ZW50aW9uIl19LCJyZXNvdXJjZV9hY2Nlc3MiOnsiYWNjb3VudCI6eyJyb2xlcyI6WyJtYW5hZ2UtYWNjb3VudCIsIm1hbmFnZS1hY2NvdW50LWxpbmtzIiwidmlldy1wcm9maWxlIl19fSwic2NvcGUiOiJvcGVuaWQgcHJvZmlsZSBvcmdhbmlzYXRpb24gZW1haWwiLCJzaWQiOiIwMDQ3ZjI1My0zYTljLTQyZTMtOTM0Ni02ODZjN2Q5ZmZkYTIiLCJlbWFpbF92ZXJpZmllZCI6dHJ1ZSwibmFtZSI6IkRldiBBY2NvdW50Iiwib3JnYW5pc2F0aW9uIjoiMTciLCJwcmVmZXJyZWRfdXNlcm5hbWUiOiJkZXYiLCJnaXZlbl9uYW1lIjoiRGV2IiwiZmFtaWx5X25hbWUiOiJBY2NvdW50IiwiZW1haWwiOiJkZXZAdGVzdC5jb20ifQ.Mdt6-50DDA3pPLcUvaSDerE-eOkPM7EI_cPaN7KzFLhf1Cc0nXvf1ZoCHVYYetN0dvHUOyeQ5OHMsyEeFmxwIet27gAPyQTNoUaBo9rat9NarSzR_9f7f6eHFV2S5L-ut1dIsX0coHyJ78joDvgGpa9llBrDDdNVDgCkUxchqaHycZr8fBPJh7CYg56eruC82HBcWMCGUZSLgxsKal3SwlvcyxekQE641Ce4hPldLjS2B8y-WD4uu_ohgsGC-tNW28u-OkqnEC4cAr0-RsZo0RDa6iiP8rp5sLTnJZRAan765bJOLiEAV8M9q03BbIfUjZh1eftuLhLwrDz--o7y7Q");
                        string jsonRequestBody = JsonSerializer.Serialize(req.RequestData);
                        var jsonResponseBody = client.UploadString(url, jsonRequestBody);
                        var responseBody = JsonSerializer.Deserialize<object>(jsonResponseBody);
//                        requestBody.AdditionalData.Add(responseBody);
                    }
                }
                catch (Exception e)
                {
  //                  requestBody.AdditionalData.Add(new {  Error = "Failed to get JSON data from " + url + ":" + e, RequestData = req.RequestData });
                }
            }

        }

        internal static void EnrichInSitu(RequestBody requestBody)
        {
            var enrichmentRequests = requestBody.EnrichmentRequests;
            

            foreach (var req in enrichmentRequests)
            {
                var url = req.Adapter.StartsWith("$") ?
                            Environment.GetEnvironmentVariable(req.Adapter.Substring(1)) :
                            req.Adapter;

                try
                {
                    using (WebClient client = new WebClient())
                    {

                        string jsonRequestBody = JsonSerializer.Serialize(req.RequestData);
                        var jsonResponseBody = client.UploadString(url, jsonRequestBody);
                        var responseBody = JsonSerializer.Deserialize<object>(jsonResponseBody);
                        requestBody.AdditionalData.Add(responseBody);
                    }
                }
                catch (Exception e)
                {
                    requestBody.AdditionalData.Add(new {  Error = "Failed to get JSON data from " + url + ":" + e, RequestData = req.RequestData });
                }
            }

        }


    }
}
