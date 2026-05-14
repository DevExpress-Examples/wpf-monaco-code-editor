using System;
using System.Windows.Media;

namespace CodeEditor.Theming {
    public static class ColorExtensions {
        public static Color Lighten(this Color color, double amount) {
            return ChangeLightness(color, Math.Abs(amount));
        }

        public static Color Darken(this Color color, double amount) {
            return ChangeLightness(color, -Math.Abs(amount));
        }

        static Color ChangeLightness(Color color, double delta) {
            (double h, double s, double l) = ToHsl(color);

            l = Math.Clamp(l + delta, 0, 1);

            return FromHsl(h, s, l, color.A);
        }

        static (double h, double s, double l) ToHsl(Color color) {
            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            double delta = max - min;

            double h = 0;
            double l = (max + min) / 2;
            double s = 0;

            if(delta != 0) {
                s = delta / (1 - Math.Abs(2 * l - 1));

                if(max == r)
                    h = 60 * (((g - b) / delta) % 6);
                else if(max == g)
                    h = 60 * (((b - r) / delta) + 2);
                else
                    h = 60 * (((r - g) / delta) + 4);

                if(h < 0)
                    h += 360;
            }

            return (h, s, l);
        }

        static Color FromHsl(double h, double s, double l, byte alpha) {
            double c = (1 - Math.Abs(2 * l - 1)) * s;
            double x = c * (1 - Math.Abs((h / 60) % 2 - 1));
            double m = l - c / 2;

            double r1 = 0, g1 = 0, b1 = 0;

            if(h < 60) { r1 = c; g1 = x; } else if(h < 120) { r1 = x; g1 = c; } else if(h < 180) { g1 = c; b1 = x; } else if(h < 240) { g1 = x; b1 = c; } else if(h < 300) { r1 = x; b1 = c; } else { r1 = c; b1 = x; }

            return Color.FromArgb(
                alpha,
                (byte)Math.Round((r1 + m) * 255),
                (byte)Math.Round((g1 + m) * 255),
                (byte)Math.Round((b1 + m) * 255));
        }
    }
}

