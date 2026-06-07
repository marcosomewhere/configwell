namespace ConfigWell.WinForms.Models
{
    public enum ValueKind
    {
        Text,
        Boolean,
        Number,
        Unknown
    }

    public class ConfigEntry
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Comment { get; set; }
        public ValueKind Kind { get; set; }
        public bool IsHeuristic { get; set; }
        public int LineNumber { get; set; }

        public ConfigEntry()
        {
            Key = string.Empty;
            Value = string.Empty;
            Comment = string.Empty;
            Kind = ValueKind.Text;
        }

        public static ValueKind DetectKind(string value)
        {
            if (string.IsNullOrEmpty(value))
                return ValueKind.Text;

            string lower = value.Trim().ToLowerInvariant();

            if (lower == "true" || lower == "false" ||
                lower == "yes" || lower == "no" ||
                lower == "on" || lower == "off" ||
                lower == "enabled" || lower == "disabled" ||
                lower == "1" || lower == "0")
                return ValueKind.Boolean;

            double dummy;
            if (double.TryParse(value.Trim(), System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out dummy))
                return ValueKind.Number;

            return ValueKind.Text;
        }
    }
}
