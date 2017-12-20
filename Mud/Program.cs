using System.IO;
using Mud.Data.Configuration;
using Newtonsoft.Json;

namespace Mud
{
    class Program
    {
        static void Main(string[] args)
        {
            var appSettings = LoadSettings();
        }

        private static AppSettings LoadSettings()
        {
            using (StreamReader r = new StreamReader("AppSettings.json"))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<AppSettings>(json);
            }
        }

    }
}
