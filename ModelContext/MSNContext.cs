using Microsoft.EntityFrameworkCore;
using MSN.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Components;
using System;

namespace MSN.ModelContext
{
    public class MSNContext : DbContext
    {
        public DbSet<ImagePost> ImagePosts { get; set; }
        public DbSet<StatusUpdateEntry> StatusUpdateEntries { get; set; }
        public DbSet<Album> Albums { get; set; }
        
        public DbSet<ApplicationSettings> ApplicationSettings { get; set; }
        public DbSet<Note> Notes { get; set; }        
        public DbSet<LoginAttempt> LoginAttempt { get; set; }
        public DbSet<AccountUser> AccountUser { get; set; }
        
        public DbSet<EventItem> EventItems { get; set; }
        public string ConnectionString { get; set; } = "Data Source=Data/MSN.db;";
        public MSNContext(string connectionString = null)
        {
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                ConnectionString = connectionString;
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(ConnectionString);
        }
    }
}