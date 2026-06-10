# ThemeBehavior

**Requires:** `CompatibilitySettings.UseLightweightThemes = true`

Behavior that automatically synchronizes Monaco Editor theme with DevExpress LightweightThemes palette.

## Properties

| Name | Type | Description |
|------|------|-------------|
| `ApplyDevExpressColors` | `bool` | Gets or sets whether DevExpress palette colors are applied to Monaco Editor UI elements (background, foreground, selection, line numbers, scrollbar, brackets). This is a dependency property. Default: `true` |
| `Rules` | `IReadOnlyList<MonacoThemeRule>?` | Gets or sets optional syntax token style rules (keywords, comments, strings). This is a dependency property. |

## Static Methods

| Name | Description |
|------|-------------|
| `CreateFromDXTheme(string, IReadOnlyList<MonacoThemeRule>?, bool)` | Creates a `MonacoTheme` from a DevExpress theme name with optional rules and color mapping. |
| `GetColor(string)` | Retrieves a color from the current DevExpress theme palette by color key. Returns null if color not found. |
