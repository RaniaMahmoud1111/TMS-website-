using Microsoft.EntityFrameworkCore;
using Trainning__Management.Models;

namespace Trainning__Management.Data
{
    public class T_MDbContext:DbContext
    {
        public T_MDbContext(DbContextOptions<T_MDbContext> options):base(options)
        { 
          
        }
       public DbSet<Training>training {  get; set; }
        public DbSet<Materials> materials { get; set; }
        public DbSet<Instructor> instructors { get; set; }
        
        public DbSet<Trainee> trainees { get; set; }
        public DbSet<Tasks> tasks { get; set; }
        public DbSet<Admin> admins { get; set; }
        public DbSet<attend> attends { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Hash the passwords
            string hashedPassword1 = BCrypt.Net.BCrypt.HashPassword("rania111");
            string hashedPassword2 = BCrypt.Net.BCrypt.HashPassword("marwa111");
            string hashedPassword3 = BCrypt.Net.BCrypt.HashPassword("manar111");

            // Seed data with hashed passwords
            modelBuilder.Entity<Admin>().HasData(
                new Admin
                {
                    Id = 1,
                    Username = "RaniaMM",
                    Email = "admin1@.com",
                    Phone = "1234567890",
                    Level = "Graduate",
                    ImageUrl = "defult.png",
                    Password = hashedPassword1, // Store hashed password
                    ConfPassword = hashedPassword1 // Ensure the confirm password is also hashed
                },
                new Admin
                {
                    Id = 2,
                    Username = "MarwaMM",
                    Email = "admin2@.com",
                    Phone = "0987654321",
                    Level = "Graduate",
                    ImageUrl = "defult.png",
                    Password = hashedPassword2, // Store hashed password
                    ConfPassword = hashedPassword2 // Ensure the confirm password is also hashed
                },
                new Admin
                {
                    Id = 3,
                    Username = "ManarMM",
                    Email = "admin3@.com",
                    Phone = "1122334455",
                    Level = "Graduate",
                    ImageUrl = "defult.png",
                    Password = hashedPassword3, // Store hashed password
                    ConfPassword = hashedPassword3 // Ensure the confirm password is also hashed
                }
            );
        }


    }
}
