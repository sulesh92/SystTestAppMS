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

        /// <summary>
        /// Метод расчитывает зарплаты всех сотрудников группы
        /// EMployee
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


    }
}
