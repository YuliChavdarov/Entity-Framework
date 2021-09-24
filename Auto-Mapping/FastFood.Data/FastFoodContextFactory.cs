using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastFood.Data
{
    public class FastFoodContextFactory : IDesignTimeDbContextFactory<FastFoodContext>
    {
        public FastFoodContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<FastFoodContext>();
            optionsBuilder.UseSqlServer("Server=.;Integrated security=true;Database=FastFood");

            return new FastFoodContext(optionsBuilder.Options);
        }
    }
}
