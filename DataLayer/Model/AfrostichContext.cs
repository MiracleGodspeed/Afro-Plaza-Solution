﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataLayer.Model
{
    public class AfrostichContext : DbContext
    {
        public AfrostichContext(DbContextOptions<AfrostichContext> options)
      : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            foreach (var relationship in modelbuilder.Model.GetEntityTypes().SelectMany(f => f.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
            base.OnModelCreating(modelbuilder);
        }

        public DbSet<Gender> GENDER { get; set; }
        public DbSet<Person> PERSON { get; set; }
        public DbSet<User> USER { get; set; }
        public DbSet<Role> ROLE { get; set; }
    }
}
