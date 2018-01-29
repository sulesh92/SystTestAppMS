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
        private List<Salary> salaries;
        private List<Group> groups;
        private List<Subordinate> subordinates;

        public SalaryPage()
        {
            this.InitializeComponent();

            this.Loaded += SalaryPage_Loaded;
            persons = new ObservableCollection<Person>();
            salaries = new List<Salary>();
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
            
            Person targetPerson = currentStaffList.SelectedItem as Person; //сотрудник для, которого необходимо посчитать зарплату
            int numberOfStaff = persons.Last().Id + 1; // список persons формируется сразу после загрузки страницы
            DateTimeOffset accountingDate = datePickerOnSalaryPage.Date;

            SecondSolution.StartCalculations(targetPerson, numberOfStaff, accountingDate);

            //List<int> tree = FirstSolution.CreateTree(targetPerson, numberOfStaff);
            //Person lastPerson = CommonTools.FindPersonById(tree.Last());


            //double salary = FirstSolution.CalculateSalary(lastPerson, targetPerson, numberOfStaff, accountingDate);

            //CommonTools.ShowMessageAsync("Зарплата " + targetPerson.Name + "равна " + salary.ToString());
        }

        private void CurrentStaffList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            certainStaffScrollViewer.Visibility = Visibility.Visible;
            var d = currentStaffList.SelectedItem as Person;
            certainStaffList.ItemsSource = salaries.Where(s => s.PersonId == d.Id);
        }



        /////////////////////////////////////////МОЗГ//////////////////////////////////////////////////

        ////Dictionary<string, Subordinate> tree = new Dictionary<string, Subordinate>();

        //private List<int> CreateTree(Person chosenPerson, int numberOfStaff)
        //{
        //    List<int> tree = new List<int>();

        //    Queue<Person> turn = new Queue<Person>(); //хранит сотрудников, которые подлежат проверке 
        //    turn.Enqueue(chosenPerson); //первый в очереди сотрудник для проверки


        //    while (turn.Count != 0)
        //    {
        //        Person person = turn.Dequeue();

        //        using (DataModelContext context = new DataModelContext())
        //        {
        //            List<Group> groups = context.Groups.ToList();
        //            List<Subordinate> tempSubList = context.Subordinates.ToList();

        //            if (person.Subordinates.Count() > 0)
        //            {
        //                for (int i = 0; i < person.Subordinates.Count; i++)
        //                {
        //                    var index = string.Format("{0}.{1}", person.Id, i);
        //                    List<Subordinate> subs = person.Subordinates.ToList();
        //                    //tree.Add(index, subs[i]); занимает много лишней памяти,
        //                    //экономней сохранять OwnPersonId подчиненного, который
        //                    //является его Id в таблице сотрудников

        //                    tree.Add(subs[i].OwnPersonId);

        //                    //поиск подчиненного в Таблице сотрудников
        //                    //подчиненный заносится в список как следующий _person для проверки
        //                    //на наличии своих подчиненных. 
        //                    Person nextPerson = context.Persons.Find(subs[i].OwnPersonId);
        //                    turn.Enqueue(nextPerson);              //добавляет сотрудника в очередь
        //                }
        //            }
        //        }
        //    }

        //    return tree;
        //}

        //private double CalculateSalary(Person lastPerson, Person targerPerson, int numberOfStaff)
        //{
        //    bool[] isPersonSalaryCalculated = new bool[numberOfStaff];
        //    //Stack<Person> stack = new Stack<Person>();
        //    //stack.Push(lastPerson);
        //    Queue<Person> turn = new Queue<Person>(); //хранит сотрудников, которые подлежат проверке 
        //    turn.Enqueue(lastPerson); //первый в очереди сотрудник для проверки

        //    double subsWeightTotal = 0D;
        //    double salary = 0D;

        //    while (isPersonSalaryCalculated[targerPerson.Id] == false)
        //    {
        //        using (DataModelContext context = new DataModelContext())
        //        {
        //            List<Subordinate> tempSubList = context.Subordinates.ToList();
        //            List<Salary> salaries = context.Salaries.ToList();
        //            List<Group> groups = context.Groups.ToList();

        //            Person p = context.Persons.Find(turn.Dequeue().Id);

        //            //Осуществляется переход на уровень ниже, если у текущего сотрудника, 
        //            //есть не просчитанные подчиненные
        //            if (p.Subordinates.Count > 0)
        //            {
        //                int i = 0; //количество подчиненных с нерасчитанными зарплатами
        //                foreach(Subordinate s in p.Subordinates)
        //                {
        //                    //Каждый подчиненный записывается в очередь, как сотрудник
        //                    if(isPersonSalaryCalculated[s.OwnPersonId] == false)
        //                    {
        //                        i++;
        //                        turn.Enqueue(context.Persons.Find(s.OwnPersonId));
        //                    }
        //                }
        //                if(i != 0)
        //                {
        //                    continue;
        //                }
        //            }

        //            //Иначе расчитывается зарплата текущего сотрудника и осуществляется
        //            //переход на уровень выше

        //                DateTimeOffset accountingDate = datePickerOnSalaryPage.Date;

        //                //Возвращает зарплату текущего сотрудника
        //                SalaryCalculators.CalculatePersonSalary(p, accountingDate, subsWeightTotal);

        //                isPersonSalaryCalculated[p.Id] = true;
        //                //Переход на уровень выше. Для этого текущего сотрудника находим в таблице подчиненных, поскольку 
        //                //в классе Subperson хранится ссылка на начальника. После чего начальник добавляется в список Stack, 

        //                /*  context.Subordinates.Find("OwnPersonId", p.Id);*/ //если у человека есть два начальника,  
        //                                                                      //то тут возникнет ошибка, поэтому 
        //                                                                      //сохраняем результат в список подчиненных

        //                List<Subordinate> subs = context.Subordinates.Where(s => s.OwnPersonId == p.Id).ToList();

        //                //Создаем класс person в котором хранится информация о начальнике
        //                Person superviser = context.Persons.Find(subs[0].PersonId);

        //                if (!turn.Any(per => per.Id == superviser.Id))
        //                {
        //                    turn.Enqueue(superviser);
        //                }
        //        }
        //    }
        //    return salary;
        //}


    }
}
