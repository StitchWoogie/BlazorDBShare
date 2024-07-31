using System.Data;
using System.Data.OleDb;

public class DatabaseService
{
    private readonly string _connectionString;

    public DatabaseService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DataTable ExecuteQuery(string query)
    {
        using (var connection = new OleDbConnection(_connectionString))
        {
            connection.Open();
            using (var command = new OleDbCommand(query, connection))
            {
                using (var adapter = new OleDbDataAdapter(command))
                {
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
        }
    }

    public List<string> GetTableNames()
    {
        var tables = new List<string>();
        using (var connection = new OleDbConnection(_connectionString))
        {
            connection.Open();
            var schema = connection.GetSchema("Tables");
            foreach (DataRow row in schema.Rows)
            {
                string tableName = row["TABLE_NAME"].ToString();
                if (!tableName.StartsWith("MSys"))
                {
                    tables.Add(tableName);
                }
            }
        }
        return tables;
    }

    public int ExecuteNonQuery(string query, Dictionary<string, object> parameters = null)
    {
        using (var connection = new OleDbConnection(_connectionString))
        {
            connection.Open();
            using (var command = new OleDbCommand(query, connection))
            {
                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        command.Parameters.AddWithValue(param.Key, param.Value);
                    }
                }
                return command.ExecuteNonQuery();
            }
        }
    }
}