using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Wonderm.SQLiteNs;

public class Test
{
    //[MenuItem("Tool/Excel/Test &1")]
    public static void ShowXlS()
    {
        string path = Application.streamingAssetsPath + "/Test/Test3.xlsx";

        ExcelToSQLite.ConvertToSQLite(path);
    }

    private static string SqlitePath
    {
        get
        {
            return Application.streamingAssetsPath + "/DataBase/Data.db";
        }
    }

    //[MenuItem("Tool/Excel/Test &2")]
    public static void ReadSqlite()
    {
        var db = new SQLiteHelper(SqlitePath, false);

        var datas = DbReader.Read<Test2Data>(db);

        if (datas == null) return;

        foreach (var d in datas)
        {
            Debug.Log(JsonUtility.ToJson(d));
        }
    }
}