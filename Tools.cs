using System.Text;
//一些无法分类的函数就在这里
class Tools
{
    public static bool ReadConfig(string fileName, char[] separator, int count, out Dictionary<string, string> output)
    {
        output = new Dictionary<string, string>();
        try
        {
            string[] lines = System.IO.File.ReadAllLines(fileName);
            foreach (string line in lines)
            {
                string line1 = line.Trim();
                if (line1[0] == '#')
                    continue;
                string[] parts = line1.Split(separator, count);
                parts[0] = parts[0].Trim();
                parts[1] = parts[1].Trim();
                output[parts[0]] = parts[1];
            }
        }
        catch
        {
            return false;
        }
        return true;
    }
    public static bool TryGetEncodedString(byte[] input, Encoding encoding, out string output)
    {
        try
        {
            string x = encoding.GetString(input);
            byte[] y = encoding.GetBytes(x);
            if (Enumerable.SequenceEqual(input, y))
            {
                output = x;
                return true;
            }
            output = "";
            return false;
        }
        catch
        {
            output = "";
            return false;
        }
    }
    public static bool CheckEncoding(byte[] input, Encoding encoding)
    {
        try
        {
            string x = encoding.GetString(input);
            byte[] y = encoding.GetBytes(x);
            if (input == y)
            {
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }
    public static int FindDoubleCRLF(byte[] bytes)
    {
        // Check if the array is null or empty
        if (bytes == null || bytes.Length == 0)
        {
            return -1;
        }

        // Loop through the array
        for (int i = 0; i < bytes.Length - 3; i++)
        {
            // Check if the current four bytes are \r\n\r\n
            if (bytes[i] == 13 && bytes[i + 1] == 10 && bytes[i + 2] == 13 && bytes[i + 3] == 10)
            {
                // Return the index
                return i;
            }
        }

        // Return -1 if not found
        return -1;
    }
    public static int FindDoubleLF(byte[] bytes)
    {
        // Check if the array is null or empty
        if (bytes == null || bytes.Length == 0)
        {
            return -1;
        }

        // Loop through the array
        for (int i = 0; i < bytes.Length - 1; i++)
        {
            // Check if the current four bytes are \n\n
            if (bytes[i] == 10 && bytes[i + 1] == 10)
            {
                // Return the index
                return i;
            }
        }

        // Return -1 if not found
        return -1;
    }
    public static bool IsNum(char x)
    {
        if (x >= '0' && x <= '9')
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    //判断一个string是否是数字+单位结构的
    public static bool String2NumUnit(string a, out NumUnit x)
    {
        int l = 0;
        int firstNoNum = 0;
        bool p = true;
        foreach (char c in a)
        {
            if (p == true)
            {
                if (!IsNum(c))
                {
                    p = false;
                    l++;
                    continue;
                }
                firstNoNum++;
            }
            l++;
        }
        int firstLetter = l - 1;
        for (int i = l - 1; i >= 0; --i)
        {
            if (IsNum(a[i]))
            {
                break;
            }
            firstLetter = i;
        }
        if (firstLetter == firstNoNum)
        {
            string N = "";
            string U = "";
            int i = 0;
            foreach (char c in a)
            {
                if (i < firstLetter)
                {
                    N += c;
                }
                else
                {
                    U += c;
                }
                ++i;
            }
            int NUM;
            if (!int.TryParse(N, out NUM))
            {
                x = null;
                return false;
            }
            x = new NumUnit(NUM, U);
            return true;
        }
        x = null;
        return false;
    }
    //将string格式的时间转换为以ms为单位的int
    public static bool StringTime2MsInt(string time, out int x)
    {
        NumUnit nu;
        if (!String2NumUnit(time, out nu))
        {
            x = 0;
            return false;
        }
        int ms;
        switch (nu.unit.ToLower())
        {
            case "ms":
                ms = nu.num;
                break;
            case "s":
                ms = nu.num * 1000;
                break;
            case "m":
                ms = nu.num * 1000 * 60;
                break;
            case "h":
                ms = nu.num * 1000 * 60 * 60;
                break;
            case "d":
                ms = nu.num * 1000 * 60 * 60 * 24;
                break;
            default:
                x = 0;
                return false;
                break;
        }
        x = ms;
        return true;
    }
}
class NumUnit
{
    public NumUnit(int x, string y)
    {
        num = x;
        unit = y;
    }
    public int num;
    public string unit;
}