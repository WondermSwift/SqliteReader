using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wonderm.SQLiteNs;

[DbReader.DbTable("Test2")]
public class Test2Data
{
    [DbReader.DbField("ID")]
    public int ID;

    [DbReader.DbField("Name")]
    public string Name;

    [DbReader.DbField("Type")]
    public int Type;

    [DbReader.DbField("Desc")]
    public string Desc;

    [DbReader.DbField("HP")]
    public float HP;

    [DbReader.DbField("MP")]
    public float MP;

    [DbReader.DbField("Monster")]
    public bool Monster;
}