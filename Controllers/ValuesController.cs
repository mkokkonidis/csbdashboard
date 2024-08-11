using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using System;

namespace CSBDashboardServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        const string baseUrl = $"https://test-retention.biomed.ntua.gr";
        
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
