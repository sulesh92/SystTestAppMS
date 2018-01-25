using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace TestAppSysTech
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class StaffEditPage : Page
    {
        List<Group> groups;
        List<Person> persons;
        List<Subordinate> subordinates;

        public StaffEditPage()
        {
            this.InitializeComponent();
            this.Loaded += StaffEditPage_Loaded;

            using (DataModelContext context = new DataModelContext())
            {
                groups = context.Groups.ToList();
                persons = context.Persons.ToList();
                subordinates = context.Subordinates.ToList();
            }
        }

        private void StaffEditPage_Loaded(object sender, RoutedEventArgs e)
        {
            personsList.ItemsSource = persons;

            supevisersList.ItemsSource = persons;
            supevisersList.DisplayMemberPath = "Name";

            groupList.ItemsSource = groups;
            groupList.DisplayMemberPath = "Name";
        }

        private void AddPerson_button_Click(object sender, RoutedEventArgs e)
        {
            if (addPersonPanel.Visibility == Visibility.Collapsed)
            {
                ChangeInterface("showPanel");
                return;
            }

            //создаем группу, выбранную для сотрудника
            Group group = groupList.SelectedItem as Group;
            if (groupList.SelectedItem == null)
            {
                ShowMessageAsync("Выберите группу для сотрудника");
                return;
            }
            
            //заполняем профиль сотрудника 
            Person p = new Person();

            p.Group = group;

            if (string.IsNullOrEmpty(personNameTextbox.Text))
            {
                ShowMessageAsync("Укажите имя сотрудника");
                return;
            }
            else
            {
                p.Name = personNameTextbox.Text;
            }

            try
            {
                p.BaseSalary = Convert.ToDouble(baseSalaryTextbox.Text);
            }
            catch (Exception)
            {
                ShowMessageAsync("Недопустимые символы введены в поле Ставка");
                return;
            }

            TextBlock rootRight = rootComboBox.SelectedValue as TextBlock;

            //если пользователь не устанавливает значение, то комплиятор выдает ошибку
            try
            {
                if (rootRight.Text == "Админ") 
                {
                    p.IsRoot = true;
                }
            }
            catch { } //Значение isRoot = false - устанавливается по умолчанию
           
            //определяется начальник
            if (supevisersList.SelectedValue != null)
            {
                p.Supervisor = supevisersList.SelectedValue.ToString();
            }

            p.Login = loginTextbox.Text;
            p.Password = passwordTextbox.Text;

            if (dateOfStartPicker.Date.HasValue)
            {
                p.DateOfStart = (DateTimeOffset)dateOfStartPicker.Date;
            }
            else
            {
                ShowMessageAsync("Выберите дату начала работы сотрудника");
            }
         
            //создаем контекст данных для передачи профиля сотрудника в БД
            using (DataModelContext context = new DataModelContext())
            {
                context.Groups.Attach(group);
                context.Persons.Add(p);
                context.SaveChanges();
                personsList.ItemsSource = context.Persons.ToList();
            }

            //persons.Add(p); 
          

            //Заполняем таблицу подчиненных со ссылкой на начальника
            int subNumber = subPersonsList.SelectedItems.Count;
            


            if (subNumber > 0)
            {
              
                using (DataModelContext context = new DataModelContext())
                {
                    for (int i = 0; i < subNumber; i++)
                    {
                        Subordinate subordinate = new Subordinate();

                        Person selectedPerson = (Person)subPersonsList.SelectedItems[i]; //сохраняем данные о выбранном пользователе
                        subordinate.Name = selectedPerson.Name;
                        subordinate.Group = selectedPerson.Group.Name;
                        subordinate.PersonId = p.Id;

                        context.Persons.Attach(p);
                        context.Subordinates.Add(subordinate);
                        context.SaveChanges();
                    }
                }
            }

           
            ChangeInterface("hidePanel");
        }


        private void AddSubordinate(Person person, Subordinate subordinate)
        {

        }

        private async void ShowMessageAsync(string message)
        {
            var messageDialog = new MessageDialog(message);
            await messageDialog.ShowAsync();

        }

        private void CanselEditing_button_Click(object sender, RoutedEventArgs e)
        {
            ChangeInterface("hidePanel");
        }

        private void ChangeInterface(string status)
        {
            switch (status)
            {
                case "showPanel":

                    addPerson_button.Background = new SolidColorBrush(Color.FromArgb(255, 119, 221, 119));
                    addButtonTitle.Text = "Сохранить";
                    addPersonPanel.Visibility = Visibility.Visible;
                    cancel_button.Visibility = Visibility.Visible;
                    subPersonListPanel.Visibility = Visibility.Visible;
                    break;

                case "hidePanel":

                    addPersonPanel.Visibility = Visibility.Collapsed;
                    addPerson_button.Background = new SolidColorBrush(Color.FromArgb(255, 198, 100, 82));
                    addButtonTitle.Text = "Добавить";
                    cancel_button.Visibility = Visibility.Collapsed;
                    subPersonListPanel.Visibility = Visibility.Collapsed;
                    personNameTextbox.Text = "";
                    baseSalaryTextbox.Text = "";
                    rootComboBox.SelectedItem = null;
                    dateOfStartPicker.Date = null;
                    supevisersList.SelectedValue = null;
                    loginTextbox.Text = "";
                    passwordTextbox.Text = "";
                    groupList.SelectedItem = null;
                    break;
            }



        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Edit_button_Click(object sender, RoutedEventArgs e)
        {
            Person person = personsList.SelectedItem as Person;

            personNameTextbox.Text = person.Name;
            baseSalaryTextbox.Text = person.BaseSalary.ToString();
            rootComboBox.SelectedItem = person.IsRoot;
            dateOfStartPicker.Date = person.DateOfStart;
            supevisersList.SelectedValue = person.Supervisor;
            loginTextbox.Text = person.Login;
            passwordTextbox.Text = person.Password;
            groupList.SelectedItem = person.Group;
            
        }

        public void SetDataToProfile (Person p)
        {
             

        }



        /// <summary>
        /// Удаляет, выбранные пользователем, данные из БД
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delete_button_Click(object sender, RoutedEventArgs e)
        {
           using (DataModelContext context = new DataModelContext())
            {
                for (int i = 0; i < personsList.SelectedItems.Count; i++)
                {
                    Person person = personsList.SelectedItems[i] as Person;
                    context.Persons.Remove(person);
                    context.SaveChanges();
                }
                personsList.ItemsSource = context.Persons.ToList();
            }

        }


        /// <summary>
        /// Изменяет цвет кнопки в зависимости от количества выбранных строк 
        /// на панели данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void personsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedItemsNumber = personsList.SelectedItems.Count();
            if (selectedItemsNumber > 1)
            {
                delete_button.Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                delete_button.IsEnabled = true;
                edit_button.Background = new SolidColorBrush(Color.FromArgb(255, 83, 83, 83));
                edit_button.IsEnabled = false;
                showSubordinates_button.Background = new SolidColorBrush(Color.FromArgb(255, 83, 83, 83));
                showSubordinates_button.IsEnabled = false;
                return;
            }
            if(selectedItemsNumber == 1)
            {
                delete_button.Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                delete_button.IsEnabled = true;
                edit_button.Background = new SolidColorBrush(Color.FromArgb(255, 255, 215, 0));
                edit_button.IsEnabled = true;
                showSubordinates_button.Background = new SolidColorBrush(Color.FromArgb(255, 206, 31, 99));
                showSubordinates_button.IsEnabled = true;
                return;
            }
            else
            {
                delete_button.Background = new SolidColorBrush(Color.FromArgb(255, 83, 83, 83));
                delete_button.IsEnabled = false;
                edit_button.Background = new SolidColorBrush(Color.FromArgb(255, 83, 83, 83));
                edit_button.IsEnabled = false;
                showSubordinates_button.Background = new SolidColorBrush(Color.FromArgb(255, 83, 83, 83));
                showSubordinates_button.IsEnabled = false;
                return;
            }
            
        }

        private void ShowSubordinates_button_Click(object sender, RoutedEventArgs e)
        {

        }
    }


}
