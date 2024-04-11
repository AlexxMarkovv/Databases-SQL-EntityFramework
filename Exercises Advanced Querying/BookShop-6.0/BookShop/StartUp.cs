namespace BookShop
{
    using BookShop.Models;
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using System.Globalization;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var context = new BookShopContext();
            DbInitializer.ResetDatabase(context);

            string input = Console.ReadLine();

            Console.WriteLine(GetBooksByCategory(context, input));

        }

        // 2.Age Restriction
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            // Variant 1

            //bool hasParsed = Enum.TryParse(typeof(AgeRestriction), command, true, out object? ageRestrictionObj);
            //AgeRestriction ageRestriction;

            //if (hasParsed)
            //{
            //    ageRestriction = (AgeRestriction)ageRestrictionObj;


            //    var bookTitles = context.Books
            //      .Where(b => b.AgeRestriction == ageRestriction)
            //      .Select(b => b.Title)
            //      .OrderBy(b => b)
            //      .ToList();

            //    return string.Join(Environment.NewLine, bookTitles);
            //}

            //return null;


            // Variant 2
            if (!Enum.TryParse<AgeRestriction>(command, true, out var parsedAgeRestr))
            {
                return $"Command \"{command}\" is not valid!";
            }

            var bookTitles = context.Books
                .Where(b => b.AgeRestriction == parsedAgeRestr)
                .Select(b => b.Title)
                .ToList();

            return string.Join(Environment.NewLine, bookTitles);

            // Variant 3
            //try
            //{
            //    AgeRestriction ageRestriction = Enum.Parse<AgeRestriction>(command, true);

            //    var bookTitles = context.Books
            //        .Where(b => b.AgeRestriction == ageRestriction)
            //        .Select(b => b.Title)
            //        .OrderBy(b => b)
            //        .ToArray();

            //    return string.Join(Environment.NewLine, bookTitles);
            //}
            //catch (Exception e)
            //{
            //    return e.Message;
            //}
        }

        // 3.Golden Books
        public static string GetGoldenBooks(BookShopContext context)
        {
            var bookTitles = context.Books
                    .Where(b => b.EditionType == EditionType.Gold && b.Copies < 5000)
                    .Select(b => new { b.BookId, b.Title })
                    .OrderBy(b => b.BookId)
                    .ToArray();

            return string.Join(Environment.NewLine, bookTitles.Select(b => b.Title));
        }

        // 4.Books by Price
        public static string GetBooksByPrice(BookShopContext context)
        {
            var bookTitles = context.Books
                .Where(b => b.Price > 40)
                .Select(b => new
                {
                    BookTitle = b.Title,
                    BookPrice = b.Price
                })
                .OrderByDescending(b => b.BookPrice)
                .ToList();

            StringBuilder sb = new StringBuilder();
            foreach (var book in bookTitles)
            {
                sb.AppendLine($"{book.BookTitle} - ${book.BookPrice:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        // 5.Not Released In
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var bookTitles = context.Books
                .Where(b => b.ReleaseDate.HasValue && b.ReleaseDate.Value.Year != year)
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToList();

            return string.Join(Environment.NewLine, bookTitles);
        }

        // 6.Book Titles by Category
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            string[] categories = input
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.ToLower())
                .ToArray();

            var bookTitles = context.Books
                .Select(b => new
                {
                    b.Title,
                    b.BookCategories
                })
                .Where(b => b.BookCategories
                    .Any(bc => categories.Contains(bc.Category.Name.ToLower())))
                .OrderBy(b => b.Title)
                .ToArray();

            return string.Join(Environment.NewLine, bookTitles.Select(b => b.Title));
        }

        // 7.Released Before Date
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            DateTime parsedDate = DateTime
                .ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var bookTitles = context.Books
                .Where(b => b.ReleaseDate < parsedDate)
                .Select(b => new
                {
                    BookTitle = b.Title,
                    b.EditionType,
                    b.Price,
                    b.ReleaseDate
                })
                .OrderByDescending(b => b.ReleaseDate)
                .ToList();

            StringBuilder sb = new StringBuilder();
            foreach (var book in bookTitles)
            {
                sb.AppendLine($"{book.BookTitle} - {book.EditionType} - ${book.Price:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        // 8.Author Search
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authorNames = context.Authors
                .Where(a => a.FirstName.EndsWith(input.ToLower()))
                .Select(a => new
                {
                    FullName = a.FirstName + " " + a.LastName
                })
                .OrderBy(a => a.FullName)
                .ToList();

            return string.Join(Environment.NewLine, authorNames.Select(a => a.FullName));

            //StringBuilder sb = new();
            //foreach (var author in authorNames)
            //{
            //    sb.AppendLine(author.FullName);
            //}

            //return sb.ToString().TrimEnd();
        }

        // 9.Book Search
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var bookTitles = context.Books
                .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                .Select(b => new { b.Title })
                .OrderBy(b => b.Title)
                .ToList();

            return string.Join(Environment.NewLine, bookTitles.Select(b => b.Title));
        }

        // 10. Book Search by Author
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var books = context.Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .Select(b => new
                {
                    b.Title,
                    b.BookId,
                    AuthorName = b.Author.FirstName + " " + b.Author.LastName,
                })
                .OrderBy(b => b.BookId)
                .ToList();

            return string.Join(Environment.NewLine, books
                .Select(b => $"{b.Title} ({b.AuthorName})"));
        }

        // 11.Count Books
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var booksCount = context.Books
                .Count(b => b.Title.Length > lengthCheck);
            return booksCount;
        }

        // 12.Total Book Copies
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authors = context.Authors
                .Select(a => new
                {
                    AuthorName = a.FirstName + " " + a.LastName,
                    TotalBookCopies = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(a => a.TotalBookCopies)
                .ToList();

            return string.Join(Environment.NewLine, authors
                .Select(a => a.AuthorName + " - " + a.TotalBookCopies));
        }

        // 13.Profit by Category
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categories = context.Categories
                .Select(c => new
                {
                    c.Name,
                    TotalProfit = c.CategoryBooks.Sum(cb => cb.Book.Copies * cb.Book.Price)
                })
                .OrderByDescending(cb => cb.TotalProfit)
                .ThenBy(c => c.Name)
                .ToList();

            return string.Join(Environment.NewLine, categories
                .Select(c => $"{c.Name} ${c.TotalProfit:F2}"));
        }

        // 14.Most Recent Books
        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categories = context.Categories
                .Select(c => new
                {
                    CategoryName = c.Name,
                    Books = c.CategoryBooks
                        .Select(bc => new
                        {
                            BookTitle = bc.Book.Title,
                            BookReleaseDate = bc.Book.ReleaseDate
                        })
                        .OrderByDescending(cb => cb.BookReleaseDate)
                        .Take(3)
                        .ToList()
                })
                .OrderBy(c => c.CategoryName)
                .ToList();

            var result = new StringBuilder();
            foreach (var category in categories)
            {
                result.AppendLine($"--{category.CategoryName}");
                foreach (var book in category.Books)
                {
                    result.AppendLine($"{book.BookTitle} ({book.BookReleaseDate.Value.Year})");
                }
            }


            return result.ToString().TrimEnd();
        }

        // 15. Increase Prices
        public static void IncreasePrices(BookShopContext context)
        {
            List<Book> books = context.Books
                .Where(b => b.ReleaseDate.HasValue &&
                            b.ReleaseDate.Value.Year < 2010)
                .ToList(); // Materializing the query doesn't detach entities from ChangeTracker

            int addValueToPrice = 5;
            foreach (var book in books)
            {
                book.Price += addValueToPrice;
            }

            context.SaveChanges();
        }

        //16. Remove Books
        public static int RemoveBooks(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.Copies < 4200)
                .ToList();

            context.RemoveRange(books);
            context.SaveChanges();

            return books.Count();
        }
    }
}


