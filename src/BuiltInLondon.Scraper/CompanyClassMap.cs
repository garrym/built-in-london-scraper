using CsvHelper.Configuration;

namespace BuiltInLondon.Scraper
{
    public sealed class CompanyClassMap : CsvClassMap<Company>
    {
        public CompanyClassMap()
        {
            Map(x => x.Name);
            Map(x => x.BuiltInLondonUrl);
            Map(x => x.Founders);
            Map(x => x.Address);
            Map(x => x.Employees);
            Map(x => x.Url);
            Map(x => x.JobUrl);
            Map(x => x.Twitter);
        }
    }
}