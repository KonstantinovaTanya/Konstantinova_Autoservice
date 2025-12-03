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
        private bool isEditing = false;
        public AddEditPage(Service SelectedService)
        {

            InitializeComponent();

            if (SelectedService != null)
            {
                _currentService = SelectedService;
                isEditing = true;
            }
            else
            {
                isEditing = false;
            }

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
                errors.AppendLine("Укажите скидку от 0 до 100");

            if (_currentService.DurationIn <= 0)
                errors.AppendLine("Укажите длительность услуги");

            if (_currentService.DurationIn >= 14400 || _currentService.DurationIn < 0)
                errors.AppendLine("Длительность не может быть больше 240 минут или меньше 0");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            // Проверка на уникальность названия услуги
            bool serviceExists;

            if (isEditing)
            {
                // При редактировании: ищем услуги с таким же названием, но исключая текущую
                serviceExists = КонстантиноваАвтосервисEntities1.GetContext().Service
                    .Any(s => s.Title == _currentService.Title && s.ID != _currentService.ID);
            }
            else
            {
                // При добавлении новой услуги: просто проверяем существование
                serviceExists = КонстантиноваАвтосервисEntities1.GetContext().Service
                    .Any(s => s.Title == _currentService.Title);
            }

            if (serviceExists)
            {
                MessageBox.Show("Уже существует услуга с таким названием");
                return;
            }

            // Добавляем услугу в контекст, если это новая услуга
            if (!isEditing)
                КонстантиноваАвтосервисEntities1.GetContext().Service.Add(_currentService);
            //добавить в контекст текущие значения новой услуги
            if (_currentService.ID == 0)
                КонстантиноваАвтосервисEntities1.GetContext().Service.Add(_currentService);

            var allServices = КонстантиноваАвтосервисEntities1.GetContext().Service.ToList();
            allServices = allServices.Where(p => p.Title == _currentService.Title).ToList();

            if (allServices.Count == 0 || isEditing == true)
            {
                if (_currentService.ID == 0)
                    КонстантиноваАвтосервисEntities1.GetContext().Service.Add(_currentService);
                try
                {
                    КонстантиноваАвтосервисEntities1.GetContext().SaveChanges();
                    MessageBox.Show("Информация сохранена");
                    Manager.MainFrame.GoBack();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            else
            {
                MessageBox.Show("Уже существует такая услуга");
            }
          
        }

    }
}
