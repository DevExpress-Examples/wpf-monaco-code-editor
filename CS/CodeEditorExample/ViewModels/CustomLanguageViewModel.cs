using DevExpress.Mvvm;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace CodeEditorExample.ViewModels {
    public class CustomLanguageViewModel : ViewModelBase, IDataErrorInfo {
        static readonly Regex LanguageIdRegex = new("^[a-zA-Z0-9\\-_]+$", RegexOptions.Compiled);

        public string LanguageId {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string? Monarch {
            get { return GetValue<string?>(); }
            set { SetValue(value); }
        }

        public string? Configuration {
            get { return GetValue<string?>(); }
            set { SetValue(value); }
        }

        public string Error => string.Empty;

        public string this[string columnName] {
            get {
                if(columnName == nameof(LanguageId)) {
                    if(string.IsNullOrWhiteSpace(LanguageId))
                        return "Language Name is required.";

                    if(!LanguageIdRegex.IsMatch(LanguageId))
                        return "Language Name may contain only letters, digits, '-' and '_'.";

                    if(LanguageId.Length > 50)
                        return "Language Name is too long.";
                }

                return string.Empty;
            }
        }
    }
}
