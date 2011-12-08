using System.IO;

namespace FoundOps.Common.Composite
{
    public static class FileTools
    {
#if SILVERLIGHT
        public static byte[] ReadBytesFromResource(string resourcePath)
        {
            System.Windows.Resources.StreamResourceInfo sri =
                System.Windows.Application.GetResourceStream(new System.Uri(resourcePath, System.UriKind.Relative));

            Stream stream = sri.Stream;
            byte[] fileBytes = new byte[stream.Length];
            stream.Read(fileBytes, 0, (int) stream.Length);
            return fileBytes;
        }
#else
        public static byte[] FileToBytes(string file) //Not used in Silverlight (because of permissions)
        {
            FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            byte[] fileBytes = new byte[fileStream.Length];
            fileStream.Read(fileBytes, 0, (int)fileStream.Length);
            return fileBytes;
        }
#endif
    }
}
