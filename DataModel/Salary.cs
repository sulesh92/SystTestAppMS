using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAppSysTech
{
    public class Salary
    {
        public int Id { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public int NumberOfSubordinates { get; set; }
        public string Group { get; set; }
        public double CurrentSalary { get; set; }

        public int PersonId { get; set; }
        public Person Person { get; set; }
    }
}
