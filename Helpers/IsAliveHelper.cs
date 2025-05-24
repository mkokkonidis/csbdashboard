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

            //try
            //{
            //    //staging: curl   http://172.27.0.3:8080/fhir/metadata 
            //    //staging: curl   -i http://localhost:8081/fhir/metadata 
            //    //test: curl   -i http://localhost:8080/fhir/metadata 
            //    using (WebClient client = new WebClient())
            //    {
            //        string fhirRequestUrl = $"{fhirBaseUrlDirect}/fhir/metadata".Replace("//m", "/m");
            //        client.Headers.Add("Accept", "application/fhir+json");
            //        client.Headers.Add("Content-Type", "application/json");
            //        client.Headers.Add("Cache-Control", "no-cache");
            //        var jsonResponseBody =
            //            client.DownloadData(fhirRequestUrl);
            //        var responseBody = (System.Text.Json.JsonElement)
            //            JsonSerializer.Deserialize<dynamic>(jsonResponseBody);
            //    }
            //}
            //catch
            //{
            //    throw new HttpRequestException("FHIR is down", null, System.Net.HttpStatusCode.InternalServerError);
            //}

            //try
            //{
            //    //staging: curl http://172.19.0.12:5000/api/Status
            //    //staging:curl -i  localhost:5050/api/Status
            //    //test: curl -i  localhost:5051/api/Status

            //    using (WebClient client = new WebClient())
            //    {
            //        string fhirRequestUrl = $"{fhirBaseUrlDirect}/api/Status".Replace("//m", "/m");
            //        client.Headers.Add("Cache-Control", "no-cache");
            //        var emptyBody =
            //            client.DownloadData(fhirRequestUrl);
            //    }
            //}
            //catch
            //{
            //    throw new HttpRequestException("Non-FHIR is down", null, System.Net.HttpStatusCode.InternalServerError);
            //}

            //try
            //{
            //    //staging: curl 172.19.0.11:8080/api/patients
            //    //
            //    using (WebClient client = new WebClient())
            //{
            //    string patientManagerequestUrl = $"{patientManagerBaseUrlDirect}/api/patients".Replace("//p", "/p");
            //    client.Headers.Add("Cache-Control", "no-cache");
            //    var jsonResponseBody =
            //        client.DownloadData(patientManagerequestUrl);
            //    var responseBody = (System.Text.Json.JsonElement)
            //        JsonSerializer.Deserialize<dynamic>(jsonResponseBody);
            //}
            //}
            //catch
            //{
            //    throw new HttpRequestException("PatientManager is down", null, System.Net.HttpStatusCode.InternalServerError);
            //}



            return "OK";                
 
    
    }

    }
}

