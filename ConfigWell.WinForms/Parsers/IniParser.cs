using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ConfigWell.WinForms.Models;

namespace ConfigWell.WinForms.Parsers
{
    public class IniParser : IConfigParser
    {
        private static readonly Regex SectionRegex = new Regex(@"^\[(.+)\]$", RegexOptions.Compiled);

        public ParsedConfig Parse(string filePath, string rawText)
        {
            var config = new ParsedConfig
            {
                FilePath = filePath,
                FileType = FileType.Ini,
                RawText = rawText
            };

            var currentSection = new ConfigSection("(global)");
            config.Sections.Add(currentSection);

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

                if (trimmed.StartsWith("#") || trimmed.StartsWith(";"))
                {
                    pendingComments.Add(trimmed.TrimStart('#', ';').Trim());
                    continue;
                }

                var sectionMatch = SectionRegex.Match(trimmed);
                if (sectionMatch.Success)
                {
                    currentSection = new ConfigSection(sectionMatch.Groups[1].Value);
                    config.Sections.Add(currentSection);
                    pendingComments.Clear();
                    continue;
                }

                int eqIdx = trimmed.IndexOf('=');
                if (eqIdx > 0)
                {
                    string key = trimmed.Substring(0, eqIdx).Trim();
                    string value = trimmed.Substring(eqIdx + 1).Trim();

                    // Strip inline comment
                    int inlineComment = value.IndexOf(';');
                    string inlineCommentText = string.Empty;
                    if (inlineComment > 0)
                    {
                        inlineCommentText = value.Substring(inlineComment + 1).Trim();
                        value = value.Substring(0, inlineComment).Trim();
                    }

                    string comment = string.Join(" ", pendingComments);
                    if (!string.IsNullOrEmpty(inlineCommentText))
                        comment = string.IsNullOrEmpty(comment) ? inlineCommentText : comment + " " + inlineCommentText;

                    var entry = new ConfigEntry
                    {
                        Key = key,
                        Value = value,
                        Comment = comment,
                        Kind = ConfigEntry.DetectKind(value),
                        LineNumber = i + 1
                    };
                    currentSection.Entries.Add(entry);
                }

                pendingComments.Clear();
            }

            return config;
        }
    }
}
