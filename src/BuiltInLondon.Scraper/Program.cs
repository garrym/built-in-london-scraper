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
        private static readonly Uri BaseAddress = new Uri("http://www.builtinlondon.com/");
        private static readonly string FilePath = ConfigurationManager.AppSettings["FilePath"];

        static void Main()
        {
            Scrape();
            //Read();
        }

        private static void Read()
        {
            using (var streamReader = new StreamReader(FilePath))
            using (var csvReader = new CsvReader(streamReader))
            {
                csvReader.Configuration.RegisterClassMap<CompanyClassMap>();

                foreach (var company in csvReader.GetRecords<Company>())
                {
                    Console.WriteLine(company.Name);
                }
            }
        }

        private static void Scrape()
        {
            var html = Get("/");

            var document = new HtmlDocument();
            document.LoadHtml(html);

            var ul = document.DocumentNode.SelectNodes("//ul").SingleOrDefault(x => x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("startup-list"));
            Contract.Assert(ul != null);

            var companyParser = new CompanyParser();

            using (var streamWriter = new StreamWriter(FilePath))
            using (var csvWriter = new CsvWriter(streamWriter))
            {
                csvWriter.Configuration.RegisterClassMap<CompanyClassMap>();
                csvWriter.WriteHeader<Company>();

                foreach (var li in ul.SelectNodes("li"))
                {
                    var url = li.SelectSingleNode("a").Attributes["href"].Value;
                    Console.WriteLine(url);
                    var companyHtml = Get(url);
                    var company = companyParser.Parse(url, companyHtml);

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
                client.BaseAddress = BaseAddress;
                return client.GetStringAsync(relativeUrl).Result;
            }
        }
    }
}
