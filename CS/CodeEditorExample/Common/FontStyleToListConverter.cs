using CodeEditor.Theming;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace CodeEditorExample.Common {

    internal class FontStyleToListConverter : MarkupExtension, IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if(value == null) {
                return value;
            }

            MonacoFontStyle selectedStyles = (MonacoFontStyle)value;
            List<object> list = new List<object>();
            Array values = Enum.GetValues(typeof(MonacoFontStyle));
            foreach(object enumValue in values) {
                MonacoFontStyle style = (MonacoFontStyle)enumValue;
                if(selectedStyles.HasFlag(style)) {
                    list.Add(style);
                }
            }

            return list;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            if(value == null) {
                return value;
            }

            List<object> source = (List<object>)value;
            MonacoFontStyle result = MonacoFontStyle.None;
            foreach(object sourceValue in source) {
                MonacoFontStyle style = (MonacoFontStyle)sourceValue;
                result |= style;
            }

            return result;
        }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
    }
}
