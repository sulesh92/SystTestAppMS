using System.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace TestAppSysTech
{
    /// <summary>
    /// Страница используется для перехода внутри фрейма. 
    /// Главной страницей является страница расчет зарплат.
    /// В Main Paige реализованы общие для всех страниц элементы 
    /// интерфейса. См. xaml разметку
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            //Устанавливаем контент приложения. При запуске приложение
            //в фрейм CurrentContentFrame, который определен в Xaml 
            //разметке, загружается страница расчета зарплаты
            CurrentContentFrame.Navigate(typeof(SalaryPage));
            //скрываем кнопку назад
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            SystemNavigationManager.GetForCurrentView().BackRequested += AppBackRequested;

            Loaded += CheckGroupsInDb; //проверяем наличие записей о группах в БД
        }

        /// <summary>
        /// Обработчик системного события BackRequested. 
        /// Осуществляет переход на предыдущюю страницу. 
        /// Информация о переходах хранится в журнале фрейма.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame frame = CurrentContentFrame as Frame; //переход будет осуществляться внутри фрейма CurrentContentFrame
            if (frame.CanGoBack) //проверка возможности возрвата назад
            {
                frame.GoBack();//возвращает предыдущий фрейм, однако в боковом меню изменение не отображается

                string[] currentFrameSplitMenuButton = CurrentContentFrame.SourcePageType.ToString().Split('.'); //получаем массив с именами страниц
                var selectedObjectName = string.Format("{0}Button", currentFrameSplitMenuButton[1]); //добавлеяем к названию текущей страницы Button
                var SelectedObject = MainPageListBox.FindName(selectedObjectName); //Находим объект с именем нужной кнопки в списке меню
                var index = MainPageListBox.Items.IndexOf(SelectedObject); //определяем индекс кнопки меню
                MainPageListBox.SelectedIndex = index;// Выбираем кнопку в меню соотвествующую названию текущей страницы

                //При переходе назад  MainPageListBox создает событие SelectionChanged, которое также необходимо обработать. 
                //frame.GoBack();    
                e.Handled = true;  //необходимо сообщить о том, что событие возврат назад было обработано 
                                   // MainPageListBox.SelectionChanged 
            }
            
            //Скрывает системную кнопку Назад при переходе на главную страницу
            if (CurrentContentFrame.SourcePageType == typeof(SalaryPage))
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed; //кнопка назад скрыта
            }
        }
        /// <summary>
        /// Проверяет БД на наличие записей о группах, 
        /// если таковых нет вообще, то метод добавляет
        /// указанные в ТЗ
        /// </summary>
        private void CheckGroupsInDb(object sender, RoutedEventArgs e)
        {
            using(DataModelContext context =new DataModelContext())
            {
                System.Collections.Generic.List<Group> groups = context.Groups.ToList();
                if (groups.Count() == 0)
                {
                    Group g1 = new Group { Name = "Manager", SubNumberCoeff = 0.005D, YearsCoefficient = 0.05D, Limit = 0.4D };
                    Group g2 = new Group { Name = "Salesman", SubNumberCoeff = 0.003D, YearsCoefficient = 0.01D,Limit=0.35D };
                    Group g3 = new Group { Name = "Employee", SubNumberCoeff = 0D, YearsCoefficient = 0.03D, Limit=0.3D };
                    context.Groups.Add(g1);
                    context.Groups.Add(g2);
                    context.Groups.Add(g3);
                    context.SaveChanges();
                }
            }

        }

        /// <summary>
        /// Обработчик кнопки бокового меню. 
        /// Изменяет состояние панели на обратное (Открыто, Скрыто)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            splitView.IsPaneOpen = !splitView.IsPaneOpen;
        }

        private void MainPageListBox_Click(object sender, SelectionChangedEventArgs e)
        {
            if (StaffSalaryPageButton.IsSelected)
            {
                CurrentContentFrame.Navigate(typeof(SalaryPage));
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed; //кнопка назад скрыта
            }
            if (StaffEditPageButton.IsSelected)
            {
                CurrentContentFrame.Navigate(typeof(EditStaffPage));
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible; //кнопка назад доступна
            }
        }
    }
}

