using ConfigWell.WinForms.Models;

namespace ConfigWell.WinForms.Parsers
{
    public interface IConfigParser
    {
        ParsedConfig Parse(string filePath, string rawText);
    }
}
