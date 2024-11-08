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
    /// Логика взаимодействия для SignUpPage.xaml
    /// </summary>
    /// 
    public partial class SignUpPage : Page
    {
        private Service _currentService = new Service();

        public SignUpPage(Service SelectedService)
        {
            InitializeComponent();
            if (SelectedService != null)
                this._currentService = SelectedService;
            DataContext = _currentService;

            var _currentClient = Abdeev_autoserviceEntities.GetContext().Client.ToList();
            ComboClient.ItemsSource = _currentClient;
        }
        private ClientService _currentClientService = new ClientService();
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (ComboClient.SelectedItem == null)
                errors.AppendLine("Укажите ФИО клиента");
            if (StartDate.SelectedDate == null || StartDate.SelectedDate.Value.Date < DateTime.Today)
                errors.AppendLine("Укажите дату услуги или дату, которая не идет на предыдущие дни");
            if (TBStart.Text == "")
                errors.AppendLine("Укажите время начала услуги");
            //else
            //{
            //    DateTime dateTime;
            //    string dateTimeString = StartDate.Text + " " + TBStart.Text;
            //    // Попытка преобразования строки в DateTime
            //    if (!DateTime.TryParse(dateTimeString, out dateTime))
            //        errors.AppendLine("Неверный формат даты и времени");
            //    else
            //        _currentClientService.StartTime = dateTime;
            //}

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            _currentClientService.ClientID = ComboClient.SelectedIndex + 1;
            _currentClientService.ServiceID = _currentService.ID;
            _currentClientService.StartTime = Convert.ToDateTime(StartDate.Text + " " + TBStart.Text);

            if (_currentClientService.ID == 0)
                Abdeev_autoserviceEntities.GetContext().ClientService.Add(_currentClientService);

            try
            {
                Abdeev_autoserviceEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void TBStart_TextChanged(object sender, TextChangedEventArgs e)
        {
            string s = TBStart.Text;

            if (s.Length >= 3 || !s.Contains(":"))
                TBEnd.Text = "";
            else
            {
                string[] start = s.Split(new char[] { ':' });
                int startHour = Convert.ToInt32(start[0].ToString()) * 60;
                int startMin = Convert.ToInt32(start[1].ToString());

                int sum = startHour + startMin + _currentService.Duration;

                int EndHour = sum / 60;
                int EndMin = sum % 60;
                s = EndHour.ToString() + ":" + EndMin.ToString();
                TBEnd.Text = s;
            }
        }
    }
}
