using Kent.Boogaart.KBCsv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FoundOps.Common.NET
{
    public static class CSVTools
    {
        /// <summary>
        /// Converts a DataTable to a csv string
        /// </summary>
        /// <param name="dataTable">The dataTable</param>
        /// <param name="headers">(Optional) Headers</param>
        /// <param name="ignore">(Optional) Keys to ignore</param>
        public static string ToCSV(this IEnumerable<Dictionary<string, Object>> dataTable, string[] headers = null, string[] ignore = null)
        {
            if (ignore == null)
                ignore = new string[] { };

            var stringWriter = new StringWriter();
            var csvWriter = new CsvWriter(stringWriter);

            if (headers == null)
            {
                headers = (from kvp in dataTable.First()
                           where !ignore.Contains(kvp.Key)
                           select kvp.Key).ToArray();
            }

            csvWriter.WriteHeaderRecord(headers);

            foreach (var record in dataTable)
            {
                var values = (from kvp in record
                              where !ignore.Contains(kvp.Key)
                              select  kvp.Value == null? "":
                              kvp.Value.ToString()).ToArray();

                csvWriter.WriteDataRecord(values);
            }
            csvWriter.Close();

            var csv = stringWriter.ToString();
            return csv;
        }
    }
}
