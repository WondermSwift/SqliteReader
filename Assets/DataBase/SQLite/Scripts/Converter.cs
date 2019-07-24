using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wonderm.SQLiteNs
{
    public class Converter
    {
        public static int ToInt (SQLiteDataReader reader, string fieldName)
        {
            string text = "";
            try
            {
                text = reader.GetString (reader.GetOrdinal (fieldName));
            }
            catch
            {
                Debug.Log (fieldName);
            }

            if (string.IsNullOrEmpty (text))
            {
                return 0;
            }

            int ret;

            if (!int.TryParse (text, out ret))
            {
                Debug.LogError ("parse to int error, [" + fieldName + "]=[" + text + "]");
            }

            return ret;
        }

        public static int[] GetIntArray (SQLiteDataReader reader, string fieldName, char separator)
        {
            return ParseIntArray (ToString (reader, fieldName), separator);
        }

        public static float ToFloat (SQLiteDataReader reader, string fieldName)
        {
            return float.Parse (reader.GetString (reader.GetOrdinal (fieldName)));
            // return System.Convert.ToSingle (reader.GetString (reader.GetOrdinal (fieldName)));
        }

        public static float[] ToFloatArray (SQLiteDataReader reader, string fieldName, char separator = ',')
        {
            return ParseFloatArray (ToString (reader, fieldName), separator);
        }

        public static string ToString (SQLiteDataReader reader, string fieldName)
        {
            return reader.GetString (reader.GetOrdinal (fieldName));
        }

        public static string[] ToStringArray (SQLiteDataReader reader, string fieldName, char separator = ',')
        {
            var str = reader.GetString (reader.GetOrdinal (fieldName));
            return str.Split (separator);
        }

        public static bool ToBool (SQLiteDataReader reader, string fieldName)
        {
            return ToInt (reader, fieldName) == 0 ? false : true;
        }

        public static Vector3 ToVector3 (SQLiteDataReader reader, string fieldName, char separator = ',')
        {
            float[] array = ToFloatArray (reader, fieldName, separator);
            if (array == null || array.Length != 3)
            {
                throw new Exception ("db field error:" + fieldName);
            }

            return new Vector3 (array[0], array[1], array[2]);
        }

        public static Vector2 ToVector2 (SQLiteDataReader reader, string fieldName, char separator = ',')
        {
            float[] array = ToFloatArray (reader, fieldName, separator);
            if (array == null || array.Length != 2)
            {
                throw new Exception ("db field error:" + fieldName);
            }

            return new Vector3 (array[0], array[1]);
        }

        public static int[] ParseIntArray (string text, char separator = ',')
        {
            if (string.IsNullOrEmpty (text))
            {
                return null;
            }

            try
            {
                string[] tmplist = text.Split (separator);
                int[] intArray = new int[tmplist.Length];
                for (int i = 0; i < tmplist.Length; i++)
                {
                    intArray[i] = Convert.ToInt32 (tmplist[i]);
                }
                return intArray;
            }
            catch (Exception)
            {
                Debug.LogError ("separate text:" + text + " by[" + separator + "] parse int array error");
                throw;
            }
        }

        public static float[] ParseFloatArray (string text, char separator = ',')
        {
            if (string.IsNullOrEmpty (text))
            {
                return null;
            }

            try
            {
                string[] tmplist = text.Split (separator);
                float[] array = new float[tmplist.Length];
                for (int i = 0; i < tmplist.Length; i++)
                {
                    array[i] = Convert.ToSingle (tmplist[i]);
                }
                return array;
            }
            catch (Exception)
            {
                Debug.LogError ("separate text:" + text + " by[" + separator + "] parse float array error");
                throw;
            }
        }

        public static string[] ParseStringArray (string text, char separator = ',')
        {
            if (string.IsNullOrEmpty (text))
                return null;

            try
            {
                return text.Split (separator);
            }
            catch (Exception)
            {
                Debug.LogError ("separate text:" + text + " by[" + separator + "] parse string array error");
                throw;
            }
        }

        public static Vector3 ParseVector3 (string str, char c = ',')
        {
            if (string.IsNullOrEmpty (str))
            {
                return Vector3.zero;
            }

            string[] posString = str.Split (c);

            if (posString.Length != 3)
            {
                return Vector3.zero;
            }

            return new Vector3 (float.Parse (posString[0]), float.Parse (posString[1]), float.Parse (posString[2]));
        }
    }
}