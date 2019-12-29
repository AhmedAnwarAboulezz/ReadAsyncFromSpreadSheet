using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Microsoft.AspNetCore.Mvc;

namespace IntegrateWithCloud.Controllers
{
    [Route("api/[controller]/[action]")]
    public class DefaultController : Controller
    {
        private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private const string SpreadsheetId = "1FGoY6PWv6b2qL1XeNskNPH3UV2odVGky2RrYYs50XU0";
        private const string GoogleCredentialsFileName = "google-credentials.json";
        private const string ReadRange = "A:C";

        // GET api/values
        [HttpGet]
        public async Task<IEnumerable<string>> GetAsync()
        {
            await Get2();
            return new string[] { "value1", "value2" };
        }

        static async Task Get2()
        {
            var serviceValues = GetSheetsService().Spreadsheets.Values;
            await ReadAsync(serviceValues);
        }




        private static SheetsService GetSheetsService()
        {
            using (var stream = new FileStream(GoogleCredentialsFileName, FileMode.Open, FileAccess.Read))
            {
                var serviceInitializer = new BaseClientService.Initializer
                {
                    HttpClientInitializer = GoogleCredential.FromStream(stream).CreateScoped(Scopes)
                };
                return new SheetsService(serviceInitializer);
            }
        }

        private static async Task ReadAsync(SpreadsheetsResource.ValuesResource valuesResource)
        {
            var response = await valuesResource.Get(SpreadsheetId, ReadRange).ExecuteAsync();
            var values = response.Values;
            if (values == null || !values.Any())
            {
                Console.WriteLine("No data found.");
                return;
            }
            var header = string.Join(" ", values.First().Select(r => r.ToString()));
            Console.WriteLine($"Header: {header}");

            foreach (var row in values.Skip(1))
            {
                var res = string.Join(" ", row.Select(r => r.ToString()));
                Console.WriteLine(res);
            }
        }

    }
}