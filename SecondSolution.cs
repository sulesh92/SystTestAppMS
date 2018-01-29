using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAppSysTech
{
    class SecondSolution
    {
        public static List<int> tree; //Дерево хранит в себе targetPerson - сотрудников из группы Salesman

        public static Dictionary<int, double> salaryWeightDictionary;

        public static void StartCalculations(Person targetPerson, int numberOfStaff, DateTimeOffset accountingDate)
        {
            //Хранит id сотрудника из группы Salesman и 
            //его зарплату в сумме с зп подчиненных. 
            salaryWeightDictionary = new Dictionary<int, double>();

            List<int> mainTree = new List<int>() { targetPerson.Id};

            //Создает первичное "древо" из набора сотрудников группы Salesman
            //Возвращает сотрудника с последнего нижнего уровня
            Person lastPerson = CreateTree(targetPerson);

            foreach (int Id in tree)
            {
                mainTree.Add(Id);
            }
           
            bool[] isPersonSalaryCalculated = new bool[numberOfStaff];

            for (int i = mainTree.Count() - 1; i >= 0; i--)
            {
                targetPerson = CommonTools.FindPersonById(mainTree[i]);
                lastPerson = CreateTree(targetPerson);
                double subSalaryWeight = CalculateSalary(lastPerson, targetPerson, accountingDate, isPersonSalaryCalculated);
                salaryWeightDictionary.Add(targetPerson.Id, subSalaryWeight);
            }

            salaryWeightDictionary.Clear();

        }

        public static Person CreateTree(Person targetPerson)
        {
            tree = new List<int>();

            Queue<Person> turn = new Queue<Person>(); //хранит сотрудников, которые подлежат проверке 
            turn.Enqueue(targetPerson); //первый в очереди сотрудник для проверки

            Person currentPerson = new Person();

            while (turn.Count != 0)
            {
                currentPerson = turn.Dequeue();

                using (DataModelContext context = new DataModelContext())
                {
                    List<Group> groups = context.Groups.ToList();
                    List<Subordinate> tempSubList = context.Subordinates.ToList();

                    if (currentPerson.Subordinates.Count() > 0)
                    {
                        for (int i = 0; i < currentPerson.Subordinates.Count; i++)
                        {
                            var index = string.Format("{0}.{1}", currentPerson.Id, i);
                            List<Subordinate> subs = currentPerson.Subordinates.ToList();
                            //tree.Add(index, subs[i]); занимает много лишней памяти,
                            //экономней сохранять OwnPersonId подчиненного, который
                            //является его Id в таблице сотрудников
                            if (subs[i].Group == "Salesman")
                            {
                                tree.Add(subs[i].OwnPersonId);
                            }

                            //поиск подчиненного в Таблице сотрудников
                            //подчиненный заносится в список как следующий _person для проверки
                            //на наличии своих подчиненных. 
                            Person nextPerson = context.Persons.Find(subs[i].OwnPersonId);
                            turn.Enqueue(nextPerson);              //добавляет сотрудника в очередь
                        }
                    }
                }
            }

            return currentPerson;
        }


        public static double CalculateSalary(Person lastPerson, Person targetPerson,
                                       DateTimeOffset accountingDate, bool [] isPersonSalaryCalculated)
        {
            

            Queue<Person> turn = new Queue<Person>(); //хранит сотрудников, которые подлежат проверке 
            turn.Enqueue(lastPerson); //первый в очереди сотрудник для проверки

            double subsWeightTotal = 0D;
            double salary = 0D;

            while (isPersonSalaryCalculated[targetPerson.Id] == false)
            {
                using (DataModelContext context = new DataModelContext())
                {
                    List<Subordinate> tempSubList = context.Subordinates.ToList();
                    List<Salary> salaries = context.Salaries.ToList();
                    List<Group> groups = context.Groups.ToList();

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
                            if (isPersonSalaryCalculated[s.OwnPersonId] == false)
                            {
                                i++;
                                turn.Enqueue(context.Persons.Find(s.OwnPersonId));
                            }

                            //Если среди подчиненных оказывается Salesman, тогда
                            //из словаря достается сумма всех зарпалат подченных 
                            //сотрудника из группы Salesman
                            if (s.Group == "Salesman")
                            {
                                double _salaryWeight;
                                salaryWeightDictionary.TryGetValue(s.OwnPersonId, out _salaryWeight);

                                subsWeightTotal += _salaryWeight;
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
                    subsWeightTotal += salary;

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
            return subsWeightTotal;
        }
    }
}
