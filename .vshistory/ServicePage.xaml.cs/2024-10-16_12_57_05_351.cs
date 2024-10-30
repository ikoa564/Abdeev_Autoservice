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

            UpdateServices();
        }
        private void UpdateServices()
        {
            var currentSevices = Abdeev_autoserviceEntities.GetContext().Service.ToList();
            if(ComboType.SelectedIndex == 0)
                currentSevices = currentSevices.Where(p => (Convert.ToDouble(p.Discount) >= 0 && Convert.ToDouble(p.Discount) <=1)).ToList();
            if(ComboType.SelectedIndex == 1)
                currentSevices = currentSevices.Where(p => (Convert.ToDouble(p.Discount) >= 0 && Convert.ToDouble(p.Discount) < 0.05)).ToList();
            if(ComboType.SelectedIndex == 2)
                currentSevices = currentSevices.Where(p => (Convert.ToDouble(p.Discount) >= 0.05 && Convert.ToDouble(p.Discount) < 0.15)).ToList();
            if(ComboType.SelectedIndex == 3)
                currentSevices = currentSevices.Where(p => (Convert.ToDouble(p.Discount) >= 0.15 && Convert.ToDouble(p.Discount) < 0.30)).ToList();
            if(ComboType.SelectedIndex == 4)
                currentSevices = currentSevices.Where(p => (Convert.ToDouble(p.Discount) >= 0.30 && Convert.ToDouble(p.Discount) < 0.70)).ToList();
            if(ComboType.SelectedIndex == 5)
                currentSevices = currentSevices.Where(p => (Convert.ToDouble(p.Discount) >= 0.70 && Convert.ToDouble(p.Discount) <= 1)).ToList();
            currentSevices = currentSevices.Where(p => p.Title.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();

            ServiceListView.ItemsSource = currentSevices.ToList();

            if(RButtonDown.IsChecked.Value)
                ServiceListView.ItemsSource = currentSevices.OrderByDescending(p => p.Cost).ToList();
            if(RButtonUp.IsChecked.Value)
                ServiceListView.ItemsSource = currentSevices.OrderBy(p => p.Cost).ToList();
            
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateServices();
        }

        private void RButtonUp_Checked(object sender, RoutedEventArgs e)
        {
            UpdateServices();
        }

        private void RButtonDown_Checked(object sender, RoutedEventArgs e)
        {
            UpdateServices();
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateServices();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null));
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage((sender as Button).DataContext as Service));
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(Visibility == Visibility.Visible)
            {
                Abdeev_autoserviceEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                ServiceListView.ItemsSource = Abdeev_autoserviceEntities.GetContext().Service.ToList();
            }
        }
    }
}
