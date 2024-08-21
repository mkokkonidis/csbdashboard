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
        
        static dynamic CompactFHIRObservations(string auth, string apiBaseUrlSlash, int patient,string spec)
        {
            string? componentCode = 
                spec
                    .Split('&')
                    .Where(_=>_.StartsWith("component-code="))
                    .Select(_ => _.Split('=')[1])
                    .FirstOrDefault();

            const int pageSize = 100;
            var retList = new List<decimal[]>();
            var infoList = new List<string>();

            string url = $"{apiBaseUrlSlash}fhir/Observation?patient=P{patient}&{spec}";
            if(Verbose) infoList.Add($"Info: Results from {url} "+(componentCode!=null?$" with component-code={componentCode}":""));
            try
            {
                int page = 0;
                //while (true)
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
                            var value =
                                componentCode == null ?
                                    resource.GetProperty("valueQuantity").GetProperty("value").GetDecimal() :
                                    resource.GetProperty("component")
                                    .EnumerateArray().ToList()
                                            .Where(_ =>
                                                   _.GetProperty("code")
                                                    .GetProperty("coding")[0]
                                                    .GetProperty("code").GetString() == componentCode)
                                            .FirstOrDefault()
                                            .GetProperty("valueQuantity").GetProperty("value").GetDecimal();


                            retList.Add(new decimal[] { 
                                (Convert.ToDateTime(effectiveDateTime).Ticks - epochTicks)/10000, 
                                value });
                            count++;
                        }
                    }

                    //if (count < pageSize) break;
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

        static dynamic NonFHIRObservations(string auth, string apiBaseUrlSlash, string endPath, object? data = null)
        {
            const int pageSize = 100;

            var infoList = new List<string>();

            string url = $"{apiBaseUrlSlash}nonfhir/api/{endPath}";
            if (Verbose) infoList.Add($"Info: Results from {url} ");
            try
            {
                int page = 0;
                //while (true)
                {

                    int count = 0;

                    using (WebClient client = new WebClient())
                    {
                        client.Headers.Add("Accept", "application/json");
                        client.Headers.Add("Content-Type", "application/json");
                        client.Headers.Add("Authorization", auth);
                        var jsonResponseBody = data == null?
                            client.DownloadString(url):
                            client.UploadString(url, JsonSerializer.Serialize(data));
                        var responseBody = (System.Text.Json.JsonElement)
                            JsonSerializer.Deserialize<dynamic>(jsonResponseBody);

                        return new
                        {
                            results = responseBody,
                            debug = infoList
                        };
                        //if (Verbose) infoList.Add($"Info: deserialised");
                        //var list = responseBody.GetProperty("entry").EnumerateArray();
                        //if (Verbose) infoList.Add($"Info: entry array found");
                        //foreach (var item in list)
                        //{
                        //    var resource = item.GetProperty("resource");
                        //    var effectiveDateTime = resource.GetProperty("effectiveDateTime").GetString();
                        //    var value =
                        //        componentCode == null ?
                        //            resource.GetProperty("valueQuantity").GetProperty("value").GetDecimal() :
                        //            resource.GetProperty("component")
                        //            .EnumerateArray().ToList()
                        //                    .Where(_ =>
                        //                           _.GetProperty("code")
                        //                            .GetProperty("coding")[0]
                        //                            .GetProperty("code").GetString() == componentCode)
                        //                    .FirstOrDefault()
                        //                    .GetProperty("valueQuantity").GetProperty("value").GetDecimal();
                        //    retList.Add(new decimal[] {
                        //        (Convert.ToDateTime(effectiveDateTime).Ticks - epochTicks)/10000,
                        //        value });
                        //    count++;
                        //}
                    }

                    //if (count < pageSize) break;
                }
            }
            catch (Exception ex)
            {
                infoList.Add($"Info: {ex.Message}");
                return new
                {
                    //results = new { },
                    debug = infoList
                };
            }

            //Sort
            //retList.Sort((a, b) => a[0].CompareTo(b[0]));

            ////Ensure retList is a function from time to values
            //retList = retList
            //    .GroupBy(pair => pair[0])  // Group by the first element
            //    .Select(group => group.First()) // Select the first pair in each group
            //    .ToList();

            //return new
            //{
            //    results = retList,
            //    debug = infoList
            //};
        }


        // GET /<ValuesController>/5
        [HttpGet("{id}")]
        public async Task<object> Get(int id)
        {
            var auth = Request.Headers.Authorization.ToString();
            var apiBaseUrlSlash = Environment.GetEnvironmentVariable("MAIN_URL"); // e.g., https://test-retention.biomed.ntua.gr/api/

            Func<string, Task<object>> O = async (spec) => await Task.Run(() => CompactFHIRObservations(auth, apiBaseUrlSlash, id, spec));
            Func<string, Task<object>> GNO = async (path) => await Task.Run(() => NonFHIRObservations(auth, apiBaseUrlSlash, path));
            Func<string, object, Task<object>> PNO = async (path, data) => await Task.Run(() => NonFHIRObservations(auth, apiBaseUrlSlash, path, data));

            var lightSleepTask = O("code=762636008&category=29373008");
            var deepSleepTask = O("code=762636008&category=60984000");
            var remSleepTask = O("code=762636008&category=89129007");
            var awakeningsTask = O("code=192004002");
            var caloriesTask = O("code=258790008");
            var metresAscendedTask = O("code=310");
            var distanceTask = O("code=246132006");
            var stepsTask = O("code=309");
            var oxygenTask = O("code=442440005&category=408746007");
            var bodyTemperatureTask = O("code=386725007&category=408746007");
            var heartRateWatchMinTask = O("code=364075005&category=62482003");
            var heartRateWatchAvgTask = O("code=364075005&category=255586005");
            var heartRateWatchMaxTask = O("code=364075005&category=75540009");
            var heartRateBloodPressureMeterTask = O("code=364075005&category=408746007");
            var heartRateOximeterTask = O("code=364075005&category=59181002");
            var weightTask = O("code=726527001&category=408746007");
            var bloodPressureDiastolicTask = O("code=75367002&category=408746007&component-code=271649006");
            var bloodPressureSystolicTask = O("code=75367002&category=408746007&component-code=271650006");
            var rasberryTask = PNO("Measurement/get?limit=3000&offset=0", new { userId = $"P{id}" });

            await Task.WhenAll(lightSleepTask, deepSleepTask, remSleepTask, awakeningsTask, caloriesTask, metresAscendedTask, distanceTask, stepsTask, oxygenTask, bodyTemperatureTask, heartRateWatchMinTask, heartRateWatchAvgTask, heartRateWatchMaxTask, heartRateBloodPressureMeterTask, heartRateOximeterTask, weightTask, bloodPressureDiastolicTask, bloodPressureSystolicTask, rasberryTask);

            var ret = new
            {
                lightSleep = await lightSleepTask,
                deepSleep = await deepSleepTask,
                remSleep = await remSleepTask,
                awakenings = await awakeningsTask,
                calories = await caloriesTask,
                metresAscended = await metresAscendedTask,
                distance = await distanceTask,
                steps = await stepsTask,
                oxygen = await oxygenTask,
                bodyTemperature = await bodyTemperatureTask,
                heartRateWatchMin = await heartRateWatchMinTask,
                heartRateWatchAvg = await heartRateWatchAvgTask,
                heartRateWatchMax = await heartRateWatchMaxTask,
                heartRateBloodPressureMeter = await heartRateBloodPressureMeterTask,
                heartRateOximeter = await heartRateOximeterTask,
                weight = await weightTask,
                bloodpresureDiastolic = await bloodPressureDiastolicTask,
                bloodpresureSystolic = await bloodPressureSystolicTask,
                rasberry = await rasberryTask
            };

            return ret;
        }

        //// GET /<ValuesController>/5
        //[HttpGet("{id}")]
        //public object Get(int id)
        //{
        //    var auth = 
        //        Request.Headers.Authorization.ToString();            
        //    var apiBaseUrlSlash = Environment.GetEnvironmentVariable("MAIN_URL"); //eg https://test-retention.biomed.ntua.gr/api/

        //    Func<string, object> O = (spec) => CompactObservations(auth, apiBaseUrlSlash, id, spec);


        //    var ret = new {
        //        lightSleep = O("code=762636008&category=29373008"),
        //        deepSleep = O("code=762636008&category=60984000"),
        //        remSleep = O("code=762636008&category=89129007"),
        //        awakenings = O("code=192004002"),
        //        calories = O("code=258790008"),
        //        metresAscended = O("code=310"),
        //        distance = O("code=246132006"),
        //        steps = O("code=309"),
        //        oxygen = O("code=442440005&category=408746007"),
        //        bodyTemperature = O("code=386725007&category=408746007"),
        //        heartRateWatchMin = O("code=364075005&category=62482003"),
        //        heartRateWatchAvg = O("code=364075005&category=255586005"),
        //        heartRateWatchMax = O("code=364075005&category=75540009"),
        //        heartRateBloodPressureMeter = O("code=364075005&category=408746007"),
        //        heartRateOximeter = O("code=364075005&category=59181002"),
        //        weight = O("code=726527001&category=408746007"),
        //        bloodpresureDiastolic = O("code=75367002&category=408746007&component-code=271649006"),
        //        bloodpresureSystolic =  O("code=75367002&category=408746007&component-code=271650006") 
        //    };
        //    return ret;
        //}


    }
}
