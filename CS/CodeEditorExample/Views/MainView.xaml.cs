using CodeEditorExample.ViewModels;

namespace CodeEditorExample
{
    public partial class MainView
    {
        public MainView()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
