using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ConfigWell.WinForms.Models;

namespace ConfigWell.WinForms.Parsers
{
    // Parses .bat/.cmd: SET KEY=value and SET "KEY=value"
    public class BatParser : IConfigParser
    {
        private static readonly Regex SetRegex = new Regex(
            @"^[Ss][Ee][Tt]\s+""?([^=""]+)=([^""]*)""?",
            RegexOptions.Compiled);

        public ParsedConfig Parse(string filePath, string rawText)
        {
            var config = new ParsedConfig
            {
                FilePath = filePath,
                FileType = FileType.Bat,
                RawText = rawText
            };

            var section = new ConfigSection("Variables");
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

                if (trimmed.StartsWith("::") || trimmed.StartsWith("REM ", StringComparison.OrdinalIgnoreCase) || trimmed.Equals("REM", StringComparison.OrdinalIgnoreCase))
                {
                    string comment = trimmed.StartsWith("::")
                        ? trimmed.Substring(2).Trim()
                        : trimmed.Substring(3).Trim();
                    pendingComments.Add(comment);
                    continue;
                }

                var match = SetRegex.Match(trimmed);
                if (match.Success)
                {
                    string key = match.Groups[1].Value.Trim();
                    string value = match.Groups[2].Value.Trim();

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
