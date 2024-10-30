using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        int CountRecords;
        int CountPage;
        int CurrentPage = 0;
        List<Service> CurrentPageList = new List<Service>();
        List<Service> TableList;

        private void ChangePage(int direction, int? selectedPage)
        {
            CurrentPageList.Clear();
            CountRecords = TableList.Count;
            if (CountRecords % 10 > 0)
                CountPage = CountRecords / 10 + 1;
            else
                CountPage = CountRecords / 10;

            Boolean ifUpdate = true;
            int min;
            if (selectedPage.HasValue)
            {
                if (selectedPage >= 0 && selectedPage <= CountPage)
                {
                    CurrentPage = (int)selectedPage;
                    min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                    for (int i = CurrentPage * 10; i < min; i++)
                        CurrentPageList.Add(TableList[i]);
                }
            }
            else
            {
                switch (direction)
                {
                    case 1:
                        if (CurrentPage > 0)
                        {
                            CurrentPage--;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                            for (int i = CurrentPage * 10; i < min; i++)
                                CurrentPageList.Add(TableList[i]);
                        }
                        else
                            ifUpdate = false;
                        break;
                    case 2:
                        if (CurrentPage < CountPage - 1)
                        {
                            CurrentPage++;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                            for (int i = CurrentPage * 10; i < min; i++)
                                CurrentPageList.Add(TableList[i]);
                        }
                        else
                            ifUpdate = false;
                        break;
                }
            }
            if (ifUpdate)
            {
                PageListBox.Items.Clear();
                for (int i = 1; i <= CountPage; i++)
                    PageListBox.Items.Add(i);
                PageListBox.SelectedIndex = CurrentPage;

                min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                TBCount.Text = min.ToString();
                TBAllRecords.Text = " из " + CountRecords.ToString();

                ServiceListView.ItemsSource = CurrentPageList;
                ServiceListView.Items.Refresh();
            }
        }
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
            if (ComboType.SelectedIndex == 0)
                currentSevices = currentSevices.Where(p => (Convert.ToDouble(p.Discount) >= 0 && Convert.ToDouble(p.Discount) <= 1)).ToList();
            if (ComboType.SelectedIndex == 1)
                currentSevices = currentSevices.Where(p => (Convert.ToDouble(p.Discount) >= 0 && Convert.ToDouble(p.Discount) < 0.05)).ToList();
            if (ComboType.SelectedIndex == 2)
                currentSevices = currentSevices.Where(p => (Convert.ToDouble(p.Discount) >= 0.05 && Convert.ToDouble(p.Discount) < 0.15)).ToList();
            if (ComboType.SelectedIndex == 3)
                currentSevices = currentSevices.Where(p => (Convert.ToDouble(p.Discount) >= 0.15 && Convert.ToDouble(p.Discount) < 0.30)).ToList();
            if (ComboType.SelectedIndex == 4)
                currentSevices = currentSevices.Where(p => (Convert.ToDouble(p.Discount) >= 0.30 && Convert.ToDouble(p.Discount) < 0.70)).ToList();
            if (ComboType.SelectedIndex == 5)
                currentSevices = currentSevices.Where(p => (Convert.ToDouble(p.Discount) >= 0.70 && Convert.ToDouble(p.Discount) <= 1)).ToList();
            currentSevices = currentSevices.Where(p => p.Title.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();

            ServiceListView.ItemsSource = currentSevices.ToList();

            if (RButtonDown.IsChecked.Value)
               currentSevices = currentSevices.OrderByDescending(p => p.Cost).ToList();
            if (RButtonUp.IsChecked.Value)
               currentSevices = currentSevices.OrderBy(p => p.Cost).ToList();

            ServiceListView.ItemsSource = currentSevices;
            TableList = currentSevices;
            ChangePage(0, 0);
            UpdateServices();

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
            if (Visibility == Visibility.Visible)
            {
                Abdeev_autoserviceEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                ServiceListView.ItemsSource = Abdeev_autoserviceEntities.GetContext().Service.ToList();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var currentService = (sender as Button).DataContext as Service;
            var currentClientService = Abdeev_autoserviceEntities.GetContext().ClientService.ToList();
            currentClientService = currentClientService.Where(p => p.ServiceID == currentService.ID).ToList();
            if (currentClientService.Count != 0)
                MessageBox.Show("Невозможно выполнить удаление, т.к. существуют записи на эту услугу");
            else
            {
                if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        Abdeev_autoserviceEntities.GetContext().Service.Remove(currentService);
                        Abdeev_autoserviceEntities.GetContext().SaveChanges();
                        ServiceListView.ItemsSource = Abdeev_autoserviceEntities.GetContext().Service.ToList();
                        UpdateServices();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
        }

        private void LeftDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(1, null);
        }

        private void RightDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(2, null);

        }

        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangePage(0, Convert.ToInt32(PageListBox.SelectedItem.ToString()) - 1);
        }
    }
}
