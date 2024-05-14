using Newtonsoft.Json;

namespace Arcam.Data
{
    public class DataSerializer<T>
    {
        public List<T>? Data;
        private string fileName = $"{Constants.ConfigDirectory}/baddates.config";
        public DataSerializer()
        {
            if (!File.Exists(fileName))
                File.Create(fileName).Close();
            Data = new List<T>();
        }
        public DataSerializer(string fileName)
        {
            this.fileName = fileName;
            if (!File.Exists(fileName))
                File.Create(fileName).Close();
            Data = new List<T>();
        }
        public void LoadData()
        {
            using (var f = new StreamReader(fileName))
            {
                Data = JsonConvert.DeserializeObject<List<T>>(f.ReadToEnd() ?? string.Empty);
            }
        }
        public void SaveData()
        {
            using (var f = new StreamWriter(fileName))
            {
                f.WriteLine(JsonConvert.SerializeObject(Data));
            }
        }
    }
}
