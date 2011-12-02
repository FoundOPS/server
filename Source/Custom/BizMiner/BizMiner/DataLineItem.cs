using System.Linq;

namespace BizMiner
{
    public class DataLineItem
    {
        public string IndustryCode { get; set; }
        public string Name { get; set; }
        public string Sector { get; set; }

        public string TotalEstablishments2010 { get; set; }
        public string TotalFirms { get; set; }
        public string TotalBranches { get; set; }
        public string TotalSmallBusinesses2010 { get; set; }

        public string SiteAvgSales2010 { get; set; }
        public string FirmAvgSales2010 { get; set; }
        public string SmallBusinessAvgSales2010 { get; set; }

        public string StartupFirmMarketShare { get; set; }

        public string IndustryStartupRate { get; set; }
        public string IndustryStartupIndex { get; set; }

        public static DataLineItem StripFromBizMinerPDF(string text)
        {
            var dataLineItem = new DataLineItem();

            dataLineItem.IndustryCode = ExtractString(text, "[", "]");
            dataLineItem.Name = ExtractString(text, "]", "Sector:");
            dataLineItem.Sector = ExtractString(text, "Sector:", "For non");

            dataLineItem.TotalEstablishments2010 = ExtractString(text, "Establishments", "Small Businesses").Split(' ').Last();

            dataLineItem.TotalFirms = ExtractString(text, "Firms", "Establishments").Split(' ').Last();
            dataLineItem.TotalBranches = ExtractString(text, "Branches", "The Industry Population").Split(' ').Last();

            dataLineItem.TotalSmallBusinesses2010 = ExtractString(text, "Small Businesses", "Branches").Split(' ').Last();

            if (text.Contains("2010 Avg Sales:"))
            {
                var avgSales2010Data = ExtractString(text, "2010 Avg Sales:", "Change").Split();

                dataLineItem.SiteAvgSales2010 = avgSales2010Data[0];
                dataLineItem.FirmAvgSales2010 = avgSales2010Data[1];
                dataLineItem.SmallBusinessAvgSales2010 = avgSales2010Data[2];
            }
            else
            {
                var avgSales2010Data = ExtractString(text, "\r\nAverage Annual Sales\r\nas of: 2010\r\n", "\r\nAverage Annual Sales").Replace("\r\n", "");
                dataLineItem.SiteAvgSales2010 = ExtractString(avgSales2010Data, "Site Sales ", "Firms").Trim();
                dataLineItem.FirmAvgSales2010 = ExtractString(avgSales2010Data, "Firms", "Small Business").Trim();

                dataLineItem.SmallBusinessAvgSales2010 = ExtractString(avgSales2010Data, "Small Business", "").Trim();
            }

            dataLineItem.StartupFirmMarketShare = ExtractString(text, "Startup Firm Market Share", "2010q2 Startup Firms");

            dataLineItem.IndustryStartupRate = ExtractString(text, "Industry Startup Rate", "US All-Industry Startup Rate");
            dataLineItem.IndustryStartupIndex = ExtractString(text, "Industry Startup Index ", "Industry Startup Activity: The");

            return dataLineItem;
        }

        private static string ExtractString(string text, string previousString, string followingString)
        {
            var previousStringIndex = text.IndexOf(previousString);
            var previousStringLength = previousString.Count();

            var stringLength = text.IndexOf(followingString, previousStringIndex + previousStringLength) - previousStringIndex - previousStringLength;

            var extractedString = stringLength > 0
                                      ? text.Substring(previousStringIndex + previousStringLength, stringLength)
                                      : text.Substring(previousStringIndex + previousStringLength);

            return extractedString.Trim();
        }
    }
}
