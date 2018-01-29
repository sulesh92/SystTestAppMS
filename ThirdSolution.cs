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

        public static void StartCalculationsForAllPersons(DateTimeOffset accountingDate, int numberOfPersons, 
                                                           Person employee)
        {     
            //опеределенние самого высокого узла иерархии
            CreateRealTree(employee, numberOfPersons);
            Person targetPerson = CommonTools.FindPersonById(tree.Last());

            SecondSolution.StartCalculations(targetPerson, numberOfPersons, accountingDate);
        }

        /// <summary>
        /// Метод для расчета зарплаты всех сотрудников группы
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

                foreach (Person employee in employies)
                {
                    SalaryCalculators.CalculatePersonSalary(employee, accountingDate, 0);
                }
            }
        }

        public static void CreateRealTree(Person lastPerson, int numberOfPersons)
        {
            Queue<Person> turn = new Queue<Person>(); //хранит сотрудников, которые подлежат проверке 
            turn.Enqueue(lastPerson); //первый в очереди сотрудник для проверки
            bool[] isChecked = new bool[numberOfPersons];

            while (turn.Count() != 0)
            {
                using (DataModelContext context = new DataModelContext())
                {
                    List<Group> groups = context.Groups.ToList();
                    List<Subordinate> tempSubList = context.Subordinates.ToList();
                    List<Salary> salaries = context.Salaries.ToList();

                    tree = new List<int>();

                    

                    //Текущий сотрудник, для которого выполняется расчет зарплаты
                    Person p = context.Persons.Find(turn.Dequeue().Id);

                    //Осуществляется переход на уровень ниже, если у текущего сотрудника, 
                    //есть не просчитанные подчиненные
                    if (p.Subordinates.Count > 0)
                    {
                        int i = 0; //количество подчиненных с нерасчитанными зарплатами

                        foreach (Subordinate s in p.Subordinates)
                        {
                            //Каждый подчиненный записывается в очередь, как сотрудник
                            if (isChecked[s.OwnPersonId] == false)
                            {
                                i++;
                                turn.Enqueue(context.Persons.Find(s.OwnPersonId));
                            }
                            //Если среди подчиненных оказывается Salesman, тогда
                            //из словаря достается сумма всех зарпалат подченных 
                            //сотрудника из группы Salesman
                            if (s.Group == "Salesman")
                            {
                                tree.Add(s.OwnPersonId);

                            }
                        }

                        if (i != 0)
                        {
                            continue;
                        }
                    }

                    isChecked[p.Id] = true;

                    //Переход на уровень выше. Для этого текущего сотрудника находим в таблице подчиненных, поскольку 
                    //в классе Subperson хранится ссылка на начальника. После чего начальник добавляется в список Stack, 

                    List<Subordinate> subs = context.Subordinates.Where(s => s.OwnPersonId == p.Id).ToList();
                    if(subs.Count == 0)
                    {
                        tree.Add(p.Id);
                    }

                    //Создаем класс person в котором хранится информация о начальнике
                    Person superviser = new Person();
                    try
                    {
                        superviser = context.Persons.Find(subs[0].PersonId);
                    }
                    catch
                    {
                        continue;
                    }

                    if (!turn.Any(per => per.Id == superviser.Id))
                    {
                        turn.Enqueue(superviser);
                    }
                }
            }
        }
    }
}
