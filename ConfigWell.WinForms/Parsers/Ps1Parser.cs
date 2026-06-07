using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ConfigWell.WinForms.Models;

namespace ConfigWell.WinForms.Parsers
{
    // Parses .ps1: $Name = "Value", $Flag = $true, $Number = 123
    public class Ps1Parser : IConfigParser
    {
        private static readonly Regex StringAssign = new Regex(
            @"^\$([A-Za-z_][A-Za-z0-9_]*)\s*=\s*""([^""]*)""",
            RegexOptions.Compiled);
        private static readonly Regex BoolAssign = new Regex(
            @"^\$([A-Za-z_][A-Za-z0-9_]*)\s*=\s*(\$true|\$false)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex NumberAssign = new Regex(
            @"^\$([A-Za-z_][A-Za-z0-9_]*)\s*=\s*(-?[\d]+(?:\.[\d]+)?)",
            RegexOptions.Compiled);
        private static readonly Regex SingleQuoteAssign = new Regex(
            @"^\$([A-Za-z_][A-Za-z0-9_]*)\s*=\s*'([^']*)'",
            RegexOptions.Compiled);

        public ParsedConfig Parse(string filePath, string rawText)
        {
            var config = new ParsedConfig
            {
                FilePath = filePath,
                FileType = FileType.Ps1,
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

                if (trimmed.StartsWith("#"))
                {
                    pendingComments.Add(trimmed.Substring(1).Trim());
                    continue;
                }

                ConfigEntry entry = null;

                var m = BoolAssign.Match(trimmed);
                if (m.Success)
                {
                    entry = new ConfigEntry
                    {
                        Key = m.Groups[1].Value,
                        Value = m.Groups[2].Value.ToLower() == "$true" ? "true" : "false",
                        Kind = ValueKind.Boolean,
                        LineNumber = i + 1
                    };
                }

                if (entry == null)
                {
                    m = NumberAssign.Match(trimmed);
                    if (m.Success)
                    {
                        entry = new ConfigEntry
                        {
                            Key = m.Groups[1].Value,
                            Value = m.Groups[2].Value,
                            Kind = ValueKind.Number,
                            LineNumber = i + 1
                        };
                    }
                }

                if (entry == null)
                {
                    m = StringAssign.Match(trimmed);
                    if (m.Success)
                    {
                        entry = new ConfigEntry
                        {
                            Key = m.Groups[1].Value,
                            Value = m.Groups[2].Value,
                            Kind = ValueKind.Text,
                            LineNumber = i + 1
                        };
                    }
                }

                if (entry == null)
                {
                    m = SingleQuoteAssign.Match(trimmed);
                    if (m.Success)
                    {
                        entry = new ConfigEntry
                        {
                            Key = m.Groups[1].Value,
                            Value = m.Groups[2].Value,
                            Kind = ValueKind.Text,
                            LineNumber = i + 1
                        };
                    }
                }

                if (entry != null)
                {
                    entry.Comment = string.Join(" ", pendingComments);
                    section.Entries.Add(entry);
                    pendingComments.Clear();
                }
                else
                {
                    pendingComments.Clear();
                }
            }

            return config;
        }
    }
}
