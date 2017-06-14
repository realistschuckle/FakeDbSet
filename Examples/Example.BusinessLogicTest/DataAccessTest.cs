using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using FakeDbSet;
using NUnit.Framework;
using Example.BusinessLogic;
using Example.Data;

namespace Example.BusinessLogicTest
{
    /// <summary>
    /// An example which shows how we can test EF code by creating our own fake
    /// database entities.
    /// </summary>
    [TestFixture]
    public class DataAccessTest
    {
        /// <summary>
        /// Tests the ListBooksCreatedBy method. 
        /// </summary>
        [Test]
        public void ListBooksCreatedByReturnsAppropriateResults()
        {
            // Arrange.
            // Create an instance of the DataAccess class, passing in a factory
            // which will create test data for the DataAccess class to use.
            var data = new DataAccess(new FakeBookStoreEntitiesFactory());

            // Act.
            // Attempt to call the method.
            List<Book> books = data.ListBooksCreatedBy("Last Name 2");

            // Assert.
            // Check that the expected behaviour is seen.
            Assert.That(books.Count, Is.EqualTo(1));
            Assert.That(books.First().Author.LastName, Is.EqualTo("Last Name 2"));
        }

        [Test]
        public void Setup_fake_dbset_returns_expected()
        {
            // Arrange
            var entities = new BookStoreEntities();
            var book = new Book { Title = "title" };

            // Act
            entities.SetupFakeDbSet(x => x.Books).Add(book);

            // Assert
            Assert.That(entities.Books.Count(), Is.EqualTo(1));
            Assert.That(entities.Books.First().Title, Is.EqualTo("title"));
        }

        [Test]
        public void Setup_fake_dbset_with_add_range_returns_expected()
        {
            // Arrange
            var entities = new BookStoreEntities();
            var books = new[]
            {
                new Book { Title = "title 1" },
                new Book { Title = "title 2" }
            };

            // Act
            var dbSet = entities.SetupFakeDbSet(x => x.Books);
            dbSet.Add(books[0]);
            dbSet.Add(books[1]);

            // Assert
            Assert.That(entities.Books.Count(), Is.EqualTo(2));
            Assert.That(entities.Books.First().Title, Is.EqualTo("title 1"));
            Assert.That(entities.Books.Last().Title, Is.EqualTo("title 2"));
        }

        [Test]
        public void Setup_fake_dbset_with_add_many_returns_expected()
        {
            // Arrange
            var entities = new BookStoreEntities();
            var book1 = new Book { Title = "title 1" };
            var book2 = new Book { Title = "title 2" };

            // Act
            var dbSet = entities.SetupFakeDbSet(x => x.Books);
            dbSet.Add(book1);
            dbSet.Add(book2);

            // Assert
            Assert.That(entities.Books.Count(), Is.EqualTo(2));
            Assert.That(entities.Books.First().Title, Is.EqualTo("title 1"));
            Assert.That(entities.Books.Last().Title, Is.EqualTo("title 2"));
        }

        [Test]
        public async Task Setup_fake_dbset_async_returns_expected()
        {
            // Arrange
            var entities = new BookStoreEntities();

            var books = new[]
            {
                new Book { Title = "title 1" },
                new Book { Title = "title 2" }
            };

            // Act
            var dbSet = entities.SetupFakeDbSet(x => x.Books);
            dbSet.Add(books[0]);
            dbSet.Add(books[1]);

            // Assert
            Assert.That(await entities.Books.CountAsync(), Is.EqualTo(2));
            Assert.That((await entities.Books.FirstAsync()).Title, Is.EqualTo("title 1"));
            Assert.That(await entities.Books.ToListAsync(), Is.EquivalentTo(books));
        }
    }
}
