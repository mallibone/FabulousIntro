using Windows.Foundation;
using Windows.UI.ViewManagement;

namespace AsyncDemo.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            LoadApplication(new AsyncDemo.App());
        }
    }
}
