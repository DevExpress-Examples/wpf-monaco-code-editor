using System.Collections.Generic;
using System.Windows.Media;

namespace CodeEditor.Theming {

    public enum MonacoThemeBase {
        Light,
        Dark,
        HighContrast
    }

    public sealed class MonacoTheme {
        public required string Name { get; init; }
        public MonacoThemeBase Base { get; init; }
        public bool Inherit { get; init; } = true;
        public Dictionary<string, Color>? Colors { get; init; } = new();
        public IReadOnlyList<MonacoThemeRule>? Rules { get; init; } = new List<MonacoThemeRule>();
    }
}
