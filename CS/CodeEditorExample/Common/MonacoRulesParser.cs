using CodeEditor.Theming;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace CodeEditorExample.Common {
    public static class MonacoRulesParser {

        static string? ConvertFontStyle(MonacoFontStyle style) {
            if(style == MonacoFontStyle.None)
                return null;

            StringBuilder sb = new StringBuilder(32);

            if((style & MonacoFontStyle.Bold) != 0)
                sb.Append("bold ");

            if((style & MonacoFontStyle.Italic) != 0)
                sb.Append("italic ");

            if((style & MonacoFontStyle.Underline) != 0)
                sb.Append("underline ");

            if(sb.Length == 0)
                return null;

            sb.Length--;
            return sb.ToString();
        }

        static string ToHex(Color c, bool addHashTag = true)
           => $"{(addHashTag ? "#" : string.Empty)}{c.R:X2}{c.G:X2}{c.B:X2}";

        public static string Serialize(IReadOnlyList<MonacoThemeRule> rules) {
            if(rules == null || rules.Count == 0)
                return "[]";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[");

            for(int i = 0; i < rules.Count; i++) {
                MonacoThemeRule r = rules[i];

                sb.Append("    { ");
                sb.Append($"token: \"{r.Token}\"");

                if(r.Foreground is Color fg)
                    sb.Append($", foreground: \"{ToHex(fg, false)}\"");

                if(r.Background is Color bg)
                    sb.Append($", background: \"{ToHex(bg, false)}\"");

                string? fontStyle = ConvertFontStyle(r.FontStyle ?? MonacoFontStyle.None);
                if(!string.IsNullOrEmpty(fontStyle))
                    sb.Append($", fontStyle: \"{fontStyle}\"");

                sb.Append(" }");

                if(i < rules.Count - 1)
                    sb.Append(",");

                sb.AppendLine();
            }

            sb.Append("]");

            return sb.ToString();
        }

        public static bool TryParse(
            string text,
            out List<MonacoThemeRule> rules) {
            rules = new List<MonacoThemeRule>();

            if(string.IsNullOrWhiteSpace(text))
                return false;

            try {
                string normalized = NormalizeJsObject(text);

                using JsonDocument doc = JsonDocument.Parse(normalized);

                if(doc.RootElement.ValueKind != JsonValueKind.Array)
                    return false;

                foreach(JsonElement el in doc.RootElement.EnumerateArray()) {
                    if(el.ValueKind != JsonValueKind.Object)
                        return false;

                    if(!el.TryGetProperty("token", out JsonElement tokenProp))
                        return false;

                    string? token = tokenProp.GetString();
                    if(string.IsNullOrWhiteSpace(token))
                        return false;

                    MonacoThemeRule rule = new MonacoThemeRule {
                        Token = token
                    };

                    if(el.TryGetProperty("foreground", out JsonElement fgProp)) {
                        if(!TryParseColor(fgProp.GetString(), out Color fg))
                            return false;

                        rule.Foreground = fg;
                    }

                    if(el.TryGetProperty("background", out JsonElement bgProp)) {
                        if(!TryParseColor(bgProp.GetString(), out Color bg))
                            return false;

                        rule.Background = bg;
                    }

                    if(el.TryGetProperty("fontStyle", out JsonElement fsProp)) {
                        if(!TryParseFontStyle(fsProp.GetString(), out MonacoFontStyle style))
                            return false;

                        rule.FontStyle = style;
                    }

                    rules.Add(rule);
                }

                return true;
            } catch {
                return false;
            }
        }

        static string NormalizeJsObject(string input) {
            input = Regex.Replace(input, @"//.*?$", "", RegexOptions.Multiline);

            input = Regex.Replace(input, @",(\s*[\]}])", "$1");

            input = input.Replace("'", "\"");

            input = Regex.Replace(
                input,
                @"(?<=\{|,)\s*(\w+)\s*:",
                m => $" \"{m.Groups[1].Value}\":");

            return input;
        }

        static bool TryParseColor(string? hex, out Color color) {
            color = default;

            if(string.IsNullOrWhiteSpace(hex))
                return false;

            hex = hex.TrimStart('#');

            if(hex.Length != 6)
                return false;

            try {
                byte r = Convert.ToByte(hex.Substring(0, 2), 16);
                byte g = Convert.ToByte(hex.Substring(2, 2), 16);
                byte b = Convert.ToByte(hex.Substring(4, 2), 16);

                color = Color.FromRgb(r, g, b);
                return true;
            } catch {
                return false;
            }
        }

        static bool TryParseFontStyle(string? value, out MonacoFontStyle style) {
            style = MonacoFontStyle.None;

            if(string.IsNullOrWhiteSpace(value))
                return true;

            foreach(string part in value.Split(' ', StringSplitOptions.RemoveEmptyEntries)) {
                switch(part) {
                    case "bold":
                        style |= MonacoFontStyle.Bold;
                        break;
                    case "italic":
                        style |= MonacoFontStyle.Italic;
                        break;
                    case "underline":
                        style |= MonacoFontStyle.Underline;
                        break;
                    default:
                        return false;
                }
            }

            return true;
        }
    }
}

