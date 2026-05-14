using DevExpress.Mvvm.UI;
using CodeEditor.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CodeEditor {
    public interface ICodeEditorService {
        void MarkAsSaved();
        Task<IReadOnlyCollection<string>> GetLanguagesAsync(CancellationToken cancellationToken = default);
        void RegisterLanguage(LanguageDescriptor language);
    }

    public class CodeEditorService : ServiceBase, ICodeEditorService {
        public async Task<IReadOnlyCollection<string>> GetLanguagesAsync(CancellationToken cancellationToken = default) {
            if(AssociatedObject is not CodeEditor editor)
                throw new InvalidOperationException(
                    "CodeEditor is not attached.");

            return await editor.GetAvailableLanguagesAsync();
        }

        public void MarkAsSaved() {
            if(AssociatedObject is CodeEditor editor) {
                editor.MarkAsSaved();
            }
        }

        public void RegisterLanguage(LanguageDescriptor language) {
            if(AssociatedObject is CodeEditor editor) {
                editor.RegisterLanguage(language);
            }
        }
    }
}
