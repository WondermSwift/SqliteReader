using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ExcelNs;
using Wonderm.SQLiteNs;

public class Test
{
    [MenuItem("Tool/Excel/Test &1")]
    private static void ShowXlS()
    {
        var excel = ExcelHelper.LoadExcel(Application.streamingAssetsPath + "/Test/Test3.xlsx");
        if (excel == null) return;

        ExcelToSQLite.ConvertToSQLite(excel);

        UnityEditor.AssetDatabase.Refresh();
    }

    private static string SqlitePath
    {
        get
        {
            return Application.streamingAssetsPath + "/DataBase/Data.db";
        }
    }

    [MenuItem("Tool/Excel/Test &2")]
    private static void ReadSqlite()
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