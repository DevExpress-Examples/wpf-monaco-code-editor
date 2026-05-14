using System.ComponentModel.DataAnnotations;
using System.Windows.Media;

namespace CodeEditor.Theming {
    public sealed class MonacoThemeRule {
        [Required]
        public required string Token { get; set; }
        public Color? Foreground { get; set; }
        public Color? Background { get; set; }
        public MonacoFontStyle? FontStyle { get; set; }
    }
}
