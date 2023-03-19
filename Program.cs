using System.Text;
class Program
{
    static Thread web = new Thread(Web.WebMain);
    public static Setting setting = new Setting();
    static Logger logger = new Logger("Main");
    static void Main(string[] args)
    {
        logger.Log(LogLevel.Info, "Loading");
        setting.Load();
        logger.Log(LogLevel.Info, Translation.GetTranslation("main.welcome"));
        logger.Log(LogLevel.Info, Translation.GetTranslation("main.starting"));

        //线程处理
        web.Start();
        while (true)
        {
            if(Data.IsWebOK)
                break;
        }

        //等待结束
        web.Join();


        logger.Log(LogLevel.Info, Translation.GetTranslation("main.end"));
    }
}