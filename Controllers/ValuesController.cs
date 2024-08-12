using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using System;
using Microsoft.AspNetCore.Http.Extensions;

namespace CSBDashboardServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        //const string baseUrl = $"https://test-retention.biomed.ntua.gr";
        
        static dynamic CompactObservations(string auth, string baseUrl, int patient,string spec)
        {
            var retList = new List<decimal[]>();
            var infoList = new List<string>();

            string url = $"{baseUrl}/api/fhir/Observation?patient=P{patient}&{spec}";
            infoList.Add($"Info: Will try to obtain results from {url}");
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("Accept", "application/json");
                    client.Headers.Add("Content-Type", "application/json");
                    client.Headers.Add("Authorization", auth);
                    var jsonResponseBody = client.DownloadData(url);
                    var responseBody = (System.Text.Json.JsonElement)JsonSerializer.Deserialize<dynamic>(jsonResponseBody);
                    infoList.Add($"Info: deserialised");
                    var list = responseBody.GetProperty("entry").EnumerateArray();
                    infoList.Add($"Info: entry array found");
                    foreach (var item in list)
                    {
                        var resource = item.GetProperty("resource");
                        var effectiveDateTime = resource.GetProperty("effectiveDateTime").GetString();
                        var value = resource.GetProperty("valueQuantity").GetProperty("value").GetDecimal();
                        retList.Add(new decimal[] { Convert.ToDateTime(effectiveDateTime).Ticks, value });
                    }
                    return retList;
                }
            } catch (Exception ex) {
                infoList.Add($"Info: {ex.Message}");
            }
            return new {
                results = retList,
                debug = infoList
                };
        }


        // GET /<ValuesController>/5
        [HttpGet("{id}")]
        public object Get(int id)
        {
            var auth = 
                Request.Headers.Authorization.ToString();            
            var baseUrlParts = 
                Request.GetEncodedUrl().ToLower()
                    .Split(new string[] { "://"}, StringSplitOptions.None);
            var baseUrl =
                baseUrlParts[0]+
                "://" +
                baseUrlParts[1].Split('/')[0];

            var ret = new { 
                lightsleep = CompactObservations(auth, baseUrl, id, "code=762636008&category=29373008")
            };
            return ret;
        }


    }
}
