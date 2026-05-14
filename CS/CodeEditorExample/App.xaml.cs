using DevExpress.Xpf.Core;
using System.Windows;

namespace CodeEditorExample
{
    public partial class App : Application
    {
        static App()
        {
            CompatibilitySettings.UseLightweightThemes = true;
            ApplicationThemeHelper.Preload(PreloadCategories.Core, PreloadCategories.Grid, PreloadCategories.Ribbon, PreloadCategories.Controls, PreloadCategories.LayoutControl);
            ApplicationThemeHelper.ApplicationThemeName = LightweightTheme.Win11Light.Name;
        }
    }
}
