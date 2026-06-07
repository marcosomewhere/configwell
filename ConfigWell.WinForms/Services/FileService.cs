using System;
using System.IO;
using System.Text;
using ConfigWell.WinForms.Models;
using ConfigWell.WinForms.Parsers;

namespace ConfigWell.WinForms.Services
{
    public class FileService
    {
        public ParsedConfig OpenFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found.", filePath);

            Encoding encoding = DetectEncoding(filePath);
            string rawText = File.ReadAllText(filePath, encoding);

            var parser = ParserFactory.GetParser(filePath);
            var config = parser.Parse(filePath, rawText);
            config.Encoding = encoding;
            config.HasUnsavedChanges = false;

            return config;
        }

        private Encoding DetectEncoding(string filePath)
        {
            byte[] bom = new byte[4];
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                int read = fs.Read(bom, 0, 4);
                if (read >= 3 && bom[0] == 0xEF && bom[1] == 0xBB && bom[2] == 0xBF)
                    return Encoding.UTF8;
                if (read >= 2 && bom[0] == 0xFF && bom[1] == 0xFE)
                    return Encoding.Unicode;
                if (read >= 2 && bom[0] == 0xFE && bom[1] == 0xFF)
                    return Encoding.BigEndianUnicode;
            }
            return new UTF8Encoding(false); // UTF-8 without BOM
        }
    }
}
