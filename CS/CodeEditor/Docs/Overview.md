## Introduction

`CodeEditor` is a WPF control that embeds the [Monaco Editor](https://microsoft.github.io/monaco-editor/) (the editor engine used by Visual Studio Code) using [Microsoft WebView2](https://learn.microsoft.com/en-us/microsoft-edge/webview2/).

The editor includes modern code editing capabilities — syntax highlighting, folding, minimap, theming, and custom languages — within WPF applications.

The primary editing surface is the `CodeEditor` control, which exposes a focused, WPF-friendly API.

The library also includes:

- `ThemeBehavior` - integrates Monaco with DevExpress themes.
- `CodeEditorService` - a lightweight service for cases where direct interaction with the control is not sufficient.

The control is designed as a **state-driven wrapper**: WPF owns the editor state through dependency properties, and Monaco reflects that state while propagating changes back to WPF in an MVVM-friendly way.

> **Supported Monaco version:** 0.55.1  
> The control is built and validated against this specific release.

## Prerequisites

- Windows with [Microsoft Edge WebView2 Runtime](https://learn.microsoft.com/en-us/microsoft-edge/webview2/concepts/distribution)
- DevExpress WPF **26.1** (or newer compatible version)

The required .NET version depends on the DevExpress WPF version in use.  
See DevExpress documentation:  
[WPF Controls → Prerequisites → .NET / .NET Core](https://docs.devexpress.com/WPF/8091/prerequisites#netnet-core)

> The `CodeEditor` project must be compiled against the same DevExpress WPF version that is used in the consuming application.

## Quick Start

### 1. Add the `CodeEditor` project

Add the `CodeEditor` project to your solution and reference it from your WPF application:

- Add existing project → `CodeEditor`
- Add project reference from your WPF app
- Ensure DevExpress WPF and WebView2 dependencies are resolved

### 2. Declare the XML namespace

```xml
<Window
    x:Class="YourApp.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:ce="clr-namespace:CodeEditor;assembly=CodeEditor">

    <Grid>
        <ce:CodeEditor
            Text="{Binding Code, Mode=TwoWay}"
            EditorLanguage="csharp"
            ThemeName="vs-dark" />
    </Grid>

</Window>
```

### 3. Create a ViewModel

```csharp
using DevExpress.Mvvm;

public class MainViewModel : BindableBase
{
    private string _code = @"using System;

class Program
{
    static void Main()
    {
        Console.WriteLine(""Hello, Monaco!"");
    }
}";

    public string Code
    {
        get => _code;
        set => SetProperty(ref _code, value);
    }
}
```

Set the `DataContext` of the window to an instance of `MainViewModel`.

The editor content is automatically synchronized with the bound property.

> **Note:**  
> `CodeEditor` does not automatically track DevExpress theme changes.  
> To synchronize Monaco with the active DevExpress theme, use `ThemeBehavior`.

## Theming

`CodeEditor` supports built-in Monaco themes, DevExpress theme integration, and custom theme registration.

### Built-in Monaco Themes

You can use any of the standard Monaco themes:

```csharp
editor.ThemeName = "vs";
editor.ThemeName = "vs-dark";
editor.ThemeName = "hc-black";
```

These themes are provided by Monaco.

### DevExpress Theme Integration

By default, `CodeEditor` does not automatically track DevExpress theme changes.

To synchronize Monaco with the active DevExpress theme, attach `ThemeBehavior`:

```xml
xmlns:ce="clr-namespace:CodeEditor;assembly=CodeEditor"
xmlns:cet="clr-namespace:CodeEditor.Theming;assembly=CodeEditor"
xmlns:dxmvvm="http://schemas.devexpress.com/winfx/2008/xaml/mvvm"

...

<ce:CodeEditor
    Text="{Binding Code}"
    EditorLanguage="csharp">

    <dxmvvm:Interaction.Behaviors>
        <cet:ThemeBehavior ApplyDevExpressColors="True" />
    </dxmvvm:Interaction.Behaviors>

</ce:CodeEditor>
```

`ThemeBehavior`:

- Detects the current DevExpress theme
- Resolves Light / Dark / High Contrast base
- Registers and applies a corresponding Monaco theme
- Optionally maps DevExpress palette colors to Monaco color keys via the `ApplyDevExpressColors` property

The theme is updated automatically when the DevExpress theme changes.

### Custom Themes

You can register a custom Monaco theme programmatically:

```csharp
var theme = new MonacoTheme
{
    Name = "my-theme",
    Base = MonacoThemeBase.Dark,
    Colors = new Dictionary<string, Color>
    {
        ["editor.background"] = Color.FromRgb(30, 30, 30),
        ["editor.foreground"] = Color.FromRgb(220, 220, 220)
    }
};

editor.RegisterTheme(theme);
editor.ThemeName = "my-theme";
```

For a complete list of commonly used Monaco theme color keys, see the official VS Code theme color reference (Monaco uses the same keys): [Theme Color](https://code.visualstudio.com/api/references/theme-color).

### Rules

In addition to UI colors, Monaco themes support token-level styling via `Rules`.

Rules define how specific token types (such as `keyword`, `comment`, `string`, and other) are rendered.

You can use `Rules` to override the appearance of existing token types or to define styling for custom tokens introduced by a custom language definition.

```csharp
var theme = new MonacoTheme
{
    Name = "my-theme",
    Base = MonacoThemeBase.Dark,
    Rules = new[]
    {
        new MonacoThemeRule
        {
            Token = "keyword",
            Foreground = Color.FromRgb(86, 156, 214),
            FontStyle = MonacoFontStyle.Bold
        },
        new MonacoThemeRule
        {
            Token = "comment",
            Foreground = Color.FromRgb(106, 153, 85),
            FontStyle = MonacoFontStyle.Italic
        }
    }
};
```

`Rules` is an init-only property and must be provided during theme initialization.

If you use `ThemeBehavior`, token styling can also be supplied through its `Rules` property, and it allows rule customization alongside DevExpress theme integration.

## Languages

`CodeEditor` supports both built-in Monaco languages and custom language registration.

### Built-in Languages

To use a built-in Monaco language, specify the `EditorLanguage` property:

```csharp
editor.EditorLanguage = "csharp";
editor.EditorLanguage = "javascript";
editor.EditorLanguage = "json";
```

The value must match a language ID supported in Monaco Editor.

### Custom Languages

You can register custom languages using `LanguageDescriptor`:

```csharp
var language = new LanguageDescriptor
{
    Id = "mylang",

    Monarch = @"{
        tokenizer: {
            root: [
                [/\b(if|else|while)\b/, 'keyword'],
                [/\/\/.*$/, 'comment'],
                [/"".*?""/, 'string'],
                [/\d+/, 'number']
            ]
        }
    }",

    Configuration = @"{
        comments: {
            lineComment: '//'
        },
        brackets: [
            ['{', '}'],
            ['[', ']'],
            ['(', ')']
        ]
    }"
};

editor.RegisterLanguage(language);
editor.EditorLanguage = "mylang";
```

### Monarch Definition

The `Monarch` property defines the syntax highlighting rules.

The property must contain a valid Monaco Monarch tokenizer definition specified as a JavaScript object literal.

All standard Monarch features are supported:

- Multiple states
- Nested states
- Regular expressions
- Rule objects
- Includes and state transitions

Refer to the following topic for additional information: [Monarch Documentation](https://microsoft.github.io/monaco-editor/monarch.html)

### Language Configuration

The optional `Configuration` property defines editor behavior such as:

- Line and block comments
- Brackets
- Auto-closing pairs
- Surrounding pairs

The configuration must also be provided as a JavaScript object literal.

For language configuration details (brackets, comments, auto-closing pairs, etc.),  
refer to the following topic: [Language Configuration Guide](https://code.visualstudio.com/api/language-extensions/language-configuration-guide)

## See also

### API Reference

- [CodeEditor API Reference](CodeEditor.md)
- [CodeEditorService API Reference](CodeEditorService.md)
- [ThemeBehavior API Reference](ThemeBehavior.md)

### Monaco Resources

- [Monaco Editor](https://microsoft.github.io/monaco-editor/)
- [Monarch Documentation](https://microsoft.github.io/monaco-editor/monarch.html)
- [Monaco Playground – Custom Languages Example](https://microsoft.github.io/monaco-editor/playground.html?source=v0.55.1#example-extending-language-services-custom-languages)