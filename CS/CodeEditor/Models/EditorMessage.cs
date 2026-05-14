using System.Text.Json;
using System.Text.Json.Serialization;

namespace CodeEditor.Models {

    public enum EditorMessageType {
        TextChanged,
        EditorReady,
        IsDirtyChanged,
        Languages
    }

    public sealed class EditorMessage {

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EditorMessageType Type { get; set; }
        public JsonElement Payload { get; set; }
    }
}
