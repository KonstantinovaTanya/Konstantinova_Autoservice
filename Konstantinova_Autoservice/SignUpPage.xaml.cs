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

namespace Konstantinova_Autoservice
{
    /// <summary>
    /// Логика взаимодействия для SignUpPage.xaml
    /// </summary>
    public partial class SignUpPage : Page
    {
        //добавим новое поле, которое будет хранить в себе экземпляр добавляемого сервиса
        private Service _currentService = new Service();

        public SignUpPage(Service SelectedService)
        {
            InitializeComponent();
            if(SelectedService != null)
                this._currentService = SelectedService;

            //при инициализации установим DataContext - этот созданный объект
            //чтобы на форму подгрузить выбранные наименование услуги и длительность
            DataContext = _currentService;

            //вытащим из БД таблицу Клиент
            var _currentClient = КонстантиноваАвтосервисEntities1.GetContext().Client.ToList();
            //свяжем ее с комбобоксом
            ComboClient.ItemsSource = _currentClient;
        }

        private ClientService _currentClientService = new ClientService();

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (ComboClient.SelectedItem == null)
                errors.AppendLine("Укажите ФИО клиента");

            if (StartDate.Text == "")
                errors.AppendLine("Укажите дату услуги");

            if (TBEnd.Text == "")
                errors.AppendLine("Укажите время начала услуги в формате ЧЧ:ММ");
            
            if (TBStart.Text == "")
                errors.AppendLine("Укажите время начала услуги");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }


            //добавить текущие значения новой записи

            //_currentClientService.ClientID = ComboClient.SelectedIndex + 1;
            Client selectedClient = ComboClient.SelectedItem as Client;
            _currentClientService.ClientID = selectedClient.ID;
            _currentClientService.ServiceID = _currentService.ID;
            _currentClientService.StartTime = Convert.ToDateTime(StartDate.Text + " " + TBStart.Text);

            if (_currentClientService.ID == 0)
                КонстантиноваАвтосервисEntities1.GetContext().ClientService.Add(_currentClientService);

            //сохранить изменения, если никаких ошибок не получилось при этом
            try
            {
                КонстантиноваАвтосервисEntities1.GetContext().SaveChanges();
                MessageBox.Show("информация сохранена");
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

            if (s.Length < 3 || !s.Contains(':'))
                TBEnd.Text = "";
            else
            {
                try
                {
                    string[] start = s.Split(new char[] { ':' });
                    int startHour = Convert.ToInt32(start[0].ToString());
                    int startMinute = Convert.ToInt32(start[1].ToString());

                    if (startHour < 0 || startHour > 23 || startMinute <0 || startMinute >=60)
                    {
                        TBEnd.Text = "";
                        return;
                    }

                    // Конвертируем введенное время в секунды
            int startTimeInSeconds = (startHour * 3600) + (startMinute * 60);

                    // Длительность уже в секундах (из БД)
                    int durationInSeconds = _currentService.DurationIn;

                    // Вычисляем время окончания
                    int endTimeInSeconds = startTimeInSeconds + durationInSeconds;

                    // Обрабатываем переход через полночь
                    int secondsInDay = 24 * 3600;
                    if (endTimeInSeconds >= secondsInDay)
                        endTimeInSeconds -= secondsInDay;

                    // Конвертируем обратно в часы:минуты
                    int endHour = endTimeInSeconds / 3600;
                    int endMinute = (endTimeInSeconds % 3600) / 60;

                    TBEnd.Text = $"{endHour:D2}:{endMinute:D2}";
                }
                catch
                {
                    TBEnd.Text = "";
                }
            }
        }


        private void TBStart_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string newText = textBox.Text + e.Text;

            // Разрешаем только цифры и двоеточие
            if (!char.IsDigit(e.Text, 0) && e.Text != ":")
            {
                e.Handled = true;
                return;
            }

            // Проверяем формат времени
            if (e.Text == ":")
            {
                // Двоеточие можно вводить только один раз
                if (textBox.Text.Contains(':'))
                {
                    e.Handled = true;
                    return;
                }
            }

            if (newText.Length > 5)
            {
                e.Handled = true;
                return;
            }
        }



    }
}
