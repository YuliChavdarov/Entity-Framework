using System;
using System.Collections.Generic;
using CodeFirst.Models;

namespace CodeFirst
{
    class Program
    {
        static void Main(string[] args)
        {
            NewsDbContext db = new NewsDbContext();
            db.Database.EnsureCreated();

            db.Categories.Add(new Category
            {
                Title = "Exciting news",
                News = new List<News>
                {
                    new News
                    {
                        Title = "Very exciting...",
                        Content = "asdf",
                        Comments = new List<Comment>
                        {
                            new Comment { Content = "true" },
                        }
                    }
                }
            });

            db.SaveChanges();
        }
    }
}
