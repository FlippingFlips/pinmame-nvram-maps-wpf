using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Documents;

namespace PinMAME.NvMaps.Win.Shell.Help
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : UserControl
    {
        public About()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            string link = ((Hyperlink)sender).NavigateUri.ToString();
            Process.Start(link);
        }
    }
}
