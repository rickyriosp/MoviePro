﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieProMVC.Models.Database;

namespace MovieProMVC.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Collection> Collection { get; set; }
        public DbSet<Movie> Movie { get; set; }
        public DbSet<MovieCollection> MovieCollection { get; set; }
        public DbSet<MovieCast> Cast { get; set; }
        public DbSet<MovieCrew> Crew { get; set; }
        public DbSet<MovieReview> Review { get; set; }
        public DbSet<MovieSimilar> Similar { get; set; }
    }
}