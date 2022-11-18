using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopDB.Models;

namespace ShopDB
{
    internal static class Config
    {
        public static UserSettings UserSettings { get; set; }
        public static string Path = App.WorkingDirectory + "\\properties.json";

        public static void Load()
        {
            if (UserSettings != null)
                return;

            if (File.Exists(Path))
            {
                UserSettings = JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(Path));
                return;
            }

            UserSettings = new UserSettings();
        }

        public static void Save()
        {
            if (UserSettings == null) return;

            File.WriteAllText(Path, JsonConvert.SerializeObject(UserSettings,new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Populate,
                Formatting = Formatting.Indented
            }));
        }
    }
}
