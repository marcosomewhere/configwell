using System.Collections.Generic;
using System.Text;

namespace ConfigWell.WinForms.Models
{
    public class ParsedConfig
    {
        public string FilePath { get; set; }
        public FileType FileType { get; set; }
        public Encoding Encoding { get; set; }
        public List<ConfigSection> Sections { get; set; }
        public string RawText { get; set; }
        public bool HasUnsavedChanges { get; set; }

        public ParsedConfig()
        {
            FilePath = string.Empty;
            Sections = new List<ConfigSection>();
            RawText = string.Empty;
            Encoding = Encoding.UTF8;
        }

        public int TotalEntryCount
        {
            get
            {
                int count = 0;
                foreach (var section in Sections)
                    count += section.Entries.Count;
                return count;
            }
        }
    }
}
