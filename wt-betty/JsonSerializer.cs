using System;
using System.Net;
using Newtonsoft.Json;

namespace wt_betty
{
    public static class JsonSerializer
    {
        public static T _download_serialized_json_data<T>(string url) where T : new()
        {

            using (var w = new WebClient())
            {
                var json_data = string.Empty;

                //Get JSON data from the provided URL as string
                try
                {
                    json_data = w.DownloadString(url);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);

                }
                //If JSON data is not empty then instantiate the class and deserialize.

                return !string.IsNullOrEmpty(json_data) ? JsonConvert.DeserializeObject<T>(json_data) : new T();
            }
        }
    }
    /*
    public static class JsonExport
    {
        public static 
    }*/
}