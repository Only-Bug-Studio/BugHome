class Program
{
    static Setting setting = new Setting();
    static Logger logger = new Logger("Main");
    static void Main(string[] args)
    {
        logger.Log(LogLevel.Info,"Loading");
        setting.Load();
        logger.Log(LogLevel.Info,Translate.GetTranslate("main.welcome"));
        logger.Log(LogLevel.Info,Translate.GetTranslate("main.starting"));
    }
}