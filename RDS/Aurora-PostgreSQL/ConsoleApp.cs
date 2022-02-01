using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aurora_PostgreSQL
{
    public class ConsoleApp
    {
        Database Database;
        private const string tableName = "Users";
        public ConsoleApp(Database database)
        {
            Database = database;
        }

        public async Task Run()
        {
            try
            {
                GetPostgreSqlVersion();
                DeleteTable();
                CreateTable();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
            await Task.CompletedTask;
        }
        private void GetPostgreSqlVersion()
        {
            string sqlQuery = "SELECT version()";
            var response = Database.ExecuteScalar(sqlQuery).ToString();
            Console.WriteLine($"PostgreSQL version: {response}");
        }

        private void DeleteTable()
        {
            string sqlQuery = $"DROP TABLE IF exists {tableName}";        
            Database.ExecuteNonQuery(sqlQuery);
            Console.WriteLine($"Table- {tableName} deleted");
        }

        private void CreateTable()
        {
            string sqlQuery = $"CREATE TABLE IF NOT EXISTS {tableName}(id INT GENERATED ALWAYS AS IDENTITY,name character varying(200) NOT NULL,age numeric NOT NULL)";
            Database.ExecuteNonQuery(sqlQuery);
            Console.WriteLine($"Table- {tableName} created");
        }
    }
}