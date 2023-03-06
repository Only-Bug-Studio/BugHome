using System.Dynamic;
class Setting
{
    Dictionary<string, string> settings = new Dictionary<string, string>();
    Logger logger = new Logger("Setting");
    public Setting()
    {
        settings["language"] = "zh-cn";
    }
    public void SetValue(string key, string value)
    {
        settings[key] = value;
    }
    public string GetValue(string key)
    {
        string a;
        if (settings.TryGetValue(key, out a))
        {
            return a;
        }
        logger.Log(LogLevel.Error, key + " can't find");
        return "";
    }
    public void Save()
    {
        FileStream fs = new FileStream("Access/setting.txt", FileMode.Create, FileAccess.Write, FileShare.None);
        StreamWriter sw = new StreamWriter(fs);
        foreach (var item in settings)
        {
            sw.WriteLine(item.Key + "=" + item.Value);
        }
        sw.Close();
        fs.Close();
    }
    public void Load()
    {
        try
        {
            string[] lines = System.IO.File.ReadAllLines("Access/setting.txt");
            foreach (string line in lines)
            {
                string[] parts = line.Split(new char[] { '=' }, 2);
                settings[parts[0]] = parts[1];
            }
        }
        catch//(Exception e)
        {
            logger.Log(LogLevel.Warn, "Can't find setting file");
            Save();
        }
        Translation.LoadTranslation(settings["language"]);
        logger.Log(LogLevel.Info, Translation.GetTranslation("setting.load.OK"));
    }
}