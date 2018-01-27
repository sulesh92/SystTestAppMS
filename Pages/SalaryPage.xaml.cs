using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
        
        public SalaryPage()
        {
            this.InitializeComponent();

            this.Loaded += SalaryPage_Loaded;
            persons = new ObservableCollection<Person>();
        }

        private void SalaryPage_Loaded(object sender, RoutedEventArgs e)
        {
            using (DataModelContext context = new DataModelContext())
            {
                List < Group > d = context.Groups.ToList();
                foreach (Person p in context.Persons)
                {
                    persons.Add(p);
                }
            }
        }

        private void GroupSelectComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
