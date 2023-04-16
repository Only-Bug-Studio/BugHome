using System.Text;

enum HTTP_Method { GET, HEAD, POST, PUT, DELETE, CONNECT, OPTIONS, TRACE, PATCH, UNKNOWN };
class HttpClientMessage
{
    //数据段
    bool HaveBody = false;
    byte[] Body = { 0 };
    HTTP_Method Method;
    string Target = "/";
    string Version = "HTTP/1.1";
    Dictionary<string, string> Headers = new Dictionary<string, string>();
    // 定义一个包含换行符的字符数组
    static char[] delimiterChars = { '\r', '\n' };
    static Logger logger = new Logger("HttpClientMessage");
    public HttpClientMessage()
    {
        return;
    }
    public bool Parsed_successfully = false;
    public HttpClientMessage(byte[] bytes)
    {
        string message = "";
        //非空
        if (bytes.Length == 0 || bytes == null)
        {
            logger.Log(LogLevel.Error, Translation.GetTranslation("HttpClientMessage.empty"));
            return;
        }
        //分割
        bool isLF = true;
        int index = Tools.FindDoubleLF(bytes);
        bool IsCannotFind = index == -1;
        if (IsCannotFind)
        {
            isLF = false;
            index = Tools.FindDoubleCRLF(bytes);
        }
        IsCannotFind = index == -1;
        if (IsCannotFind)
        {
            HaveBody = false;
            if (!Tools.TryGetEncodedString(bytes, Encoding.UTF8, out message))
            {
                logger.Log(LogLevel.Error, Translation.GetTranslation("HttpClientMessage.notUTF8"));
                return;
            }
        }
        else
        {
            HaveBody = true;
            if (isLF)
            {
                byte[] part1 = new byte[index];
                byte[] part2 = new byte[bytes.Length - index - 2];
                //将前半部分放入part1
                Array.Copy(bytes, 0, part1, 0, index);
                //将后半部分放入part2
                Array.Copy(bytes, index + 2, part2, 0, bytes.Length - index - 2);
                if (!Tools.TryGetEncodedString(part1, Encoding.UTF8, out message))
                {
                    logger.Log(LogLevel.Error, Translation.GetTranslation("HttpClientMessage.notUTF8"));
                    return;
                }
                Body = part2;
            }
            else
            {
                byte[] part1 = new byte[index];
                byte[] part2 = new byte[bytes.Length - index - 4];
                //将前半部分放入part1
                Array.Copy(bytes, 0, part1, 0, index);
                //将后半部分放入part2
                Array.Copy(bytes, index + 2, part2, 0, bytes.Length - index - 4);
                if (!Tools.TryGetEncodedString(part1, Encoding.UTF8, out message))
                {
                    logger.Log(LogLevel.Error, Translation.GetTranslation("HttpClientMessage.notUTF8"));
                    return;
                }
                Body = part2;
            }
        }
        //解析头部
        // 使用 String.Split 方法按换行符分割字符串
        string[] head = message.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
        bool first = true;
        foreach (string line in head)
        {
            if (first)
            {
                string[] words = line.Split(' ');
                if (words.Length != 3)
                {
                    logger.Log(LogLevel.Error, Translation.GetTranslation("HttpClientMessage.notHttpMessage"));
                    foreach (string word in words)
                    {
                        logger.Log(LogLevel.Debug, word);
                    }
                    logger.Log(LogLevel.Debug, Encoding.UTF8.GetString(bytes));
                    return;
                }
                words[0] = words[0].ToUpper();
                switch (words[0])
                {
                    case "GET":
                        Method = HTTP_Method.GET;
                        break;
                    case "HEAD":
                        Method = HTTP_Method.HEAD;
                        break;
                    case "POST":
                        Method = HTTP_Method.POST;
                        break;
                    case "PUT":
                        Method = HTTP_Method.PUT;
                        break;
                    case "CONNECT":
                        Method = HTTP_Method.CONNECT;
                        break;
                    case "DELETE":
                        Method = HTTP_Method.DELETE;
                        break;
                    case "OPTIONS":
                        Method = HTTP_Method.OPTIONS;
                        break;
                    case "TRACE":
                        Method = HTTP_Method.TRACE;
                        break;
                    case "PATCH":
                        Method = HTTP_Method.PATCH;
                        break;
                    default:
                        Method = HTTP_Method.UNKNOWN;
                        logger.Log(LogLevel.Warn, Translation.GetTranslation("HttpClientMessage.verb.unknown", words[0]));
                        break;
                }
                Target = words[1];
                Version = words[2];
                first = false;
                continue;
            }
            string[] parts = line.Split(new char[] { ':' }, 2);
            parts[0] = parts[0].Trim().ToLower();//根据https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Messages，不区分大小写
            parts[1] = parts[1].Trim();
            Headers[parts[0]] = parts[1];
            Parsed_successfully = true;
        }
    }
    public string GetHeaderValue(string key)
    {
        string a;
        if (Headers.TryGetValue(key.ToLower(), out a))
        {
            return a;
        }
        return "";
    }
    public void SetHeaderValue(string key, string value)
    {
        Headers[key] = value;
    }
    public HTTP_Method GetMethod()
    {
        return Method;
    }
    public void SetMethod(HTTP_Method method)
    {
        Method = method;
    }
    public byte[] GetBody()
    {
        return Body;
    }
    public void SetBody(byte[] newBody)
    {
        if (newBody == null || newBody.Length == 0)
            HaveBody = false;
        else
        {
            HaveBody = true;
            SetHeaderValue("Content-Length", newBody.Length.ToString());
        }

        Body = newBody;
    }
    public bool IsHaveBody()
    {
        return HaveBody;
    }
    public void SetBody(string newBody)
    {
        SetBody(Encoding.UTF8.GetBytes(newBody));
    }
    public string GetTarget()
    {
        return Target;
    }
    public void SetTarget(string newTarget)
    {
        Target = newTarget;
    }
    public string GetVersion()
    {
        return Version;
    }
    public void SetVersion(string newVersion)
    {
        Version = newVersion;
    }
    public byte[] ToBytes()
    {
        string startLine = $"{Method} {Target} {Version}";
        string headers = "";
        foreach (var header in Headers)
        {
            string x = header.Key;
            string y = header.Value;
            headers += $"{x}: {y}\r\n";
        }
        //不用两个\n\\r 一个就能空行
        string _HEAD = $"{startLine}\r\n{headers}\r\n";
        byte[] HEAD = Encoding.UTF8.GetBytes(_HEAD);
        if (HaveBody)
        {
            byte[] message = HEAD.Concat(Body).ToArray();
            return message;
        }
        return HEAD;
    }
}