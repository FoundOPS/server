using System.IO;
using System.Linq;
using org.pdfbox.pdmodel;
using org.pdfbox.util;
using System.Collections.Generic;

namespace BizMiner
{
    public class PDFLoader
    {
        private const string FolderLocation = @"C:\FoundOps\Company\Research\Target\SubIndustry";

        /// <summary>
        /// Converts the PDF files to text files.
        /// </summary>
        public static void ConvertPDFFilesToTextFiles()
        {
            //Only convert PDF files
            var pdfFilesToConvert = Directory.GetFiles(FolderLocation).Where(f => f.Contains(".pdf")).ToList();

            var txtFilesArray = pdfFilesToConvert.Where(f => f.Contains(".txt")).ToArray();

            //Remove PDF files that already have text files
            pdfFilesToConvert.RemoveAll(pdfFile => txtFilesArray.Contains(pdfFile.Replace(".pdf", ".txt")));

            //For testing, only do 1
            //pdfFilesToConvert.RemoveRange(1, files.Count - 1);

            //Parallelize the PDF conversion
            pdfFilesToConvert.AsParallel().ForAll(filePath =>
            {
                var doc = PDDocument.load(filePath);
                var pdfStripper = new PDFTextStripper();
                var text = pdfStripper.getText(doc);
                File.WriteAllText(filePath.Replace(".pdf", ".txt"), text);
            });
        }

        public static List<DataLineItem> LoadDataLineItems()
        {
            return Directory.GetFiles(FolderLocation).ToList().Where(f => f.Contains(".txt"))
                .Select(file => File.OpenText(file).ReadToEnd())
                .Select(DataLineItem.StripFromBizMinerPDF).ToList();
        }
    }
}
