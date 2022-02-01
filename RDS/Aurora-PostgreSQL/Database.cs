using System;
using System.Linq;
using System.Collections.Generic;
using Npgsql;

namespace Aurora_PostgreSQL
{
    public class Database
    {
        private readonly string connectionString="";

        public Database()
        {

        }
        public NpgsqlConnection CreateConnection()
        {
            var conn = new NpgsqlConnection();
            conn.ConnectionString = connectionString;
            conn.Open();
            return conn;
        }

        public T ExecuteScalar<T>(T commandText)
        {
            using (var connection = CreateConnection())
            {
                return ExecuteScalar(connection, commandText);
            }
        }

        private T ExecuteScalar<T>(NpgsqlConnection connection, T defaultValue)
        {
            using (var command = new NpgsqlCommand(defaultValue.ToString(), connection))
            using (var ds = command.ExecuteReader())
            {
                if (ds.Read())
                {
                    if (ds.IsDBNull(0))
                        return defaultValue;

                    return (T)ds.GetValue(0);
                }
            }
            return defaultValue;
        }

        public List<object[]> ExecuteQuery(string commandText)
        {
            using (var connection = CreateConnection())
            {
                return ExecuteQuery(connection, commandText);
            }
        }

        private List<object[]> ExecuteQuery(NpgsqlConnection connection, string commandText)
        {
            var result = new List<object[]>();
            using (var command = new NpgsqlCommand(commandText, connection))
            using (var ds = command.ExecuteReader())
            {
                while (ds.Read())
                {
                    var count = ds.FieldCount;
                    var values = new object[count];
                    for (int i = 0; i < count; i++)
                        if (!ds.IsDBNull(i))
                            values[i] = ds.GetValue(i);
                    result.Add(values);
                }
            }
            return result;
        }

        public int ExecuteNonQuery(string commandText, Dictionary<string, object> parameters = null)
        {
            using (var connection = CreateConnection())
            using (var command = new NpgsqlCommand(commandText, connection))
            {
                var newCommand = AddParameters(command, parameters);
                var response = newCommand.ExecuteNonQuery();
                return response;
            }
        }

        private static NpgsqlCommand AddParameters(NpgsqlCommand command, Dictionary<string, object> parameters)
        {
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = param.Key;
                    parameter.Value = param.Value ?? DBNull.Value;
                    command.Parameters.Add(parameter);
                }
            }
            return command;
        }
        
    }
}