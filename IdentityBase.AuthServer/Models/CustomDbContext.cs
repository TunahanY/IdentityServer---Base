using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityBased.AuthServer.Models
{
    public class CustomDbContext : DbContext
    {
        public CustomDbContext(DbContextOptions opts) : base(opts)
        {

        }


        public DbSet<CustomUser> customUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //md6, sha256, sha512.. We need two method to validate passwords hashes
            modelBuilder.Entity<CustomUser>().HasData(
                new CustomUser() { Id = 1 , Email = "mtunahanyollar@gmail.com",Password = "password",City = "Ankara", UserName="tunahanyollar"},
                new CustomUser() { Id = 2 , Email = "mty@gmail.com",Password = "password2",City = "Istanbul", UserName="tunahanyollar2"},
                new CustomUser() { Id = 3 , Email = "testmail@gmail.com",Password = "password3",City = "Munich", UserName="tunahanyollar3"});

            base.OnModelCreating(modelBuilder);
        }
    }
}
