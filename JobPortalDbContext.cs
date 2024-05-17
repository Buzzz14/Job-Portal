using Microsoft.EntityFrameworkCore;
using JobPortal.Models;

namespace JobPortal
{
    public class JobPortalDbContext : DbContext
    {
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<User> Users { get; set; }
        public string DbPath { get; }
        public JobPortalDbContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "jobportal_new.db");
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
        public DbSet<JobPortal.Models.JobViewModel> JobViewModel { get; set; } = default!;
        public DbSet<JobPortal.Models.UserViewModel> UserViewModel { get; set; } = default!;
    }

    public class Job
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }  
    }
    public class Organization
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ContactNo { get; set; }

        public List<Job> job = new();
    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime CreatedDate{ get; set; }
    }

}