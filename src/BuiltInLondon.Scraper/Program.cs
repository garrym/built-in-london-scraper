using System;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using CsvHelper;
using HtmlAgilityPack;

namespace BuiltInLondon.Scraper
{
    class Program
    {
        private static readonly Uri baseAddress = new Uri("http://www.builtinlondon.com/");

        static void Main()
        {
            var filePath = ConfigurationManager.AppSettings["FilePath"];

            var html = Get("/");

            var document = new HtmlDocument();
            document.LoadHtml(html);

            var ul = document.DocumentNode.SelectNodes("//ul").SingleOrDefault(x => x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("startup-list"));
            Contract.Assert(ul != null);

            //var companies = new List<Company>();
            var companyParser = new CompanyParser();

            using (var streamWriter = new StreamWriter(filePath))
            using (var csvWriter = new CsvWriter(streamWriter))
            {
                csvWriter.Configuration.RegisterClassMap<CompanyClassMap>();

                foreach (var li in ul.SelectNodes("li"))
                {
                    var url = li.SelectSingleNode("a").Attributes["href"].Value;
                    Console.WriteLine(url);
                    var companyHtml = Get(url);
                    var company = companyParser.Parse(url, companyHtml);
                    //companies.Add(company);

                    csvWriter.WriteRecord(company);
                    streamWriter.Flush();
                    Thread.Sleep(3000);
                }

            }
       }

        private static string Get(string relativeUrl)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = baseAddress;
                return client.GetStringAsync(relativeUrl).Result;
            }
        }
    }
}
