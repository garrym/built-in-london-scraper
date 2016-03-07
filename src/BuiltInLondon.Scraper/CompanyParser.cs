using System;
using System.Linq;
using HtmlAgilityPack;

namespace BuiltInLondon.Scraper
{
    public class CompanyParser
    {
        public Company Parse(string url, string html)
        {
            var document = new HtmlDocument();
            document.LoadHtml(html);

            var company = new Company();
            company.BuiltInLondonUrl = url;
            company.Name = document.DocumentNode.SelectSingleNode("//title").InnerHtml.Replace(" - Built in London", "");

            foreach (var node in document.DocumentNode.Descendants())
            {
                if (node.Name.Equals("p"))
                {
                    if (HasClass(node, "founders"))
                        company.Founders = GetValue(node);

                    if (HasClass(node, "address"))
                        company.Address = GetValue(node);

                    if (HasClass(node, "employees"))
                        company.Employees = GetValue(node);

                    if (HasClass(node, "url"))
                        company.Url = GetValue(node);

                    if (HasClass(node, "twitter-username"))
                        company.Twitter = GetValue(node);

                    if (HasClass(node, "is-hiring"))
                        company.JobUrl = GetValue(node);
                }
            }

            return company;
        }

        private static bool HasClass(HtmlNode node, string className)
        {
            var @class = node.Attributes["class"];
            if (@class != null && @class.Value.Contains(className))
                return true;
            return false;
        }

        private static string GetValue(HtmlNode node)
        {

            var a = node.ChildNodes.SingleOrDefault(x => x.Name.Equals("a", StringComparison.OrdinalIgnoreCase));
            if (a != null)
                return a.Attributes["href"].Value;
            return node.InnerHtml.Replace(Environment.NewLine, "").Trim();
        }
    }
}