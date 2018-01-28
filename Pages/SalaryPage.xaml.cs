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

                subordinates = context.Subordinates.ToList();

                foreach(Salary s in context.Salaries)
                {
                    salaries.Add(s);
                }
               
                foreach (Person p in context.Persons)
                {
                    persons.Add(p);
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
                //List<Group> g = context.Groups.ToList();
                var _persons = context.Persons.Where(p => p.GroupId == targetGroup.Id);

                foreach (Person p in _persons)
                {
                    persons.Add(p);
                }
            }
        }


        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            CreateDictionary();

        }




        ///////////////////////////////////////МОЗГ//////////////////////////////////////////////////

        //Dictionary<string, Subordinate> tree = new Dictionary<string, Subordinate>();
        List<int> tree = new List<int>();

        private void CreateDictionary()
        {
            Person person = currentStaffList.SelectedItem as Person; //сотрудник для, которого необходимо посчитать зарплату
            int numberOfStaff = persons.Last().Id + 1; // список persons формируется сразу после загрузки страницы
            CreateTree(person, numberOfStaff);
        }

        private void CreateTree(Person chosenPerson, int numberOfStaff)
        {
            Queue<Person> turn = new Queue<Person>(); //хранит сотрудников, которые подлежат проверке 
            turn.Enqueue(chosenPerson); //первый в очереди сотрудник для проверки
            

            while (turn.Count != 0)
            {
                Person person = turn.Dequeue();
                
                using (DataModelContext context = new DataModelContext())
                {
                    List<Subordinate> tempSubList = context.Subordinates.ToList();

                    if (person.Subordinates.Count() > 0)
                    {
                        for (int i = 0; i < person.Subordinates.Count; i++)
                        {
                            var index = string.Format("{0}.{1}", person.Id, i);
                            List<Subordinate> subs = person.Subordinates.ToList();
                            //tree.Add(index, subs[i]); занимает много лишней памяти,
                            //экономней сохранять OwnPersonId подчиненного, который
                            //является его Id в таблице сотрудников

                            tree.Add(subs[i].OwnPersonId);
                                                       
                            //поиск подчиненного в Таблице сотрудников
                            //подчиненный заносится в список как следующий _person для проверки
                            //на наличии своих подчиненных. 
                            Person nextPerson = context.Persons.Find(subs[i].OwnPersonId);
                            turn.Enqueue(nextPerson);              //добавляет сотрудника в очередь
                        }
                    }
                }
            }
        }

        private void CalculateSalary(Person lastPerson,int numberOfStaff)
        {
            bool[] isPersonSalaryCalculated = new bool[numberOfStaff];
            Queue<Person> turn = new Queue<Person>();
            turn.Enqueue(lastPerson);
           
            double subsWeightTotal = 0D;

            while (turn.Count != 0)
            {
                Person p = turn.Dequeue();

                using (DataModelContext context = new DataModelContext())
                {
                    List<Subordinate> tempSubList = context.Subordinates.ToList();
                    List<Salary> salaries = context.Salaries.ToList();

                    //Осуществляется переход на уровень ниже, если у текущего сотрудника, 
                    //есть не просчитанные подчиненные
                    if (p.Subordinates.Count > 0)
                    {       
                        foreach(Subordinate s in p.Subordinates)
                        {   
                            //Каждый подчиненный записывается в очередь, как сотрудник
                            if(isPersonSalaryCalculated[s.OwnPersonId] == false)
                            {
                                turn.Enqueue(context.Persons.Find(s.OwnPersonId));
                            }
                        }
                        
                        //Начальник добавляется последним в очередь
                        turn.Enqueue(p);
                        continue;
                    }

                    //Иначе расчитывается зарплата текущего сотрудника и осуществляется
                    //переход на уровень выше
                    else
                    {
                        DateTimeOffset accountingDate = datePickerOnSalaryPage.Date;

                        //Возвращает зарплату текущего сотрудника
                        SalaryCalculators.CalculatePersonSalary(p, accountingDate, subsWeightTotal);

                        isPersonSalaryCalculated[p.Id] = true;
                        //Переход на уровень выше
                        /*  context.Subordinates.Find("OwnPersonId", p.Id);*/ //если у человека есть два начальника,  
                                                                              //то тут возникнет ошибка

                        List<Subordinate> subs = context.Subordinates.Where(s => s.OwnPersonId == p.Id).ToList();
                        turn.Enqueue(subs[0].Person);

                    }
                }
                
            }
        }

       
    }
}
