using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite;
using System;
using Community.CsharpSqlite;

namespace Wonderm.SQLiteNs
{
    public class SQLiteHelper : IDisposable
    {
        #region static

        /// <summary>
        /// 创建数据库文件
        /// </summary>
        /// <param name="path"></param>
        public static void CreateSQLiteDB(string path)
        {
            new SQLiteConnection(path);
        }

        #endregion static

        /// <summary>
        /// 数据库连接定义
        /// </summary>
        private SQLiteConnection dbConnection;

        /// <summary>
        /// 操作数据库命令
        /// </summary>
        private SQLiteCommand dbCommand;

        /// <summary>
        /// 操作结果流
        /// </summary>
        private SQLiteDataReader dbReader;

        /// <summary>
        /// 数据读取定义
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="write">开启写模式文件不存在时会自动创建</param>
        public SQLiteHelper(string path, bool write = false)
        {
            try
            {
                ConnectToDatabase(path, write);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        /// <summary>
        /// 链接数据库
        /// </summary>
        private void ConnectToDatabase(string path, bool write = false)
        {
            if (write)
            {
                dbConnection = new SQLiteConnection(path, SQLiteOpenFlags.ReadWrite);
            }
            else
            {
                dbConnection = new SQLiteConnection(path, SQLiteOpenFlags.ReadOnly);
            }
            dbReader = new SQLiteDataReader();
            dbCommand = dbConnection.CreateCommand(string.Empty);
        }

        private SQLiteDataReader ExecuteQuery(string queryString)
        {
            dbCommand.CommandText = queryString;
            dbReader.SetCmd(dbCommand);

            return dbReader;
        }

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public void CloseConnection()
        {
            (this as IDisposable).Dispose();
        }

        /// <summary>
        /// 创建数据表
        /// </summary> +
        /// <returns>The table.</returns>
        /// <param name="tableName">数据表名</param>
        /// <param name="colNames">字段名</param>
        /// <param name="colTypes">字段名类型</param>
        public void CreateTable(string tableName, string[] colNames, string[] colTypes)
        {
            string queryString = "CREATE TABLE " + tableName + "( " + colNames[0] + " " + colTypes[0];
            for (int i = 1; i < colNames.Length; i++)
            {
                queryString += ", " + colNames[i] + " " + colTypes[i];
            }
            queryString += "  ) ";

            dbConnection.Execute(queryString);
        }

        /// <summary>
        /// 向指定数据表中插入数据
        /// </summary>
        /// <returns>The values.</returns>
        /// <param name="tableName">数据表名称</param>
        /// <param name="values">插入的数值</param>
        public void InsertValues(string tableName, string[] values)
        {
            string queryString = "INSERT INTO " + tableName + " VALUES (" + values[0];
            for (int i = 1; i < values.Length; i++)
            {
                queryString += ", " + "'" + values[i] + "'";
            }
            queryString += " )";
            dbConnection.Execute(queryString);
        }

        /// <summary>
        /// 读取整张表
        /// </summary>
        /// <param name="table"></param>
        public SQLiteDataReader ReadFullTable(string tableName)
        {
            string query =  "SELECT * FROM " + tableName;
            return ExecuteQuery(query);
        }

        /// <summary>
        /// 读取一行
        /// </summary>
        /// <param name="tableName"></param>
        public SQLiteDataReader ReadSingle(string tableName, string item, string col, string operation, string values)
        {
            string query = "SELECT " + item + " FROM " + tableName + " WHERE " + col + operation + values;
            return ExecuteQuery(query);
        }

        void IDisposable.Dispose()
        {
            if (dbReader != null)
            {
                dbReader.Dispose();
            }
            dbReader = null;

            if (dbCommand != null)
            {
                dbCommand.Dispose();
            }
            dbCommand = null;

            if (dbConnection != null)
            {
                dbConnection.Close();
            }
            dbConnection = null;
        }
    }
}