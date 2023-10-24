using Newtonsoft.Json;

namespace Arcam.Data
{
    class TelegramUserSerializer
    {
        public void SaveUsers(Dictionary<long, long> Users)
        {
            string json = JsonConvert.SerializeObject(Users);
            using (StreamWriter sw = new StreamWriter($"{Constants.TempDirectory}/users.json"))
            {
                sw.WriteLine(json);
            }
        }

        public Dictionary<long, long>? ReadUsers()
        {
            string json;
            if (!System.IO.File.Exists($"{Constants.TempDirectory}/users.json"))
            {
                return new Dictionary<long, long>();
            }
            using (StreamReader sr = new StreamReader($"{Constants.TempDirectory}/users.json"))
            {
                json = sr.ReadToEnd();
            }
            return JsonConvert.DeserializeObject<Dictionary<long, long>>(json);
        }
    }
}
