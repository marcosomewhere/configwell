using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using ConfigWell.WinForms.Models;

namespace ConfigWell.WinForms.Services
{
    // Writes changed entry values back into the raw text, preserving formatting and comments.
    public class ConfigWriterService
    {
        public string ApplyChanges(ParsedConfig config)
        {
            switch (config.FileType)
            {
                case FileType.Properties: return ApplyKeyValue(config, '=');
                case FileType.Ini:        return ApplyIni(config);
                case FileType.XmlConfig:  return ApplyXml(config);
                case FileType.Bat:        return ApplyBat(config);
                case FileType.Ps1:        return ApplyPs1(config);
                default:                  return config.RawText;
            }
        }

        private string ApplyKeyValue(ParsedConfig config, char separator)
        {
            var lines = config.RawText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var entryMap = BuildEntryMap(config);

            for (int i = 0; i < lines.Length; i++)
            {
                string trimmed = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#") || trimmed.StartsWith(";") || trimmed.StartsWith("!"))
                    continue;

                int eqIdx = trimmed.IndexOf(separator);
                if (eqIdx > 0)
                {
                    string key = trimmed.Substring(0, eqIdx).Trim();
                    if (entryMap.ContainsKey(key))
                    {
                        string indent = GetIndent(lines[i]);
                        lines[i] = indent + key + separator + entryMap[key];
                    }
                }
            }

            return string.Join(GetNewline(config.RawText), lines);
        }

        private string ApplyIni(ParsedConfig config)
        {
            var lines = config.RawText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var entryMap = BuildEntryMap(config);

            for (int i = 0; i < lines.Length; i++)
            {
                string trimmed = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#") || trimmed.StartsWith(";") || trimmed.StartsWith("["))
                    continue;

                int eqIdx = trimmed.IndexOf('=');
                if (eqIdx > 0)
                {
                    string key = trimmed.Substring(0, eqIdx).Trim();
                    if (entryMap.ContainsKey(key))
                    {
                        string indent = GetIndent(lines[i]);
                        // Preserve inline comment
                        string remainder = trimmed.Substring(eqIdx + 1).Trim();
                        int commentIdx = remainder.IndexOf(';');
                        string inlineComment = commentIdx >= 0 ? " ;" + remainder.Substring(commentIdx + 1) : string.Empty;
                        lines[i] = indent + key + "=" + entryMap[key] + inlineComment;
                    }
                }
            }

            return string.Join(GetNewline(config.RawText), lines);
        }

        private string ApplyXml(ParsedConfig config)
        {
            var entryMap = BuildEntryMap(config);
            string result = config.RawText;

            foreach (var kvp in entryMap)
            {
                // Replace value="..." in <add key="KEY" value="..."/> and <add value="..." key="KEY"/>
                string escaped = EscapeXml(kvp.Value);
                result = Regex.Replace(result,
                    @"(<add\b[^>]*\bkey=""" + Regex.Escape(kvp.Key) + @"""[^>]*\bvalue="")[^""]*(""\s*/>)",
                    m => m.Groups[1].Value + escaped + m.Groups[2].Value);
                result = Regex.Replace(result,
                    @"(<add\b[^>]*\bvalue="")[^""]*(""\s[^>]*\bkey=""" + Regex.Escape(kvp.Key) + @"""[^>]*/>)",
                    m => m.Groups[1].Value + escaped + m.Groups[2].Value);
            }

            return result;
        }

        private string ApplyBat(ParsedConfig config)
        {
            var lines = config.RawText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var entryMap = BuildEntryMap(config);
            var setRegex = new Regex(@"^([Ss][Ee][Tt]\s+)""?([^=""]+)=([^""]*)""?(.*)$");

            for (int i = 0; i < lines.Length; i++)
            {
                var m = setRegex.Match(lines[i].Trim());
                if (m.Success)
                {
                    string key = m.Groups[2].Value.Trim();
                    if (entryMap.ContainsKey(key))
                    {
                        string indent = GetIndent(lines[i]);
                        string value = entryMap[key];
                        // Use quoted form if value contains spaces
                        if (value.Contains(" "))
                            lines[i] = indent + "SET \"" + key + "=" + value + "\"";
                        else
                            lines[i] = indent + "SET " + key + "=" + value;
                    }
                }
            }

            return string.Join(GetNewline(config.RawText), lines);
        }

        private string ApplyPs1(ParsedConfig config)
        {
            var lines = config.RawText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var entryMap = BuildEntryMap(config);
            var assignRegex = new Regex(@"^(\$[A-Za-z_][A-Za-z0-9_]*)\s*=\s*(.+)$");

            foreach (var section in config.Sections)
            {
                foreach (var entry in section.Entries)
                {
                    if (!entryMap.ContainsKey(entry.Key)) continue;
                    int lineIdx = entry.LineNumber - 1;
                    if (lineIdx < 0 || lineIdx >= lines.Length) continue;

                    string newValue = entryMap[entry.Key];
                    string indent = GetIndent(lines[lineIdx]);

                    if (entry.Kind == ValueKind.Boolean)
                        lines[lineIdx] = indent + "$" + entry.Key + " = " + (newValue.ToLower() == "true" ? "$true" : "$false");
                    else if (entry.Kind == ValueKind.Number)
                        lines[lineIdx] = indent + "$" + entry.Key + " = " + newValue;
                    else
                        lines[lineIdx] = indent + "$" + entry.Key + " = \"" + newValue + "\"";
                }
            }

            return string.Join(GetNewline(config.RawText), lines);
        }

        private Dictionary<string, string> BuildEntryMap(ParsedConfig config)
        {
            var map = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var section in config.Sections)
                foreach (var entry in section.Entries)
                    map[entry.Key] = entry.Value;
            return map;
        }

        private string GetIndent(string line)
        {
            int i = 0;
            while (i < line.Length && (line[i] == ' ' || line[i] == '\t'))
                i++;
            return line.Substring(0, i);
        }

        private string GetNewline(string text)
        {
            if (text.Contains("\r\n")) return "\r\n";
            if (text.Contains("\r")) return "\r";
            return "\n";
        }

        private string EscapeXml(string s)
        {
            return s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
        }
    }
}
