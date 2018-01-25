using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

        public StaffEditPage()
        {
            this.InitializeComponent();
            this.Loaded += StaffEditPage_Loaded;

            using (DataModelContext context = new DataModelContext())
            {
                groups = context.Groups.ToList();
                persons = context.Persons.ToList();
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
                addPerson_button.Background = new SolidColorBrush(Color.FromArgb(255, 119, 221, 119));
                addButtonTitle.Text = "Сохранить";
                addPersonPanel.Visibility = Visibility.Visible;
                cancel_button.Visibility = Visibility.Visible;

                return;
            }
            //создаем группу, выбранную для сотрудника
            Group group = groupList.SelectedItem as Group;
            if (group == null)
            {
                ShowMessageAsync("Выберите группу для сотрудника");
            }

            //заполняем профиль сотрудника 
            Person p = new Person();

            p.Name = personNameTextbox.Text;
            p.BaseSalary = Convert.ToDouble(baseSalaryTextbox.Text);

            var rootRight = rootComboBox.SelectedValue.ToString();
            if (rootRight == "Админ")
            {
                p.IsRoot = true;
            }

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

            p.Group = group;

            //создаем контекст данных для передачи профиля сотрудника в БД
            using (DataModelContext context = new DataModelContext())
            {
                context.Groups.Attach(group);
                context.Persons.Add(p);
                context.SaveChanges();

                if (context.SaveChanges() > 0)
                {
                    persons.Add(p);
                }
            }


            ChangeInterface();
        }

        private async void ShowMessageAsync(string message)
        {
            var messageDialog = new MessageDialog("Выберите группу для сотрудника");
            await messageDialog.ShowAsync();

        }

        private void CanselEditing_button_Click(object sender, RoutedEventArgs e)
        {
            ChangeInterface();
        }

        private void ChangeInterface()
        {
            addPersonPanel.Visibility = Visibility.Collapsed;
            addPerson_button.Background = new SolidColorBrush(Color.FromArgb(255, 198, 100, 82));
            addButtonTitle.Text = "Добавить";
            cancel_button.Visibility = Visibility.Collapsed;
        }

        private void Edit_button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Delete_button_Click(object sender, RoutedEventArgs e)
        {

        }

        
    }

   
}
