using System.IO;

namespace FoundOps.Common.Composite.Tools
{
    public static class StreamHelpers
    {
        /// <summary>
        /// Converts a stream to a byte array.
        /// </summary>
        public static byte[] ReadFully(this Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
