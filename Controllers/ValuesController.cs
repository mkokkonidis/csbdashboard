using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using System;

namespace CSBDashboardServer.Controllers
{
    [Route("dapi/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        const string baseUrl = $"https://test-retention.biomed.ntua.gr";
        //const string auth = "Bearer eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJNM3ZEaGYwcjRhamFsQmlaQVRPUjA5dmlpTmdGZlRqbjh5NVBrcTZFczVFIn0.eyJleHAiOjE3MjMzNzE0MDIsImlhdCI6MTcyMzM3MTEwMiwiYXV0aF90aW1lIjoxNzIzMzY4Nzc0LCJqdGkiOiI1ZDc5OTQxZi1iMjU0LTQ0YjgtYTk4NC1kMjBkMDQyYTg2NjgiLCJpc3MiOiJodHRwczovL3Rlc3QtcmV0ZW50aW9uLmJpb21lZC5udHVhLmdyL2F1dGgvcmVhbG1zL3JldGVudGlvbiIsImF1ZCI6ImFjY291bnQiLCJzdWIiOiJmZjBiZTliOS1hNDhhLTQ3MTItODBkYi05YmRkZWE1MzViZmUiLCJ0eXAiOiJCZWFyZXIiLCJhenAiOiJkYXNoYm9hcmQiLCJub25jZSI6IjM5Y2ExMjY0LTliZDItNGVhMy05Y2ExLWFkYWEwZmRkNGUxYiIsInNlc3Npb25fc3RhdGUiOiI2MmI2ZDg3YS1hZmJjLTQ1NTQtOGUwZC00NTk5MzliNjZlZDYiLCJhY3IiOiIwIiwiYWxsb3dlZC1vcmlnaW5zIjpbImh0dHBzOi8vdGVzdC1yZXRlbnRpb24uYmlvbWVkLm50dWEuZ3IiLCJodHRwOi8vbG9jYWxob3N0OjQyMDAiXSwicmVhbG1fYWNjZXNzIjp7InJvbGVzIjpbImNsaW5pY2FsX2Nhc2VfbWFuYWdlciIsImNzYl9hZG1pbmlzdHJhdG9yIiwib2ZmbGluZV9hY2Nlc3MiLCJ1bWFfYXV0aG9yaXphdGlvbiIsImRlZmF1bHQtcm9sZXMtcmV0ZW50aW9uIl19LCJyZXNvdXJjZV9hY2Nlc3MiOnsiYWNjb3VudCI6eyJyb2xlcyI6WyJtYW5hZ2UtYWNjb3VudCIsIm1hbmFnZS1hY2NvdW50LWxpbmtzIiwidmlldy1wcm9maWxlIl19fSwic2NvcGUiOiJvcGVuaWQgcHJvZmlsZSBvcmdhbmlzYXRpb24gZW1haWwiLCJzaWQiOiI2MmI2ZDg3YS1hZmJjLTQ1NTQtOGUwZC00NTk5MzliNjZlZDYiLCJlbWFpbF92ZXJpZmllZCI6dHJ1ZSwibmFtZSI6IkRldiBBY2NvdW50Iiwib3JnYW5pc2F0aW9uIjoiMTciLCJwcmVmZXJyZWRfdXNlcm5hbWUiOiJkZXYiLCJnaXZlbl9uYW1lIjoiRGV2IiwiZmFtaWx5X25hbWUiOiJBY2NvdW50IiwiZW1haWwiOiJkZXZAdGVzdC5jb20ifQ.CYEkBhYsL2zon_qE2epJxmtUGeB655KnnTXB4u8NWZBnIeh0vr2zJdM55CJyfl2lZgTckML14vso8Gnb_2s_PMisnrUDyxGsE2UGEG0rWZi5Dps_A3RwVKKYZqOefLfQ4y7P6Fl5XJW-OmWBhbKcIEzgIDSHxckIqnpxUCbaDC6YHmez5pKwsVl82uVfbaQO68j2SKiHBILt-xoF6ygox3omOMfm7vP_PdInRvW_cpAgaWQdv_r1p05GaDaCUNKQE0NaqWwhzs5gPhDDWPjMGgMBCVTll7cwA8g5EsrnTm5uFhFVuvmMsihkxCVkVYbEahyWWm8ad3q59mDfUn1qtw";
        
        static dynamic CompactObservations(string auth, int patient,string spec)
        {
            string url = $"{baseUrl}/api/fhir/Observation?patient=P{patient}&{spec}";
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("Accept", "application/json");
                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("Authorization", auth);
                var jsonResponseBody = client.DownloadData(url);
                var responseBody = (System.Text.Json.JsonElement)JsonSerializer.Deserialize<dynamic>(jsonResponseBody);
                var list = responseBody.GetProperty("entry").EnumerateArray();
                var ret = new List<decimal[]>();

                foreach (var item in list)
                {
                    var resource = item.GetProperty("resource");
                    var effectiveDateTime = resource.GetProperty("effectiveDateTime").GetString();
                    var value = resource.GetProperty("valueQuantity").GetProperty("value").GetDecimal();
                    ret.Add(new decimal[] { Convert.ToDateTime(effectiveDateTime).Ticks, value });
                }
                return ret;
            }
        }


        // GET api/<ValuesController1>/5
        [HttpGet("{id}")]
        public object Get(int id)
        {
            var auth = Request.Headers.Authorization.ToString();            
            var ret = new { 
                lightsleep = CompactObservations(auth, id, "code=762636008&category=29373008")
            };
            return ret;
        }


    }
}
