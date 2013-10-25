﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;

namespace Justin.FrameWork
{
    public class OracleBulkCopyWrapper : IBulkCopyWrapper
    {
        public OracleBulkCopyWrapper(DbConnection conn)
        {
            OracleConnection sqlConn = conn as OracleConnection;
            bulkCopy = new OracleBulkCopy(sqlConn);
            this.ColumnMappings = new Dictionary<string, string>();
        }
        private OracleBulkCopy bulkCopy { get; set; }
        public int BatchSize
        {
            get
            {
                return bulkCopy.BatchSize;
            }
            set
            {
                bulkCopy.BatchSize = value;
            }
        }

        public int BulkCopyTimeout
        {
            get
            {
                return bulkCopy.BulkCopyTimeout;
            }
            set
            {
                bulkCopy.BulkCopyTimeout = value;
            }
        }


        public Dictionary<string, string> ColumnMappings { get; set; }

        public string DestinationTableName
        {
            get
            {
                return bulkCopy.DestinationTableName;
            }
            set
            {
                bulkCopy.DestinationTableName = value;
            }
        }

        public void Close() { this.bulkCopy.Close(); }

        public void FillColumnMappings()
        {
            this.bulkCopy.ColumnMappings.Clear();

            foreach (var item in this.ColumnMappings)
            {
                this.bulkCopy.ColumnMappings.Add(item.Value, item.Key);
            }
        }

        public void WriteToServer(DataRow[] rows)
        {
            this.FillColumnMappings();
            bulkCopy.WriteToServer(rows);
        }

        public void WriteToServer(DataTable table)
        {
            this.FillColumnMappings();
            bulkCopy.WriteToServer(table);
        }

        public void WriteToServer(IDataReader reader)
        {
            this.FillColumnMappings();
            bulkCopy.WriteToServer(reader);
        }

        public void WriteToServer(DataTable table, DataRowState rowState)
        {
            this.FillColumnMappings();
            bulkCopy.WriteToServer(table, rowState);
        }

        public void Dispose()
        {
            this.bulkCopy.Dispose();
        }
    }

    public class BulkCopy
    {
        private IBulkCopyWrapper bulkCopyWrapper;
        public BulkCopy(string connStr, BulkCopySupportDB DbType)
        {
            switch (DbType)
            {
                case BulkCopySupportDB.MSSQL: bulkCopyWrapper = new SqlBulkCopyWrapper(new SqlConnection(connStr)); break;
                case BulkCopySupportDB.Oracle: bulkCopyWrapper = new OracleBulkCopyWrapper(new OracleConnection(connStr)); break;
                default: throw new Exception("Not Support DbType");
            }
        }

        public BulkCopy(OleDbConnection conn)
        {
            OleDbConnectionStringBuilder oleDbStringBuilder = new OleDbConnectionStringBuilder(conn.ConnectionString);
            oleDbStringBuilder.Remove("provider");

            IBulkCopyWrapper bulkCopyWrapper = null;
            switch (conn.Provider)
            {
                case "sqloledb":
                    bulkCopyWrapper = new SqlBulkCopyWrapper(new SqlConnection(oleDbStringBuilder.ConnectionString));
                    break;
                case "oraoledb":
                    bulkCopyWrapper = new OracleBulkCopyWrapper(new OracleConnection(oleDbStringBuilder.ConnectionString));
                    break;
                default: throw new Exception("Not Support OleDbConnection");
            }
        }
        public BulkCopy(string oleDbConnString)
        {
            OleDbConnectionStringBuilder oleDbStringBuilder = new OleDbConnectionStringBuilder(oleDbConnString);
            string provider = oleDbStringBuilder.Provider;
            oleDbStringBuilder.Remove("provider");

            IBulkCopyWrapper bulkCopyWrapper = null;
            switch (provider)
            {
                case "sqloledb":
                    bulkCopyWrapper = new SqlBulkCopyWrapper(new SqlConnection(oleDbStringBuilder.ConnectionString));
                    break;
                case "oraoledb":
                    bulkCopyWrapper = new OracleBulkCopyWrapper(new OracleConnection(oleDbStringBuilder.ConnectionString));
                    break;
                default: throw new Exception("Not Support OleDbConnection");
            }
        }
        public void Insert(string tableName, DataTable sourceData, Dictionary<string, string> columnMappings = null)
        {
            bulkCopyWrapper.DestinationTableName = tableName;
            this.FillDefaultColumnmappingsIfNull(columnMappings, sourceData);
            bulkCopyWrapper.WriteToServer(sourceData);
        }
        public void Insert(string tableName, DataTable sourceData, DataRowState state, Dictionary<string, string> columnMappings = null)
        {
            bulkCopyWrapper.DestinationTableName = tableName;
            this.FillDefaultColumnmappingsIfNull(columnMappings, sourceData);
            bulkCopyWrapper.WriteToServer(sourceData, state);
        }
        public void FillDefaultColumnmappingsIfNull(Dictionary<string, string> columnMappings, DataTable sourceData)
        {
            this.bulkCopyWrapper.ColumnMappings.Clear();
            if (columnMappings == null)
            {
                foreach (var item in sourceData.Columns.Cast<DataColumn>())
                {
                    this.bulkCopyWrapper.ColumnMappings.Add(item.ColumnName, item.ColumnName);
                }
            }
            else
            {
                this.bulkCopyWrapper.ColumnMappings = columnMappings;
            }
        }

        public static BulkCopySupportDB GetSupportedDBType(string providerName)
        {
            switch (providerName)
            {
                case "sqloledb":
                    return BulkCopySupportDB.MSSQL;
                case "oraoledb":
                    return BulkCopySupportDB.Oracle;
                default: throw new Exception("Not Support OleDbConnection");
            }
        }
        public enum BulkCopySupportDB
        {
            MSSQL,
            Oracle
        }

        public static void BulkInsert(DbConnection conn, string providerName, string tableName, DataTable sourceData, DataRowState state, Dictionary<string, string> columnMappings = null)
        {
            BulkCopy bulkCopy = new BulkCopy(conn.ConnectionString, BulkCopy.GetSupportedDBType(providerName));
            bulkCopy.Insert(tableName, sourceData, state, columnMappings);

        }

        public static void BulkInsert(DbConnection conn, string providerName, string tableName, DataTable sourceData, Dictionary<string, string> columnMappings = null)
        {
            BulkCopy bulkCopy = new BulkCopy(conn.ConnectionString, BulkCopy.GetSupportedDBType(providerName));
            bulkCopy.Insert(tableName, sourceData, columnMappings);
        }

        public static void BulkInsert(OleDbConnection conn, string tableName, DataTable sourceData, DataRowState state, Dictionary<string, string> columnMappings = null)
        {
            BulkCopy bulkCopy = new BulkCopy(conn);
            bulkCopy.Insert(tableName, sourceData, state, columnMappings);
        }

        public static void BulkInsert(OleDbConnection conn, string tableName, DataTable sourceData, Dictionary<string, string> columnMappings = null)
        {
            BulkCopy bulkCopy = new BulkCopy(conn);
            bulkCopy.Insert(tableName, sourceData, columnMappings);
        }
    }
}
