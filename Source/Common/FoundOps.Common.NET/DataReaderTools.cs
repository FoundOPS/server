using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace FoundOps.Common.NET
{
    public static class DataReaderTools
    {
        /// <summary>
        /// Execute a SqlCommand and return it's data.
        /// </summary>
        /// <returns>
        /// List of each row as a Dictionary with the Name as the Key, and Value as the Value
        /// </returns>
        public static List<Dictionary<string, object>> GetDynamicSqlData(string connectionstring, SqlCommand comm)
        {
            List<Dictionary<string, object>> result;

            using (var conn = new SqlConnection(connectionstring))
            {
                comm.Connection = conn;
                using (comm)
                {
                    conn.Open();

                    using (var reader = comm.ExecuteReader())
                    {
                        result = reader.Cast<DbDataRecord>().AsParallel()
                            //In parallel convert to dictionaries, force iterate with ToList so the connection/reader can be disposed
                             .Select(RecordToDictionary).ToList();
                    }
                    conn.Close();
                }
            }

            return result;
        }

        private static Dictionary<string, object> RecordToDictionary(DbDataRecord record)
        {
            var dictionary = new Dictionary<string, object>();

            for (var i = 0; i < record.FieldCount; i++)
                dictionary.Add(record.GetName(i), record[i]);

            return dictionary;
        }
    }
}