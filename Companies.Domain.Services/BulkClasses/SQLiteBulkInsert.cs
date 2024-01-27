using Companies.Domain.Abstraction.Services;
using Microsoft.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Companies.Domain.Services.BulkClasses
{
    public class SQLiteBulkInsert : IDisposable
    {
        private readonly IDataBaseContext _dataBaseContext;
        private SqliteCommand m_cmd;
        private SqliteTransaction m_trans;
        private Dictionary<object, SqliteParameter> m_parameters = new Dictionary<object, SqliteParameter>();

        private uint m_counter = 0;

        private string m_beginInsertText;

        public SQLiteBulkInsert(string tableName, IDataBaseContext dataBaseContext)
        {
            _dataBaseContext = dataBaseContext;
            m_tableName = tableName;

            StringBuilder query = new StringBuilder(255);
            query.Append("INSERT INTO ["); query.Append(tableName); query.Append("] (");
            m_beginInsertText = query.ToString();
        }

        private bool m_allowBulkInsert = true;
        public bool AllowBulkInsert { get { return m_allowBulkInsert; } set { m_allowBulkInsert = value; } }

        public string CommandText
        {
            get
            {
                if (m_parameters.Count < 1)
                    throw new SQLiteException("You must add at least one parameter.");

                StringBuilder sb = new StringBuilder(255);
                sb.Append(m_beginInsertText);

                foreach (string param in m_parameters.Keys)
                {
                    sb.Append('[');
                    sb.Append(param);
                    sb.Append(']');
                    sb.Append(", ");
                }
                sb.Remove(sb.Length - 2, 2);

                sb.Append(") VALUES (");

                foreach (string param in m_parameters.Keys)
                {
                    sb.Append(m_paramDelim);
                    sb.Append(param);
                    sb.Append(", ");
                }
                sb.Remove(sb.Length - 2, 2);

                sb.Append(")");

                return sb.ToString();
            }
        }

        private uint m_commitMax = 10000;
        public uint CommitMax { get { return m_commitMax; } set { m_commitMax = value; } }

        private string m_tableName;
        public string TableName { get { return m_tableName; } }

        private string m_paramDelim = ":";
        public string ParamDelimiter { get { return m_paramDelim; } }

        public void AddParameter(string name, DbType dbType)
        {
            SqliteParameter param = new SqliteParameter(m_paramDelim + name, dbType);
            m_parameters.Add(name, param);
        }

        public async Task Flush()
        {
            try
            {
                if (m_trans != null)
                    await m_trans.CommitAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Could not commit transaction. See InnerException for more details", ex);
            }
            finally
            {
                if (m_trans != null)
                {
                    m_trans.Dispose();
                    m_trans = null;
                }

                m_counter = 0;
            }
        }

        public async Task Insert(object[] paramValues)
        {
            if (paramValues.Length != m_parameters.Count)
                throw new Exception("The values array count must be equal to the count of the number of parameters.");

            m_counter++;

            if (m_counter == 1)
            {
                if (m_allowBulkInsert)
                    m_trans = _dataBaseContext.GetConnection().BeginTransaction();

                m_cmd = _dataBaseContext.GetConnection().CreateCommand();
                foreach (SqliteParameter par in m_parameters.Values)
                    m_cmd.Parameters.Add(par);

                m_cmd.CommandText = CommandText;
            }

            int i = 0;
            foreach (SqliteParameter par in m_parameters.Values)
            {
                par.Value = paramValues[i];
                i++;
            }

            await m_cmd.ExecuteNonQueryAsync();

            if (m_counter == m_commitMax)
            {
                await Flush();
            }
        }

        public void Dispose()
        {
            m_cmd?.Dispose();
            m_trans?.Dispose();
        }
    }
}
