using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MSN.BlazorServer.Data
{
    public class MSNBlazorServerContext : IdentityDbContext<IdentityUser>
    {
        public string ConnectionString { get; set; } = "Data Source=Data/MSNSecurity.db;";
        public DbSet<SecurityStatement> SecurityStatements { get; set; }        
        public MSNBlazorServerContext(DbContextOptions<MSNBlazorServerContext> options)
            : base(options)
        {
        }

        public MSNBlazorServerContext(string connectionString )
        {
           if (!string.IsNullOrWhiteSpace(connectionString))
           {
               ConnectionString = connectionString;
           }
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           optionsBuilder.UseSqlite(ConnectionString);
        }
    }
    
    public class SecurityStatement
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid AspNetUsersID { get; set; } 
        public string Statement { get; set; } = string.Empty;

    }
}
