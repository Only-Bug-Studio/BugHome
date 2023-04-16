using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;

class Connect
{
    public EndPoint IP;
    public NetworkStream client;
    public X509Certificate2 cert;
    static Logger logger = new Logger("Connect");
    bool IsTLS = false;
    int RTO;
    int WTO;
    public Connect(EndPoint IP, NetworkStream client)
    {

        if (!Tools.StringTime2MsInt(Program.setting.GetValue("WaitTimeout"), out RTO))
        {
            logger.Log(LogLevel.Fatal, Translation.GetTranslation("Web.TimeoutError"));
        }
        logger.Log(LogLevel.Debug, $"WaitTimeout:{RTO}");

        if (!Tools.StringTime2MsInt(Program.setting.GetValue("WriteTimeout"), out WTO))
        {
            logger.Log(LogLevel.Fatal, Translation.GetTranslation("Web.TimeoutError"));
        }
        logger.Log(LogLevel.Debug, $"WriteTimeout:{WTO}");
        this.IP = IP;
        this.client = client;
        IsTLS = false;
    }
    public Connect(EndPoint IP, NetworkStream client, X509Certificate2 cert)
    {
        if (!Tools.StringTime2MsInt(Program.setting.GetValue("WaitTimeout"), out RTO))
        {
            logger.Log(LogLevel.Fatal, Translation.GetTranslation("Web.TimeoutError"));
        }
        logger.Log(LogLevel.Debug, $"WaitTimeout:{RTO}");

        if (!Tools.StringTime2MsInt(Program.setting.GetValue("WriteTimeout"), out WTO))
        {
            logger.Log(LogLevel.Fatal, Translation.GetTranslation("Web.TimeoutError"));
        }
        logger.Log(LogLevel.Debug, $"WriteTimeout:{WTO}");
        this.IP = IP;
        this.client = client;
        this.cert = cert;
        IsTLS = true;
    }
    public void HTTP_Solve()
    {
        if (IsTLS)
        {
            HTTPS_Solve();
            return;
        }
        client.ReadTimeout = RTO;
        client.WriteTimeout = WTO;
        byte[] buffer = new byte[10 * 1024];
        while (true)
        {
            try
            {
                client.Read(buffer);
                client.Write(WebServer(new HttpClientMessage(buffer)).GetBytes());
            }
            catch
            {
                break;
            }
        }
        return;
    }
    public void HTTPS_Solve()
    {
        if (!IsTLS)
        {
            HTTP_Solve();
            return;
        }
        SslStream sslStream = new SslStream(client, false);
        try
        {
            sslStream.AuthenticateAsServer(cert);
            logger.Log(LogLevel.Info, Translation.GetTranslation("Web.Ssl.Error", IP.ToString()));
        }
        catch
        {
            return;
        }
        client.ReadTimeout = RTO;
        client.WriteTimeout = WTO;
        byte[] buffer = new byte[10 * 1024];
        while (true)
        {
            try
            {
                sslStream.Read(buffer);
                sslStream.Write(WebServer(new HttpClientMessage(buffer)).GetBytes());
            }
            catch
            {
                break;
            }
        }
        return;
    }
    public HttpServerMessage WebServer(HttpClientMessage cl)
    {
        HttpServerMessage re = new HttpServerMessage();
        if (Program.setting.GetValue("usingTLS") == "true")
        {
            if (Program.setting.GetValue("CompatibleWithHTTP") != "true")
            {
                if (!IsTLS)
                {
                    re.SetStatusCode(308);
                    re.SetStatusText("Permanent Redirect");
                    string host = cl.GetHeaderValue("Host");
                    string[] hs = host.Split(':');
                    re.SetHeaderValue("Location", $"https://{hs[0]}:{Program.setting.GetValue("https_port")}");
                    return re;
                }
            }
        }
        re.SetStatusCode(200);
        re.SetStatusText("OK");
        re.SetBody($"你的IP地址是:{IP.ToString()}");
        re.SetHeaderValue("Content-Type", "text/plain; charset=utf-8");
        if (!cl.Parsed_successfully)
        {
            re.SetStatusCode(500);
            re.SetStatusText("Internal Server Error");
            re.SetHeaderValue("Content-Type", "text/plain; charset=utf-8");
            re.SetBody("解析错误");
        }
        return re;
    }
}