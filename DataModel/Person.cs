using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAppSysTech 
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double BaseSalary { get; set; }
        public bool IsRoot { get; set; }
        public bool Supervisor { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public int? GroupId { get; set; }
        public Group Group { get; set; }

        public ICollection<Salary> SalaryHistory { get; set; }
        public Person()
        {
            SalaryHistory = new List<Salary>();
        }
    }
}
