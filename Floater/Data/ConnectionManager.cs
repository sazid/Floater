using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Windows;

/**
 * Author: Mohammed Sazid Al Rashid
 * GitHub: https://github.com/sazid/
 */
namespace sazid.github.io
{
    public class ConnectionManager
    {
        private string ConnectionString { get; }
        private SqlConnection _connection;
        private SqlCommand _sqlCommand;

        public ConnectionManager()
        {
            // the database file will always be served within the application folder
            ConnectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={Environment.CurrentDirectory}\floaterdb.mdf;Integrated Security=True;Connect Timeout=30";
        }

        public ConnectionManager(string connectionString)
        {
            ConnectionString = connectionString;
        }

        ~ConnectionManager()
        {
            if (_connection?.State == ConnectionState.Open)
                _connection?.Close();
        }

        public int NonQuery(string query)
        {
            using (_connection = new SqlConnection(ConnectionString))
            {
                if (_connection != null && _connection.State == ConnectionState.Closed)
                    _connection.Open();

                _sqlCommand = new SqlCommand(query, _connection);
                int rowsAffected = _sqlCommand.ExecuteNonQuery();

                _sqlCommand.Dispose();
                return rowsAffected;
            }
        }

        public DataTable Query(string query)
        {
            using (_connection = new SqlConnection(ConnectionString))
            {
                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();

                _sqlCommand = new SqlCommand(query, _connection);
                SqlDataAdapter da = new SqlDataAdapter(_sqlCommand);
                DataTable dt = new DataTable();
                da.Fill(dt);

                _sqlCommand.Dispose();
                return dt;
            }
        }

        public static string Escape(string q) => q.Replace("'", "").Replace("%", "").Replace("\"\"", "");

        /**
         * This method builds a query and "escapes" all single quotes so that SQL injection is not possible
         */
        /*
        public DataTable QueryTable(string tableName, string[] columnNames = null, string[] whereColumns = null, string[] whereValues = null)
        {
            if (whereColumns != null)
            {
                if (whereColumns.Length != whereValues.Length)
                    throw new Exception("sazid.github.io.ConnectionManager: Where columns and values must have the same length");
            }

            using (connection = new SqlConnection(connectionString))
            {
                string query = "SELECT ";
                // build the query

                if (columnNames == null)
                {
                    query += " * ";
                }
                else
                {
                    for (int col = 0; col < columnNames.Length; ++col)
                    {
                        query += columnNames[col].Replace("'", "''");

                        // add a comma after every column name except the last one
                        if (col != columnNames.Length - 1) query += ", ";
                        else query += " ";
                    }
                }

                query += " FROM " + tableName + " ";

                if (whereColumns != null)
                {
                    query += " WHERE ";

                    for (int i = 0; i < whereColumns.Length; ++i)
                    {
                        query += string.Format("{0}='{1}'", whereColumns[i].Replace("'", "''"), whereValues[i].Replace("'", "''"));

                        if (i!= whereColumns.Length - 1) query += ", ";
                        else query += " ";
                    }
                }

                return Query(query);
            }
        }
        */
    }
}
