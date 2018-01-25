using Microsoft.EntityFrameworkCore;

namespace TestAppSysTech  
{
    class DataModelContext : DbContext 
    {
        public DbSet<Person> Persons { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Salary> Salaries { get; set; }
         

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=HrDataBase.db");
        }
    }
}
