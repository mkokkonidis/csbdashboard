using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using System;
using Microsoft.AspNetCore.Http.Extensions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.Http.Headers;
using System.Collections.Generic;
using static System.Net.WebRequestMethods;
using System.Text;
using System.Diagnostics.Metrics;

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

            const int pageSize = 1000;
            var retList = new List<decimal[]>();
            var infoList = new List<string>();

            string url = $"{apiBaseUrlSlash}fhir/Observation?patient=P{patient}&{spec}";
            if(Verbose) infoList.Add($"Info: Results from {url} "+(componentCode!=null?$" with component-code={componentCode}":""));
            try
            {
                while (true)
                {

                    using (WebClient client = new WebClient())
                    {
                        client.Headers.Add("Accept", "application/json");
                        client.Headers.Add("Content-Type", "application/json");
                        client.Headers.Add("Authorization", auth);
                        var jsonResponseBody = 
                            client.DownloadData(url + $"&_getpagesoffset={retList.Count()}&_count={pageSize}");
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
                        }
                        if (list.Count() < pageSize) break;
                    }

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

        static async Task<dynamic> NonFHIRObservations(string auth, string apiBaseUrlSlash, string endPath, object? data = null)
        {
            const int limit = 3000;
            var infoList = new List<string>();
            var list = new List<object>();

            try
            {
                int page = 0;
                while (true)
                {

                    var handler = new HttpClientHandler
                    {
                        AllowAutoRedirect = true
                    };

                    //var client = new HttpClient(handler);

                    // Force IPv4
                    System.Net.ServicePointManager.Expect100Continue = false;
                    System.Net.ServicePointManager.UseNagleAlgorithm = false;
                    System.Net.ServicePointManager.EnableDnsRoundRobin = true;
                    System.Net.ServicePointManager.DnsRefreshTimeout = 0; // Disa
                    using (HttpClient client = new HttpClient(handler))
                    {
                        string url = apiBaseUrlSlash+$"nonfhir/api/{endPath}?limit={limit}&offset={page}&ascending=false";
                        if (Verbose) infoList.Add($"Info: Results from {url} ");

                        client.DefaultRequestHeaders.Add("Accept", "application/json");
                        //client.Default.Add("Content-Type", "application/json");
                        client.DefaultRequestHeaders.Add("Authorization", auth);
                        string jsonResponseBody;
                        if (data == null)
                        {
                            jsonResponseBody = await client.GetStringAsync(url);
                        }
                        else { 
                            var response = await client.PostAsync(url, new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json"));
                            response.EnsureSuccessStatusCode();
                            jsonResponseBody = await response.Content.ReadAsStringAsync();
                        }



                        var responseBody = (System.Text.Json.JsonElement)
                            JsonSerializer.Deserialize<dynamic>(jsonResponseBody);

                        var measurementEntries = responseBody.GetProperty("measurementEntries").EnumerateArray();
                        int count = 0;
                        foreach (var entry in measurementEntries) {
                            /*{
          "createdDateTime": "2024-09-20T20:00:06.821604",
          "deviceId": "rpi",
          "deviceImei": "e4:5f:01:e1:8b:08",
          "measurementDatetime": "2024-09-20T20:00:06",
          "measurementId": 14595,
          "metadata": null,
          "modifiedDateTime": "2024-09-20T20:00:06.821601",
          "state": "1",
          "type": "pollutionIndex",
          "userId": "P72"
        },*/
                            list.Add(entry);
                            count++;
                        }
                        if (count < limit) break;

                        page++;


                    }

                }
                return new
                {
                    results = new {  measurementEntries = list } ,
                    debug = infoList
                };


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


            if(id == 0)
            {
                id =72;
                apiBaseUrlSlash = "https://staging-retention.biomed.ntua.gr/api/";
                auth = "Bearer eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICI3eUtxLWdIQ3RFLTF2T1d2Q2otMFVzb3ZnaGY5UG4tOGp1NXZ2SWx1S3JRIn0.eyJleHAiOjE3MjY5NTg3OTQsImlhdCI6MTcyNjk1ODQ5NCwiYXV0aF90aW1lIjoxNzI2OTU3NDQ1LCJqdGkiOiIyNmM0ZThlYy0wMWQ2LTRiZGUtYjgzZi1iZjZiNTMzZDU4NzciLCJpc3MiOiJodHRwczovL3N0YWdpbmctcmV0ZW50aW9uLmJpb21lZC5udHVhLmdyL2F1dGgvcmVhbG1zL3JldGVudGlvbiIsImF1ZCI6ImFjY291bnQiLCJzdWIiOiI1ODJiNzUzNi1mMGY0LTRkY2UtYjcxNC1jYTZkNzdiYmExN2MiLCJ0eXAiOiJCZWFyZXIiLCJhenAiOiJkYXNoYm9hcmQiLCJub25jZSI6IjNmOWM1YmZiLWJiOWMtNGNmYy04ZTEzLTUwNTMyZTdmZWZjMyIsInNlc3Npb25fc3RhdGUiOiI5NThhN2JjYS03ODFlLTQ3M2EtODY0Mi1mYjkwMzI4ZDI0MTYiLCJhY3IiOiIwIiwiYWxsb3dlZC1vcmlnaW5zIjpbImh0dHBzOi8vc3RhZ2luZy1yZXRlbnRpb24uYmlvbWVkLm50dWEuZ3IiLCJodHRwOi8vbG9jYWxob3N0OjQyMDAiXSwicmVhbG1fYWNjZXNzIjp7InJvbGVzIjpbImNsaW5pY2FsX2Nhc2VfbWFuYWdlciIsIm9mZmxpbmVfYWNjZXNzIiwidW1hX2F1dGhvcml6YXRpb24iLCJkZWZhdWx0LXJvbGVzLXJldGVudGlvbiJdfSwicmVzb3VyY2VfYWNjZXNzIjp7ImFjY291bnQiOnsicm9sZXMiOlsibWFuYWdlLWFjY291bnQiLCJtYW5hZ2UtYWNjb3VudC1saW5rcyIsInZpZXctcHJvZmlsZSJdfX0sInNjb3BlIjoib3BlbmlkIG9yZ2FuaXNhdGlvbiBlbWFpbCBwcm9maWxlIiwic2lkIjoiOTU4YTdiY2EtNzgxZS00NzNhLTg2NDItZmI5MDMyOGQyNDE2IiwiZW1haWxfdmVyaWZpZWQiOnRydWUsIm5hbWUiOiJEZXYgQWNjb3VudCIsIm9yZ2FuaXNhdGlvbiI6IjE3IiwicHJlZmVycmVkX3VzZXJuYW1lIjoiZGV2IiwiZ2l2ZW5fbmFtZSI6IkRldiIsImZhbWlseV9uYW1lIjoiQWNjb3VudCIsImVtYWlsIjoiZGV2QHRlc3QuY29tIn0.XvBZUSdXLAjvT5uFfcsqdfwL-tgHvunBWOpxkLqH_ar3If86429sI_R8XQirI7--GPRtR6scNX2d_-mO2_XuTmbhfUaG9HbCgO4MTjALzBSHRAhPcMtbWo2E1WhADqBl1AiWxsVl1msjhFFN00308QYEVzwUzueUNBiDK0SmMiYZSkd35_PToxuHvnwUp4p1EtuMuoECMGN5gHaMRxe8rC-SKxlWAplcYJbtBG7cgMB3FuKXANg_VD714Sq9pbOt9Cl3qNwGh8nrAxdfngcbRpa1rcR2lAUmzjseASP_Kw1SY6SgL_tb4Zi_U6dK5rRYU1w3zz_zFbY-C-_P3CXzaw";
            }

            Func<string, Task<object>> O = async (spec) => await Task.Run(() => CompactFHIRObservations(auth, apiBaseUrlSlash, id, spec));
            //Func<string, Task<object>> GNO = async (path) => await Task.Run(() => NonFHIRObservations(auth, apiBaseUrlSlash, path));
            //Func<string, object, Task<object>> PNO = async (path, data) => await Task.Run(() => NonFHIRObservations(auth, apiBaseUrlSlash, path, data));
            Func<string, object, Task<object>> PNO =  (path, data) => NonFHIRObservations(auth, apiBaseUrlSlash, path, data);

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
            var rasberryTask = PNO("Measurement/get", new { userId = $"P{id}" });

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
