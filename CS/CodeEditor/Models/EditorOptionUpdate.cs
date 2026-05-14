using System.Text.Json.Serialization;

namespace CodeEditor.Models {
    public enum EditorOption {
        LineNumbers,
        Minimap,
        GlyphMargin,
        Folding,
        ScrollBeyondLastLine,
        ScrollBeyondLastColumn,
        ContextMenu,
        SmoothScrolling,
        DragAndDrop,
        MouseWheelZoom,
        LineNumbersMinChars,
        WordWrap,
        StickyScroll,
        TabSize,
        InsertSpaces,
        DetectIndentation,
        AutoIndent,
        EnableQuickSuggestions,
        EnableWordBasedSuggestions,
        EnableSuggestOnTriggerCharacters,
        EnableParameterHints
    }

    public sealed class EditorOptionUpdate {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EditorOption Option { get; init; }
        public object? Value { get; init; }
    }
}
