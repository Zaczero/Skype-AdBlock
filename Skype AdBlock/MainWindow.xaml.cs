using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Skype_AdBlock
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AdManager _adManager = new AdManager();

        public MainWindow()
        {
            InitializeComponent();

            if (!_adManager.GetAdStatus())
            {
                DisableAds(null, null);
            }
        }

        private void DisableAds(object sender, RoutedEventArgs e)
        {
            // Skip disable process if function got executed directly
            if (sender != null)
            {
                _adManager.DisableAds();
            }

            DisableAdsBtn.Visibility = Visibility.Hidden;
            DoneText.Visibility = Visibility.Visible;
            EnableAdsText.Visibility = Visibility.Visible;
        }

        private void EnableAds(object sender, MouseButtonEventArgs e)
        {
            _adManager.EnableAds();
            DisableAdsBtn.Visibility = Visibility.Visible;
            DoneText.Visibility = Visibility.Hidden;
            EnableAdsText.Visibility = Visibility.Hidden;
        }
    }
}
