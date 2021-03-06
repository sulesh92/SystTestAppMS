﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class GroupEditPage : Page
    {
        List<Group> groups;
        List<Person> persons;
        Person newPerson = new Person();

        public GroupEditPage()
        {
            this.InitializeComponent();
            Loaded += EditStaffPage_Loaded;

            using (DataModelContext context = new DataModelContext())
            {
                groups = context.Groups.ToList();
                persons = context.Persons.ToList();
            }

        }

   
        /// <summary>
        /// Устанавливает ресурсы ListView 
        /// для отображения информации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditStaffPage_Loaded(object sender, RoutedEventArgs e)
        {
            personsList.ItemsSource = persons;

            supevisersList.ItemsSource = persons;
            supevisersList.DisplayMemberPath = "Name";

            groupList.ItemsSource = groups;
            groupList.DisplayMemberPath = "Name";
        }

        /// <summary>
        /// Проверяет наличие данных на панели AddPersonPanel. Вызывает методы по считыванию
        /// и сохранению данных из полей в БД. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddPerson_button_Click(object sender, RoutedEventArgs e)
        {
            //Открывает панель добавления сотрудников
            if (addPersonPanel.Visibility == Visibility.Collapsed)
            {
                ChangeInterface("showPanel");
                return;
            }

            //создает группу, выбранную для сотрудника

            if (groupList.SelectedItem == null)
            {
                CommonTools.ShowMessageAsync("Выберите группу для сотрудника");
                return;
            }

            if (string.IsNullOrEmpty(personNameTextbox.Text))
            {
                CommonTools.ShowMessageAsync("Укажите имя сотрудника");
                return;
            }

            try
            {
                var BaseSalary = Convert.ToDouble(baseSalaryTextbox.Text);
            }
            catch (Exception)
            {
                CommonTools.ShowMessageAsync("Недопустимые символы введены в поле Ставка");
                return;
            }

            if (!dateOfStartPicker.Date.HasValue)
            {
                CommonTools.ShowMessageAsync("Выберите дату начала работы сотрудника");
                return;
            }

            if (newPerson.Name == null)
            {
                newPerson = SetDataToProfile(newPerson, false);
            }
            else
            {
                newPerson = SetDataToProfile(newPerson, true);
            }

            UpdateItemsListOnStaffEditPage();

            //Заполняем таблицу подчиненных со ссылкой на начальника
            var index = subPersonsList.SelectedItems.Count;
            if (index > 0)
            {
                for (int i = 0; i < index; i++)
                {
                    Person selectedPerson = (Person)subPersonsList.SelectedItems[i]; //сохраняем данные о выбранном пользователе
                    AddSubordinate(selectedPerson, newPerson);
                }

            }

            //Очищяем поля
            newPerson = new Person();
            ChangeInterface("hidePanel");
        }

        /// <summary>
        /// Устанавливает данные из панели AddPersonPanel
        /// -Панель ввода данных для профиля сотрудника
        /// в поля класса Person
        /// </summary>
        /// <param name="p"></param>
        public Person SetDataToProfile(Person p, bool isUpdating)
        {
            Group g = groupList.SelectedItem as Group;
            p.Group = g;
            p.Name = personNameTextbox.Text;
            p.BaseSalary = Convert.ToDouble(baseSalaryTextbox.Text);
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

            //добавляет начальника
            if (supevisersList.SelectedValue != null)
            {
                Person superviser = supevisersList.SelectedItem as Person;
                AddSubordinate(p, superviser);
            }

            p.Login = loginTextbox.Text;
            p.Password = passwordTextbox.Text;
            p.DateOfStart = (DateTimeOffset)dateOfStartPicker.Date;

            switch (isUpdating)
            {
                case true:
                    p = UpdateProfileInTable(p, g);
                    break;
                case false:
                    p = AddProfileToTable(p, g);
                    break;
            }

            return p;
        }

        /// <summary>
        /// Метод добавляет профиль Person в БД
        /// </summary>
        /// <param name="p">Person p = new Person()</param>
        /// <param name="g">Группа сотрудника</param>
        /// <returns></returns>
        private Person AddProfileToTable(Person p, Group g)
        {
            //создаем контекст данных для передачи профиля сотрудника в БД
            using (DataModelContext context = new DataModelContext())
            {
                context.Groups.Attach(g);
                context.Persons.Add(p);
                context.SaveChanges();
            }
            return p;
        }

        /// <summary>
        /// Метод обновляет профиль сотрудника в БД
        /// </summary>
        /// <param name="p">профиль сотрудника</param>
        /// <param name="g">измененную группу</param>
        /// <returns></returns>
        private Person UpdateProfileInTable(Person p, Group g)
        {
            //создаем контекст данных для передачи профиля сотрудника в БД
            using (DataModelContext context = new DataModelContext())
            {
                if (p.Group != g)
                {
                    context.Groups.Attach(g);
                }
                context.Persons.Update(p);
                context.SaveChanges();
            }
            return p;
        }

        private void UpdateItemsListOnStaffEditPage()
        {
            using (DataModelContext context = new DataModelContext())
            {
                personsList.ItemsSource = context.Persons.ToList();
                supevisersList.ItemsSource = context.Persons.ToList();
            }
        }

        /// <summary>
        /// Сохраняет выбранных подчиненых в таблицу Subordinate
        /// </summary>
        private void AddSubordinate(Person subPerson, Person superviser)
        {
            using (DataModelContext context = new DataModelContext())
            {
                Subordinate subordinate = new Subordinate();

                subordinate.Name = subPerson.Name;
                subordinate.Group = subPerson.Group.Name;

                subordinate.PersonId = superviser.Id;
                subordinate.OwnPersonId = subPerson.Id;
                context.Persons.Attach(superviser);

                context.Subordinates.Add(subordinate);

                context.SaveChanges();
            }
        }


        /// <summary>
        /// Отображает и скрывает панель AddPersonPanel, а также 
        /// очищает введенные данные
        /// </summary>
        /// <param name="status"></param>
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
                    subordinatesListPanel.Visibility = Visibility.Collapsed;

                    break;

                case "hidePanel":

                    addPersonPanel.Visibility = Visibility.Collapsed;
                    addPerson_button.Background = new SolidColorBrush(Color.FromArgb(255, 198, 100, 82));
                    addButtonTitle.Text = "Добавить";
                    cancel_button.Visibility = Visibility.Collapsed;
                    subPersonListPanel.Visibility = Visibility.Collapsed;
                    subordinatesListPanel.Visibility = Visibility.Collapsed;

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
        /// Вызывает ChangeInterface, для отмены редактирования
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanselEditing_button_Click(object sender, RoutedEventArgs e)
        {
            ChangeInterface("hidePanel");
        }

        /// <summary>
        /// Открывает панель редактирования и заполняет необходимые поля
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Edit_button_Click(object sender, RoutedEventArgs e)
        {
            newPerson = personsList.SelectedItem as Person;

            personNameTextbox.Text = newPerson.Name;
            baseSalaryTextbox.Text = newPerson.BaseSalary.ToString();
            rootComboBox.SelectedItem = newPerson.IsRoot;
            dateOfStartPicker.Date = newPerson.DateOfStart;
            loginTextbox.Text = newPerson.Login;
            passwordTextbox.Text = newPerson.Password;
            groupList.SelectedItem = newPerson.Group;

            ChangeInterface("showPanel");
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
            if (selectedItemsNumber == 1)
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
            Person p = personsList.SelectedItem as Person;

            using (DataModelContext context = new DataModelContext())
            {
                var subordinates = context.Subordinates.Where(s => s.PersonId == p.Id);

                subordinatesListPanel.Visibility = Visibility.Visible;
                subordinatesList.ItemsSource = subordinates;
            }
        }

        /// <summary>
        /// Скрывает панель со список подчиненных определенного
        /// сотрудника
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseSubordinatesListPanelButton_Click(object sender, RoutedEventArgs e)
        {
            subordinatesListPanel.Visibility = Visibility.Collapsed;
        }

    }
}
