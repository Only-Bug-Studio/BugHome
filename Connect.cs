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
    public Connect(EndPoint IP, NetworkStream client)
    {
        this.IP = IP;
        this.client = client;
        IsTLS = false;
    }
    public Connect(EndPoint IP, NetworkStream client, X509Certificate2 cert)
    {
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
        client.ReadTimeout = 1000 * 60 * 20;
        client.WriteTimeout = 1000 * 10;
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
        //Read方法
        sslStream.ReadTimeout = 1000 * 60 * 20;//单位ms
        //Write方法
        sslStream.WriteTimeout = 1000 * 10;//单位ms
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
        re.SetBody($"你的IP地址是:{IP.ToString()}");
        re.SetHeaderValue("Content-Type","text/plain; charset=utf-8");
        if (!cl.Parsed_successfully)
        {
            re.SetBody("114514");
        }
        return re;
    }
}