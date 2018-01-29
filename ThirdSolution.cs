using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAppSysTech
{
    class ThirdSolution
    {
        List<Group> groups;
        List<Person> persons;
        List<Subordinate> subordinates;

        //Дерево хранит в себе targetPerson - сотрудников из группы Salesman
        //Ключевы узлы
        public static List<int> tree; 

        //Сумма зарплат во фрагменте иерархии с одним
        //Salesman
        public class SalesmanMoneyBag
        {
            public double SalesmanSalaryBag { get; set; }
            public List<int> TakenById { get; set; }

            public SalesmanMoneyBag(double salaryBag, List<int> takenById)
            {
                SalesmanSalaryBag = salaryBag;
                TakenById = takenById;

            }
        }

        //Хранит данные о вышестояших Salesman, которые обратились к нижестоящему
        //за информацией о сумме зарплат. Нужна для того, чтобы избежать повторного 
        //вклада суммы одноги из salesman в общую корзину
        public static Dictionary<int, SalesmanMoneyBag> salaryWeightDictionary;

        public void StartCalculationsForAllPersons(DateTimeOffset accountingDate)
        {
            CountEmployeeSalary(accountingDate);

            CreateRealTree();
        }

        /// <summary>
        /// Метод расчитывает зарплаты всех сотрудников группы
        /// Employee
        /// </summary>
        public void CountEmployeeSalary(DateTimeOffset accountingDate)
        {
            using (DataModelContext context = new DataModelContext())
            {
                groups = context.Groups.ToList();
                persons = context.Persons.ToList();
                subordinates = context.Subordinates.ToList();
                
                List<Person> employies = persons.Where(p => p.Group.Name == "Employee").ToList();
                
                foreach(Person employee in employies)
                {
                    SalaryCalculators.CalculatePersonSalary(employee, accountingDate, 0);
                }
            }
        }

        public void CreateRealTree(Person lastPerson)
        {
            Queue<Person> turn = new Queue<Person>(); //хранит сотрудников, которые подлежат проверке 
            turn.Enqueue(lastPerson); //первый в очереди сотрудник для проверки

            while(turn.Count() != 0)
            {

            }
        }

    }
}
