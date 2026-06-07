using System;
using System.IO;
using ConfigWell.WinForms.Models;

namespace ConfigWell.WinForms.Parsers
{
    public static class ParserFactory
    {
        public static IConfigParser GetParser(string filePath)
        {
            string ext = Path.GetExtension(filePath).ToLowerInvariant();
            switch (ext)
            {
                case ".properties":
                case ".conf":       return new PropertiesParser();
                case ".ini":
                case ".definition": return new IniParser();
                case ".xml":
                case ".config":     return new XmlConfigParser();
                case ".bat":
                case ".cmd":        return new BatParser();
                case ".ps1":        return new Ps1Parser();
                default:            return new PropertiesParser(); // fallback
            }
        }

        public static FileType DetectFileType(string filePath)
        {
            string ext = Path.GetExtension(filePath).ToLowerInvariant();
            switch (ext)
            {
                case ".properties":
                case ".conf":       return FileType.Properties;
                case ".ini":
                case ".definition": return FileType.Ini;
                case ".xml":
                case ".config":     return FileType.XmlConfig;
                case ".bat":
                case ".cmd":        return FileType.Bat;
                case ".ps1":        return FileType.Ps1;
                default:            return FileType.Unknown;
            }
        }
    }
}
