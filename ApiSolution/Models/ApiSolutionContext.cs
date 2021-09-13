using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiSolution.Models;

namespace ApiSolution.Models
{
    public class ApiSolutionContext : DbContext
    {
        public ApiSolutionContext(DbContextOptions<ApiSolutionContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<ApiSolution.Models.Genre> Genre { get; set; }
    }
}
