using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureAfrica.Models
{
    public class AppDbContex : IdentityDbContext<ApplicationUser>
    {
        public AppDbContex(DbContextOptions<AppDbContex> options) :base(options)
        {

        }

        public DbSet<Incident> Incidents { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<IncidentType> IncidentTypes { get; set; }
        public DbSet<NotifiedUser> NotifiedUsers { get; set; }
        public  DbSet<IncidentUserMessage> IncidentUserMessages { get; set; }


        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Conventions

        //        // modelBuilder.Conventions.Add(
        //        //new AttributeToColumnAnnotationConvention<TableColumnAttribute, string>(
        //        //    "ServiceTableColumn", (property, attributes) => attributes.Single().ColumnType.ToString()));


        //    // modelBuilder.Entity<Person>()
        //    //.Property(p => p.DisplayName)
        //    //.HasComputedColumnSql("[LastName] + ', ' + [FirstName]");
        //}

    }
}
