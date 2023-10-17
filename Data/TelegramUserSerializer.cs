using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Arcam.Data
{
    class TelegramUserSerializer
    {
        public void SaveUsers(Dictionary<long, long> Users)
        {
            string json = JsonConvert.SerializeObject(Users);
            using (StreamWriter sw = new StreamWriter("users.json"))
            {
                sw.WriteLine(json);
            }
        }

        public Dictionary<long, long>? ReadUsers()
        {
            string json;
            if (!System.IO.File.Exists("users.json"))
            {
                return new Dictionary<long, long>();
            }
            using (StreamReader sr = new StreamReader("users.json"))
            {
                json = sr.ReadToEnd();
            }
            return JsonConvert.DeserializeObject<Dictionary<long, long>>(json);
        }
    }
}
