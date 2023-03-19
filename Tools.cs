using System.Text;
//一些无法分类的函数就在这里
class Tools
{
    public static bool ReadConfig(string fileName,char[] separator,int count ,out Dictionary<string, string> output)
    {
        output = new Dictionary<string, string>();
        try
        {
            string[] lines = System.IO.File.ReadAllLines(fileName);
            foreach (string line in lines)
            {
                string[] parts = line.Split(separator, count);
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
            if (Enumerable.SequenceEqual(input,y))
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
}