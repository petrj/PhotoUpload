using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;
using System.IO;

namespace GAPI
{
    public class GAPIBaseObject
    {
        public static string AppDataDir
        {
            get
            {
                var sep = Path.DirectorySeparatorChar.ToString();
                var dir = System.AppDomain.CurrentDomain.BaseDirectory;
                if (!dir.EndsWith(sep))
                {
                    dir += sep;
                }

                return dir;
            }
        }

        public void SaveToFile(string name)
        {
            try
            {
                if (!Path.IsPathRooted(name))
                {
                    name = AppDataDir + name;
                }

                File.WriteAllText(name, this.ToString());

                Logger.Info($"Saved to file: {name}");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }

        public static T LoadFromFile<T>(string name)
        {
            try
            {
                if (!Path.IsPathRooted(name))
                {
                    name = AppDataDir + name;
                }
                string s = File.ReadAllText(name);

                var obj = JsonConvert.DeserializeObject<T>(s);

                return obj;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
