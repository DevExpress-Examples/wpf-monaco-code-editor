namespace CodeEditor.Models {
    public sealed class LanguageDescriptor {
        public string Id { get; init; } = null!;

        //String with JS object definition of monarch, see https://microsoft.github.io/monaco-editor/monarch.html
        public string? Monarch { get; init; }

        //String with JS object definition of language configuration
        public string? Configuration { get; init; }
    }
}
