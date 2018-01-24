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

        public DateTime DateOfStart { get; set; }

        public int? GroupId { get; set; }
        public Group Group { get; set; }

        public ICollection<Salary> SalaryHistory { get; set; }
        public Person()
        {
            SalaryHistory = new List<Salary>();
        }
    }
}
