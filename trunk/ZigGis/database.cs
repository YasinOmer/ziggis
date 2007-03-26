// Copyright (C) 2005 Pulp
// http://www.digital-pulp.com
// Abe Gillespie, abe@digital-pulp.com
//
// This program is free software; you can redistribute it
// and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation;
// either version 2 of the License, or (at your option) any
// later version.
//
// This program is distributed in the hope that it will be
// useful, but WITHOUT ANY WARRANTY; without even the implied
// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
// PURPOSE. See the GNU General Public License for more
// details.
//
// You should have received a copy of the GNU General Public
// License along with this program; if not, write to the Free
// Software Foundation, Inc., 59 Temple Place, Suite 330,
// Boston, MA 02111-1307 USA

using System;
using System.Data;
using Npgsql;
using System.Text;
using ZigGis.Utilities;

namespace ZigGis.PostGis
{
    public class Connection
    {
        static private readonly CLogger log = new CLogger(typeof(Connection));
        
        public Connection(string connectionString, bool test)
        {
            m_conStr = connectionString;

            // Setup the connection and test it.
            if (test)
            {
                IDbConnection con = openConnection();
                con.Dispose();
            }
        }

        private string m_conStr;
        private string connectionString { get { return m_conStr; } }

        private IDbConnection openConnection()
        {
            log.enterFunc("openConnection");
            
            IDbConnection con = new NpgsqlConnection(connectionString);
            con.Open();

            log.leaveFunc();
            
            return con;
        }

        public Layer getLayer(string schema, string view)
        {
            return new Layer(this, schema, view);
        }

        public Layer getLayer(int oid)
        {
            return new Layer(this, oid);
        }

        internal AutoDataReader doQuery(string sql)
        {
            log.enterFunc("doQuery");
            
            IDbConnection con;
            IDbCommand cmd;
            createCommand(sql, out con, out cmd);
            AutoDataReader retVal = null;
            using (cmd)
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("Executing reader ...");
                    log.Debug(cmd.CommandText);
                }
                IDataReader dr = cmd.ExecuteReader();
                retVal = new AutoDataReader(con, dr);
                if (log.IsDebugEnabled) log.Debug("Reader executed.");
            }

            log.leaveFunc();

            return retVal;
        }
    
        internal object doScalarQuery(string sql)
        {
            IDbConnection con;
            IDbCommand cmd;
            createCommand(sql, out con, out cmd);
            using (con)
            {
                using (cmd) {return cmd.ExecuteScalar();}
            }
        }

        private void createCommand(string sql, out IDbConnection connection, out IDbCommand command)
        {
            log.enterFunc("createCommand");
            
            connection = openConnection();
            command = connection.CreateCommand();
            command.CommandText = sql;

            log.leaveFunc();
        }
    }

    public class AutoDataReader : IDataReader
    {
        public AutoDataReader(IDbConnection connection, IDataReader dataReader)
        {
            m_con = connection;
            m_dr = dataReader;
        }

        ~AutoDataReader()
        {
            Dispose();
        }

        private IDbConnection m_con;
        private IDbConnection connection { get { return m_con; } }

        private IDataReader m_dr;
        private IDataReader dataReader { get { return m_dr; } }

        #region IDataReader
        public void Close()
        {
            dataReader.Close();
        }

        public int Depth
        {
            get { return dataReader.Depth; }
        }

        public DataTable GetSchemaTable()
        {
            return dataReader.GetSchemaTable();
        }

        public bool IsClosed
        {
            get { return dataReader.IsClosed; }
        }

        public bool NextResult()
        {
            return dataReader.NextResult();
        }

        public bool Read()
        {
            return dataReader.Read();
        }

        public int RecordsAffected
        {
            get { return dataReader.RecordsAffected; }
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            if (dataReader != null)
            {
                dataReader.Dispose();
                m_dr = null;
            }
            if (connection != null)
            {
                connection.Dispose();
                m_con = null;
            }
        }
        #endregion

        #region IDataRecord
        public int FieldCount
        {
            get { return dataReader.FieldCount; }
        }

        public bool GetBoolean(int i)
        {
            return dataReader.GetBoolean(i);
        }

        public byte GetByte(int i)
        {
            return dataReader.GetByte(i);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return dataReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        public char GetChar(int i)
        {
            return dataReader.GetChar(i);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return dataReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        public IDataReader GetData(int i)
        {
            return dataReader.GetData(i);
        }

        public string GetDataTypeName(int i)
        {
            return dataReader.GetDataTypeName(i);
        }

        public DateTime GetDateTime(int i)
        {
            return dataReader.GetDateTime(i);
        }

        public decimal GetDecimal(int i)
        {
            return dataReader.GetDecimal(i);
        }

        public double GetDouble(int i)
        {
            return dataReader.GetDouble(i);
        }

        public Type GetFieldType(int i)
        {
            return dataReader.GetFieldType(i);
        }

        public float GetFloat(int i)
        {
            return dataReader.GetFloat(i);
        }

        public Guid GetGuid(int i)
        {
            return dataReader.GetGuid(i);
        }

        public short GetInt16(int i)
        {
            return dataReader.GetInt16(i);
        }

        public int GetInt32(int i)
        {
            return dataReader.GetInt32(i);
        }

        public long GetInt64(int i)
        {
            return dataReader.GetInt64(i);
        }

        public string GetName(int i)
        {
            return dataReader.GetName(i);
        }

        public int GetOrdinal(string name)
        {
            return dataReader.GetOrdinal(name);
        }

        public string GetString(int i)
        {
            return dataReader.GetString(i);
        }

        public object GetValue(int i)
        {
            return dataReader.GetValue(i);
        }

        public int GetValues(object[] values)
        {
            return dataReader.GetValues(values);
        }

        public bool IsDBNull(int i)
        {
            return dataReader.IsDBNull(i);
        }

        public object this[string name]
        {
            get { return dataReader[name]; }
        }

        public object this[int i]
        {
            get { return dataReader[i]; }
        }
        #endregion
    }

    internal class DbHelper
    {
        static public string createSelectSql(string table, string fields)
        {
            return createSelectSql(table, fields, null, null, null);
        }
        
        static public string createSelectSql(string table, string fields, string where)
        {
            return createSelectSql(table, fields, null, where, null);
        }

		static public string createSelectSql(string table, string fields, string join, string where, string otherClause)
        {
            nullString(ref join);
            nullString(ref where);
            
            StringBuilder sb = new StringBuilder("select ");
            sb.Append(fields);
            sb.Append(" from ");
            sb.Append(table);
            if (join != null)
            {
                sb.Append(" ");
                sb.Append(join);
            }
            if (where != null)
            {
                sb.Append(" where ");
                sb.Append(where);
            }
			if (otherClause != null)
			{
				sb.Append(" " + otherClause);
			}
            return sb.ToString();
        }

        static public string quote(string unquoted)
        {
            StringBuilder sb = new StringBuilder("'");
            sb.Append(unquoted);
            sb.Append("'");
            return sb.ToString();
        }

        static public string getValueAsString(object value)
        {
            string retVal = "";
            if (value != DBNull.Value)
                retVal = (string)value;
            return retVal;
        }

        static public void nullString(ref string value)
        {
            if (value != null)
            {
                if (value.Trim() == "")
                    value = null;
            }
        }
    }
}
