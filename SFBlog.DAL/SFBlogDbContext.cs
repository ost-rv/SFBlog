using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SFBlog.DAL.Models;
using SFBlog.DAL.Models.Config;
using SFBlog.DAL.Repository;

namespace SFBlog.DAL
{
    /// <summary>
    /// Класс контекста, предоставляющий доступ к сущностям базы данных
    /// </summary>
    public class SFBlogDbContext : DbContext
    {
        ServiceCollection _serviceProvider = new ServiceCollection();
        /// Ссылка на таблицу Users
        //public DbSet<User> Users { get; set; }
        public SFBlogDbContext(DbContextOptions<SFBlogDbContext> options) : base(options)
        {
            //база данных создается при необходимости
            //Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            if (!optionsBuilder.IsConfigured)
            {

            }
 
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new PostTagConfiguration());

        }

    }


}
