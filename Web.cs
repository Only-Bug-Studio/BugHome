using System;
using System.Net.Security;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Security.Cryptography.X509Certificates;

class Web
{
    static bool usingTLS = false;
    static Int32 httpsPort = 0;
    static Int32 httpPort;
    static TcpListener HttpServer = null;
    static TcpListener HttpsServer = null;
    static Logger logger = new Logger("Web");
    static X509Certificate2 cert;

    static Thread httpsL;
    public static void WebMain()
    {
        if (!Int32.TryParse(Program.setting.GetValue("http_port"), out httpPort))
        {
            logger.Log(LogLevel.Fatal, Translation.GetTranslation("Web.portNotNum"));
        }
        if (Program.setting.GetValue("usingTLS") == "true")
        {
            try
            {
                cert = new X509Certificate2(Program.setting.GetValue("certificate"), Program.setting.GetValue("certificatePassword"));
            }
            catch
            {
                logger.Log(LogLevel.Fatal, Translation.GetTranslation("Web.https.certError"));
            }
            usingTLS = true;
            if (!Int32.TryParse(Program.setting.GetValue("https_port"), out httpsPort))
            {
                logger.Log(LogLevel.Fatal, Translation.GetTranslation("Web.portNotNum"));
            }
            httpsL = new Thread(httpsListen);
            httpsL.Start();
        }
        if (Program.setting.GetValue("OpenHttp") == "true")
        {
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            HttpServer = new TcpListener(localAddr, httpPort);
            logger.Log(LogLevel.Debug, $"{httpsPort}");
            HttpServer.Start();
            Data.IsWebOK = true;
            logger.Log(LogLevel.Info, Translation.GetTranslation("Web.Open"));
            Thread s = null;
            while (true)
            {
                if (!Data.IsOpen)
                {
                    break;
                }
                TcpClient tcpClient = HttpServer.AcceptTcpClient();
                Connect c = new Connect(tcpClient.Client.RemoteEndPoint, tcpClient.GetStream());
                //交给新线程
                s = new Thread(c.HTTP_Solve);
                s.Start();
            }
            HttpServer.Stop();
        }
        if (Program.setting.GetValue("usingTLS") == "true")
        {
            try
            {
                httpsL.Join();
            }
            catch
            {
            }
        }
    }
    public static void httpsListen()
    {
        IPAddress localAddr = IPAddress.Parse("127.0.0.1");
        HttpsServer = new TcpListener(localAddr, httpsPort);
        HttpsServer.Start();
        logger.Log(LogLevel.Info, Translation.GetTranslation("Web.Open.https"));
        Thread s = null;
        while (true)
        {
            if (!Data.IsOpen)
            {
                break;
            }
            TcpClient tcpClient = HttpsServer.AcceptTcpClient();
            Connect c = new Connect(tcpClient.Client.RemoteEndPoint, tcpClient.GetStream(), cert);
            //交给新线程
            s = new Thread(c.HTTPS_Solve);
            s.Start();
        }
        HttpsServer.Stop();
    }
}