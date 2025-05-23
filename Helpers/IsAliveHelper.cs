using CSBDashboardServer.Controllers;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace CSBDashboardServer.Helpers
{
    public static class IsAliveHelper
    {

        public static dynamic IsAlive(string fhirBaseUrlDirect, string nonfhirBaseUrlDirect, string patientManagerBaseUrlDirect)
        {


            using (WebClient client = new WebClient())
            {
                string fhirRequestUrl = $"{fhirBaseUrlDirect}/metadata".Replace("//m","/m");
                client.Headers.Add("Accept", "application/fhir+json");
                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("Cache-Control", "no-cache");
                var jsonResponseBody =
                    client.DownloadData(fhirRequestUrl);
                var responseBody = (System.Text.Json.JsonElement)
                    JsonSerializer.Deserialize<dynamic>(jsonResponseBody);
            }


            using (WebClient client = new WebClient())
            {
                string patientManagerequestUrl = $"{patientManagerBaseUrlDirect}/patientmanager/api/patients".Replace("//p", "/p");
                client.Headers.Add("Cache-Control", "no-cache");
                var jsonResponseBody =
                    client.DownloadData(patientManagerequestUrl);
                var responseBody = (System.Text.Json.JsonElement)
                    JsonSerializer.Deserialize<dynamic>(jsonResponseBody);
            }



            return "OK";                
 
    
    }

    }
}

