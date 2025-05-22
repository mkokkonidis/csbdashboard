using CSBDashboardServer.Controllers;
using System.Net;
using System.Text.Json;

namespace CSBDashboardServer.Helpers
{
    public static class IsAliveHelper
    {

        public static dynamic IsAlive(string fhirBaseUrlDirect, string nonfhirBaseUrlDirect, string patientManagerBaseUrlDirect)
        {

            string url = $"{fhirBaseUrlDirect}/metadata";
            
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("Accept", "application/fhir+json");
                client.Headers.Add("Content-Type", "application/json");
                var jsonResponseBody =
                    client.DownloadData(url);
                var responseBody = (System.Text.Json.JsonElement)
                    JsonSerializer.Deserialize<dynamic>(jsonResponseBody);
            }



            return "OK";                
 
    
    }

    }
}

