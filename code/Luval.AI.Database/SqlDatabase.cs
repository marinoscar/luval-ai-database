using System.Data;
using System.Data.Common;

namespace Luval.AI.Database
{
    public class SqlDatabase
    {
        private Func<IDbConnection> _connectionFactory;

        public SqlDatabase(Func<IDbConnection> connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }


        public string ExecuteToCsv(string sqlCommand)
        {
            var sw = new StringWriter();
            var isFirst = true;
            var values = new object[10];
            UsingReader(sqlCommand, (r) =>
            {
                if (isFirst)
                {
                    var names = new List<string>();
                    for (int i = 0; i < r.FieldCount; i++)
                    {
                        names.Add(r.GetName(i));
                    }
                    values = new object[r.FieldCount];
                    sw.WriteLine(string.Join(",", names));
                    isFirst = false;
                }
                r.GetValues(values);
                sw.WriteLine(string.Join(",", values));
            });
            return sw.ToString();
        }

        public string ExecuteToKeyPair(string sqlCommand)
        {
            var sw = new StringWriter();
            var isFirst = true;
            var values = new object[10];
            sw.Write("[");
            UsingReader(sqlCommand, (r) =>
            {
                if (isFirst)
                {
                    sw.Write("(");
                    values = new object[r.FieldCount];
                    isFirst = false;
                }
                else sw.Write(",(");

                r.GetValues(values);
                sw.Write(string.Join(",", values));
                sw.Write(")");
            });
            sw.Write("]");
            return sw.ToString();
        }

        public IEnumerable<IDictionary<string, object>> ExecuteToList(string sqlCommand)
        {
            var result = new List<IDictionary<string, object>>();
            UsingReader(sqlCommand, (r) =>
            {
                var d = new Dictionary<string, object>();
                for (int i = 0; i < r.FieldCount; i++)
                {
                    d[r.GetName(i)] = r.GetValue(i);
                }
                result.Add(d);
            });
            return result;
        }

        public DataTable ExecuteDataTable(string sqlCommand)
        {
            DataTable dt = null;
            UsingReader(sqlCommand, (r) =>
            {
                if (dt == null) dt = CreateTableSchema(r);
                dt.Rows.Add(CreateFromRecord(r, dt));
            });
            return dt;
        }

        public IDataReader ExecuteToDataReader(string sqlCommand)
        {
            IDataReader dataReader = null;
            UsingCommand(sqlCommand, (cmd) =>
            {
                dataReader = cmd.ExecuteReader();
            });
            return dataReader;
        }


        private void UsingCommand(string sqlCommand, Action<IDbCommand> runCommand)
        {
            try
            {
                using (var conn = _connectionFactory())
                {
                    OpenConnection(conn);
                    using (var cmd = conn.CreateCommand())
                    {
                        using (var tran = conn.BeginTransaction())
                        {
                            try
                            {
                                PrepareCommand(conn, cmd, tran, sqlCommand);
                                runCommand(cmd);
                                tran.Commit();
                            }
                            catch (Exception ex)
                            {
                                tran.Rollback();
                                throw new DataException("Failed to run sql command", new Exception(string.Format("Command Failure: {0}", cmd.CommandText), ex));
                            }
                        }
                    }
                    CloseConnection(conn);
                }
            }
            catch (Exception ex)
            {
                throw new DataException("Command failed", ex);
            }
        }

        private void UsingReader(string sqlCommand, Action<IDataRecord> runCommand)
        {
            UsingCommand(sqlCommand, (cmd) =>
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        runCommand(reader);
                    }
                }
            });
        }

        private DataTable CreateTableSchema(IDataRecord record)
        {
            var dt = new DataTable();
            for (int i = 0; i < record.FieldCount; i++)
            {
                dt.Columns.Add(record.GetName(i), record.GetFieldType(i));
            }
            return dt;
        }

        private DataRow CreateFromRecord(IDataRecord record, DataTable dt)
        {
            var row = dt.NewRow();
            for (int i = 0; i < record.FieldCount; i++)
                row[record.GetName(i)] = record.GetValue(i);
            return row;
        }

        private void OpenConnection(IDbConnection conn)
        {
            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
            }
            catch (Exception ex)
            {
                throw new DataException("Unable to open the connection", ex);
            }
        }

        private void CloseConnection(IDbConnection conn)
        {
            try
            {
                if (conn != null && conn.State != ConnectionState.Closed) conn.Close();
            }
            catch (Exception ex)
            {

                throw new DataException("Unable to close the connection", ex);
            }
        }

        private void PrepareCommand(IDbConnection conn, IDbCommand cmd, IDbTransaction tran, string sqlCommand, CommandType commandType = CommandType.Text)
        {
            cmd.Transaction = tran;
            cmd.CommandText = sqlCommand;
            cmd.CommandTimeout = conn.ConnectionTimeout;
            cmd.CommandType = commandType;
        }
    }
}