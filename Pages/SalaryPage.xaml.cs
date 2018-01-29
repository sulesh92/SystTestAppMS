using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel.Channels;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class SalaryPage : Page
    {
        //Коллекция данных для отображения на Панели 1
        //Список текущих сотрудников на странице Расчет Зарплат
        private ObservableCollection<Person> persons;
        private ObservableCollection<Salary> salaries;
        private List<Group> groups;
        private List<Subordinate> subordinates;

        public SalaryPage()
        {
            this.InitializeComponent();

            this.Loaded += SalaryPage_Loaded;
            persons = new ObservableCollection<Person>();
            salaries = new ObservableCollection<Salary>();
        }

        /// <summary>
        /// Загружает данные из БД в элементы на странице расчета зарплат
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SalaryPage_Loaded(object sender, RoutedEventArgs e)
        {
            using (DataModelContext context = new DataModelContext())
            {

                groups = context.Groups.ToList();
                groupSelectComboBox.ItemsSource = groups;
                groupSelectComboBox.DisplayMemberPath = "Name";
                groupSelectComboBox.SelectedValue = "Id";

                
                foreach (Person p in context.Persons)
                {
                    persons.Add(p);
                }

                subordinates = context.Subordinates.ToList();

                foreach (Salary s in context.Salaries)
                {
                    salaries.Add(s);
                }
            }
        }

        /// <summary>
        /// Возвращает список с сотрудниками из
        /// определенной группы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroupSelectComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            persons.Clear();
            Group targetGroup = groupSelectComboBox.SelectedItem as Group;
            using (DataModelContext context = new DataModelContext())
            {
                
                var _persons = context.Persons.Where(p => p.GroupId == targetGroup.Id);

                foreach (Person p in _persons)
                {
                    persons.Add(p);
                }
            }
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {

            Person targetPerson = currentStaffList.SelectedItem as Person; //сотрудник для, которого необходимо посчитать зарплату
            int numberOfStaff = persons.Last().Id + 1; // список persons формируется сразу после загрузки страницы
            DateTimeOffset accountingDate = datePickerOnSalaryPage.Date;

            SecondSolution.StartCalculations(targetPerson, numberOfStaff, accountingDate);
        }

        private void CurrentStaffList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            certainStaffScrollViewer.Visibility = Visibility.Visible;
            var d = currentStaffList.SelectedItem as Person;
            certainStaffList.ItemsSource = salaries.Where(s => s.PersonId == d.Id);
        }

        private void CalculateAllButton_Click(object sender, RoutedEventArgs e)
        {
            int numberOfStaff = persons.Last().Id + 1; // список persons формируется сразу после загрузки страницы
            DateTimeOffset accountingDate = datePickerOnSalaryPage.Date;

            List<Person> employies = new List<Person>();
            List<Group> groups = new List<Group>();
            List<Subordinate> subs = new List<Subordinate>();
            using (DataModelContext context = new DataModelContext())
            {
                groups = context.Groups.ToList();
                employies = context.Persons.Where(p => p.Group.Name == "Employee").ToList();
                subs = context.Subordinates.ToList();
            }


            ThirdSolution.StartCalculationsForAllPersons(accountingDate, numberOfStaff, employies[0]);
        }
    }
}
