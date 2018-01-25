using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAppSysTech 
{
    public class Person
    {
        public int Id { get; set; }

        [MaxLength(40)]
        public string Name { get; set; }

        public double BaseSalary{ get; set; }

        [DefaultValue("false")]
        public bool IsRoot { get; set; }

        public string Supervisor { get; set; }

        [MaxLength(40)]
        public string Login { get; set; }

        [MaxLength(40)] [MinLength(6)]
        public string Password { get; set; }

        public DateTimeOffset DateOfStart { get; set; }
        
        //Ссылка на группу к которой относится сотрудник
        public int? GroupId { get; set; }
        public Group Group { get; set; }

        //Зарплатная ведомость сотрудника
        public ICollection<Salary> SalaryHistory { get; set; }

        ////Список подчиненных

        public virtual ICollection<Person> SubPersons { get; set; }
        //public virtual List<Person> Supervisers { get; set; }

        //Конструктор класса Person
        public Person()
        {
            SubPersons = new List<Person>();
            SalaryHistory = new List<Salary>();
        }
    }
}
