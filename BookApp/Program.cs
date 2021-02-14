using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
// Run the Package Manager Console
// Install EntityFrameworkCore.SqlServer Package
// Install EntityFrameworkCore.Tools Package
// Install EntityFrameworkCore.Design Package
// Add-Migration InitialCreate => Migration is created
// Update-Database => Database is created. When we control our SQL server database we will see tables named Product and Category

// What if I want to add a column to one of the tables in the database?
// Add-Migration NewColumn => Migration is created, I will see the new migration in the MIGRATIONS folder
// Then again write the command Update-Database and we will see the new column added in our MSSQL database
//And again I added another Migration - another class => NewTable

// What if I do not need the Order Table anymore => I can go back to the previous migration called NewColumn=> HOW? => Update-Database NewColumn=> Then the order table will be crashed
// We deleted it from the Database but what about the VS project? Not deleted...But we can...=> HOW?=> Remove-Migration=> This command deletes the migration which added as the last one.
//We can also delete all the tables etc from the database using the command => Update-Database 0
// And lastly, we can delete the whole database(in this project ShopAppDB) using the command=> Drop-Database


namespace BookApp
{
    public class BookContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<PriceOffer> PriceOffers { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<BookAuthor> BookAuthors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer("Server=(local);Trusted_Connection=True;Database=BookAppDB");
            var options = optionsBuilder.Options;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookAuthor>().HasKey(table => new { table.AuthorId, table.BookId });
          

        }
    }

    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublishedOn { get; set; }
        public string Publisher { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        //-----------------------------------------------
        //relationships
        public PriceOffer PriceOffers { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<BookAuthor> BookAuthors { get; set; }
    }

    public class Author
    {
        public int AuthorId { get; set; }
        public string Name { get; set; }
        public string WebUrl { get; set; }
        public ICollection<BookAuthor> BookAuthors { get; set; }
    }

    public class BookAuthor
    {
        public int BookId { get; set; }
        public int AuthorId { get; set; }
        public byte Order { get; set; }
        //-----------------------------
        //Relationships
        public Book Book { get; set; }
        public Author Author { get; set; }
    }

    public class PriceOffer
    {
        public int PriceOfferId { get; set; }
        public decimal NewPrice { get; set; }
        public string PromotionalText { get; set; }
        //-----------------------------------------------
        //Relationships
        public int BookId { get; set; }
    }

    public class Review
    {
        public int ReviewId { get; set; }
        public string VoterName { get; set; }
        public int NumStars { get; set; }
        public string Comment { get; set; }
        //-----------------------------------------
        //Relationships
        public int BookId { get; set; }
    }



    class Program
    {
        static void Main(string[] args)
        {

            using (var context = new BookContext())
            {
                var query1 = context.Books.Include(x => x.BookAuthors).ThenInclude(y => y.Author).
                    Select(b => new
                                {
                                    Title = b.Title,
                                    AuthorName = b.BookAuthors.Select(x => x.Author.Name)
                                }
                          ).Where(n => n.AuthorName.Contains("Jared Diamond")).ToList();

                foreach (var item in query1)
                {
                    Console.WriteLine($"Book Name : {item.Title}");
                }

            }



            Console.WriteLine("Added Successfully");
           
        }
    }
}

