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

namespace Bikbulatov_Eyes
{
    /// <summary>
    /// Логика взаимодействия для ServicePage.xaml
    /// </summary>
    public partial class ServicePage : Page
    {
        int CountRecords; // количество записей в таблице
        int CountPage; // общее количество страниц
        int CurrentPage = 0; // текущая страница
        List<Agent> CurrentPageList = new List<Agent>();
        List<Agent> TableList;

        public ServicePage()
        {
            InitializeComponent();
            // добавляем строки
            // загрузить в список из бд
            var currentServices = Bikbulatov_eyesEntities.GetContext().Agent.ToList();
            // связать с нашим листвью
            ServiceListView.ItemsSource = currentServices;

            ComboType.SelectedIndex = 0;
            ComboFilter.SelectedIndex = 0;

            UpdateAgents();
        }


        private void UpdateAgents()
        {
            // берем из бд данные таблицы Agent
            var currentAgent = Bikbulatov_eyesEntities.GetContext().Agent.ToList();

            // прописываем фильтрацию по условию задачи
            if (ComboFilter.SelectedIndex == 0)
            {
                currentAgent = currentAgent.ToList();
            }
            if (ComboFilter.SelectedIndex == 1)
            {
                currentAgent = currentAgent.Where(p => (p.AgentTypeString == "ЗАО")).ToList();
            }
            if (ComboFilter.SelectedIndex == 2)
            {
                currentAgent = currentAgent.Where(p => (p.AgentTypeString == "МКК")).ToList();
            }
            if (ComboFilter.SelectedIndex == 3)
            {
                currentAgent = currentAgent.Where(p => (p.AgentTypeString == "МФО")).ToList();
            }
            if (ComboFilter.SelectedIndex == 4)
            {
                currentAgent = currentAgent.Where(p => (p.AgentTypeString == "ОАО")).ToList();
            }
            if (ComboFilter.SelectedIndex == 5)
            {
                currentAgent = currentAgent.Where(p => (p.AgentTypeString == "ООО")).ToList();
            }
            if (ComboFilter.SelectedIndex == 6)
            {
                currentAgent = currentAgent.Where(p => (p.AgentTypeString == "ПАО")).ToList();
            }

            // реализуем поиск данных в листвью при вводе текста в окно поиска
            currentAgent = currentAgent.Where(p => p.Title.ToLower().Contains(TBoxSearch.Text.ToLower()) || 
                p.Phone.Replace("+7", "8").Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "").Contains(TBoxSearch.Text.Replace("+7", "8").Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", ""))
                || p.Email.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();

            // реализуем сортировку ...
            if (ComboType.SelectedIndex == 0) 
            {
                currentAgent = currentAgent.ToList();
            }
            if (ComboType.SelectedIndex == 1)
            {
                currentAgent = currentAgent.OrderBy(p => p.Title).ToList();
            }
            if (ComboType.SelectedIndex == 2)
            {
                currentAgent = currentAgent.OrderByDescending(p => p.Title).ToList();
            }
            if (ComboType.SelectedIndex == 3)
            {
                // скидка
            }
            if (ComboType.SelectedIndex == 4)
            {
                // скидка
            }
            if (ComboType.SelectedIndex == 5)
            {
                currentAgent = currentAgent.OrderBy(p => p.Priority).ToList();
            }
            if (ComboType.SelectedIndex == 6)
            {
                currentAgent = currentAgent.OrderByDescending(p => p.Priority).ToList();
            }

            // отображаем итоги поиска/фильтрации/сортировки
            ServiceListView.ItemsSource = currentAgent;

            // для отображения итого фильтра и поиска в листвью
            ServiceListView.ItemsSource = currentAgent;
            TableList = currentAgent;
            ChangePage(0, 0);
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateAgents();
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgents();
        }

        private void ComboFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgents();
        }

        // функция отвечающая за разделение листа
        private void ChangePage(int direction, int? selectedPage)
        {
            // direction - направление. 0 - начало, 1 - предыдущая страница, 2 - следующая страница
            // selectedPage - при нажатии на стрелочки передается null,
            // при выборе определенной страницы в этой переменной находится номер страницы

            CurrentPageList.Clear(); // начальная очистка листа
            CountRecords = TableList.Count; // определение количества записей во всем списке
            // определение количества страниц
            if (CountRecords % 10 > 0)
            {
                CountPage = CountRecords / 10 + 1;
            }
            else
            {
                CountPage = CountRecords / 10;
            }

            Boolean Ifupdate = true;

            int min;

            if (selectedPage.HasValue) 
            {
                if (selectedPage >= 0 && selectedPage <= CountPage)
                {
                    CurrentPage = (int)selectedPage;
                    min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                    for (int i = CurrentPage * 10; i < min; i++)
                    {
                        CurrentPageList.Add(TableList[i]);
                    }
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
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            Ifupdate = false;
                        }
                        break;

                    case 2:
                        {
                            if (CurrentPage < CountPage - 1)
                            {
                                CurrentPage++;
                                min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                                for (int i = CurrentPage * 10; i < min; i++)
                                {
                                    CurrentPageList.Add(TableList[i]);
                                }
                            }
                            else
                            {
                                Ifupdate = false;
                            }
                            break;
                        }
                }
            }

            // если currentPage не вышел из диапазона то
            if (Ifupdate)
            {
                PageListBox.Items.Clear();
                for (int i = 1; i <= CountPage; i++)
                {
                    PageListBox.Items.Add(i);
                }
                PageListBox.SelectedIndex = CurrentPage;

                ServiceListView.ItemsSource = CurrentPageList;
                ServiceListView.Items.Refresh();
            }

            // вывод количества записей на странице и общего количества
            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
            TBCount.Text = min.ToString();
            TBAllRecords.Text = " из " + CountRecords.ToString();

            
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

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // открыть окно редактирования услуг
            Manager.MainFrame.Navigate(new AddEditPage(null));

            UpdateAgents();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage((sender as Button).DataContext as Agent));

            UpdateAgents();
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Bikbulatov_eyesEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                ServiceListView.ItemsSource = Bikbulatov_eyesEntities.GetContext().Agent.ToList();
            }
            UpdateAgents();
        }

        private void ChangePriorityButton_Click(object sender, RoutedEventArgs e)
        {
            int max = 0;
            foreach (Agent agent in ServiceListView.SelectedItems)
            {
                if (agent.Priority >= max)
                {
                    max = agent.Priority;
                }
            }
            PriorityWindow window = new PriorityWindow(max);
            window.ShowDialog();
            if (string.IsNullOrEmpty(window.PriorityTB.Text)) return;
            MessageBox.Show(window.PriorityTB.Text);

            foreach (Agent agent in ServiceListView.SelectedItems)
            {

                agent.Priority = Convert.ToInt32(window.PriorityTB.Text);
            }
            try
            {
                Bikbulatov_eyesEntities.GetContext().SaveChanges();
                MessageBox.Show("информация сохранена");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            UpdateAgents();
        }

        private void ServiceListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ServiceListView.SelectedItems.Count > 1)
            {
                ChangePriorityButton.Visibility = Visibility.Visible;
            }
            else
            {
                ChangePriorityButton.Visibility = Visibility.Hidden;
            }
        }
    }
}
