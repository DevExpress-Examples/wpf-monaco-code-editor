using DevExpress.Data.Filtering;
using System;
using System.Windows.Markup;

namespace CodeEditorExample.Common {
    public class CriteriaOperatorExtension : MarkupExtension {
        public string? FilterString { get; set; }
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return string.IsNullOrEmpty(FilterString) ? null : CriteriaOperator.Parse(FilterString);
        }
    }
}
