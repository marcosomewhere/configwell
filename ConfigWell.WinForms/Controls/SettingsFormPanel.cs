using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ConfigWell.WinForms.Models;

namespace ConfigWell.WinForms.Controls
{
    public class SettingsFormPanel : Panel
    {
        private ParsedConfig _config;
        private readonly Dictionary<ConfigEntry, Control> _controls = new Dictionary<ConfigEntry, Control>();
        public event EventHandler ConfigChanged;

        public SettingsFormPanel()
        {
            AutoScroll = true;
            Padding = new Padding(8);
            BackColor = SystemColors.Window;
        }

        public void LoadConfig(ParsedConfig config)
        {
            _config = config;
            _controls.Clear();
            Controls.Clear();
            RenderForm();
        }

        private void RenderForm()
        {
            if (_config == null) return;

            int y = 8;
            const int labelWidth = 200;
            const int controlWidth = 280;
            const int rowHeight = 48;
            const int commentHeight = 16;
            int totalWidth = labelWidth + controlWidth + 40;

            foreach (var section in _config.Sections)
            {
                if (section.Entries.Count == 0) continue;

                // Section header
                var sectionLabel = new Label
                {
                    Text = section.Name,
                    Left = 4,
                    Top = y,
                    Width = totalWidth,
                    Height = 22,
                    Font = new Font(Font.FontFamily, 9.5f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(0, 80, 160),
                    BackColor = Color.FromArgb(230, 240, 255)
                };
                Controls.Add(sectionLabel);
                y += 26;

                foreach (var entry in section.Entries)
                {
                    int entryY = y;
                    bool hasComment = !string.IsNullOrEmpty(entry.Comment);

                    // Key label
                    var keyLabel = new Label
                    {
                        Text = entry.Key,
                        Left = 4,
                        Top = entryY + 4,
                        Width = labelWidth - 8,
                        Height = 20,
                        Font = new Font(Font.FontFamily, 8.5f, FontStyle.Regular),
                        ForeColor = SystemColors.ControlText,
                        TextAlign = ContentAlignment.MiddleLeft
                    };

                    if (entry.IsHeuristic)
                    {
                        keyLabel.ForeColor = Color.DarkOrange;
                        keyLabel.Text = entry.Key + " *";
                        new ToolTip().SetToolTip(keyLabel, "Heuristisch erkannt — Wert möglicherweise ungenau");
                    }

                    Controls.Add(keyLabel);

                    // Value control
                    Control valueControl = null;

                    if (entry.Kind == ValueKind.Boolean)
                    {
                        var cb = new CheckBox
                        {
                            Left = labelWidth,
                            Top = entryY + 4,
                            Width = controlWidth,
                            Height = 22,
                            Checked = IsTruthy(entry.Value),
                            Text = string.Empty
                        };
                        cb.CheckedChanged += (s, e) =>
                        {
                            string oldVal = entry.Value.ToLowerInvariant();
                            if (oldVal == "yes" || oldVal == "no")
                                entry.Value = cb.Checked ? "yes" : "no";
                            else if (oldVal == "on" || oldVal == "off")
                                entry.Value = cb.Checked ? "on" : "off";
                            else if (oldVal == "enabled" || oldVal == "disabled")
                                entry.Value = cb.Checked ? "enabled" : "disabled";
                            else if (oldVal == "1" || oldVal == "0")
                                entry.Value = cb.Checked ? "1" : "0";
                            else
                                entry.Value = cb.Checked ? "true" : "false";
                            _config.HasUnsavedChanges = true;
                            ConfigChanged?.Invoke(this, EventArgs.Empty);
                        };
                        valueControl = cb;
                    }
                    else
                    {
                        var tb = new TextBox
                        {
                            Left = labelWidth,
                            Top = entryY + 4,
                            Width = controlWidth,
                            Height = 22,
                            Text = entry.Value,
                            BorderStyle = BorderStyle.FixedSingle
                        };
                        tb.TextChanged += (s, e) =>
                        {
                            entry.Value = tb.Text;
                            _config.HasUnsavedChanges = true;
                            ConfigChanged?.Invoke(this, EventArgs.Empty);
                        };
                        valueControl = tb;
                    }

                    Controls.Add(valueControl);
                    _controls[entry] = valueControl;

                    if (hasComment)
                    {
                        var commentLabel = new Label
                        {
                            Text = entry.Comment,
                            Left = labelWidth,
                            Top = entryY + 26,
                            Width = controlWidth,
                            Height = commentHeight,
                            Font = new Font(Font.FontFamily, 7.5f, FontStyle.Italic),
                            ForeColor = Color.Gray,
                            AutoEllipsis = true
                        };
                        Controls.Add(commentLabel);
                        new ToolTip().SetToolTip(commentLabel, entry.Comment);
                        y += rowHeight + commentHeight;
                    }
                    else
                    {
                        y += rowHeight;
                    }
                }

                y += 8;
            }
        }

        private bool IsTruthy(string value)
        {
            string lower = (value ?? "").Trim().ToLowerInvariant();
            return lower == "true" || lower == "yes" || lower == "on" || lower == "enabled" || lower == "1";
        }

        public void RefreshValues()
        {
            if (_config == null) return;
            foreach (var section in _config.Sections)
            {
                foreach (var entry in section.Entries)
                {
                    if (!_controls.TryGetValue(entry, out Control ctrl)) continue;

                    if (ctrl is CheckBox cb)
                        cb.Checked = IsTruthy(entry.Value);
                    else if (ctrl is TextBox tb)
                        tb.Text = entry.Value;
                }
            }
        }
    }
}
