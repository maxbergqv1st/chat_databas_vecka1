using System;
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
            string connectionString = "Data Source=mydatabase.db";
            using var connection = new SqliteConnection(connectionString);

            connection.Open();

            bool running = true;
            bool loggedIn = false;

            while (running)
            {
                  if (!loggedIn)
                  {
                        Console.Clear();
                        Console.WriteLine("1. Logga in");
                        Console.WriteLine("2. Lägg till användare");
                        Console.WriteLine("\nQ. Quit\n");
                        Console.Write("Val: ");
                        string? choice = Console.ReadLine();
                        switch (choice)
                        {
                              case "1":
                                    {
                                          loggedIn = Login(connection);
                                    }
                                    break;
                              case "2":
                                    {
                                          AddUser(connection);
                                    }
                                    break;
                              case "3":
                                    {
                                    }
                                    break;
                              case "Q":
                              case "q":
                                    {
                                          running = false;
                                    }
                                    break;
                        }
                  }
                  else
                  {
                        Console.Clear();
                        Console.WriteLine("Welcome!!");
                        Console.WriteLine("1] Profil ");
                        Console.WriteLine("2] Browse other users ");
                        Console.WriteLine("3] Chatta med vänner");
                        Console.WriteLine("L] Logga ut ");
                        switch (Console.ReadLine())
                        {
                              case "1":
                                    {
                                          ProfileLoggedIn(connection);
                                    }
                                    break;
                              case "2":
                                    {
                                          BrowseFriends(connection);
                                          Console.ReadLine();
                                    }
                                    break;
                              case "3":
                                    {
                                          ChatWithFriends(connection);
                                          Console.ReadLine();
                                    }
                                    break;
                              case "L":
                              case "l":
                                    {
                                          loggedIn = false;
                                    }
                                    break;
                        }
                  }
            }
            connection.Close();
      }
      string? loggedInUsername = null;
      bool Login(SqliteConnection connection)
      {
            Console.Clear();
            Console.WriteLine("Logga in");
            Console.WriteLine("Användarnamn");
            string? username = Console.ReadLine();
            Console.WriteLine("Lösenord: ");
            string? password = Console.ReadLine();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
            SELECT * FROM User
            WHERE Username = $un AND Password = $pw";
            cmd.Parameters.AddWithValue("$un", username);
            cmd.Parameters.AddWithValue("$pw", password);



            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                  Console.WriteLine("Inlogging lyckades!");
                  loggedInUsername = username;
                  return true;
            }
            else
            {
                  Console.WriteLine("Användare hittades inte...");
                  return false;
            }
      }
      void AddUser(SqliteConnection connection)
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
                  VALUES($fn, $ln, $em, $un, $pw, $sf)";
            cmd.Parameters.AddWithValue("$fn", firstname);
            cmd.Parameters.AddWithValue("$ln", lastname);
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
      void ProfileLoggedIn(SqliteConnection connection)
      {
            if (loggedInUsername == null)
            {
                  Console.WriteLine("Ingen användare är inloggad.");
                  return;
            }

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
            SELECT UserId, FirstName, LastName, Email, Username 
            FROM User
            WHERE Username = $un";
            cmd.Parameters.AddWithValue("$un", loggedInUsername);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                  Console.Clear();
                  Console.WriteLine("Din profil: \n");
                  Console.WriteLine($"ID: {reader.GetInt32(0)}");
                  Console.WriteLine($"Förnamn: {reader.GetString(1)}");
                  Console.WriteLine($"Efternamn: {reader.GetString(2)}");
                  Console.WriteLine($"Email: {reader.GetString(3)}");
                  Console.WriteLine($"Användarnamn: {reader.GetString(4)}");
                  Console.ReadLine();
            }
      }
      void BrowseFriends(SqliteConnection connection)
      {
            if (loggedInUsername == null)
            {
                  Console.WriteLine("Du måste vara inloggad för att se andra användare.");
                  return;
            }

            // Hämta inloggad användares ID
            var getLoggedInId = connection.CreateCommand();
            getLoggedInId.CommandText = "SELECT UserId FROM User WHERE Username = $un";
            getLoggedInId.Parameters.AddWithValue("$un", loggedInUsername);
            int loggedInId = Convert.ToInt32(getLoggedInId.ExecuteScalar()); // kör frågan och retunerar endast ett värde av UserId och sparar den i LoggedInId

            // Hämta alla användare utom den inloggade
            var users = new List<(int Id, string Name, string Username)>();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT UserId, FirstName, LastName, Username FROM User WHERE UserId != $id";
            cmd.Parameters.AddWithValue("$id", loggedInId);

            using var reader = cmd.ExecuteReader();
            Console.Clear();
            Console.WriteLine("===== Bläddra bland andra användare =====\n");
            while (reader.Read())
            {
                  users.Add((
                        reader.GetInt32(0),
                        $"{reader.GetString(1)} {reader.GetString(2)}",
                        reader.GetString(3)
                  ));
            }

            if (users.Count == 0)
            {
                  Console.WriteLine("Det finns inga andra användare just nu.");
                  return;
            }

            // Skriv ut alla användare
            foreach (var user in users)
            {
                  Console.WriteLine($"{user.Id}: {user.Name} ({user.Username})");
            }

            Console.WriteLine("\nSkriv ID på den du vill lägga till som vän, eller tryck Enter för att gå tillbaka:");
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int friendId))
            {
                  AddFriend(connection, loggedInId, friendId);
            }
      }
      void AddFriend(SqliteConnection connection, int userId1, int userId2)
      {
            // Kontrollera om vänskap redan finns
            var checkCmd = connection.CreateCommand();
            checkCmd.CommandText = @"
            SELECT COUNT(*) FROM Friends
            WHERE (UserId1 = $a AND UserId2 = $b)
            OR (UserId1 = $b AND UserId2 = $a)";
            checkCmd.Parameters.AddWithValue("$a", userId1);
            checkCmd.Parameters.AddWithValue("$b", userId2);

            long exists = (long)checkCmd.ExecuteScalar();
            if (exists > 0)
            {
                  Console.WriteLine("Ni är redan vänner!");
                  return;
            }

            // Lägg till vänskapen
            var insertCmd = connection.CreateCommand();
            insertCmd.CommandText = @"
            INSERT INTO Friends (UserId1, UserId2)
            VALUES ($a, $b)";
            insertCmd.Parameters.AddWithValue("$a", userId1);
            insertCmd.Parameters.AddWithValue("$b", userId2);
            insertCmd.ExecuteNonQuery();

            Console.WriteLine("Vän tillagd!");
      }
      void ChatWithFriends(SqliteConnection connection)
      {
            if (loggedInUsername == null)
            {
                  Console.WriteLine("Du måste vara inloggad för att chatta");
                  return;
            }
            // Hämta inloggad användares ID
            var getLoggedInId = connection.CreateCommand();
            getLoggedInId.CommandText = "SELECT UserId FROM User WHERE Username = $un";
            getLoggedInId.Parameters.AddWithValue("$un", loggedInUsername);
            int loggedInId = Convert.ToInt32(getLoggedInId.ExecuteScalar()); // kör frågan och retunerar endast ett värde av UserId och sparar den i LoggedInId

            // Hämta alla användare utom den inloggade
            var friendsCmd = connection.CreateCommand();
            friendsCmd.CommandText = @"
                  SELECT U.UserId, U.Username, U.FirstName, U.LastName
                  FROM Friends F
                  JOIN User U ON (U.UserId = F.UserId1 OR U.UserId = F.UserId2)
                  WHERE($id = F.UserId1 OR $id = F.UserId2)
                  AND U.UserId != $id";
            friendsCmd.Parameters.AddWithValue("$id", loggedInId);

            using var reader = friendsCmd.ExecuteReader();
            var friends = new List<(int Id, string Username, string Name)>();
            Console.Clear();
            Console.WriteLine("===== Dina vänner =====");
            while (reader.Read())
            {
                  friends.Add((reader.GetInt32(0), reader.GetString(1), $"{reader.GetString(2)} {reader.GetString(3)}"));
            }
            if (friends.Count == 0)
            {
                  Console.WriteLine("You have no friends");
            }

            foreach (var f in friends)
            {
                  Console.WriteLine($"{f.Id}: {f.Name} {f.Username}");
            }
            Console.WriteLine("\n Välj ett id för att chatta: ");
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int friendId))
            {
                  StartChat(connection, loggedInId, friendId);
            }
      }

      void StartChat(SqliteConnection connection, int senderId, int receiverId)
      {
            Console.Clear();
            Console.WriteLine("=== Chat ===");
            Console.WriteLine("Skriv 'exit' för att avsluta chatten.\n");

            // Visa tidigare meddelanden
            var showCmd = connection.CreateCommand();
            showCmd.CommandText = @"
                  SELECT 
                        CASE WHEN SenderId = $me THEN 'Du' ELSE U.Username END AS Sender,
                        MessageText, Timestamp
                  FROM Messages
                  JOIN User U ON U.UserId = Messages.SenderId
                  WHERE (SenderId = $me AND ReceiverId = $them)
                  OR (SenderId = $them AND ReceiverId = $me)
                  ORDER BY Timestamp";
            showCmd.Parameters.AddWithValue("$me", senderId);
            showCmd.Parameters.AddWithValue("$them", receiverId);

            using (var reader = showCmd.ExecuteReader())
            {
                  while (reader.Read())
                  {
                        Console.WriteLine($"[{reader.GetString(0)}]: {reader.GetString(1)}");
                  }
            }

            // Skicka nya meddelanden
            while (true)
            {
                  Console.Write("\nDu: ");
                  string? message = Console.ReadLine();
                  if (message == null || message.ToLower() == "exit") break;

                  var insertCmd = connection.CreateCommand();
                  insertCmd.CommandText = @"
                        INSERT INTO Messages (SenderId, ReceiverId, MessageText)
                        VALUES ($s, $r, $msg)";
                  insertCmd.Parameters.AddWithValue("$s", senderId);
                  insertCmd.Parameters.AddWithValue("$r", receiverId);
                  insertCmd.Parameters.AddWithValue("$msg", message);
                  insertCmd.ExecuteNonQuery();

                  Console.WriteLine("Meddelande skickat!");
            }
      }
}

