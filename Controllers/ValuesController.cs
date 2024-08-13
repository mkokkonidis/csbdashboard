using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using System;
using Microsoft.AspNetCore.Http.Extensions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.Http.Headers;
using System.Collections.Generic;

namespace CSBDashboardServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        const bool Verbose = false;
        static readonly long epochTicks = new DateTime(1970, 1, 1, 0, 0, 0, 0).Ticks;
        
        static dynamic CompactObservations(string auth, string apiBaseUrlSlash, int patient,string spec)
        {
            const int pageSize = 2;
            var retList = new List<decimal[]>();
            var infoList = new List<string>();

            string url = $"{apiBaseUrlSlash}fhir/Observation?patient=P{patient}&{spec}";
            infoList.Add($"Info: Will try to obtain results from {url}");
            try
            {
                int page = 0;
                while (true)
                {

                    int count = 0;

                    using (WebClient client = new WebClient())
                    {
                        client.Headers.Add("Accept", "application/json");
                        client.Headers.Add("Content-Type", "application/json");
                        client.Headers.Add("Authorization", auth);
                        var jsonResponseBody = 
                            client.DownloadData(url + $"&_page={++page}&_count={pageSize}");
                        var responseBody = (System.Text.Json.JsonElement)
                            JsonSerializer.Deserialize<dynamic>(jsonResponseBody);
                        if (Verbose) infoList.Add($"Info: deserialised");
                        var list = responseBody.GetProperty("entry").EnumerateArray();
                        if (Verbose) infoList.Add($"Info: entry array found");
                        foreach (var item in list)
                        {
                            var resource = item.GetProperty("resource");
                            var effectiveDateTime = resource.GetProperty("effectiveDateTime").GetString();
                            var value = resource.GetProperty("valueQuantity").GetProperty("value").GetDecimal();
                            retList.Add(new decimal[] { 
                                (Convert.ToDateTime(effectiveDateTime).Ticks - epochTicks)/10000, 
                                value });
                            count++;
                        }
                    }

                    if (count < pageSize) break;
                }
            } catch (Exception ex) {
                infoList.Add($"Info: {ex.Message}");
            }

            //Sort
            retList.Sort((a, b) => a[0].CompareTo(b[0]));

            //Ensure retList is a function from time to values
            retList = retList
                .GroupBy(pair => pair[0])  // Group by the first element
                .Select(group => group.First()) // Select the first pair in each group
                .ToList();

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
            var apiBaseUrlSlash = Environment.GetEnvironmentVariable("MAIN_URL"); //eg https://testing-retention.biomed.ntua.gr/api/

            Func<string, object> O = (spec) => CompactObservations(auth, apiBaseUrlSlash, id, spec);

            //var bloodPressures = getBloodPressures(auth, apiBaseUrlSlash, id, "code=75367002&category=408746007");


            var ret = new {
                lightSleep = O("code=762636008&category=29373008"),
                deepSleep = O("code=762636008&category=60984000"),
                remSleep = O("code=762636008&category=89129007"),
                awakenings = O("code=192004002"),
                calories = O("code=258790008"),
                metresAscended = O("code=310"),
                distance = O("code=246132006"),
                steps = O("code=309"),
                oxygen = O("code=442440005&category=408746007"),
                bodyTemperature = O("code=386725007&category=408746007"),
                heartRateWatchMin = O("code=364075005&category=62482003"),
                heartRateWatchAvg = O("code=364075005&category=255586005"),
                heartRateWatchMax = O("code=364075005&category=75540009"),
                heartRateBloodPressureMeter = O("code=364075005&category=408746007"),
                heartRateOximeter = O("code=364075005&category=59181002"),
                weight = O("code=726527001&category=408746007"),
                //bloodpresureDiastolic = bloodPressures.Item1,
                //bloodpresureSystolic = bloodPressures.Item2,

            };
            return ret;
        }


    }
}
