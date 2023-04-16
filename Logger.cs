using System.Diagnostics;
//new bing 辅助
enum LogLevel { Trace, Debug, Info, Warn, Error, Fatal }
class Logger
{
    string name = "unknown";
    public Logger(string name)
    {
        this.name = name;
    }

    // 定义一个方法，根据警告级别设置控制台前景色
    static void SetConsoleColor(LogLevel level)
    {
        switch (level)
        {
            case LogLevel.Trace:
                Console.ForegroundColor = ConsoleColor.Gray;
                break;
            case LogLevel.Debug:
                Console.ForegroundColor = ConsoleColor.DarkGray;
                break;
            case LogLevel.Info:
                Console.ForegroundColor = ConsoleColor.White;
                break;
            case LogLevel.Warn:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case LogLevel.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            case LogLevel.Fatal:
                Console.ForegroundColor = ConsoleColor.Magenta;
                break;
        }
    }

    // 定义一个方法，输出指定警告级别和内容的日志
    public void Log(LogLevel level, string message)
    {
        if (level == LogLevel.Debug)
        {
            if (!Data.IsDEBUG)
            {
                return;
            }
        }
        // 获取当前时间和时区
        var time = DateTimeOffset.Now.ToString("yyyy-MM-dd THH:mm:sszzz");
        // 设置控制台前景色
        SetConsoleColor(level);
        // 输出日志格式
        Console.WriteLine($"[{time}] {name} {level} \"{message}\"");
        if (level == LogLevel.Fatal)
        {
            Console.WriteLine("The Program will exit after 10s!");
            Console.WriteLine("程序将在10秒后退出！");
            Thread.Sleep(10 * 1000); // 等待10秒
            Environment.Exit(1);//异常退出
        }
        // 恢复默认前景色
        Console.ResetColor();
    }
}