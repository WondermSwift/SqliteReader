using System;
using System.Collections;
using System.Collections.Generic;
using Community.CsharpSqlite;
using SQLite;
using UnityEngine;

namespace Wonderm.SQLiteNs
{
    public class SQLiteDataReader : IDisposable
    {
        private SQLite.SQLiteCommand _cmd;
        private Sqlite3.Vdbe _vd;
        private List<string> _columnNameList = new List<string> (8);

        public int FieldCount
        {
            get { return Sqlite3.sqlite3_column_count (_vd); }
        }

        public void SetCmd (SQLite.SQLiteCommand cmd)
        {
            _cmd = cmd;
            _vd = _cmd.M_Prepare ();
        }

        public bool Read ()
        {
            if (SQLite3.Step (_vd) == SQLite3.Result.Row)
            {
                _columnNameList.Clear ();

                int count = FieldCount;
                for (int i = 0; i < count; i++)
                {
                    string n = SQLite3.ColumnName16 (_vd, i);
                    _columnNameList.Add (n);
                }

                return true;
            }
            return false;
        }

        public int GetOrdinal (string name)
        {
            for (int i = 0; i < _columnNameList.Count; i++)
            {
                if (string.Compare (_columnNameList[i], name, true) == 0)
                    return i;
            }

            Debug.LogError ("Error：can not find column named【" + name + "】！");

            return 0;
        }

        public int GetInt32 (int i)
        {
            return SQLite3.ColumnInt (_vd, i);
        }

        public string GetString (int i)
        {
            return SQLite3.ColumnString (_vd, i);
        }

        public void Dispose ()
        {
            _columnNameList.Clear ();
            _cmd = null;
            _vd = null;
        }
    }
}