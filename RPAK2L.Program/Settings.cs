using System;
using System.Collections.Generic;
using System.IO;

namespace RPAK2L.Program
{
    public class Settings
    {
        public static Ini IniInstance;
        private static Dictionary<string, string> keyValue;
        public static void Init(string iniFilePath)
        {
            keyValue = new Dictionary<string, string>();
            IniInstance = new Ini(iniFilePath);
            
        }

        public static void Load()
        {
            IniInstance.Load();
            
            if(IniInstance.GetValue("ExportPath","","lmaothatshitisnull") == "lmaothatshitisnull")
            {
                IniInstance.WriteValue("ExportPath",Path.Combine(Environment.CurrentDirectory, "Export"));
            }
            
            
            string[] keys = IniInstance.GetKeys("");
            for (int i = 0; i < keys.Length; i++)
            {
                if(!keyValue.ContainsKey(keys[i]))
                    keyValue.Add(keys[i], IniInstance.GetValue(keys[i]));
            }
        }

        public static void Save()
        {
            var keys = keyValue.Keys;
            foreach (string key in keys)
            {
                string val = "";
                if(keyValue.TryGetValue(key, out val))
                    IniInstance.WriteValue(key, val);
            }
            IniInstance.Save();
        }

        public static string Get(string key, string ifNotFound = "")
        {
            string val = "";
            if (keyValue.TryGetValue(key, out val))
                return val;
            else
            {
                return ifNotFound;
            }
        }

        public static void Set(string key, string value)
        {
            if (keyValue.ContainsKey(key))
                keyValue.Remove(key);
            keyValue.Add(key,value);
        }
    }
}