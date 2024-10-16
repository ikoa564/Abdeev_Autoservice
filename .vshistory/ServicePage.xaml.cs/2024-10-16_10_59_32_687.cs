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

namespace Abdeev_Autoservice
{
    /// <summary>
    /// Логика взаимодействия для ServicePage.xaml
    /// </summary>
    public partial class ServicePage : Page
    {
        public ServicePage()
        {
            InitializeComponent();
            var currentSevices = Abdeev_autoserviceEntities.GetContext().Service.ToList();
            ServiceListView.ItemsSource = currentSevices;

            ComboType.SelectedIndex = 0;

            UpdateServices()
        }
        private void UpdateServices()
        {
            var currentSevices = Abdeev_autoserviceEntities.GetContext().Service.ToList();
            if(ComboType.SelectedIndex == 0)
                currentSevices = currentSevices.Where(p => (Convert.ToInt32(p.Discount) >= 0 && Convert.ToInt32(p.Discount) <=100)).ToList();
            if(ComboType.SelectedIndex == 1)
                currentSevices = currentSevices.Where(p => (Convert.ToInt32(p.Discount) >= 0 && Convert.ToInt32(p.Discount) < 5)).ToList();
            if(ComboType.SelectedIndex == 2)
                currentSevices = currentSevices.Where(p => (Convert.ToInt32(p.Discount) >= 5 && Convert.ToInt32(p.Discount) < 15)).ToList();
            if(ComboType.SelectedIndex == 3)
                currentSevices = currentSevices.Where(p => (Convert.ToInt32(p.Discount) >= 15 && Convert.ToInt32(p.Discount) < 30)).ToList();
            if(ComboType.SelectedIndex == 4)
                currentSevices = currentSevices.Where(p => (Convert.ToInt32(p.Discount) >= 30 && Convert.ToInt32(p.Discount) < 70)).ToList();
            if(ComboType.SelectedIndex == 5)
                currentSevices = currentSevices.Where(p => (Convert.ToInt32(p.Discount) >= 70 && Convert.ToInt32(p.Discount) <=100)).ToList();
            currentSevices = currentSevices.Where(p => p.Title.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();

            ServiceListView.ItemsSource = currentSevices.ToList();

            if(RButtonDown.IsChecked.Value)
                ServiceListView.ItemsSource = currentSevices.OrderByDescending(p => p.Cost).ToList();
            if(RButtonUp.IsChecked.Value)
                ServiceListView.ItemsSource = currentSevices.OrderBy(p => p.Cost).ToList();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage());
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateServices()
        }

        private void RButtonUp_Checked(object sender, RoutedEventArgs e)
        {
            UpdateServices()
        }

        private void RButtonDown_Checked(object sender, RoutedEventArgs e)
        {
            UpdateServices()
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateServices()
        }
    }
}
