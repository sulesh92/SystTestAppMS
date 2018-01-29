using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAppSysTech
{
    class ExampleClass
    {
        public void AddTestPersons()
        {
            List<String> personsList = new List<string> { "S", "N", "M", "E", "L", "K", "F", "D", "R", "G", "Q" };
            using (DataModelContext context = new DataModelContext())
            {
                List<Group> groups = context.Groups.ToList();
                Person S = new Person
                {
                    Name = "S",
                    BaseSalary = 30000,
                    Group = groups[0] as Group,
                    Login = "root",
                    Password = "root",
                    DateOfStart = Convert.ToDateTime("2017-08-31 22:26:59.4535923+02:00")
                };

                Person N = new Person
                {
                    Name = "N",
                    BaseSalary = 23000,
                    Group = groups[0] as Group,
                    Login = "roo2t",
                    Password = "roo2t",
                    DateOfStart = Convert.ToDateTime("2015-03-31 22:26:59.4535923+02:00")
                };

                Person M = new Person
                {
                    Name = "М",
                    BaseSalary = 20000,
                    Group = groups[1] as Group,
                    Login = "roo2t",
                    Password = "roo2t",
                    DateOfStart = Convert.ToDateTime("2015-08-31 22:26:59.4535923+02:00")
                };

                Person E = new Person
                {
                    Name = "E",
                    BaseSalary = 13000,
                    Group = groups[2] as Group,
                    Login = "roo2t",
                    Password = "roo2t",
                    DateOfStart = Convert.ToDateTime("2015-08-31 22:26:59.4535923+02:00")
                };

                Person L = new Person
                {
                    Name = "L",
                    BaseSalary = 12000,
                    Group = groups[2] as Group,
                    Login = "roo2t",
                    Password = "roo2t",
                    DateOfStart = Convert.ToDateTime("2015-03-31 22:26:59.4535923+02:00")
                };

                Person K = new Person
                {
                    Name = "K",
                    BaseSalary = 15000,
                    Group = groups[2] as Group,
                    Login = "roo2t",
                    Password = "roo2t",
                    DateOfStart = Convert.ToDateTime("2015-07-31 22:26:59.4535923+02:00")
                };

                Person F = new Person
                {
                    Name = "F",
                    BaseSalary = 13000,
                    Group = groups[2] as Group,
                    Login = "roo2t",
                    Password = "roo2t",
                    DateOfStart = Convert.ToDateTime("2016-01-31 22:26:59.4535923+02:00")
                };

                Person D = new Person
                {
                    Name = "D",
                    BaseSalary = 23000,
                    Group = groups[0] as Group,
                    Login = "roo2t",
                    Password = "roo2t",
                    DateOfStart = Convert.ToDateTime("2016-01-31 22:26:59.4535923+02:00")
                };

                Person R = new Person
                {
                    Name = "R",
                    BaseSalary = 21000,
                    Group = groups[1] as Group,
                    Login = "roo2t",
                    Password = "roo2t",
                    DateOfStart = Convert.ToDateTime("2016-12-11 22:26:59.4535923+02:00")
                };

                Person G = new Person
                {
                    Name = "G",
                    BaseSalary = 21000,
                    Group = groups[0] as Group,
                    Login = "roo2t",
                    Password = "roo2t",
                    DateOfStart = Convert.ToDateTime("2016-01-31 22:26:59.4535923+02:00")
                };

                Person Q = new Person
                {
                    Name = "Q",
                    BaseSalary = 21000,
                    Group = groups[2] as Group,
                    Login = "roo2t",
                    Password = "roo2t",
                    DateOfStart = Convert.ToDateTime("2016-02-21 22:26:59.4535923+02:00")
                };

                context.Groups.Attach(S.Group);
                context.Persons.Add(S);

                context.Groups.Attach(N.Group);
                context.Persons.Add(N);

                context.Groups.Attach(M.Group);
                context.Persons.Add(M);

                context.Groups.Attach(E.Group);
                context.Persons.Add(E);

                context.Groups.Attach(L.Group);
                context.Persons.Add(L);

                context.Groups.Attach(K.Group);
                context.Persons.Add(K);

                context.Groups.Attach(F.Group);
                context.Persons.Add(F);

                context.Groups.Attach(D.Group);
                context.Persons.Add(D);

                context.Groups.Attach(R.Group);
                context.Persons.Add(R);

                context.Groups.Attach(G.Group);
                context.Persons.Add(G);

                context.Groups.Attach(Q.Group);
                context.Persons.Add(Q);

                context.SaveChanges();

                AddSubordinate(N, S);
                AddSubordinate(M, S);
                AddSubordinate(E, S);

                AddSubordinate(K, N);
                AddSubordinate(L, N);

                AddSubordinate(F, M);
                AddSubordinate(D, M);

                AddSubordinate(R, D);
                AddSubordinate(G, D);
                AddSubordinate(Q, D);
            }

           
        }

        /// <summary>
        /// Сохраняет выбранных подчиненых в таблицу Subordinate
        /// </summary>
        private void AddSubordinate(Person subPerson, Person superviser)
        {
            using (DataModelContext context = new DataModelContext())
            {
                Subordinate subordinate = new Subordinate();

                subordinate.Name = subPerson.Name;
                subordinate.Group = subPerson.Group.Name;

                subordinate.PersonId = superviser.Id;
                subordinate.OwnPersonId = subPerson.Id;
                context.Persons.Attach(superviser);

                context.Subordinates.Add(subordinate);

                context.SaveChanges();
            }
        }

    }

}
