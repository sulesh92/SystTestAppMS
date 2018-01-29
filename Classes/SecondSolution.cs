using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAppSysTech
{
    class SecondSolution
    {
        //Дерево хранит в себе targetPerson - сотрудников из группы Salesman
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

        

        public static void StartCalculations(Person targetPerson, int numberOfStaff, DateTimeOffset accountingDate)
        {
            List<int> mainTree = new List<int>();
            try
            {
                 mainTree.Add(targetPerson.Id);
            }
            catch
            {
                CommonTools.ShowMessageAsync("Сначала выберите сотрудника из списка слева");
                return;
            }

            //Хранит id сотрудника из группы Salesman и 
            //его зарплату в сумме с зп подчиненных. 
            salaryWeightDictionary = new Dictionary<int, SalesmanMoneyBag>();

            bool[] isPersonSalaryCalculated = new bool[numberOfStaff];

            //Создает первичное "древо" из набора сотрудников группы Salesman
            //Возвращает сотрудника с последнего нижнего уровня
            Person lastPerson = CreateTree(targetPerson, isPersonSalaryCalculated);

            foreach(int ind in tree)
            {
                mainTree.Add(ind);
            }
            
            for (int i = mainTree.Count() - 1; i >= 0; i--)
            {
                targetPerson = CommonTools.FindPersonById(mainTree[i]);
                lastPerson = CreateTree(targetPerson, isPersonSalaryCalculated);
                double subSalaryWeight = CalculateSalary(lastPerson, targetPerson, accountingDate, 
                                                         isPersonSalaryCalculated);
                SalesmanMoneyBag subsBag = new SalesmanMoneyBag(subSalaryWeight, new List<int>());
 
                salaryWeightDictionary.Add(targetPerson.Id, subsBag);
            }

            salaryWeightDictionary.Clear();
        }

        public static Person CreateTree(Person targetPerson, bool[] isPersonSalaryCalculated)
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
                           
                            List<Subordinate> subs = currentPerson.Subordinates.ToList();

                            if (subs[i].Group == "Salesman")
                            {
                                tree.Add(subs[i].OwnPersonId);
                            }

                            //Если зарплата для сотрудника НЕ посчитана, тогда
                            //он добавляется следующим узлом в дерево, иначе его 
                            //ветвь прерывается
                            if (!isPersonSalaryCalculated[subs[i].OwnPersonId])
                            {
                                //поиск подчиненного в Таблице сотрудников по его OwnPersonId
                                //подчиненный заносится в список как следующий сотрудник для проверки
                                //на наличии своих подчиненных. 
                                Person nextPerson = context.Persons.Find(subs[i].OwnPersonId);
                                turn.Enqueue(nextPerson);              //добавляет сотрудника в очередь
                            }
                            
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
                    List<Group> groups = context.Groups.ToList();
                    List<Subordinate> tempSubList = context.Subordinates.ToList();
                    List<Salary> salaries = context.Salaries.ToList();
                    

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
                                salaryWeightDictionary.TryGetValue(s.OwnPersonId, out SalesmanMoneyBag _salaryWeight);

                                //Если текущий начальник в первый раз прибавляет
                                //сумму подчиненного Salesman-a, тогда сумма будет
                                //записана в общий котел.
                                if (!_salaryWeight.TakenById.Contains(s.PersonId))
                                {
                                    subsWeightTotal += _salaryWeight.SalesmanSalaryBag;
                                    _salaryWeight.TakenById.Add(s.PersonId);
                                    salaryWeightDictionary[s.OwnPersonId] = new SalesmanMoneyBag(s.OwnPersonId, _salaryWeight.TakenById);
                                }
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
