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
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        //Добавим новое поле, которое будет хранить в себе экземпляр добавляемого сервиса
        private Service _currentService = new Service();
        public AddEditPage(Service SelectedService)
        {
            InitializeComponent();

            if(SelectedService != null )
            {
                _currentService = SelectedService;
            }

            //При инициализации установим DataContext страницы - этот созданный объект
            DataContext = _currentService;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentService.Title))
                errors.AppendLine("Укажите название услуги");

            if (_currentService.Cost <= 0)
                errors.AppendLine("Укажите стоимость услуги");

            if (_currentService.DiscountIt < 0 || _currentService.DiscountIt > 100)
                errors.AppendLine("Укажите скидку");

            if (string.IsNullOrWhiteSpace(_currentService.DurationIn))
                errors.AppendLine("Укажите длительность услуги");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            //добавить в контекст текущие значения новой услуги
            if (_currentService.ID == 0)
                КонстантиноваАвтосервисEntities.GetContext().Service.Add(_currentService);

            //Сохранить изменения, если никаких ошибок не получилось при этом
            try
            {
                КонстантиноваАвтосервисEntities.GetContext().SaveChanges();
                MessageBox.Show("информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

    }
}
