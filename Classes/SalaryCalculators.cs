using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAppSysTech
{
    class SalaryCalculators
    {
        /// <summary>
        /// Возвращает зарплату сотрудника
        /// </summary>
        /// <param name="p">сотрудник</param>
        /// <param name="accountingDate">Расчетная дата</param>
        /// <param name="subsWeightTotal">Зарплата всех подчиненных</param>
        /// <returns></returns>
        public static Double CalculatePersonSalary(Person p, DateTimeOffset accountingDate, double subsWeightTotal)
        {
                      
            TimeSpan difyears = accountingDate - p.DateOfStart;
            double years = difyears.TotalDays / 365;
            double salary = 0;

            switch (p.Group.Name)
            {
                case "Manager":
                    //Возвращает сумму зарплат подчиненных первого уровня
                    double subsWeightFirstLevel = CalculateFirstLevelSubWeight(p, accountingDate.Date.Month.ToString(),
                                                                               accountingDate.Date.Year.ToString());

                    //Возвращает зарплату менеджера
                    salary = CalculateSinglePersonSalary(p.BaseSalary, p.Group.YearsCoefficient,
                                       (int)years, p.Group.SubNumberCoeff, subsWeightFirstLevel,
                                       p.Group.Limit);

                    break;

                case "Salesman":

                    salary = CalculateSinglePersonSalary(p.BaseSalary, p.Group.YearsCoefficient,
                                       (int)years, p.Group.SubNumberCoeff, subsWeightTotal,
                                       p.Group.Limit);
                    break;

                case "Employee":
                    salary = CalculateSinglePersonSalary(p.BaseSalary, p.Group.YearsCoefficient,
                                       (int)years, p.Group.SubNumberCoeff, subsWeightTotal,
                                       p.Group.Limit);
                    break;
            }

            //Сохраним зарплату в БД
            AddSalaryToDatabase(p, accountingDate.Month.ToString(), accountingDate.Year.ToString(), salary);

            return salary;
        }

        /// <summary>
        /// Возвращает зарплату подчиненных первого уровня
        /// </summary>
        /// <param name="p">Начальник</param>
        /// <param name="accountingMonth">Расчетный месяц</param>
        /// <param name="accountingYear">Расчетный год</param>
        /// <returns></returns>
        private static double CalculateFirstLevelSubWeight(Person p, string accountingMonth, string accountingYear)
        {
            double weight = 0D;

            using (DataModelContext context = new DataModelContext())
            {
                List<Salary> salaries = context.Salaries.ToList();

                foreach (Subordinate subordinate in p.Subordinates)
                {
                    Person _person = context.Persons.Find(subordinate.OwnPersonId);
                    List<Salary> _personSalary = _person.SalaryHistory.Where(s => s.Month == accountingMonth && s.Year == accountingYear).ToList();

                    weight += _personSalary[0].CurrentSalary;
                }
            }

            return weight;
        }

        private static double CalculateSinglePersonSalary(double baseSalary, double yearsCoefficient, 
                                                          int years, double subsCoefficient, double subsWeight, 
                                                          double limit)
        {
            //если меньше лимита, то возввращает расчетное значение,
            //иначе в расчете зарплаты будет учтен максимальный сумарный вклад за стаж
            double loyaltyWeight = yearsCoefficient * years <= limit ? yearsCoefficient * years : limit;
            double salary = baseSalary + baseSalary*loyaltyWeight + subsCoefficient * subsWeight;
            return salary;
        }

        private static void AddSalaryToDatabase(Person p, string month, string year, double salary)
        {
            using (DataModelContext context = new DataModelContext())
            {
                var sal = p.SalaryHistory.Where(sHistory => sHistory.Month == month 
                                                && sHistory.Year == year);
                //удаляет записи от предыдущих решений
                foreach(Salary _sal in sal)
                {
                    context.Salaries.Remove(_sal);
                } 

                Salary s = new Salary { Month = month,
                                        Year = year,
                                        Group = p.Group.Name,
                                        CurrentSalary = salary,
                                        Person = p};
                context.Persons.Attach(p);
                context.Salaries.Add(s);
                context.SaveChanges();
            }
        }
    }
}
