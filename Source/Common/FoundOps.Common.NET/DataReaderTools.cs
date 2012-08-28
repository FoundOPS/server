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
        public static Tuple<Dictionary<string, Type>, List<Dictionary<string, object>>> GetDynamicSqlData(string connectionstring, SqlCommand comm)
        {
            Tuple<Dictionary<string, Type>, List<Dictionary<string, object>>> result;

            using (var conn = new SqlConnection(connectionstring))
            {
                comm.Connection = conn;
                using (comm)
                {
                    conn.Open();

                    var columnHeaders = new Dictionary<string, Type>();

                    using (var reader = comm.ExecuteReader())
                    {
                        var rows = reader.Cast<DbDataRecord>().AsParallel()
                            //In parallel convert to dictionaries, force iterate with ToList so the connection/reader can be disposed
                             .Select(RecordToDictionary).ToList();

                        //Add the FieldTypes
                        for (int i = 0; i < reader.FieldCount; i++)
                            columnHeaders.Add(reader.GetName(i), reader.GetFieldType(i));

                        result = new Tuple<Dictionary<string, Type>, List<Dictionary<string, object>>>(columnHeaders, rows);
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