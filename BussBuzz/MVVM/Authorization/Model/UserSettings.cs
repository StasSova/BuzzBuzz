using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SharpCompress.Common;

namespace BussBuzz.MVVM.Authorization.Model
{
    public class UserSettingsData
    {
        public string UsernameIsLogged { get; set; } // Если не вошёл, то null
    }
    public class UserSettings
    {
        public static void SaveToFile(string username)
        {
            try
            {
                string fileName = "userdata.json";
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), fileName);

                UserSettingsData toFile = new UserSettingsData
                {
                    UsernameIsLogged = username,
                };
                string json = JsonConvert.SerializeObject(toFile);

                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при сохранении файла: {ex.Message}");
            }
        }

        public static string ReadFromFile()
        {
            try
            {
                string fileName = "userdata.json";
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), fileName);

                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);

                    UserSettingsData usersettings = JsonConvert.DeserializeObject<UserSettingsData>(json);
                    return usersettings.UsernameIsLogged;
                }
                else
                {
                    SaveToFile(null);
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при чтении файла: {ex.Message}");
            }
        }
    }
}
