# CodeEditor

WPF code editor control with syntax highlighting and other modern code display/navigation capabilities. This control embeds Monaco Editor (Visual Studio Code editor) using WebView2.

## Properties

| Name | Type | Description |
| ---- | ---- | ----------- |
| `Text` | `string` | Gets or sets the editor text. This is a dependency property. Supports two-way binding. |
| `EditorLanguage` | `string` | Gets or sets the programming language for syntax highlighting. This is a dependency property. Default: `"csharp"` |
| `ReadOnly` | `bool` | Gets or sets whether the editor is read-only. This is a dependency property. Default: `false` |
| `IsModified` | `bool` | Gets whether the content has been modified since the last save. This is a readonly dependency property. |
| `ShowLineNumbers` | `bool` | Gets or sets whether line numbers are displayed. This is a dependency property. Default: `true` |
| `LineNumbersMinChars` | `int` | Gets or sets the minimum number of character slots reserved for the line-number gutter. This is a dependency property. Default: `5` |
| `ShowMinimap` | `bool` | Gets or sets whether the minimap is displayed. This is a dependency property. Default: `false` |
| `ShowGlyphMargin` | `bool` | Gets or sets whether the glyph margin is displayed. This is a dependency property. Default: `false` |
| `EnableFolding` | `bool` | Gets or sets whether code folding is enabled. This is a dependency property. Default: `true` |
| `TabSize` | `int` | Gets or sets the tab size in spaces (1-64). This is a dependency property. Default: `4` |
| `InsertSpaces` | `bool` | Gets or sets whether spaces are inserted instead of tabs. This is a dependency property. Default: `true` |
| `DetectIndentation` | `bool` | Gets or sets whether indentation is detected from file content. This is a dependency property. Default: `true` |
| `AutoIndent` | `EditorAutoIndent` | Gets or sets auto-indentation mode. This is a dependency property. Default: `EditorAutoIndent.Full` |
| `WordWrap` | `EditorWordWrap` | Gets or sets word wrap mode. This is a dependency property. Default: `EditorWordWrap.Off` |
| `EnableScrollBeyondLastLine` | `bool` | Gets or sets whether scrolling beyond the last line is allowed. This is a dependency property. Default: `true` |
| `ScrollBeyondLastColumn` | `int` | Gets or sets the number of columns to scroll beyond the last character. This is a dependency property. Default: `5` |
| `EnableSmoothScrolling` | `bool` | Gets or sets whether smooth scrolling is enabled. This is a dependency property. Default: `false` |
| `EnableStickyScroll` | `bool` | Gets or sets whether sticky scroll is enabled. This is a dependency property. Default: `true` |
| `EnableContextMenu` | `bool` | Gets or sets whether the context menu is enabled. This is a dependency property. Default: `true` |
| `EnableDragAndDrop` | `bool` | Gets or sets whether drag and drop is enabled. This is a dependency property. Default: `true` |
| `EnableMouseWheelZoom` | `bool` | Gets or sets whether zooming with <kbd>Ctrl</kbd>+<kbd>MouseWheel</kbd> is enabled. This is a dependency property. Default: `false` |
| `EnableQuickSuggestions` | `bool` | Gets or sets whether quick suggestions are enabled. This is a dependency property. Default: `true` |
| `EnableWordBasedSuggestions` | `bool` | Gets or sets whether word-based suggestions are enabled. This is a dependency property. Default: `true` |
| `EnableSuggestOnTriggerCharacters` | `bool` | Gets or sets whether suggestions appear automatically when typing trigger characters. This is a dependency property. Default: `true` |
| `EnableParameterHints` | `bool` | Gets or sets whether parameter hints are displayed. This is a dependency property. Default: `true` |
| `ThemeName` | `string` | Gets or sets the Monaco Editor theme name. This is a dependency property. Default: `"vs"` |
| `MarkAsSavedCommand` | `ICommand` | Gets the command that resets the `IsModified` flag. |

## Methods

| Name | Description |
| ---- | ----------- |
| `GetAvailableLanguagesAsync(CancellationToken)` | Asynchronously retrieves a list of available programming language IDs. |
| `RegisterLanguage(LanguageDescriptor)` | Registers a custom programming language with a Monarch tokenizer. |
| `RegisterTheme(MonacoTheme)` | Registers a custom Monaco Editor theme. |
| `MarkAsSaved()` | Resets the `IsModified` property to `false`. |
| `Dispose()` | Releases WebView2 resources. |

## Events

| Name | Description |
| ---- | ----------- |
| `EditorInitialized` | Occurs when the Monaco Editor is fully initialized and ready for commands. |

## Enums

### EditorAutoIndent

| Member | Description |
| ------ | ----------- |
| `None` | No automatic indentation |
| `Keep` | Keep current indentation |
| `Brackets` | Indent based on brackets |
| `Advanced` | Advanced indentation |
| `Full` | Full automatic indentation (recommended) |

### EditorWordWrap

| Member | Description |
| ------ | ----------- |
| `Off` | No word wrapping |
| `On` | Word wrapping enabled |
