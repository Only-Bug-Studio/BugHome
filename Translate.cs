class Translation
{
    static Dictionary<string, string> TS = new Dictionary<string, string>();
    static bool inited = false;
    static Logger logger = new Logger("Translation");
    public static void TranslationInit()
    {
        try
        {
            string[] lines = System.IO.File.ReadAllLines("Translation/zh-cn.txt");
            foreach (string line in lines)
            {
                string[] parts = line.Split(new char[] { '=' }, 2);
                TS[parts[0]] = parts[1];
            }
        }
        catch//(Exception e)
        {
            logger.Log(LogLevel.Fatal, "Can't find zh-cn Translation file!");
        }
        inited = true;
    }
    public static void LoadTranslation(string name)
    {
        try
        {
            string[] lines = System.IO.File.ReadAllLines("Translation/" + name + ".txt");
            foreach (string line in lines)
            {
                string[] parts = line.Split(new char[] { '=' }, 2);
                TS[parts[0]] = parts[1];
            }
        }
        catch//(Exception e)
        {
            logger.Log(LogLevel.Error, GetTranslation("Translation.cannotFind", name));
        }
    }
    public static string GetTranslation(string key, params string[] args)
    {
        if (!inited)
        {
            TranslationInit();
        }
        string originalContent;
        if (TS.TryGetValue(key, out originalContent))
        {
            string num = new String("");
            int length = args.Length;
            bool inBrace = false;
            bool inBackslash = false;
            string processedContent = new String("");
            foreach (char processingChar in originalContent)
            {
                if (inBackslash)
                {
                    if (processingChar == 'n')
                    {
                        processedContent += '\n';
                    }
                    else
                    {
                        processedContent += processingChar;
                    }
                    inBackslash = false;
                }
                else if (inBrace)
                {
                    if (processingChar == '}')
                    {
                        int x;
                        if (int.TryParse(num, out x))
                        {
                            if (x >= length)
                            {
                                logger.Log(LogLevel.Error, GetTranslation("Translation.ParameterError"));
                                processedContent += ' ';
                                processedContent += GetTranslation("Translation.Error");
                                processedContent += ' ';
                            }
                            else
                            {
                                processedContent += ' ';
                                processedContent += args[x];
                                processedContent += ' ';
                            }
                            inBrace = false;
                        }
                        else
                        {
                            logger.Log(LogLevel.Error, GetTranslation("Translation.ParameterError"));
                            processedContent += ' ';
                            processedContent += GetTranslation("Translation.Error");
                            processedContent += ' ';
                        }
                    }
                    else
                    {
                        num += processingChar;
                    }
                }
                else if (processingChar == '\\')
                {
                    continue;
                }
                else if (processingChar == '{')
                {
                    inBrace = true;
                }
                else
                {
                    processedContent += processingChar;
                }
            }
            return processedContent;
        }
        logger.Log(LogLevel.Error, key + " can't find");
        return key;
    }
}