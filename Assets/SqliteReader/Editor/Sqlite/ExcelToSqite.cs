using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Wonderm.SQLiteNs;
using Wonderm.ExcelNs;

namespace Wonderm.SQLiteNs
{
    public class ExcelToSQLite
    {
        /// <summary>
        /// 导出表名的后缀
        /// </summary>
        private const string ExportExtra = "_Export";

        private string defaultSqlitePath = Application.streamingAssetsPath + "/DataBase/Data.db";

        private static string sqlitePath;

        public static string SqlitePath
        {
            get
            {
                return sqlitePath;
            }
        }

        private static List<string> TypeLimits = new List<string> {
        "INTEGER",
        "TEXT",
        "REAL",
        "BLOB",
    };

        private ExcelToSQLite()
        {
            sqlitePath = defaultSqlitePath;
        }

        public static void SetSqlitePath(string filePath)
        {
            sqlitePath = filePath;
        }

        public static void ConvertToSQLite(string excelPath)
        {
            var excel = ExcelHelper.LoadExcel(excelPath);
            if (excel == null) return;

            ExcelToSQLite.ConvertToSQLite(excel);

            UnityEditor.AssetDatabase.Refresh();
        }

        public static void ConvertToSQLite(Excel excel)
        {
            if (File.Exists(SqlitePath))
            {
                File.Delete(SqlitePath);
            }

            SQLiteHelper.CreateSQLiteDB(SqlitePath);

            using (var db = new SQLiteHelper(SqlitePath, true))
            {
                Convert(db, excel);
            }
        }

        public static void Convert(SQLiteHelper db, Excel excel)
        {
            List<string> colNames = new List<string>();
            List<string> coltypes = new List<string>();

            foreach (var table in excel.Tables)
            {
                if (!table.TableName.EndsWith(ExportExtra)) continue;

                #region 创建表

                if (table.NumberOfRows < 2)
                {
                    Debug.LogError("Data format error : NumberOfRows is less than 2");
                }
                string tableName = table.TableName.Replace(ExportExtra, "");

                {
                    for (int j = 1; j <= table.NumberOfColumns; j++)
                    {
                        if (colNames.Contains(table.GetCell(1, j).Value))
                        {
                            Debug.LogError(string.Format("tableName [{0}] repeated in table [{1}]", table.GetCell(1, j).Value, table.TableName));
                            return;
                        }

                        colNames.Add(table.GetCell(1, j).Value);
                        var cel = table.GetCell(2, j).Value;
                        if (!TypeLimits.Contains(cel))
                        {
                            Debug.LogError(string.Format("Type error in table [{0}] in ({1},{2}) error type {3}", tableName, 2, j, cel));
                            return;
                        }

                        coltypes.Add(cel);
                    }

                    if (colNames.Count != coltypes.Count)
                    {
                        Debug.LogError("ColNames do not match ColTypes in count : " + table.TableName);
                    }

                    db.CreateTable(tableName, colNames.ToArray(), coltypes.ToArray());
                }

                #endregion 创建表

                #region 插入数据

                for (int i = 3; i <= table.NumberOfRows; i++)
                {
                    List<string> cols = new List<string>();

                    for (int j = 1; j <= table.NumberOfColumns; j++)
                    {
                        cols.Add(table.GetCell(i, j).Value);
                    }

                    if (colNames.Count != cols.Count)
                    {
                        Debug.LogError("ColNames do not match Cols in count : " + table.TableName);
                        return;
                    }

                    db.InsertValues(tableName, cols.ToArray());
                }

                #endregion 插入数据
            }
        }
    }
}