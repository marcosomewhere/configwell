using System;
using System.Collections.Generic;
using System.Xml;
using ConfigWell.WinForms.Models;

namespace ConfigWell.WinForms.Parsers
{
    public class XmlConfigParser : IConfigParser
    {
        public ParsedConfig Parse(string filePath, string rawText)
        {
            var config = new ParsedConfig
            {
                FilePath = filePath,
                FileType = FileType.XmlConfig,
                RawText = rawText
            };

            var section = new ConfigSection("appSettings");
            config.Sections.Add(section);

            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(rawText);

                // Try appSettings first
                var appSettings = doc.SelectNodes("//appSettings/add");
                if (appSettings != null && appSettings.Count > 0)
                {
                    foreach (XmlNode node in appSettings)
                    {
                        string key = node.Attributes?["key"]?.Value ?? string.Empty;
                        string value = node.Attributes?["value"]?.Value ?? string.Empty;
                        string comment = GetPrecedingComment(node);

                        var entry = new ConfigEntry
                        {
                            Key = key,
                            Value = value,
                            Comment = comment,
                            Kind = ConfigEntry.DetectKind(value)
                        };
                        section.Entries.Add(entry);
                    }
                    return config;
                }

                // Fall back: collect all leaf-node attributes that look like key/value
                section.Name = "xml";
                CollectAllKeyValueNodes(doc.DocumentElement, config, section);
            }
            catch (XmlException ex)
            {
                var errorSection = new ConfigSection("(parse error)");
                errorSection.Entries.Add(new ConfigEntry
                {
                    Key = "error",
                    Value = ex.Message,
                    Kind = ValueKind.Text,
                    IsHeuristic = true
                });
                config.Sections.Add(errorSection);
            }

            return config;
        }

        private void CollectAllKeyValueNodes(XmlNode node, ParsedConfig config, ConfigSection section, string prefix = "")
        {
            if (node == null) return;

            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.NodeType != XmlNodeType.Element) continue;

                XmlAttribute keyAttr = child.Attributes?["key"] ?? child.Attributes?["name"];
                XmlAttribute valueAttr = child.Attributes?["value"];

                if (keyAttr != null && valueAttr != null)
                {
                    // <add key="x" value="y" /> or <item name="x" value="y" />
                    section.Entries.Add(new ConfigEntry
                    {
                        Key = keyAttr.Value,
                        Value = valueAttr.Value,
                        Comment = GetPrecedingComment(child),
                        Kind = ConfigEntry.DetectKind(valueAttr.Value),
                        IsHeuristic = false
                    });
                }
                else if (!HasElementChildren(child) && !string.IsNullOrWhiteSpace(child.InnerText))
                {
                    // <host>localhost</host> — leaf text node, unambiguous key-value
                    string key = string.IsNullOrEmpty(prefix) ? child.Name : prefix + "/" + child.Name;
                    string val = child.InnerText.Trim();
                    section.Entries.Add(new ConfigEntry
                    {
                        Key = key,
                        Value = val,
                        Comment = GetPrecedingComment(child),
                        Kind = ConfigEntry.DetectKind(val),
                        IsHeuristic = false
                    });
                }
                else if (child.Attributes != null && child.Attributes.Count > 0 && !HasElementChildren(child))
                {
                    // <server host="x" port="y" /> — multi-attribute leaf, decomposed heuristically
                    string entryPrefix = string.IsNullOrEmpty(prefix) ? child.Name : prefix + "/" + child.Name;
                    string parentComment = GetPrecedingComment(child);
                    foreach (XmlAttribute attr in child.Attributes)
                    {
                        section.Entries.Add(new ConfigEntry
                        {
                            Key = entryPrefix + "." + attr.Name,
                            Value = attr.Value,
                            Comment = parentComment,
                            Kind = ConfigEntry.DetectKind(attr.Value),
                            IsHeuristic = true
                        });
                    }
                }
                else
                {
                    string nextPrefix = string.IsNullOrEmpty(prefix) ? child.Name : prefix + "/" + child.Name;
                    CollectAllKeyValueNodes(child, config, section, nextPrefix);
                }
            }
        }

        private static bool HasElementChildren(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
                if (child.NodeType == XmlNodeType.Element) return true;
            return false;
        }

        private string GetPrecedingComment(XmlNode node)
        {
            XmlNode prev = node.PreviousSibling;
            while (prev != null && prev.NodeType == XmlNodeType.Whitespace)
                prev = prev.PreviousSibling;

            if (prev != null && prev.NodeType == XmlNodeType.Comment)
                return prev.Value?.Trim() ?? string.Empty;

            return string.Empty;
        }
    }
}
