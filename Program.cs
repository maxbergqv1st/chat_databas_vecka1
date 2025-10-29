using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using Microsoft.Data.Sqlite;
class Program
{
      static void Main()
      {
            Program app = new Program();
            app.Run();
      }
      void Run()
      {
            string connectionString = "Data Source = mydatabase.db";

            using var connection = new SqliteConnection(connectionString);
            connection.Open();
                  Console.Clear();
                  Console.WriteLine("Välj ett alternativ:");
                  Console.WriteLine("1. Lägg till användare");
                  Console.WriteLine("2. Visa användare");
                  Console.WriteLine("L. Logged in");
                  Console.WriteLine("Q. Quit");
                  Console.Write("Val: ");

            bool running = false;

                  string? choice = Console.ReadLine();
                  switch (Console.ReadLine())
                  {
                        case "1":
                              {
                                    addUser(connection);
                              }
                              break;
                        case "2":
                              {
                                    ListUsers(connection);
                              }
                              break;
                        case "L":
                        case "l":
                        {
                              Console.Clear();
                              Console.WriteLine("Sign in");
                              string? username = Console.ReadLine();
                              running = true;
                        }
                        break;
                        case "Q":
                        case "q":
                              {
                                    connection.Close();
                              }
                              break;
                  }
                  if(running = true)
                  {
                  Console.Clear();
                  Console.WriteLine("Welcome!!");
                  Console.ReadLine();

                  }
                        
            connection.Close();
      }

      void addUser(SqliteConnection connection)
      {
            Console.WriteLine("Förnamn: ");
            string? firstname = Console.ReadLine();
            Console.WriteLine("Efternamn: ");
            string? lastname = Console.ReadLine();
            Console.WriteLine("Email: ");
            string? email = Console.ReadLine();
            Console.WriteLine("Användarnamn: ");
            string? username = Console.ReadLine();
            Console.WriteLine("Lösenord: ");
            string? password = Console.ReadLine();
            Console.WriteLine("Säkerhetsord: ");
            string? safety = Console.ReadLine();

            var cmd = connection.CreateCommand();

            cmd.CommandText = @";
                  INSERT INTO User(FirstName, LastName, Email, Username, Password, Safety)
                  VALUES($fn, $1n, $em, $un, $pw, $sf)";
            cmd.Parameters.AddWithValue("$fn", firstname);
            cmd.Parameters.AddWithValue("$1n", lastname);
            cmd.Parameters.AddWithValue("$em", email);
            cmd.Parameters.AddWithValue("$un", username);
            cmd.Parameters.AddWithValue("$pw", password);
            cmd.Parameters.AddWithValue("$sf", safety);

            try
            {
                  cmd.ExecuteNonQuery();
                  Console.WriteLine("Användare tillagd");
            }
            catch (SqliteException ex)
            {
                  Console.WriteLine("Fel vid skapandet av accountet");
            }
      }
      void ListUsers(SqliteConnection connection)
      {
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT UserId, FirstName, LastName, Email, Username FROM User";

            using var reader = cmd.ExecuteReader();
            Console.WriteLine("\n Användare i databasen:");
            while(reader.Read())
            {
                  Console.WriteLine($"ID: {reader.GetInt32(0)} | {reader.GetString(1)} {reader.GetString(2)} | {reader.GetString(4)} ({reader.GetString(3)})");
            }
      }
}













      // using System;

// using Microsoft.Data.Sqlite;

// class Program
// {
// static void Main()
// {
//       string connectionString = "Data Source=mydatabase.db";

//       using var connection = new SqliteConnection(connectionString);
//       connection.Open();

//       Console.WriteLine("Välj ett alternativ:");
//       Console.WriteLine("1. Lägg till användare");
//       Console.WriteLine("2. Visa användare");
//       Console.Write("Val: ");
//       string? choice = Console.ReadLine();

//       if (choice == "1")
//       {
//             AddUser(connection);
//       }
//       else if (choice == "2")
//       {
//             ListUsers(connection);
//       }
//       else
//       {
//             Console.WriteLine("Ogiltigt val.");
//       }

//       connection.Close();
// }

// static void AddUser(SqliteConnection connection)
// {
//       Console.Write("Förnamn: ");
//       string firstname = Console.ReadLine() ?? "";

//       Console.Write("Efternamn: ");
//       string lastname = Console.ReadLine() ?? "";

//       Console.Write("E-post: ");
//       string email = Console.ReadLine() ?? "";

//       Console.Write("Användarnamn: ");
//       string username = Console.ReadLine() ?? "";

//       Console.Write("Lösenord: ");
//       string password = Console.ReadLine() ?? "";

//       Console.Write("Säkerhetsord: ");
//       string safety = Console.ReadLine() ?? "";

//       var cmd = connection.CreateCommand();
//       cmd.CommandText = @"
//             INSERT INTO User (FirstName, LastName, Email, Username, Password, Safety)
//             VALUES ($fn, $ln, $em, $un, $pw, $sf)";
//       cmd.Parameters.AddWithValue("$fn", firstname);
//       cmd.Parameters.AddWithValue("$ln", lastname);
//       cmd.Parameters.AddWithValue("$em", email);
//       cmd.Parameters.AddWithValue("$un", username);
//       cmd.Parameters.AddWithValue("$pw", password);
//       cmd.Parameters.AddWithValue("$sf", safety);

//       try
//       {
//             cmd.ExecuteNonQuery();
//             Console.WriteLine("✅ Användare tillagd!");
//       }
//       catch (SqliteException ex)
//       {
//             Console.WriteLine($"❌ Fel vid insättning: {ex.Message}");
//       }
// }

// static void ListUsers(SqliteConnection connection)
// {
//       var cmd = connection.CreateCommand();
//       cmd.CommandText = "SELECT UserId, FirstName, LastName, Email, Username FROM User";

//       using var reader = cmd.ExecuteReader();
//       Console.WriteLine("\n📋 Användare i databasen:");
//       while (reader.Read())
//       {
//             Console.WriteLine($"ID: {reader.GetInt32(0)} | {reader.GetString(1)} {reader.GetString(2)} | {reader.GetString(4)} ({reader.GetString(3)})");
//       }
// }
// }
