using System;
using System.Collections.Generic;
using ConfigWell.WinForms.Models;

namespace ConfigWell.WinForms.Parsers
{
    // Parses .properties files (key=value or key: value, comments with #, ; or !)
    public class PropertiesParser : IConfigParser
    {
        public ParsedConfig Parse(string filePath, string rawText)
        {
            var config = new ParsedConfig
            {
                FilePath = filePath,
                FileType = FileType.Properties,
                RawText = rawText
            };

            var section = new ConfigSection("(default)");
            config.Sections.Add(section);

            var lines = rawText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var pendingComments = new List<string>();

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                string trimmed = line.Trim();

                if (string.IsNullOrWhiteSpace(trimmed))
                {
                    pendingComments.Clear();
                    continue;
                }

                if (trimmed.StartsWith("#") || trimmed.StartsWith(";") || trimmed.StartsWith("!"))
                {
                    string commentText = trimmed.TrimStart('#', ';', '!').Trim();
                    pendingComments.Add(commentText);
                    continue;
                }

                string key = null, value = null;

                int eqIdx = trimmed.IndexOf('=');
                int colonIdx = trimmed.IndexOf(':');

                if (eqIdx > 0 && (colonIdx < 0 || eqIdx <= colonIdx))
                {
                    key = trimmed.Substring(0, eqIdx).Trim();
                    value = trimmed.Substring(eqIdx + 1).Trim();
                }
                else if (colonIdx > 0)
                {
                    key = trimmed.Substring(0, colonIdx).Trim();
                    value = trimmed.Substring(colonIdx + 1).Trim();
                }

                if (key != null)
                {
                    var entry = new ConfigEntry
                    {
                        Key = key,
                        Value = value,
                        Comment = string.Join(" ", pendingComments),
                        Kind = ConfigEntry.DetectKind(value),
                        LineNumber = i + 1
                    };
                    section.Entries.Add(entry);
                }

                pendingComments.Clear();
            }

            return config;
        }
    }
}
