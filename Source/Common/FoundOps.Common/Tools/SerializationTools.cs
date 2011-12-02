using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace FoundOps.Common.Tools
{
    public static class SerializationTools
    {
        public static T Deserialize<T>(string xml)
        {
            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                var dataContractSerializer = new DataContractSerializer(typeof(T));
                return (T)dataContractSerializer.ReadObject(memoryStream);
            }
        }

        public static string Serialize(object obj)
        {
            var serializer = new DataContractSerializer(obj.GetType());

            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, obj);
                stream.Position = 0L;
                var fixedStream = stream.ToArray();
                return Encoding.UTF8.GetString(fixedStream, 0, fixedStream.Length);
            }
        }

        /// <summary>
        /// Method to write an object to a file in xml format
        /// </summary>
        public static void WriteToFile(object obj, string fileLocation)
        {
            var sw = new StreamWriter(fileLocation, false, System.Text.Encoding.UTF8);
            sw.Write(Serialize(obj));
            sw.Close();
        }

        /// <summary>
        /// Method to read an xml file into an object of type T
        /// </summary>
        public static T ReadFromFile<T>(string fileLocation)
        {
            var sr = new StreamReader(fileLocation, System.Text.Encoding.UTF8);
            var deserializedObj = Deserialize<T>(sr.ReadToEnd());
            sr.Close();
            return deserializedObj;
        }
    }
}