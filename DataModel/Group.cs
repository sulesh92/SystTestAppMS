using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAppSysTech 
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Person> Persons { get; set; }
        public Group()
        {
            Persons = new List<Person>();
        }
    }
}
