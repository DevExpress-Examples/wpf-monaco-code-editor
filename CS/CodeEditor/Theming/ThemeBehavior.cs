using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace CodeEditor.Theming {
    public class ThemeBehavior : Behavior<CodeEditor> {

        public static readonly DependencyProperty ApplyDevExpressColorsProperty = DependencyProperty.Register(nameof(ApplyDevExpressColors), typeof(bool), typeof(ThemeBehavior), new PropertyMetadata(true, OnApplyDevExpressColorsChanged));

        public static readonly DependencyProperty RulesProperty = DependencyProperty.Register(nameof(Rules), typeof(IReadOnlyList<MonacoThemeRule>), typeof(ThemeBehavior), new PropertyMetadata(null, OnRulesChanged));

        static ThemeBehavior() {
            if(!CompatibilitySettings.UseLightweightThemes) {
                throw new InvalidOperationException("Lightweight themes must be used to use MonacoThemeBehavior.");
            }
        }

        public bool ApplyDevExpressColors {
            get { return (bool)GetValue(ApplyDevExpressColorsProperty); }
            set { SetValue(ApplyDevExpressColorsProperty, value); }
        }

        public IReadOnlyList<MonacoThemeRule>? Rules {
            get => (IReadOnlyList<MonacoThemeRule>?)GetValue(RulesProperty);
            set => SetValue(RulesProperty, value);
        }

        static void OnRulesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            ThemeBehavior behavior = (ThemeBehavior)sender;
            behavior.ApplyCurrentTheme();
        }

        protected override void OnAttached() {
            base.OnAttached();
            LightweightThemeManager.CurrentThemeChanged += LightweightThemeManager_CurrentThemeChanged;
            AssociatedObject.EditorInitialized += AssociatedObject_EditorInitialized;
        }

        static void OnApplyDevExpressColorsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            ThemeBehavior behavior = (ThemeBehavior)sender;
            behavior.ApplyCurrentTheme();
        }

        void LightweightThemeManager_CurrentThemeChanged(object sender, ValueChangedEventArgs<LightweightTheme> e) {
            ApplyCurrentTheme();
        }

        void AssociatedObject_EditorInitialized(object? sender, EventArgs e) {
            ApplyCurrentTheme();
        }

        void AdjustDXColors(MonacoTheme monacoTheme, string dxTheme) {
            if(monacoTheme.Colors == null)
                return;

            bool useSystemColors = ApplicationThemeHelper.ApplicationThemeName.Contains("SystemColors");

            switch(dxTheme) {
                case "Win11Light":
                    monacoTheme.Colors[MonacoColorKeys.SelectionBackground] = monacoTheme.Colors[MonacoColorKeys.SelectionBackground].Lighten(0.5);
                    monacoTheme.Colors[MonacoColorKeys.InactiveSelectionBackground] = monacoTheme.Colors[MonacoColorKeys.InactiveSelectionBackground].Lighten(0.5);
                    break;
                case "VS2019Light":
                    monacoTheme.Colors[MonacoColorKeys.SelectionBackground] = monacoTheme.Colors[MonacoColorKeys.SelectionBackground].Lighten(0.5);
                    monacoTheme.Colors[MonacoColorKeys.InactiveSelectionBackground] = monacoTheme.Colors[MonacoColorKeys.InactiveSelectionBackground].Lighten(0.5);
                    break;
                case "Office2019Colorful":
                case "Win10Light":
                case "VS2019Blue":
                    monacoTheme.Colors[MonacoColorKeys.SuggestWidgetSelectedBackground] = monacoTheme.Colors[MonacoColorKeys.SuggestWidgetSelectedBackground].Darken(0.3);
                    break;
                case "Win11Dark":
                    monacoTheme.Colors[MonacoColorKeys.SelectionBackground] = monacoTheme.Colors[MonacoColorKeys.SelectionBackground].Darken(useSystemColors ? 0.2 : 0.4);
                    monacoTheme.Colors[MonacoColorKeys.InactiveSelectionBackground] = monacoTheme.Colors[MonacoColorKeys.InactiveSelectionBackground].Darken(useSystemColors ? 0.2 : 0.4);
                    monacoTheme.Colors[MonacoColorKeys.SuggestWidgetSelectedBackground] = monacoTheme.Colors[MonacoColorKeys.SuggestWidgetSelectedBackground].Darken(useSystemColors ? 0.2 : 0.4);
                    break;
                case "Win10Dark":
                    monacoTheme.Colors[MonacoColorKeys.SelectionBackground] = monacoTheme.Colors[MonacoColorKeys.SelectionBackground].Darken(0.2);
                    monacoTheme.Colors[MonacoColorKeys.InactiveSelectionBackground] = monacoTheme.Colors[MonacoColorKeys.InactiveSelectionBackground].Darken(0.2);
                    break;
                case "VS2019Dark":
                    monacoTheme.Colors[MonacoColorKeys.SelectionBackground] = monacoTheme.Colors[MonacoColorKeys.SelectionBackground].Darken(0.1);
                    monacoTheme.Colors[MonacoColorKeys.InactiveSelectionBackground] = monacoTheme.Colors[MonacoColorKeys.InactiveSelectionBackground].Darken(0.1);
                    break;
                default:
                    break;
            }
        }

        void ApplyCurrentTheme() {
            LightweightTheme dxTheme = LightweightThemeManager.CurrentTheme;

            MonacoTheme monacoTheme = CreateFromDXTheme(dxTheme.Name, Rules, ApplyDevExpressColors);
            AdjustDXColors(monacoTheme, dxTheme.Name);

            AssociatedObject.RegisterTheme(monacoTheme);
            AssociatedObject.ThemeName = monacoTheme.Name;
        }

        public static Color? GetColor(string colorKey) {
            ResourceDictionary palette = LightweightThemeManager.CurrentTheme.Palette;
            string key = $"Color.{colorKey}";

            if(palette.Contains(key) && palette[key] is Color color) {
                return color;
            }

            return null;
        }

        public static MonacoTheme CreateFromDXTheme(string DXThemeName, IReadOnlyList<MonacoThemeRule>? rules = null, bool applyDevExpressColors = false) {
            MonacoThemeBase baseKind = ResolveBase(DXThemeName);

            MonacoTheme result = new MonacoTheme {
                Name = $"{DXThemeName.ToLowerInvariant()}",
                Base = baseKind,
                Colors = applyDevExpressColors ? CreateMonacoColors() : null,
                Rules = rules
            };

            return result;
        }

        static Dictionary<string, Color> CreateMonacoColors() {
            Dictionary<string, Color> result = new Dictionary<string, Color>();

            Color? Try(params string[] keys) {
                foreach(string key in keys) {
                    Color? color = GetColor(key);
                    if(color.HasValue)
                        return color;
                }

                return null;
            }

            void Map(string monacoKey, params string[] dxKeys) {
                Color? color = Try(dxKeys);
                if(color.HasValue)
                    result[monacoKey] = color.Value;
            }

            // === Base ===
            Map(MonacoColorKeys.EditorBackground,
                "Editor.Background",
                "Control.Background",
                "WindowBackground");

            Map(MonacoColorKeys.EditorForeground,
                "Foreground.Primary",
                "Foreground",
                "Editor.Foreground");

            // === Line numbers ===
            Map(MonacoColorKeys.LineNumberForeground,
                "Foreground.Secondary",
                "Foreground.Disabled",
                "Foreground");

            Map(MonacoColorKeys.LineNumberActiveForeground,
                "Foreground.Primary",
                "Foreground");

            // === Cursor & selection ===
            Map(MonacoColorKeys.CursorForeground,
                "Accent",
                "Foreground.Primary",
                "Foreground");

            Map(MonacoColorKeys.SelectionBackground,
                "Accent",
                "SelectionBackground",
                "Selection");

            Map(MonacoColorKeys.InactiveSelectionBackground,
                "SelectionBackground",
                "Selection",
                "Accent");

            // === Current line ===
            Map(MonacoColorKeys.LineHighlightBackground,
                "Control.Background",
                "Editor.Background");

            // === Gutter ===
            Map(MonacoColorKeys.GutterBackground,
                "Editor.Background",
                "Control.Background");

            // === Indent guides ===
            Map(MonacoColorKeys.IndentGuideBackground,
                "Delimiter",
                "Border");

            Map(MonacoColorKeys.IndentGuideActiveBackground,
                "Foreground.Secondary",
                "Foreground");

            // === Scrollbar ===
            Map(MonacoColorKeys.ScrollbarShadow,
                "Delimiter",
                "Control.Background");

            Map(MonacoColorKeys.ScrollbarSliderBackground,
                "Border",
                "Button.Background");

            Map(MonacoColorKeys.ScrollbarSliderHoverBackground,
                "Delimiter",
                "Backstage.Delimiter",
                "Control.Background");

            Map(MonacoColorKeys.ScrollbarSliderActiveBackground,
                "Border",
                "Control.Background");

            // === Brackets ===
            Map(MonacoColorKeys.BracketMatchBackground,
                "Control.Background",
                "Editor.Background");

            Map(MonacoColorKeys.BracketMatchBorder,
                "Accent",
                "Border");

            // === Find ===
            Map(MonacoColorKeys.FindMatchBackground,
                "Accent",
                "SelectionBackground",
                "Selection");

            Map(MonacoColorKeys.FindMatchHighlightBackground,
                "SelectionBackground",
                "Selection",
                "Accent");

            // === Hover ===
            Map(MonacoColorKeys.HoverWidgetBackground,
                "Delimiter",
                "Backstage.Delimiter",
                "Control.Background");

            Map(MonacoColorKeys.HoverWidgetBorder,
                "Editor.Border",
                "Border");

            // === Suggest ===
            Map(MonacoColorKeys.SuggestWidgetBackground,
                "FlyoutBackground",
                "PanelBackground",
                "Control.Background");

            Map(MonacoColorKeys.SuggestWidgetSelectedBackground,
                "SelectionBackground",
                "Selection",
                "Accent");

            return result;
        }

        static MonacoThemeBase ResolveBase(string themeName) {
            if(string.IsNullOrWhiteSpace(themeName))
                return MonacoThemeBase.Light;

            if(themeName.Contains("HighContrast", StringComparison.OrdinalIgnoreCase))
                return MonacoThemeBase.HighContrast;

            if(themeName.Contains("Dark", StringComparison.OrdinalIgnoreCase) ||
                themeName.Contains("Black", StringComparison.OrdinalIgnoreCase))
                return MonacoThemeBase.Dark;

            return MonacoThemeBase.Light;
        }

        protected override void OnDetaching() {
            if(AssociatedObject != null) {
                AssociatedObject.EditorInitialized -= AssociatedObject_EditorInitialized;
            }
            LightweightThemeManager.CurrentThemeChanged -= LightweightThemeManager_CurrentThemeChanged;

            base.OnDetaching();
        }
    }

}

