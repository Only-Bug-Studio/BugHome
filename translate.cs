class Translate
{
    static Dictionary<string, string> TS = new Dictionary<string, string>();
    static bool inited = false;
    static Logger logger = new Logger("Translate");
    public static void TranslateInit()
    {
        try
        {
            string[] lines = System.IO.File.ReadAllLines("translate/zh-cn.txt");
            foreach (string line in lines)
            {
                string[] parts = line.Split(new char[] { '=' }, 2);
                TS[parts[0]]=parts[1];
            }
        }
        catch(Exception e)
        {
            logger.Log(LogLevel.Fatal,"Can't find zh-cn translate file!");
        }
        inited = true;
    }
    public static void LoadTranslate(string name)
    {
        try
        {
            string[] lines = System.IO.File.ReadAllLines("translate/"+name+".txt");
            foreach (string line in lines)
            {
                string[] parts = line.Split(new char[] { '=' }, 2);
                TS[parts[0]]=parts[1];
            }
        }
        catch(Exception e)
        {
            logger.Log(LogLevel.Error,GetTranslate("translate.cannotFind")+name);
        }
    }
    public static string GetTranslate(string key)
    {
        if (!inited)
        {
            TranslateInit();
        }
        string a;
        if(TS.TryGetValue(key,out a))
        {
            return a;
        }
        logger.Log(LogLevel.Error,key+" can't find");
        return key;
    }
}