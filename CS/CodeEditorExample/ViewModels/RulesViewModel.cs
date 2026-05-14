using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Xpf;
using DevExpress.Xpf.CodeView;
using CodeEditor.Theming;
using CodeEditorExample.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CodeEditorExample.ViewModels {
    public class RulesViewModel : ViewModelBase {
        protected ObservableCollection<MonacoThemeRule> rules;

        public IMessageBoxService MessageBoxService => GetService<IMessageBoxService>();

        public ObservableCollection<MonacoThemeRule> Rules {
            get {
                if(rules == null) {
                    rules = new ObservableCollection<MonacoThemeRule>();
                }

                return rules;
            }
        }

        public string? RawRulesText {
            get { return GetValue<string?>(); }
            set { SetValue(value); }
        }

        public bool IsRawRulesMode {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        [Command]
        public void ApplyRules() {
            RawRulesText = MonacoRulesParser.Serialize(Rules);
        }

        [Command]
        public void ApplyJS() {
            if(!MonacoRulesParser.TryParse(RawRulesText ?? string.Empty, out List<MonacoThemeRule> parsed)) {
                MessageBoxService?.ShowMessage(
                    "Failed to parse rules. Please check the format.",
                    "Error",
                    MessageButton.OK,
                    MessageIcon.Error);

                IsRawRulesMode = true;
            }

            Rules.Clear();
            Rules.AddRange(parsed);
        }

        [Command]
        public void ApplyRulesChanges() {
            if(IsRawRulesMode) {
                ApplyJS();
            } else {
                ApplyRules();
            }
        }

        [Command]
        public void AddingNewRule(NewRowArgs args) {
            args.Item = new MonacoThemeRule() {
                Token = "new-token"
            };
        }

        [Command]
        public void Initialized() {
            ApplyRulesChanges();
        }
    }
}
