class Program
{
    static Setting setting = new Setting();
    static Logger logger = new Logger("Main");
    static void Main(string[] args)
    {
        logger.Log(LogLevel.Info, "Loading");
        setting.Load();
        logger.Log(LogLevel.Info, Translation.GetTranslation("main.welcome"));
        logger.Log(LogLevel.Info, Translation.GetTranslation("main.starting"));
    }
}