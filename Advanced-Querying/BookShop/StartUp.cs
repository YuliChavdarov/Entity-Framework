namespace BookShop
{
    using BookShop.Models;
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;
    using System.Text;
    using Z.EntityFramework.Plus;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            IncreasePrices(db);
            //Console.WriteLine(RemoveBooks(db));
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            StringBuilder sb = new StringBuilder();

            AgeRestriction restriction = Enum.Parse<AgeRestriction>(command, true);

            var books = context.Books
                .ToList()
                .Where(x => x.AgeRestriction == restriction)
                .Select(x => new { x.Title })
                .OrderBy(x => x.Title)
                .ToList();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var books = context.Books
                .ToList()
                .Where(x => x.EditionType == EditionType.Gold && x.Copies < 5000)
                .Select(x => new { x.Title, x.BookId })
                .OrderBy(x => x.BookId)
                .ToList();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var books = context.Books
                .Where(x => x.Price > 40)
                .Select(x => new { x.Title, x.Price })
                .OrderByDescending(x => x.Price)
                .ToList();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - ${book.Price:0.00}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            StringBuilder sb = new StringBuilder();

            var books = context.Books
                .Where(x => x.ReleaseDate.HasValue && x.ReleaseDate.Value.Year != year)
                .Select(x => new { x.Title, x.BookId})
                .OrderBy(x => x.BookId)
                .ToList();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            string[] categories = input.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var books = context.Books
                .Include(x => x.BookCategories)
                .ThenInclude(x => x.Category)
                .ToArray()
                .Where(x => x.BookCategories.Any(x => categories.Contains(x.Category.Name.ToLower())))
                .Select(x => new { x.Title })
                .OrderBy(x => x.Title)
                .ToArray();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title}");

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var authors = context.Authors
                .Where(x => x.FirstName.EndsWith(input))
                .Select(x => x.FirstName + " " + x.LastName)
                .OrderBy(x => x)
                .ToList();

            foreach(var author in authors)
            {
                sb.AppendLine($"{author}");
            }

            return sb.ToString().Trim();
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var titles = context.Books
                .Where(x => x.Title.ToLower().Contains(input.ToLower()))
                .OrderBy(x => x.Title)
                .Select(x => x.Title)
                .ToList();

            foreach(var title in titles)
            {
                sb.AppendLine($"{title}");
            }

            return sb.ToString().Trim();
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var books = context.Books
                .Where(x => x.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(x => x.BookId)
                .Select(x => new
                {
                    x.Title,
                    AuthorFullName = x.Author.FirstName + " " + x.Author.LastName
                })
                .ToList();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} ({book.AuthorFullName})");
            }

            return sb.ToString().Trim();
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            return context.Books
                .Where(x => x.Title.Length > lengthCheck)
                .Count();
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var authors = context.Authors
                .Select(x => new
                {
                    AuthorName = x.FirstName + " " + x.LastName,
                    CopiesCount = x.Books.Sum(x => x.Copies)
                })
                .OrderByDescending(x => x.CopiesCount)
                .ToList();

            foreach (var author in authors)
            {
                sb.AppendLine($"{author.AuthorName} - {author.CopiesCount}");
            }

            return sb.ToString().Trim();
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            StringBuilder sb = new StringBuilder();

            DateTime releaseDate = DateTime.ParseExact(date, "dd-MM-yyyy", null);

            var books = context.Books
                .Where(x => x.ReleaseDate < releaseDate)
                .OrderByDescending(x => x.ReleaseDate)
                .Select(x => new 
                { 
                    x.Title, 
                    x.EditionType, 
                    x.Price 
                })
                .ToList();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:0.00}");
            }
            return sb.ToString().Trim();
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var categories = context.Categories
                .Select(x => new
                {
                    CategoryName = x.Name,
                    TotalProfit = x.CategoryBooks.Sum(x => x.Book.Price * x.Book.Copies)
                })
                .OrderByDescending(x => x.TotalProfit)
                .ThenBy(x => x.CategoryName)
                .ToList();

            foreach (var category in categories)
            {
                sb.AppendLine($"{category.CategoryName} ${category.TotalProfit:0.00}");
            }

            return sb.ToString().Trim();
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var categories = context.Categories
                .Select(x => new
                {
                    CategoryName = x.Name,
                    MostRecent = x.CategoryBooks
                    .Where(x => x.Book.ReleaseDate.HasValue)
                    .OrderByDescending(x => x.Book.ReleaseDate)
                    .Take(3)
                    .Select(x => new
                    {
                        Title = x.Book.Title,
                        ReleaseDate = x.Book.ReleaseDate
                    })
                    .ToList()
                })
                .OrderBy(x => x.CategoryName)
                .ToList();

            foreach (var category in categories)
            {
                sb.AppendLine($"--{category.CategoryName}");
                foreach (var book in category.MostRecent)
                {
                    sb.AppendLine($"{book.Title} ({book.ReleaseDate?.Year})");
                }
            }

            return sb.ToString().Trim();
        }

        public static void IncreasePrices(BookShopContext context)
        {
            var books = context.Books
                .Where(x => x.ReleaseDate.HasValue && x.ReleaseDate.Value.Year < 2010)
                .UpdateFromQuery(x => new Book { Price = x.Price + 5M });
        }

        public static int RemoveBooks(BookShopContext context)
        {
            return context.Books.Where(x => x.Copies < 4200).DeleteFromQuery();
        }
    }
}
