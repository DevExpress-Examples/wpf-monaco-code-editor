using System.Text.Json.Serialization;

namespace CodeEditor.Models {
    public enum EditorCommandType {
        SetText,
        SetReadOnly,
        MarkAsSaved,
        GetLanguages,
        SetLanguage,
        UpdateOption,
        RegisterTheme,
        SetTheme,
        RegisterLanguage
    }

    public sealed class EditorCommand {
        // The JsonConverter attribute ensures that the enum is serialized as a string in JSON, which is easier to read and debug.
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EditorCommandType Type { get; set; }
        public object? Payload { get; set; }
    }
}
