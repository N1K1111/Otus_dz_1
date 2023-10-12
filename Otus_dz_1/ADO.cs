using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Otus_dz_1
{
    internal static class ADO
    {
        const string connectionString = "Host=localhost;Username=postgres;Password=123123;Database=Otus_dz1";

        #region CreateTables

        /// <summary>
        /// Создание Таблиц
        /// </summary>
        internal static void CreateTables()
        {
            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();
            var sql = @"
CREATE TABLE Clients
(   
    Id              SERIAL PRIMARY KEY,
    First_name      CHARACTER VARYING(255)    NOT NULL,
    Last_name       CHARACTER VARYING(255)    NOT NULL,
    Email           CHARACTER VARYING(255)    NOT NULL
);

CREATE TABLE Products
(
    Id      SERIAL PRIMARY KEY,
    Title           CHARACTER VARYING(100)    NOT NULL
);

CREATE TABLE Orders
(   
    Id              SERIAL PRIMARY KEY,
    IdClient        INTEGER                   NOT NULL,
    ProductsId      INTEGER                   NOT NULL, 
    FOREIGN KEY (IdClient) REFERENCES Clients (Id) ON DELETE CASCADE,
    FOREIGN KEY (ProductsId) REFERENCES Products (Id) ON DELETE CASCADE
);";

            using NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);

            cmd.ExecuteNonQuery();
        }
        #endregion

        #region AddDataToTables
        /// <summary>
        /// Добавление данных в таблицы
        /// </summary>
        internal static void AddDataToTables()
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            var sql = @"
INSERT INTO Clients(First_name, Last_name, Email)
VALUES 
('Петр','Иванов','ivanov@yandex.ru'),
('Александр','Смирнов','smirnov@yandex.ru'),
('Артем','Лебедев','lebedev@yandex.ru'),
('Никита','Кузнецов','kuznecov@yandex.ru'),
('Дмитрий','Соколов','sokolov@yandex.ru');

INSERT INTO Products(Title)
VALUES 
('Хлеб'),('Молоко'),('Кофе'),('Колбаса'),('Помидоры');

INSERT INTO Orders(IdClient, ProductsId)
VALUES
('1','3'),('4','1'),('2','2'),('3','4'),('5','5');
";

            using var cmd = new NpgsqlCommand(sql, connection);
            cmd.ExecuteNonQuery();

        }
        #endregion

        #region ReadData
        /// <summary>
        /// Чтение данных из таблиц
        /// </summary>
        internal static void ReadDataFromTables()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                string sql = @"
SELECT First_name, Last_name, Email FROM Clients
";
                using var cmd = new NpgsqlCommand(sql, connection);
                var reader = cmd.ExecuteReader();
                Console.WriteLine("Table ==> Clients <==");
                while (reader.Read())
                {
                    string firstName = reader.GetString(0);
                    string lastName = reader.GetString(1);
                    string email = reader.GetString(2);

                    Console.WriteLine($"FirstName: {firstName} LastName: {lastName} Email: {email}");
                }

            };


            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                string sql = @"
SELECT IdClient, ProductsId FROM Orders
";
                using var cmd = new NpgsqlCommand(sql, connection);
                var reader = cmd.ExecuteReader();
                Console.WriteLine("Table ==> Orders <==");
                while (reader.Read())
                {
                    int clientId = reader.GetInt32(0);
                    int productsId = reader.GetInt32(1);

                    Console.WriteLine($"ClientId: {clientId} ProductsId: {productsId}");
                }

            };

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                string sql = @"
SELECT Id, Title FROM Products
";
                using var cmd = new NpgsqlCommand(sql, connection);
                var reader = cmd.ExecuteReader();
                Console.WriteLine("Table ==> Products <==");
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string titles = reader.GetString(1);

                    Console.WriteLine($"Id: {id} Titles: {titles}");
                }
            };
        }
        #endregion

        #region WriteToConsole
        /// <summary>
        /// добавление данных в таблицу через консоль
        /// </summary>
        internal static void AddFromConsole()
        {
            
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                string sql = @"
SELECT c.relname AS Tables_in FROM pg_catalog.pg_class c
        LEFT JOIN pg_catalog.pg_namespace n ON n.oid = c.relnamespace
WHERE pg_catalog.pg_table_is_visible(c.oid)
        AND c.relkind = 'r'
        AND relname NOT LIKE 'pg_%';
";
                using var cmd = new NpgsqlCommand(sql, connection);
                var reader = cmd.ExecuteReader();
                Console.WriteLine("==>>Tables<<==");
                while (reader.Read())
                {
                    string table = reader.GetString(0);
                    Console.WriteLine($"Table: {table}");
                }
            }


            bool s = true;

            while (s)
            {
                Console.WriteLine("Введите название таблицы в которую хотите добавить данные или q для выхода: ");
                string useTable = Console.ReadLine().ToLower();

                switch (useTable)
                {
                    case "clients":
                        Clients();
                        break;
                    case "orders":
                        Orders();
                        break;
                    case "products":
                        Products();
                        break;
                    case "q":
                        s = false;
                        break;
                    default:
                        break;
                }
            }


                static void Clients()
                {
                    using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                    {
                        connection.Open();
                        string sql = @"
INSERT INTO clients (First_name, Last_name, Email)
VALUES (:first_name, :last_name, :email)";

                        using var cmd = new NpgsqlCommand(sql, connection);
                        var parameters = cmd.Parameters;
                        Console.WriteLine("Введите имя: ");
                        parameters.Add(new NpgsqlParameter("first_name", Console.ReadLine()));
                        Console.WriteLine("Введите фамилию: ");
                        parameters.Add(new NpgsqlParameter("last_name", Console.ReadLine()));
                        Console.WriteLine("Введите Email: ");
                        parameters.Add(new NpgsqlParameter("email", Console.ReadLine()));

                        cmd.ExecuteNonQuery();
                    }

                }


                static void Orders()
                {
                    using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                    {
                        connection.Open();
                        string sql = @"
INSERT INTO orders (IdClient, ProductsId)
VALUES (:IdClient, :ProductsId)";

                        using var cmd = new NpgsqlCommand(sql, connection);
                        var parameters = cmd.Parameters;
                        Console.Write("Введите ID клиента: ");
                        parameters.Add(new NpgsqlParameter("IdClient", int.Parse(Console.ReadLine())));
                        Console.Write("Введите ID Продукта: ");
                        parameters.Add(new NpgsqlParameter("ProductsId", int.Parse(Console.ReadLine())));

                        cmd.ExecuteNonQuery();
                    }
                }

                static void Products()
                {
                    using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                    {
                        connection.Open();
                        string sql = @"
INSERT INTO products (Id, Title)
VALUES (:Id, :Title)";

                        using var cmd = new NpgsqlCommand(sql, connection);
                        var parameters = cmd.Parameters;
                        Console.Write("Введите ID продукта: ");
                        parameters.Add(new NpgsqlParameter("Id", int.Parse(Console.ReadLine())));
                        Console.Write("Введите название продукта: ");
                        parameters.Add(new NpgsqlParameter("Title", Console.ReadLine()));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
    }
}
#endregion