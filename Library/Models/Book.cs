using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace Library.Models
{
  public class Book
  {
    private int _id;
    private string _title;
    private DateTime _publishDate;
    private string _genre;

    public Book(string title, string genre, DateTime publishDate, int id = 0)
    {
      _title = title;
      _genre = genre;
      _publishDate = publishDate;
      _id = id;
    }

    public int GetId()
    {
      return _id;
    }
    public string GetTitle()
    {
      return _title;
    }
    public DateTime GetPublishDate()
    {
      return _publishDate;
    }
    public string GetGenre()
    {
      return _genre;
    }

    public override bool Equals(System.Object otherBook)
    {
      if(!(otherBook is Book))
      {
        return false;
      }
      else
      {
        Book newBook = (Book) otherBook;
        bool idEquality = this.GetId() == newBook.GetId();
        bool titleEquality = this.GetTitle() == newBook.GetTitle();
        return (idEquality && titleEquality);
      }
    }

    public override int GetHashCode()
    {
      return this.GetTitle().GetHashCode();
    }

    public static List<Book> GetAll()
    {
      List<Book> allBooks = new List<Book> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM books;";

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int id = rdr.GetInt32(0);
        string title = rdr.GetString(1);
        string genre = rdr.GetString(2);
        DateTime publishDate = rdr.GetDateTime(3);

        Book newBook = new Book(title, genre,publishDate, id);
        allBooks.Add(newBook);
      }
      conn.Close();
      if(conn != null)
      {
        conn.Dispose();
      }
      return allBooks;
    }

    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO books (title, genre, publish_date) VALUES (@title, @genre, @publishDate);";

      MySqlParameter bookTitle = new MySqlParameter();
      bookTitle.ParameterName = "@title";
      bookTitle.Value = _title;
      cmd.Parameters.Add(bookTitle);

      MySqlParameter bookGenre = new MySqlParameter();
      bookGenre.ParameterName = "@genre";
      bookGenre.Value = _genre;
      cmd.Parameters.Add(bookGenre);

      MySqlParameter publishDate = new MySqlParameter();
      publishDate.ParameterName = "@publishDate";
      publishDate.Value = _publishDate;
      cmd.Parameters.Add(publishDate);

      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId;
      conn.Close();
      if(conn != null)
      {
        conn.Dispose();
      }
    }

    public void Update(string title, string genre, DateTime publishDate)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"UPDATE books SET title = @title, genre = @genre, publish_date = @publishDate WHERE id = @thisId;";

      MySqlParameter thisBook = new MySqlParameter();
      thisBook.ParameterName = "@thisId";
      thisBook.Value = _id;
      cmd.Parameters.Add(thisBook);

      MySqlParameter bookTitle = new MySqlParameter();
      bookTitle.ParameterName = "@title";
      bookTitle.Value = title;
      cmd.Parameters.Add(bookTitle);

      MySqlParameter bookGenre = new MySqlParameter();
      bookGenre.ParameterName = "@genre";
      bookGenre.Value = genre;
      cmd.Parameters.Add(bookGenre);

      MySqlParameter bookPublishDate = new MySqlParameter();
      bookPublishDate.ParameterName = "@publishDate";
      bookPublishDate.Value = publishDate;
      cmd.Parameters.Add(bookPublishDate);

      _title = title;
      _genre = genre;
      _publishDate = publishDate;

      cmd.ExecuteNonQuery();
      conn.Close();
      if(conn != null)
      {
        conn.Dispose();
      }
    }

    public void AddAuthor(Author newAuthor)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO authors_books (book_id, author_id) VALUES (@bookId, @authorId);";

      MySqlParameter bookId = new MySqlParameter();
      bookId.ParameterName = "@bookId";
      bookId.Value = _id;
      cmd.Parameters.Add(bookId);

      MySqlParameter authorId = new MySqlParameter();
      authorId.ParameterName = "@authorId";
      authorId.Value = newAuthor.GetId();
      cmd.Parameters.Add(authorId);

      cmd.ExecuteNonQuery();
      conn.Close();
      if(conn != null)
      {
        conn.Dispose();
      }
    }

    public List<Author> GetAuthors()
    {
      List<Author> bookAuthors = new List<Author> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT authors.* FROM books

      JOIN authors_books ON (books.id = authors_books.book_id) JOIN authors ON (authors_books.author_id = authors.id)

      WHERE books.id = @thisId;";

      MySqlParameter bookId = new MySqlParameter();
      bookId.ParameterName = "@thisId";
      bookId.Value = _id;
      cmd.Parameters.Add(bookId);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int id = rdr.GetInt32(0);
        string name = rdr.GetString(1);
        Author newAuthor = new Author(name, id);
        bookAuthors.Add(newAuthor);
      }

      conn.Close();
      if(conn != null)
      {
        conn.Dispose();
      }
      return bookAuthors;
    }

    public static void DeleteAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM books; DELETE FROM authors_books;";

      cmd.ExecuteNonQuery();
      conn.Close();

      if(conn != null)
      {
        conn.Dispose();
      }
    }

    public static Book Find (int id)
    {

      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM books WHERE id = @thisId;";

      MySqlParameter idParameter = new MySqlParameter();
      idParameter.ParameterName = "@thisId";
      idParameter.Value = id;
      cmd.Parameters.Add(idParameter);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;

      int myId = 0;
      string title = "";
      string genre = "";
      DateTime publishDate = DateTime.Now;

      while(rdr.Read())
      {
        myId = rdr.GetInt32(0);
        title = rdr.GetString(1);
        genre = rdr.GetString(2);
        publishDate = rdr.GetDateTime(3);
      }
      Book newBook = new Book(title, genre, publishDate, myId);
      conn.Close();
      if(conn != null)
      {
        conn.Dispose();
      }
      return newBook;
    }

    public static List<Book> SearchByBookTitle(string bookTitle)
    {
      List<Book> matchingBooks = new List<Book>{};

      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM books WHERE title LIKE CONCAT('%' ,@bookTitle,'%');";

      MySqlParameter book = new MySqlParameter();
      book.ParameterName = "@bookTitle";
      book.Value = bookTitle;
      cmd.Parameters.Add(book);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int id = rdr.GetInt32(0);
        string title = rdr.GetString(1);
        string genre = rdr.GetString(2);
        DateTime publishDate = rdr.GetDateTime(3);

        Book newBook = new Book(title, genre, publishDate, id);
        matchingBooks.Add(newBook);
      }
      conn.Close();
      if(conn != null)
      {
        conn.Dispose();
      }
      return matchingBooks;
    }

    public void Delete()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM books WHERE id = @thisId; DELETE FROM authors_books WHERE book_id = @thisId;";

      MySqlParameter idParameter = new MySqlParameter();
      idParameter.ParameterName = "@thisId";
      idParameter.Value = _id;
      cmd.Parameters.Add(idParameter);

       cmd.ExecuteNonQuery();

      conn.Close();
      if(conn != null)
      {
        conn.Dispose();
      }
    }
  }
}
