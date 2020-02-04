using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Wonderm.SQLiteNs
{
    public static class DbReader
    {
        [AttributeUsage (AttributeTargets.Class)]
        public class DbTableAttribute : System.Attribute
        {
            public DbTableAttribute (string tableName)
            {
                TableName = tableName;
            }

            public string TableName;
        }

        [AttributeUsage (AttributeTargets.Property | AttributeTargets.Field)]
        public class DbFieldAttribute : System.Attribute
        {
            public DbFieldAttribute (string fieldName, char separator = ',')
            {
                FieldName = fieldName;

                Separator = separator;
            }

            public string FieldName;

            public char Separator;
        }

        #region Attribute

        private static FieldInfo[] GetFieldInfos (System.Type t)
        {
            return t.GetFields (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        private static PropertyInfo[] GetPropertyInfos (System.Type t)
        {
            return t.GetProperties (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        private static DbTableAttribute GetDbTableAttribute (Type type)
        {
            if (!type.IsDefined (typeof (DbTableAttribute), true)) return null;

            return type.GetCustomAttributes (typeof (DbTableAttribute), true) [0] as DbTableAttribute;
        }

        private static DbFieldAttribute GetDbFieldAttribute (MemberInfo memInfo)
        {
            if (!memInfo.IsDefined (typeof (DbFieldAttribute), true)) return null;

            return memInfo.GetCustomAttributes (typeof (DbFieldAttribute), true) [0] as DbFieldAttribute;
        }

        private static MemberInfo[] GetFieldProperty<T> () where T : class, new ()
        {
            System.Type t = typeof (T);

            MemberInfo[] mems = t.GetMembers (BindingFlags.Instance |
                BindingFlags.NonPublic |
                BindingFlags.Public
            );

            return mems;
        }

        #endregion Attribute

        private static object GetValue (SQLiteDataReader reader, System.Type fieldType, DbFieldAttribute attr)
        {
            object v = null;

            if (fieldType == typeof (int))
            {
                v = Converter.ToInt (reader, attr.FieldName);
            }
            else if (fieldType == typeof (byte))
            {
                v = (byte) Converter.ToInt (reader, attr.FieldName);
            }
            else if (fieldType == typeof (float))
            {
                v = Converter.ToFloat (reader, attr.FieldName);
            }
            else if (fieldType == typeof (string))
            {
                v = Converter.ToString (reader, attr.FieldName);
            }
            else if (fieldType == typeof (bool))
            {
                v = Converter.ToBool (reader, attr.FieldName);
            }
            else if (fieldType == typeof (Vector3))
            {
                v = Converter.ToVector3 (reader, attr.FieldName, attr.Separator);
            }
            else if (fieldType == typeof (Vector2))
            {
                v = Converter.ToVector2 (reader, attr.FieldName, attr.Separator);
            }
            else if (fieldType.IsEnum)
            {
                v = System.Enum.Parse (fieldType, Converter.ToString (reader, attr.FieldName));
            }
            else if (fieldType == typeof (int[]))
            {
                v = Converter.GetIntArray (reader, attr.FieldName, attr.Separator);
            }
            else if (fieldType == typeof (float[]))
            {
                v = Converter.ToFloatArray (reader, attr.FieldName, attr.Separator);
            }
            else if (fieldType == typeof (string[]))
            {
                v = Converter.ToStringArray (reader, attr.FieldName, attr.Separator);
            }
            else
            {
                Debug.LogError ("not supported value type:" + fieldType);
            }
            return v;
        }

        private static int attrTEMP;
        private static List<DbFieldAttribute> dbFieldAttrTEMP;
        private static List<DbFieldAttribute> dbPropertyAttrTEMP;
        private static T BuildListItem<T> (SQLiteDataReader reader, FieldInfo[] fieldInfos, PropertyInfo[] propertyInfos, int cacheId = 0) where T : class, new ()
        {
            if (cacheId == 0 || cacheId != attrTEMP)
            {
                dbFieldAttrTEMP = null;
                dbPropertyAttrTEMP = null;
            }

            attrTEMP = cacheId;

            T item = new T ();

            for (int i = 0; i < fieldInfos.Length; i++)
            {
                var field = fieldInfos[i];
                if (dbFieldAttrTEMP == null)
                {
                    dbFieldAttrTEMP = new List<DbFieldAttribute> (8);
                }

                if (dbFieldAttrTEMP.Count < fieldInfos.Length)
                {
                    DbFieldAttribute dbFieldAttr = GetDbFieldAttribute (field);
                    dbFieldAttrTEMP.Add (dbFieldAttr);
                }
                var atrr = dbFieldAttrTEMP[i];
                if (atrr == null)
                {
                    continue;
                }

                object v = GetValue (reader, field.FieldType, atrr);

                field.SetValue (item, v);
            }

            for (int i = 0; i < propertyInfos.Length; i++)
            {
                var property = propertyInfos[i];

                if (dbPropertyAttrTEMP == null)
                {
                    dbPropertyAttrTEMP = new List<DbFieldAttribute> (8);
                }

                if (dbPropertyAttrTEMP.Count < fieldInfos.Length)
                {
                    DbFieldAttribute dbPropertyttr = GetDbFieldAttribute (property);
                    dbPropertyAttrTEMP.Add (dbPropertyttr);
                }
                var atrr = dbPropertyAttrTEMP[i];
                if (atrr == null)
                {
                    continue;
                }
                object v = GetValue (reader, property.PropertyType, atrr);

                property.SetValue (item, v, null);
            }

            return item;
        }

        private static T BuildItem<T> (SQLiteDataReader reader, FieldInfo[] fieldInfos, PropertyInfo[] propertyInfos) where T : class, new ()
        {
            T item = new T ();

            foreach (FieldInfo field in fieldInfos)
            {
                DbFieldAttribute dbFieldAttr = GetDbFieldAttribute (field);
                if (dbFieldAttr == null)
                {
                    continue;
                }

                object v = GetValue (reader, field.FieldType, dbFieldAttr);

                field.SetValue (item, v);
            }

            foreach (PropertyInfo property in propertyInfos)
            {
                DbFieldAttribute dbPropertyAttr = GetDbFieldAttribute (property);
                if (dbPropertyAttr == null)
                {
                    continue;
                }
                object v = GetValue (reader, property.PropertyType, dbPropertyAttr);

                property.SetValue (item, v, null);
            }

            return item;
        }

        public static List<T> Read<T> (SQLiteHelper helper, int capacity = 20) where T : class, new ()
        {
            var table = GetDbTableAttribute (typeof (T));
            if (table != null)
            {
                var reader = helper.ReadFullTable (table.TableName);
                return Read<T> (reader, capacity);
            }
            Debug.LogError (string.Format ("Class {0} is do not have a attribute named DbTable", typeof (T).Name));
            return null;
        }

        public static List<T> Read<T> (SQLiteDataReader reader, int capacity = 20) where T : class, new ()
        {
            System.Type t = typeof (T);
            FieldInfo[] fieldInfos = GetFieldInfos (t);
            PropertyInfo[] propertyInfos = GetPropertyInfos (t);

            List<T> list = new List<T> (capacity);

            int cacheId = reader.GetHashCode ();
            
            while (reader.Read ())
            {
                T item = BuildListItem<T> (reader, fieldInfos, propertyInfos, cacheId);
                list.Add (item);
            }

            return list;
        }

        public static T ReadItem<T> (SQLiteDataReader reader) where T : class, new ()
        {
            System.Type t = typeof (T);
            FieldInfo[] fieldInfos = GetFieldInfos (t);
            PropertyInfo[] propertyInfos = GetPropertyInfos (t);

            return BuildItem<T> (reader, fieldInfos, propertyInfos);
        }
    }
}