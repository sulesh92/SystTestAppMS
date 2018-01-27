using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace TestAppSysTech
{

    public sealed partial class SalaryPage : Page
    {
        private ObservableCollection<Person> persons;

        public SalaryPage()
        {
            this.InitializeComponent();
            this.Loaded += SalaryPage_Loaded;
            persons = new ObservableCollection<Person>();
        }

        private void InitializeComponent()
        {
            throw new NotImplementedException();
        }

        private void SalaryPage_Loaded(object sender, RoutedEventArgs e)
        {
            using (DataModelContext context = new DataModelContext())
            {
                foreach (Person p in context.Persons)
                {
                    persons.Add(p);
                }
            }
        }

    }
}
