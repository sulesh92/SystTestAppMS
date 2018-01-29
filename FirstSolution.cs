using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAppSysTech
{
    class FirstSolution
    {

        public static List<int> CreateTree(Person chosenPerson, int numberOfStaff)
        {
            List<int> tree = new List<int>();

            Queue<Person> turn = new Queue<Person>(); //хранит сотрудников, которые подлежат проверке 
            turn.Enqueue(chosenPerson); //первый в очереди сотрудник для проверки

            Person person = new Person();

            while (turn.Count != 0)
            {
                person = turn.Dequeue();

                using (DataModelContext context = new DataModelContext())
                {
                    List<Group> groups = context.Groups.ToList();
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

            return tree;
        }

        public static double CalculateSalary(Person lastPerson, Person targerPerson, int numberOfStaff, 
                                       DateTimeOffset accountingDate)
        {
            bool[] isPersonSalaryCalculated = new bool[numberOfStaff];
            //Stack<Person> stack = new Stack<Person>();
            //stack.Push(lastPerson);
            Queue<Person> turn = new Queue<Person>(); //хранит сотрудников, которые подлежат проверке 
            turn.Enqueue(lastPerson); //первый в очереди сотрудник для проверки

            double subsWeightTotal = 0D;
            double salary = 0D;

            while (isPersonSalaryCalculated[targerPerson.Id] == false)
            {
                using (DataModelContext context = new DataModelContext())
                {
                    List<Subordinate> tempSubList = context.Subordinates.ToList();
                    List<Salary> salaries = context.Salaries.ToList();
                    List<Group> groups = context.Groups.ToList();

                    Person p = context.Persons.Find(turn.Dequeue().Id);

                    //Осуществляется переход на уровень ниже, если у текущего сотрудника, 
                    //есть не просчитанные подчиненные
                    if (p.Subordinates.Count > 0)
                    {
                        int i = 0; //количество подчиненных с нерасчитанными зарплатами
                        foreach (Subordinate s in p.Subordinates)
                        {
                            //Каждый подчиненный записывается в очередь, как сотрудник
                            if (isPersonSalaryCalculated[s.OwnPersonId] == false)
                            {
                                i++;
                                turn.Enqueue(context.Persons.Find(s.OwnPersonId));
                            }
                        }
                        if (i != 0)
                        {
                            continue;
                        }
                    }

                    //Иначе расчитывается зарплата текущего сотрудника и осуществляется
                    //переход на уровень выше



                    //Возвращает зарплату текущего сотрудника
                    salary = SalaryCalculators.CalculatePersonSalary(p, accountingDate, subsWeightTotal);

                    isPersonSalaryCalculated[p.Id] = true;
                    //Переход на уровень выше. Для этого текущего сотрудника находим в таблице подчиненных, поскольку 
                    //в классе Subperson хранится ссылка на начальника. После чего начальник добавляется в список Stack, 

                    /*  context.Subordinates.Find("OwnPersonId", p.Id);*/ //если у человека есть два начальника,  
                                                                          //то тут возникнет ошибка, поэтому 
                                                                          //сохраняем результат в список подчиненных

                    List<Subordinate> subs = context.Subordinates.Where(s => s.OwnPersonId == p.Id).ToList();

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
            return salary;
        }
    }
}
