namespace AsyncAwait
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Newtonsoft.Json.Linq;

    public class Postcode
    {
        private string PostCode { get; set; }
        private string Country { get; set; }
        private double Long { get; set; }
        private double Lat { get; set; }
        private string Region { get; set; }
        private string District { get; set; }
        private string Ward { get; set; }


        private string PostcodeRegexPattern = "(GIR 0AA)|((([A-Z-[QVX]][0-9][0-9]?)|(([A-Z-[QVX]][A-Z-[IJZ]][0-9][0-9]?)|(([A-Z-[QVX]][0-9][A-HJKSTUW])|([A-Z-[QVX]][A-Z-[IJZ]][0-9][ABEHMNPRVWXY])))) [0-9][A-Z-[CIKMOV]]{2})";

        public Postcode(string postcode)
        {
            PostCode = Validate(postcode);
            Task.Run(() => Get()).Wait();
        }

        private string Validate(string postcode)
        {
            if(new Regex(PostcodeRegexPattern).IsMatch(postcode))
            {
                return postcode.Replace(" ", string.Empty).Trim().ToUpper();
            }

            throw new Exception("Invalid postcode supplied.");
        }

        private async Task<bool> DownloadData()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string response = await client.GetStringAsync(BuildUri());

                    var jsonObject = JObject.Parse(response);
                    var jsonData = jsonObject.SelectToken("result");

                    PostCode = jsonData[0].Value<string>("postcode");
                    Country = jsonData[0].Value<string>("country");
                    Long = jsonData[0].Value<double>("longitude");
                    Lat = jsonData[0].Value<double>("latitude");
                    Region = jsonData[0].Value<string>("region");
                    District = jsonData[0].Value<string>("admin_district");
                    Ward = jsonData[0].Value<string>("admin_ward");
                }

                return true;
            } catch(Exception ex)
            {
                return false;
            }
        }

        private async Task Get()
        {
            if(!await DownloadData())
            {
                throw new Exception("Unable to get postcode data");
            }
        }

        private Uri BuildUri()
        {
            var builder = new UriBuilder();
            builder.Scheme = "https";
            builder.Host = "postcodes.io";
            builder.Path = "postcodes";
            builder.Query = $"q={PostCode}";

            return builder.Uri;
        }

        public void WriteToConsole()
        {
            Console.WriteLine($"Postcode: {PostCode}");
            Console.WriteLine($"Longitude: {Long.ToString()}");
            Console.WriteLine($"Latitude: {Lat.ToString()}");
            Console.WriteLine($"Region: {Region}");
            Console.WriteLine($"District: {District}");
            Console.WriteLine($"Ward: {Ward}");
        }
    }
}
