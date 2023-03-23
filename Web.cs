using System.Net;
using System.Net.Sockets;
using System.Text;
class Web
{
    static TcpListener server = null;
    static Logger logger = new Logger("Web");
    public static void Solve(object x)
    {
        Socket client = x as Socket;
        byte[] recvBytes = new byte[1024 * 5];
        int bytes;

        while (true)
        {
            try
            {
                bytes = client.Receive(recvBytes, recvBytes.Length, 0);
                HttpClientMessage cm = new HttpClientMessage(recvBytes);
                HttpServerMessage sm = new HttpServerMessage();
                sm.SetHeaderValue("Content-Length", "5");
                sm.SetBody(Encoding.UTF8.GetBytes("hello"));
                client.Send(sm.GetBytes());
            }
            catch
            {
                break;
            }
        }
        try
        {
            client.Close();
        }
        catch
        {

        }
        return;
    }
    public static void WebMain()
    {
        Int32 port;
        if (!Int32.TryParse(Program.setting.GetValue("port"), out port))
        {
            logger.Log(LogLevel.Fatal, Translation.GetTranslation("Web.portNotNum"));
        }
        IPAddress localAddr = IPAddress.Parse("127.0.0.1");
        server = new TcpListener(localAddr, port);
        server.Start();
        Data.IsWebOK = true;
        logger.Log(LogLevel.Info, Translation.GetTranslation("Web.Open"));
        Thread s = null;
        while (true)
        {
            if (!Data.IsOpen)
            {
                break;
            }
            Socket client = server.AcceptSocket();
            //交给新线程
            s = new Thread(Solve);
            s.Start(client);
        }
        server.Stop();
    }
}