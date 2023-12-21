using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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

namespace Bikbulatov_Eyes
{
    /// <summary>
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        // добавим новое поле, которое будет хранить в себе  экземпляр
        // добавленного сервиса (услуги)
        private Agent currentAgent = new Agent();
        List<AgentType> agentTypesDBList = Bikbulatov_eyesEntities.GetContext().AgentType.ToList();

        public AddEditPage(Agent SelectedAgent)
        {
            InitializeComponent();

            if (SelectedAgent != null)
            {
                currentAgent = SelectedAgent;
                ComboType.SelectedIndex = currentAgent.AgentTypeID - 1;
            }    

            // при инициализации установим DataContext страницы - этот созданный объект
            DataContext = currentAgent;

            var currentServices = Bikbulatov_eyesEntities.GetContext().Agent.ToList();
        }

        private void ChangePictureBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            if (myOpenFileDialog.ShowDialog() == true)
            {
                currentAgent.Logo = myOpenFileDialog.FileName;
                LogoImage.Source = new BitmapImage(new Uri(myOpenFileDialog.FileName));
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(currentAgent.Title))
                errors.AppendLine("Укажите наименование агента");
            if (string.IsNullOrWhiteSpace(currentAgent.Address))
                errors.AppendLine("Укажите адрес агента");
            if (string.IsNullOrWhiteSpace(currentAgent.DirectorName))
                errors.AppendLine("Укажите ФИО директора");
            if (ComboType.SelectedItem == null)
                errors.AppendLine("Укажите тип агента");
            if (string.IsNullOrWhiteSpace(currentAgent.Priority.ToString()))
                errors.AppendLine("Укажите приоритет агента");
            if (currentAgent.Priority <= 0)
                errors.AppendLine("Укажите положительный приоритет агента");
            if (string.IsNullOrWhiteSpace(currentAgent.INN))
                errors.AppendLine("Укажите ИНН агента");
            if (string.IsNullOrWhiteSpace(currentAgent.KPP))
                errors.AppendLine("Укажите КПП агента");
            if (string.IsNullOrWhiteSpace(currentAgent.Phone) || currentAgent.Phone.Length != 11)
                errors.AppendLine("Укажите телефон агента");
            else
            {
                string ph = currentAgent.Phone.Replace("(", "").Replace("-", "").Replace("+", "");
                if (((ph[1] == '9' || ph[1] == '4' || ph[1] == '8') && ph.Length != 11)
                    || (ph[1] == '3' && ph.Length != 12))
                    errors.AppendLine("Укажите правильно телефон агента");
            }
            if (string.IsNullOrWhiteSpace(currentAgent.Email))
                errors.AppendLine("Укажите почту агента");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            // код шамиля
            var currentType = (TextBlock)ComboType.SelectedItem;
            string currentTypeContent = currentType.Text;

            foreach (AgentType type in agentTypesDBList)
            {
                if (type.Title.ToString() == currentTypeContent)
                {
                    currentAgent.AgentType = type;
                    currentAgent.AgentTypeID = type.ID;
                    break;
                }
            }
            // код шамиля

            // добавить в контекст текущие значения новой услуги
            if (currentAgent.ID == 0)
                Bikbulatov_eyesEntities.GetContext().Agent.Add(currentAgent);
            // сохранить изменения, если никаких ошибок не получилось при этом
            try
            {
                Bikbulatov_eyesEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        //private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    // Забираем агента, для которого нажата кнопка Удалить
        //    var currentAgent = (sender as Button).DataContext as Agent;
        //    if (MessageBox.Show("Вы точно хотите удалить агента?", "Внимание!",
        //        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        //    {
        //        try
        //        {
        //            Bikbulatov_eyesEntities.GetContext().Agent.Remove(currentAgent);
        //            Bikbulatov_eyesEntities.GetContext().SaveChanges();
        //            // выводим в литвью измененную таблицу
        //            Manager.MainFrame.GoBack();
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show(ex.Message.ToString());
        //        }
        //    }
        //}
        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            // Забираем агента, для которого нажата кнопка Удалить
            var currentAgent = (sender as Button).DataContext as Agent;
            var currentSale = Bikbulatov_eyesEntities.GetContext().ProductSale.ToList();
            currentSale = currentSale.Where(p => p.AgentID == currentAgent.ID).ToList();
            if (currentSale.Count != 0)
            {
                MessageBox.Show("Невозможно выполнить удаление, т.к. существует записи на эту услугу");
                return;
            }
            if (MessageBox.Show("Вы точно хотите удалить агента?", "Внимание!",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Bikbulatov_eyesEntities.GetContext().Agent.Remove(currentAgent);
                    Bikbulatov_eyesEntities.GetContext().SaveChanges();
                    // выводим в литвью измененную таблицу
                    Manager.MainFrame.GoBack();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void SalesButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new SalePage((sender as Button).DataContext as Agent));
        }
    }
}
