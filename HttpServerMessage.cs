using System.Text;

class HttpServerMessage
{
    //数据段
    bool HaveBody = false;
    byte[] Body = { 0 };
    string Version = "HTTP/1.1";
    int statusCode = 200;
    string statusText = "OK";
    Dictionary<string, string> Headers = new Dictionary<string, string>();
    // 定义一个包含换行符的字符数组
    static char[] delimiterChars = { '\r', '\n' };
    static Logger logger = new Logger("HttpServerMessage");
    public HttpServerMessage()
    {
        return;
    }
    public HttpServerMessage(byte[] bytes)
    {
        string message = "";
        //非空
        if (bytes.Length == 0 || bytes == null)
        {
            logger.Log(LogLevel.Error, Translation.GetTranslation("HttpServerMessage.empty"));
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
                logger.Log(LogLevel.Error, Translation.GetTranslation("HttpServerMessage.notUTF8"));
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
                    logger.Log(LogLevel.Error, Translation.GetTranslation("HttpServerMessage.notUTF8"));
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
                    logger.Log(LogLevel.Error, Translation.GetTranslation("HttpServerMessage.notUTF8"));
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
                string[] words = line.Split(' ', 3);
                Version = words[0];
                if (!int.TryParse(words[1], out statusCode))
                {
                    logger.Log(LogLevel.Error, Translation.GetTranslation("HttpServerMessage.notHttpMessage"));
                    return;
                }
                statusText = words[2];
                first = false;
                continue;
            }
            string[] parts = line.Split(new char[] { ':' }, 2);
            parts[0] = parts[0].Trim().ToLower();//根据https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Messages，不区分大小写
            parts[1] = parts[1].Trim();
            Headers[parts[0]] = parts[1];
        }
    }
    public int GetStatusCode()
    {
        return statusCode;
    }
    public void SetStatusCode(int x)
    {
        statusCode = x;
    }
    public string GetStatusText()
    {
        return statusText;
    }
    public void SetStatusText(string x)
    {
        statusText = x;
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
    public string GetVersion()
    {
        return Version;
    }
    public void SetVersion(string newVersion)
    {
        Version = newVersion;
    }
    public byte[] GetBytes()
    {
        string startLine = $"{Version} {statusCode} {statusText}";
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