using System.Collections.Generic;

namespace ConfigWell.WinForms.Models
{
    public class ConfigSection
    {
        public string Name { get; set; }
        public List<ConfigEntry> Entries { get; set; }

        public ConfigSection()
        {
            Name = string.Empty;
            Entries = new List<ConfigEntry>();
        }

        public ConfigSection(string name) : this()
        {
            Name = name;
        }
    }
}
